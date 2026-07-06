# Quick Migration Guide for Remaining Services

## Summary
- **Completed:** 14 services (68%)
- **Remaining:** 8 services (32%)
- **Build Status:** ✅ Passing

---

## Template Pattern

Use this pattern for EVERY remaining service:

### Step 1: Update Field Declarations
```csharp
// ❌ BEFORE (Direct DbContext)
private readonly DbContextBeli _context;
private readonly DbContextHutang _contextAp;

// ✅ AFTER (IDbContextFactory)
private readonly IDbContextFactory<DbContextBeli> _context;
private readonly IDbContextFactory<DbContextHutang> _contextAp;
```

### Step 2: Update Constructor
```csharp
// ❌ BEFORE
public PurchaseServices(DbContextBeli context, DbContextHutang contextHutang)
{
	_context = context;
	_contextAp = contextHutang;
}

// ✅ AFTER
public PurchaseServices(IDbContextFactory<DbContextBeli> context, 
						IDbContextFactory<DbContextHutang> contextHutang)
{
	_context = context;
	_contextAp = contextHutang;
}
```

### Step 3: Wrap Every Method

```csharp
// ❌ BEFORE (Long-lived context)
public List<IrTransH> GetTransH()
{
	return _context.IrTransHs
		.AsNoTracking()
		.Include(p => p.IrTransDs)
		.OrderByDescending(x => x.Tanggal)
		.ToList();
}

// ✅ AFTER (Short-lived context)
public List<IrTransH> GetTransH()
{
	using var db = _context.CreateDbContext();
	return db.IrTransHs
		.AsNoTracking()
		.Include(p => p.IrTransDs)
		.OrderByDescending(x => x.Tanggal)
		.ToList();
}
```

### Step 4: Handle Multi-Context Services

For services with multiple DbContexts (like LaporanStock with 6 contexts):

```csharp
// ❌ BEFORE
private readonly DbContextPersediaan _context;
private readonly DbContextBeli _contextIR;
private readonly DbContextJual _contextOE;
// ... 3 more

// ✅ AFTER
private readonly IDbContextFactory<DbContextPersediaan> _context;
private readonly IDbContextFactory<DbContextBeli> _contextIR;
private readonly IDbContextFactory<DbContextJual> _contextOE;
// ... 3 more

// In a method using multiple contexts:
public List<Report> GetCombinedReport()
{
	using var db = _context.CreateDbContext();
	using var dbIR = _contextIR.CreateDbContext();
	using var dbOE = _contextOE.CreateDbContext();

	var fromIC = db.IcMasters.ToList();
	var fromIR = dbIR.IrTransHs.ToList();
	var fromOE = dbOE.OeTransHs.ToList();

	return CombineReports(fromIC, fromIR, fromOE);
}
```

---

## Services Requiring Migration

### 1. eSoft.Pembelian\Services\PurchaseServices.cs
**Contexts to convert:** `_context` (DbContextBeli), `_contextAp` (DbContextHutang), `_contextIc` (DbContextPersediaan)

**Quick Migration:**
```csharp
// Field change:
private readonly IDbContextFactory<DbContextBeli> _context;
private readonly IDbContextFactory<DbContextHutang> _contextAp;
private readonly IDbContextFactory<DbContextPersediaan> _contextIc;

// Constructor - same signature, just type changes to IDbContextFactory<T>

// Wrap every method:
public IrTransH GetIrTrans(int id)
{
	using var db = _context.CreateDbContext();
	return db.IrTransHs.AsNoTracking().Include(p => p.IrTransDs).FirstOrDefault(x => x.IrTransHId == id);
}
```

---

### 2. eSoft.LaporanStock\Services\LaporanStockServices.cs
**Contexts:** 6 total (_context, _contextIR, _contextOE, _contextAR, _contextOR, _contextAP)

**Strategy:** Same pattern but for 6 factories. Watch for methods that JOIN across multiple contexts.

---

