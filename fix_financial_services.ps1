# Fix FinancialServices.cs with proper context factory pattern

$filePath = "eSoft.Financial\Services\FinancialServices.cs"

# Read the file
$content = [System.IO.File]::ReadAllText($filePath)

# Step 1: Revert any dbXX back to _contextXX
$content = $content.Replace('dbIC.', '_contextIC.')
$content = $content.Replace('dbIR.', '_contextIR.')
$content = $content.Replace('dbOE.', '_contextOE.')
$content = $content.Replace('dbAR.', '_contextAR.')
$content = $content.Replace('dbAP.', '_contextAP.')
$content = $content.Replace('dbCB.', '_contextCB.')
$content = $content.Replace('dbGL.', '_contextGL.')
$content = $content.Replace('dbFC.', '_contextFC.')
$content = $content.Replace('dbAS.', '_contextAS.')

# Step 2: Now do proper wrapping - split by lines and process each method
$lines = $content -split "`r`n"
$result = @()
$i = 0
$inMethod = $false
$methodIndent = 0

while ($i -lt $lines.Count) {
	$line = $lines[$i]

	# Check if this line declares a method
	if ($line -match '^\s*(public|private|protected|internal)\s+' -and $line -contains '{') {
		# This is a method declaration
		$result += $line

		# Look ahead to find which contexts are used
		$contextsNeeded = @()
		$braceCount = ([regex]::Matches($line, '\{') | Measure-Object).Count - ([regex]::Matches($line, '\}') | Measure-Object).Count

		# Scan ahead for context references
		$scanLimit = [Math]::Min($i + 200, $lines.Count - 1)
		for ($j = $i + 1; $j -le $scanLimit; $j++) {
			$braceCount += ([regex]::Matches($lines[$j], '\{') | Measure-Object).Count
			$braceCount -= ([regex]::Matches($lines[$j], '\}') | Measure-Object).Count

			# Check what contexts are used
			if ($lines[$j] -match '_contextIC\b' -and $contextsNeeded -notcontains 'IC') { $contextsNeeded += 'IC' }
			if ($lines[$j] -match '_contextIR\b' -and $contextsNeeded -notcontains 'IR') { $contextsNeeded += 'IR' }
			if ($lines[$j] -match '_contextOE\b' -and $contextsNeeded -notcontains 'OE') { $contextsNeeded += 'OE' }
			if ($lines[$j] -match '_contextAR\b' -and $contextsNeeded -notcontains 'AR') { $contextsNeeded += 'AR' }
			if ($lines[$j] -match '_contextAP\b' -and $contextsNeeded -notcontains 'AP') { $contextsNeeded += 'AP' }
			if ($lines[$j] -match '_contextCB\b' -and $contextsNeeded -notcontains 'CB') { $contextsNeeded += 'CB' }
			if ($lines[$j] -match '_contextGL\b' -and $contextsNeeded -notcontains 'GL') { $contextsNeeded += 'GL' }
			if ($lines[$j] -match '_contextFC\b' -and $contextsNeeded -notcontains 'FC') { $contextsNeeded += 'FC' }
			if ($lines[$j] -match '_contextAS\b' -and $contextsNeeded -notcontains 'AS') { $contextsNeeded += 'AS' }

			if ($braceCount -le 0) { break }
		}

		# Get indentation
		$methodIndent = [regex]::Match($line, '^\s*').Value.Length
		$indentStr = ' ' * ($methodIndent + 4)

		# Add using statements
		if ($contextsNeeded.Count -gt 0) {
			foreach ($ctx in $contextsNeeded) {
				switch ($ctx) {
					'IC' { $result += "$indentStr`using var contextIC = _contextIC.CreateDbContext();" }
					'IR' { $result += "$indentStr`using var contextIR = _contextIR.CreateDbContext();" }
					'OE' { $result += "$indentStr`using var contextOE = _contextOE.CreateDbContext();" }
					'AR' { $result += "$indentStr`using var contextAR = _contextAR.CreateDbContext();" }
					'AP' { $result += "$indentStr`using var contextAP = _contextAP.CreateDbContext();" }
					'CB' { $result += "$indentStr`using var contextCB = _contextCB.CreateDbContext();" }
					'GL' { $result += "$indentStr`using var contextGL = _contextGL.CreateDbContext();" }
					'FC' { $result += "$indentStr`using var contextFC = _contextFC.CreateDbContext();" }
					'AS' { $result += "$indentStr`using var contextAS = _contextAS.CreateDbContext();" }
				}
			}
		}

		$i++
	} else {
		# Regular line - replace context references with context variable references
		$line = $line.Replace('_contextIC.', 'contextIC.')
		$line = $line.Replace('_contextIR.', 'contextIR.')
		$line = $line.Replace('_contextOE.', 'contextOE.')
		$line = $line.Replace('_contextAR.', 'contextAR.')
		$line = $line.Replace('_contextAP.', 'contextAP.')
		$line = $line.Replace('_contextCB.', 'contextCB.')
		$line = $line.Replace('_contextGL.', 'contextGL.')
		$line = $line.Replace('_contextFC.', 'contextFC.')
		$line = $line.Replace('_contextAS.', 'contextAS.')

		$result += $line
		$i++
	}
}

# Write back the fixed content
$finalContent = $result -join "`r`n"
[System.IO.File]::WriteAllText($filePath, $finalContent)

Write-Host "✅ FinancialServices.cs migration completed!"
Write-Host "   - Reverted all dbXX references"
Write-Host "   - Added using var contextXX statements inside each method"
Write-Host "   - Replaced _contextXX references with contextXX inside methods"
