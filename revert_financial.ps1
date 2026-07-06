# Simple revert script for FinancialServices.cs
$filePath = "eSoft.Financial\Services\FinancialServices.cs"
$content = [System.IO.File]::ReadAllText($filePath)

# Step 1: Revert field declarations
$content = $content.Replace('private readonly IDbContextFactory<DbContextPersediaan> _contextIC;', 
						   'private readonly DbContextPersediaan _contextIC;')
$content = $content.Replace('private readonly IDbContextFactory<DbContextBeli> _contextIR;', 
						   'private readonly DbContextBeli _contextIR;')
$content = $content.Replace('private readonly IDbContextFactory<DbContextJual> _contextOE;', 
						   'private readonly DbContextJual _contextOE;')
$content = $content.Replace('private readonly IDbContextFactory<DbContextPiutang> _contextAR;', 
						   'private readonly DbContextPiutang _contextAR;')
$content = $content.Replace('private readonly IDbContextFactory<DbContextHutang> _contextAP;', 
						   'private readonly DbContextHutang _contextAP;')
$content = $content.Replace('private readonly IDbContextFactory<DbContextBank> _contextCB;', 
						   'private readonly DbContextBank _contextCB;')
$content = $content.Replace('private readonly IDbContextFactory<DbContextLedger> _contextGL;', 
						   'private readonly DbContextLedger _contextGL;')
$content = $content.Replace('private readonly IDbContextFactory<DbContextFinancial> _contextFC;', 
						   'private readonly DbContextFinancial _contextFC;')
$content = $content.Replace('private readonly IDbContextFactory<DbContextAssets> _contextAS;', 
						   'private readonly DbContextAssets _contextAS;')

# Step 2: Revert contextXX back to _contextXX
$content = $content.Replace('contextIC.', '_contextIC.')
$content = $content.Replace('contextIR.', '_contextIR.')
$content = $content.Replace('contextOE.', '_contextOE.')
$content = $content.Replace('contextAR.', '_contextAR.')
$content = $content.Replace('contextAP.', '_contextAP.')
$content = $content.Replace('contextCB.', '_contextCB.')
$content = $content.Replace('contextGL.', '_contextGL.')
$content = $content.Replace('contextFC.', '_contextFC.')
$content = $content.Replace('contextAS.', '_contextAS.')

# Step 3: Remove stray using var context lines
$lines = $content -split "`n"
$filtered = @()
foreach ($line in $lines) {
	if ($line -notmatch 'using var context\w\w') {
		$filtered += $line
	}
}
$content = $filtered -join "`n"

[System.IO.File]::WriteAllText($filePath, $content)
Write-Host "Reverted FinancialServices.cs to original state"