### 3. eSoft.Financial\Services\FinancialServices.cs (LARGEST - 9,511 lines!)
**Contexts:** 9 total (_contextIC, _contextIR, _contextOE, _contextAR, _contextAP, _contextCB, _contextGL, _contextFC, _contextAS)

**Strategy:** Apply same pattern but start with simpler methods first. Test after each ~100 methods.

---

### 4-5. Order Services
- `eSoft.Order\Services\OrderSalesServices.cs` - 3 contexts (Order, AR, IC)
- `eSoft.Order\Services\OrderPurchaseServices.cs` - 3 contexts (Order, AP, IC)

**Same pattern as PurchaseServices**

---

### 6-8. Simple Services (1 context each)
- `Accounting\Services\ExcelServices.cs` - 1 context (DbContextJual)
- `Accounting\Services\AdministrationServices.cs` - 1 context (ApplicationDbContext)
- `eSoft.Administration\Services\AdministrationServices.cs` - 1 context (IdentityDbContext)

**Simplest migration - just replace context type and add `using var db` to each method**

---

## Automated Bulk Migration Script

Use this PowerShell to migrate simpler services in batch:

```powershell
function Migrate-Service {
	param(
		[string]$FilePath,
		[string]$OldContextType,
		[string]$NewContextType
	)

	$content = Get-Content $FilePath -Raw

	# Replace field declaration
	$content = $content -replace "private readonly $OldContextType ", "private readonly IDbContextFactory<$OldContextType> "

	# Replace constructor parameter type
	$content = $content -replace "\($OldContextType context", "(IDbContextFactory<$OldContextType> context"

	# Replace _context.Entity access (this is simplified - manual review recommended)
	$lines = $content -split "`n"
	$output = @()
	$inMethod = $false
	$methodNeedsUsing = $false

	foreach ($line in $lines) {
		# Detect public method
		if ($line -match 'public\s+' -and $line -match '\{') {
			$inMethod = $true
			$methodNeedsUsing = $line -match "_context\."
			$output += $line
			if ($methodNeedsUsing) {
				$indent = [regex]::Match($line, '^\s*').Value.Length + 4
				$output += " " * $indent + "using var db = _context.CreateDbContext();"
			}
		} else {
			$output += $line
		}
	}

	Set-Content $FilePath ($output -join "`n") -Encoding UTF8
}

# Example usage:
# Migrate-Service "D:\...\PurchaseServices.cs" "DbContextBeli" "DbContextBeli"
```

---

## Testing Checklist After Migration

- [ ] Build succeeds with no errors
- [ ] No CS0103 errors ("name does not exist")
- [ ] No CS1061 errors ("does not contain definition")
- [ ] Run unit tests if available
- [ ] Test API endpoints that use the service
- [ ] Monitor application logs for DbContext issues

---

## Common Errors & Fixes

| Error | Cause | Fix |
|-------|-------|-----|
| `CS0103: 'db' does not exist` | Missing `using var db = _context.CreateDbContext();` | Add at start of method |
| `CS1061: 'IDbContextFactory' has no definition for 'Entity'` | Trying to access `_context.DbSet` directly | Use local `db.DbSet` instead |
| `The DbSet is not found` | Wrong context being used (e.g., using _contextAR instead of _context) | Check which field holds that DbSet |
| `SaveChanges failed` | Disposed context before SaveChanges | Ensure `using` statement wraps entire operation |

---

## Priority Recommendations

### If You Have 1 Hour:
Complete **ExcelServices** (simplest, 1 context) + **OrderSalesServices** + **OrderPurchaseServices**

### If You Have 3-4 Hours:
Add **PurchaseServices** + **LaporanStockServices**

### If You Have 1 Full Day:
Migrate everything including **FinancialServices** (set aside 3-4 hours just for that one!)

---

## Validation After Each Service

```powershell
# Build the solution after each service migration
dotnet build BMA-PRO.sln
```

This ensures errors are caught immediately rather than accumulating.
