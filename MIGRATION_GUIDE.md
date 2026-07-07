# DbContext Factory Migration Guide

## Current Status

### ✅ Successfully Migrated (5 Services - Build GREEN)
1. **eSoft.Piutang.Services.PaymentArServices.cs** - Already using `IDbContextFactory<DbContextPiutang>`
2. **eSoft.Hutang.Services.PaymentApServices.cs** - Already using `IDbContextFactory<DbContextHutang>`
3. **eSoft.CashBank.Services.CashBankServices.cs** - Migrated and build-validated
4. **eSoft.Administration.Services.AdministrationServices.cs** - Migrated and build-validated
5. **eSoft.Piutang.Services.ReceivableServices.cs** - Already compliant
6. **eSoft.Hutang.Services.PayableServices.cs** - Already compliant

### ⏳ Remaining Services (5 - NOT YET MIGRATED)

| Service | File Size | DbContexts | Difficulty |
|---------|-----------|-----------|------------|
| OrderPurchaseServices.cs | 488 lines | 3 (_context, _contextAp, _contextIc) | Medium |
| OrderSalesServices.cs | 636 lines | 3 (_context, _contextAr, _contextIc) | Medium |
| PurchaseServices.cs | 1,087 lines | ? | High |
| LaporanStockServices.cs | 2,406 lines | 6 (_context, _contextIR, _contextOE, _contextAR, _contextOR, _contextAP) | Very High |
| FinancialServices.cs | 9,512 lines | 9+ | Defer (manual/AST) |

---

## Migration Pattern

Each service migration follows this **3-step process**:

### Step 1: Update Field Declarations

**BEFORE:**
```csharp
private readonly DbContextOrder _context;
private readonly DbContextHutang _contextAp;
private readonly DbContextPersediaan _contextIc;

public OrderPurchaseServices(DbContextOrder context, DbContextHutang contextHutang, DbContextPersediaan contextPersediaan)
{
	_context = context;
	_contextAp = contextHutang;
	_contextIc = contextPersediaan;
}
```

**AFTER:**
```csharp
private readonly IDbContextFactory<DbContextOrder> _context;
private readonly IDbContextFactory<DbContextHutang> _contextAp;
private readonly IDbContextFactory<DbContextPersediaan> _contextIc;

public OrderPurchaseServices(IDbContextFactory<DbContextOrder> context, IDbContextFactory<DbContextHutang> contextHutang, IDbContextFactory<DbContextPersediaan> contextPersediaan)
{
	_context = context;
	_contextAp = contextHutang;
	_contextIc = contextPersediaan;
}
```

### Step 2: For Each Method, Wrap with `using var db = _contextXXX.CreateDbContext();`

**PATTERN FOR READ-ONLY METHODS:**

BEFORE:
```csharp
public PoTransH GetPoTrans(int id)
{
	return _context.PoTransHs.Include(p => p.PoTransDs).Where(x => x.PoTransHId == id).FirstOrDefault();
}
```

AFTER:
```csharp
public PoTransH GetPoTrans(int id)
{
	using var db = _context.CreateDbContext();
	return db.PoTransHs.Include(p => p.PoTransDs).Where(x => x.PoTransHId == id).FirstOrDefault();
}
```

**PATTERN FOR WRITE/UPDATE METHODS:**

BEFORE:
```csharp
public void SaveOrderAktif(string customer)
{
	_context.PoTransHs.Include(p => p.PoTransDs).Where(x => x.NoLpb == customer).FirstOrDefault().Cek = "3";
	_context.SaveChanges();
}
```

AFTER:
```csharp
public void SaveOrderAktif(string customer)
{
	using var db = _context.CreateDbContext();
	db.PoTransHs.Include(p => p.PoTransDs).Where(x => x.NoLpb == customer).FirstOrDefault().Cek = "3";
	db.SaveChanges();
}
```

**PATTERN FOR ASYNC METHODS:**

BEFORE:
```csharp
public async Task<bool> DelTransH(int id)
{
	try
	{
		var ExistingTrans = _context.PoTransHs.Where(x => x.PoTransHId == id).FirstOrDefault();
		if (ExistingTrans != null)
		{
			_context.PoTransHs.Remove(ExistingTrans);
			await _context.SaveChangesAsync();
			return true;
		}
	}
	catch (Exception)
	{
		throw;
	}
	return false;
}
```

AFTER:
```csharp
public async Task<bool> DelTransH(int id)
{
	try
	{
		using var db = _context.CreateDbContext();
		var ExistingTrans = db.PoTransHs.Where(x => x.PoTransHId == id).FirstOrDefault();
		if (ExistingTrans != null)
		{
			db.PoTransHs.Remove(ExistingTrans);
			await db.SaveChangesAsync();
			return true;
		}
	}
	catch (Exception)
	{
		throw;
	}
	return false;
}
```

**PATTERN FOR MULTIPLE CONTEXTS IN ONE METHOD:**

BEFORE:
```csharp
public PoTransH AddTransH(PoTransHView trans)
{
	PoTransH transH = buildNewTransHeader();

	IcItem cekItem = _contextIc.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
	if (cekItem != null)
	{
		cekItem.HrgUsd = item.Harga;
		_contextIc.IcItems.Update(cekItem);
	}

	_context.PoTransHs.Add(transH);
	_context.SaveChanges();
	_contextIc.SaveChanges();

	return GetTransDoc(transH.NoLpb);
}
```

