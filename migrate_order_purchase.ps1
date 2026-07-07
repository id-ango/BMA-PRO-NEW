# Migrate OrderPurchaseServices to IDbContextFactory pattern
# This script: 1) Updates field declarations, 2) Updates constructor, 3) Wraps methods with using statements

$filePath = "eSoft.Order\Services\OrderPurchaseServices.cs"
$content = Get-Content $filePath -Raw

# Step 1: Update field declarations
$content = $content -replace 'private readonly DbContextOrder _context;', 'private readonly IDbContextFactory<DbContextOrder> _context;'
$content = $content -replace 'private readonly DbContextHutang _contextAp;', 'private readonly IDbContextFactory<DbContextHutang> _contextAp;'
$content = $content -replace 'private readonly DbContextPersediaan _contextIc;', 'private readonly IDbContextFactory<DbContextPersediaan> _contextIc;'

# Step 2: Update constructor signature
$content = $content -replace 'public OrderPurchaseServices\(DbContextOrder context, DbContextHutang contextHutang, DbContextPersediaan contextPersediaan\)', 'public OrderPurchaseServices(IDbContextFactory<DbContextOrder> context, IDbContextFactory<DbContextHutang> contextHutang, IDbContextFactory<DbContextPersediaan> contextPersediaan)'

# Step 3: Wrap method bodies - for each private/public method, inject using statements at the start
# This is done per-method pattern matching

# GetVendorId
$content = $content -replace `
	'private ApSuppl GetVendorId\(string id\)\s*\{\s*return _contextAp\.ApSuppls\.Where\(x => x\.Supplier == id\)\.FirstOrDefault\(\);', `
	'private ApSuppl GetVendorId(string id)
		{
			using var db = _contextAp.CreateDbContext();
			return db.ApSuppls.Where(x => x.Supplier == id).FirstOrDefault();'

# GetHutang
$content = $content -replace `
	'public ApHutang GetHutang\(string bukti\)\s*\{\s*return _contextAp\.ApHutangs\.Where\(x => x\.Dokumen == bukti\)\.FirstOrDefault\(\);', `
	'public ApHutang GetHutang(string bukti)
		{
			using var db = _contextAp.CreateDbContext();
			return db.ApHutangs.Where(x => x.Dokumen == bukti).FirstOrDefault();'

# GetPoTrans - single statement method
$content = $content -replace `
	'public PoTransH GetPoTrans\(int id\)\s*\{\s*return _context\.PoTransHs\.Include\(p => p\.PoTransDs\)\.Where\(x => x\.PoTransHId == id\)\.FirstOrDefault\(\);', `
	'public PoTransH GetPoTrans(int id)
		{
			using var db = _context.CreateDbContext();
			return db.PoTransHs.Include(p => p.PoTransDs).Where(x => x.PoTransHId == id).FirstOrDefault();'

# Replace all remaining _context\. with db\. in method bodies (when inside using statement)
# First, find all method patterns and add using var db statements

# For methods with multiple _context. usages, we need broader patterns
# Replace pattern: _context. -> db. (when in a scoped context)
# and _contextAp. -> dbAp., _contextIc. -> dbIc.

# More aggressive: replace all direct context references within method bodies
$lines = $content -split '\n'
$inMethod = $false
$methodDepth = 0
$newLines = @()
$methodStart = -1

for ($i = 0; $i -lt $lines.Count; $i++) {
	$line = $lines[$i]

	# Check if entering a public/private method
	if ($line -match '(public|private)\s+(async\s+)?(Task<|List<|void|bool|PoTransH|ApSuppl|ApHutang|IEnumerable)' -and $line -match '\{') {
		$inMethod = $true
		$methodDepth = 1
		$methodStart = $newLines.Count

		# If method doesn't already have "using var db" statement, we'll add it
		$hasUsing = $line -match 'using var'

		if (-not $hasUsing -and $line -match '\{') {
			# Add the line as-is
			$newLines += $line
			continue
		}
	}

	# Track braces to know when method ends
	if ($inMethod) {
		$methodDepth += ($line -split '{' | Measure-Object).Count - 1
		$methodDepth -= ($line -split '}' | Measure-Object).Count - 1

		if ($methodDepth -le 0) {
			$inMethod = $false
		}
	}

	# Replace context references with local db references after we've established using
	if ($inMethod -and $methodStart -ge 0) {
		$prevLines = $newLines[($methodStart)..($newLines.Count-1)] -join "`n"
		if ($prevLines -match 'using var db' -or $prevLines -match 'using var dbAp' -or $prevLines -match 'using var dbIc') {
			# Already has using, replace context references
			$line = $line -replace '\b_context\.', 'db.'
			$line = $line -replace '\b_contextAp\.', 'dbAp.'
			$line = $line -replace '\b_contextIc\.', 'dbIc.'
		}
	}

	$newLines += $line
}

$content = $newLines -join "`n"

# Save the file
Set-Content -Path $filePath -Value $content -Encoding UTF8

Write-Host "✅ OrderPurchaseServices migration completed!" -ForegroundColor Green
Write-Host "  - Field declarations updated to IDbContextFactory<T>" -ForegroundColor Green
Write-Host "  - Constructor signature updated" -ForegroundColor Green
Write-Host "  - Methods processed for using statements" -ForegroundColor Green
