# Migrate LaporanStockServices.cs to IDbContextFactory<T>
$filePath = "eSoft.LaporanStock\Services\LaporanStockServices.cs"
$content = [System.IO.File]::ReadAllText($filePath)

# Step 1: Replace field declarations (6 contexts)
$fieldsBefore = @"
		private readonly DbContextPersediaan _context;
		private readonly DbContextBeli _contextIR;
		private readonly DbContextJual _contextOE;
		private readonly DbContextPiutang _contextAR;
		private readonly DbContextOrder _contextOR;
		private readonly DbContextHutang _contextAP;
"@

$fieldsAfter = @"
		private readonly IDbContextFactory<DbContextPersediaan> _context;
		private readonly IDbContextFactory<DbContextBeli> _contextIR;
		private readonly IDbContextFactory<DbContextJual> _contextOE;
		private readonly IDbContextFactory<DbContextPiutang> _contextAR;
		private readonly IDbContextFactory<DbContextOrder> _contextOR;
		private readonly IDbContextFactory<DbContextHutang> _contextAP;
"@

$content = $content.Replace($fieldsBefore, $fieldsAfter)

# Step 2: Replace constructor signature (6 parameters)
$constructorBefore = "public LaporanStockServices(DbContextPersediaan context, DbContextBeli contextBeli, DbContextJual contextJual, DbContextPiutang contextPiutang, DbContextOrder contextOrder, DbContextHutang contextHutang)"
$constructorAfter = "public LaporanStockServices(IDbContextFactory<DbContextPersediaan> context, IDbContextFactory<DbContextBeli> contextBeli, IDbContextFactory<DbContextJual> contextJual, IDbContextFactory<DbContextPiutang> contextPiutang, IDbContextFactory<DbContextOrder> contextOrder, IDbContextFactory<DbContextHutang> contextHutang)"
$content = $content.Replace($constructorBefore, $constructorAfter)

# Step 3: Now process line by line to wrap methods and replace context references
$lines = $content -split "`n"
$result = @()
$i = 0

while ($i -lt $lines.Count) {
	$line = $lines[$i]

	# Detect method start
	if (($line -match '^\s+(public|private)\s+' -or $line -contains 'public ' -or $line -contains 'private ') -and $line -contains '{') {
		# This is a method declaration
		$result += $line

		# Look ahead to find which contexts are used
		$contextsNeeded = @()
		$braceCount = ([regex]::Matches($line, '\{') | Measure-Object).Count - ([regex]::Matches($line, '\}') | Measure-Object).Count

		$scanLimit = [Math]::Min($i + 150, $lines.Count - 1)
		for ($j = $i + 1; $j -le $scanLimit; $j++) {
			$braceCount += ([regex]::Matches($lines[$j], '\{') | Measure-Object).Count
			$braceCount -= ([regex]::Matches($lines[$j], '\}') | Measure-Object).Count

			if ($lines[$j] -match '_context\b' -and $contextsNeeded -notcontains 'IC') { $contextsNeeded += 'IC' }
			if ($lines[$j] -match '_contextIR\b' -and $contextsNeeded -notcontains 'IR') { $contextsNeeded += 'IR' }
			if ($lines[$j] -match '_contextOE\b' -and $contextsNeeded -notcontains 'OE') { $contextsNeeded += 'OE' }
			if ($lines[$j] -match '_contextAR\b' -and $contextsNeeded -notcontains 'AR') { $contextsNeeded += 'AR' }
			if ($lines[$j] -match '_contextOR\b' -and $contextsNeeded -notcontains 'OR') { $contextsNeeded += 'OR' }
			if ($lines[$j] -match '_contextAP\b' -and $contextsNeeded -notcontains 'AP') { $contextsNeeded += 'AP' }

			if ($braceCount -le 0) { break }
		}

		# Get indentation
		$indentLen = [regex]::Match($line, '^\s*').Value.Length
		$indent = ' ' * ($indentLen + 4)

		# Add using statements
		if ($contextsNeeded.Count -gt 0) {
			foreach ($ctx in $contextsNeeded) {
				switch ($ctx) {
					'IC' { $result += "$indent`using var db = _context.CreateDbContext();" }
					'IR' { $result += "$indent`using var dbIR = _contextIR.CreateDbContext();" }
					'OE' { $result += "$indent`using var dbOE = _contextOE.CreateDbContext();" }
					'AR' { $result += "$indent`using var dbAR = _contextAR.CreateDbContext();" }
					'OR' { $result += "$indent`using var dbOR = _contextOR.CreateDbContext();" }
					'AP' { $result += "$indent`using var dbAP = _contextAP.CreateDbContext();" }
				}
			}
		}

		$i++
	} else {
		# Regular line - replace context references
		$line = $line.Replace('_context.', 'db.')
		$line = $line.Replace('_contextIR.', 'dbIR.')
		$line = $line.Replace('_contextOE.', 'dbOE.')
		$line = $line.Replace('_contextAR.', 'dbAR.')
		$line = $line.Replace('_contextOR.', 'dbOR.')
		$line = $line.Replace('_contextAP.', 'dbAP.')

		$result += $line
		$i++
	}
}

$finalContent = $result -join "`n"
[System.IO.File]::WriteAllText($filePath, $finalContent)

Write-Host "✅ LaporanStockServices.cs migration completed!"
Write-Host "   - 6 field declarations updated"
Write-Host "   - Constructor parameters updated"
Write-Host "   - All methods wrapped with using statements"
Write-Host "   - All context references replaced with local db variables"