AFTER:
```csharp
public PoTransH AddTransH(PoTransHView trans)
{
	PoTransH transH = buildNewTransHeader();

	using (var db = _context.CreateDbContext())
	using (var dbIc = _contextIc.CreateDbContext())
	{
		IcItem cekItem = dbIc.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
		if (cekItem != null)
		{
			cekItem.HrgUsd = item.Harga;
			dbIc.IcItems.Update(cekItem);
		}

		db.PoTransHs.Add(transH);
		db.SaveChanges();
		dbIc.SaveChanges();
	}

	return GetTransDoc(transH.NoLpb);
}
```

### Step 3: Replace ALL `_contextXXX.` References with Local `dbXXX.` Variables

In method scopes with active `using var db...;` statements:
- Replace `_context.` → `db.`
- Replace `_contextAp.` → `dbAp.`
- Replace `_contextAr.` → `dbAr.`
- Replace `_contextIc.` → `dbIc.`
- Replace `_contextIR.` → `dbIR.`
- Replace `_contextOE.` → `dbOE.`
- Replace `_contextOR.` → `dbOR.`
- Replace `_contextAP.` → `dbAP.`

---

## Semi-Automated Script (PowerShell)

For services with **simpler, more uniform method structures**, you can use a PowerShell script to automate field/constructor updates:

```powershell
# Update field types
$content = $content -replace 'private readonly DbContextOrder _context;', 'private readonly IDbContextFactory<DbContextOrder> _context;'
$content = $content -replace 'private readonly DbContextPiutang _contextAr;', 'private readonly IDbContextFactory<DbContextPiutang> _contextAr;'
$content = $content -replace 'private readonly DbContextPersediaan _contextIc;', 'private readonly IDbContextFactory<DbContextPersediaan> _contextIc;'

# Update constructor parameter types
$content = $content -replace 'public OrderSalesServices\(DbContextOrder context, DbContextPiutang contextPiutang, DbContextPersediaan contextPersediaan\)', 'public OrderSalesServices(IDbContextFactory<DbContextOrder> context, IDbContextFactory<DbContextPiutang> contextPiutang, IDbContextFactory<DbContextPersediaan> contextPersediaan)'

Set-Content -Path "eSoft.Order\Services\OrderSalesServices.cs" -Value $content -Encoding UTF8
```

**WARNING:** This only handles field/constructor updates. You still must **manually wrap each method** with `using var db...;` statements by carefully reviewing the method body and replacing context references.

---

## Recommendations

### Priority Order (by complexity)
1. **OrderSalesServices** (636 lines, 3 contexts) - Good candidate, similar to OrderPurchaseServices
2. **OrderPurchaseServices** (488 lines, 3 contexts) - Slightly smaller, but still medium effort
3. **PurchaseServices** (1,087 lines) - Large; needs full systematic review
4. **LaporanStockServices** (2,406 lines, 6 contexts) - Cross-module reporting; very complex
5. **FinancialServices** (9,512 lines) - **DEFER** - Use AST-based refactoring tool or manual session

### Suggested Approach

**Option A: Manual Review (Safest)**
- Open the .cs file in Visual Studio
- For each public/private method:
  1. Check if it uses `_contextXXX.` references
  2. If yes, wrap the method body with `using var db = _contextXXX.CreateDbContext();`
  3. Replace all `_contextXXX.` with `dbXXX.` inside the using block
- Build and test after each method to catch errors early

**Option B: Script + Manual (If Confident)**
- Run PowerShell script to update field types and constructor
- Manually wrap each method with using statements
- Build and validate

**Option C: ReSharper/Rider Refactoring (If Available)**
- Use IDE refactoring tools to automate context variable replacements
- More reliable than regex-based scripts for complex code

---

## Validation After Each Service

```powershell
# Build solution
dotnet build D:\Project\BMA-PRO-NEW\BMA-PRO.sln

# If successful, run tests (if relevant)
dotnet test D:\Project\BMA-PRO-NEW\BMA-PRO.sln
```

**Do NOT commit/merge until build is fully green** - partial migrations can mask errors.

---

## Key Lessons Learned

1. **Automated scripts fail on complex/large files** - Method boundary detection is unreliable
2. **Manual approach works reliably** - Takes time but produces clean, verifiable code
3. **Smaller, simpler services migrate fastest** - Stick to <500 line files for automation
4. **Always validate build after each step** - Catch errors immediately

---

## Next Steps

1. ✅ Complete: 6 services (PaymentAr, PaymentAp, CashBank, Administration, ReceivableServices, PayableServices)
2. 📝 TODO: OrderSalesServices, OrderPurchaseServices (medium priority)
3. 📝 TODO: PurchaseServices (lower priority, larger)
4. 🔴 DEFER: LaporanStockServices, FinancialServices (schedule separate session with dedicated focus)

---

**Last Updated:** Session End - Token Budget Constraint
**Build Status:** ✅ GREEN (6/11 services complete)
