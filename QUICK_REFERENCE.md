# DbContext Factory Migration - Quick Reference

## The 3-Step Pattern (Memorize This!)

### STEP 1: Fields & Constructor
```csharp
// Change THESE THREE LINES:
private readonly DbContextOrder _context;
private readonly DbContextHutang _contextAp;
private readonly DbContextPersediaan _contextIc;

// TO THIS:
private readonly IDbContextFactory<DbContextOrder> _context;
private readonly IDbContextFactory<DbContextHutang> _contextAp;
private readonly IDbContextFactory<DbContextPersediaan> _contextIc;

// Update constructor signature from:
public MyService(DbContextOrder ctx, DbContextHutang ctxAp, DbContextPersediaan ctxIc)

// TO:
public MyService(IDbContextFactory<DbContextOrder> ctx, IDbContextFactory<DbContextHutang> ctxAp, IDbContextFactory<DbContextPersediaan> ctxIc)

// Constructor body STAYS THE SAME:
_context = ctx;
_contextAp = ctxAp;
_contextIc = ctxIc;
```

### STEP 2: Wrap Every Method with `using`

**SimpleQuery Method:**
```csharp
// BEFORE
public PoTransH GetPoTrans(int id)
{
	return _context.PoTransHs.Include(p => p.PoTransDs).Where(x => x.PoTransHId == id).FirstOrDefault();
}

// AFTER
public PoTransH GetPoTrans(int id)
{
	using var db = _context.CreateDbContext();
	return db.PoTransHs.Include(p => p.PoTransDs).Where(x => x.PoTransHId == id).FirstOrDefault();
}
```

**UpdateMethod:**
```csharp
// BEFORE
public void SaveOrder(string customer)
{
	_context.PoTransHs.Where(x => x.NoLpb == customer).FirstOrDefault().Cek = "3";
	_context.SaveChanges();
}

// AFTER
public void SaveOrder(string customer)
{
	using var db = _context.CreateDbContext();
	db.PoTransHs.Where(x => x.NoLpb == customer).FirstOrDefault().Cek = "3";
	db.SaveChanges();
}
```

**AsyncMethod:**
```csharp
// BEFORE
public async Task<bool> DeleteOrder(int id)
{
	var existing = _context.PoTransHs.Where(x => x.PoTransHId == id).FirstOrDefault();
	_context.PoTransHs.Remove(existing);
	await _context.SaveChangesAsync();
	return true;
}

// AFTER
public async Task<bool> DeleteOrder(int id)
{
	using var db = _context.CreateDbContext();
	var existing = db.PoTransHs.Where(x => x.PoTransHId == id).FirstOrDefault();
	db.PoTransHs.Remove(existing);
	await db.SaveChangesAsync();
	return true;
}
```

**MultipleContexts:**
```csharp
// BEFORE
public void AddOrder(OrderView order)
{
	var item = _contextIc.IcItems.Where(x => x.ItemCode == order.ItemCode).FirstOrDefault();
	_context.Orders.Add(new Order { ... });
	_context.SaveChanges();
	_contextIc.SaveChanges();
}

// AFTER
public void AddOrder(OrderView order)
{
	using (var db = _context.CreateDbContext())
	using (var dbIc = _contextIc.CreateDbContext())
	{
		var item = dbIc.IcItems.Where(x => x.ItemCode == order.ItemCode).FirstOrDefault();
		db.Orders.Add(new Order { ... });
		db.SaveChanges();
		dbIc.SaveChanges();
	}
}
```

### STEP 3: Replace Variables
Inside each `using` block, replace:
- `_context.` → `db.`
- `_contextAp.` → `dbAp.`
- `_contextAr.` → `dbAr.`
- `_contextIc.` → `dbIc.`
- `_contextIR.` → `dbIR.`
- `_contextOE.` → `dbOE.`
- (repeat for all context fields)

---

## Context Names Reference

Used in Order Services:
- `_context` → `DbContextOrder` → variable: `db`
- `_contextAp` → `DbContextHutang` → variable: `dbAp`
- `_contextAr` → `DbContextPiutang` → variable: `dbAr`
- `_contextIc` → `DbContextPersediaan` → variable: `dbIc`

Used in LaporanStock:
- `_context` → `DbContextPersediaan` → variable: `db`
- `_contextIR` → `DbContextBeli` → variable: `dbIR`
- `_contextOE` → `DbContextJual` → variable: `dbOE`
- `_contextAR` → `DbContextPiutang` → variable: `dbAR`
- `_contextOR` → `DbContextOrder` → variable: `dbOR`
- `_contextAP` → `DbContextHutang` → variable: `dbAP`

---

## Workflow Checklist

- [ ] Open Visual Studio
- [ ] Open target service file (e.g., `OrderPurchaseServices.cs`)
- [ ] Make STEP 1 changes (field types + constructor)
- [ ] Run `dotnet build` - should still fail with method errors
- [ ] Pick first public method
- [ ] Add `using var db = _context.CreateDbContext();` at start
- [ ] Replace all `_context.` with `db.` in that method
- [ ] Repeat for next method
- [ ] Every 5-10 methods: run `dotnet build`
- [ ] If build succeeds: 🎉 Commit with git
- [ ] If build fails: Check last edited method for mistakes
- [ ] Continue until all methods done
- [ ] Final `dotnet build` and test

---

## Common Mistakes to Avoid

❌ **WRONG:** Changing field type but forgetting to add `using` to method
```csharp
// This will NOT compile:
private readonly IDbContextFactory<DbContextOrder> _context;
public Result GetX() {
	return _context.Items.ToList();  // ERROR! Factory has no Items property
}
```

✅ **RIGHT:**
```csharp
private readonly IDbContextFactory<DbContextOrder> _context;
public Result GetX() {
	using var db = _context.CreateDbContext();
	return db.Items.ToList();  // OK! db is the actual context
}
```

---

❌ **WRONG:** Nested using without closing outer context
```csharp
using (var db = _context.CreateDbContext())  // UNCLOSED!
{
	using (var dbIc = _contextIc.CreateDbContext())
	{
		// ...
	}
}  // <- Missing closing brace
```

✅ **RIGHT:**
```csharp
using (var db = _context.CreateDbContext())
using (var dbIc = _contextIc.CreateDbContext())
{
	// Both contexts available here
	db.Items.Add(...);
	dbIc.Items.Update(...);
}  // Both disposed here
```

---

## Build Command

```powershell
cd D:\Project\BMA-PRO-NEW
dotnet build BMA-PRO.sln
```

Or in Visual Studio: `Ctrl+Shift+B`

---

## Git Workflow

```powershell
# Before starting
git pull origin Ver10.update5

# After completing a service
git add eSoft.Order\Services\OrderSalesServices.cs
git commit -m "refactor: migrate OrderSalesServices to IDbContextFactory<T>"
git push origin Ver10.update5
```

---

## Services Left to Migrate

| # | Service | Lines | Time | Status |
|---|---------|-------|------|--------|
| 1 | OrderSalesServices | 636 | 3h | Ready |
| 2 | OrderPurchaseServices | 488 | 2h | Ready |
| 3 | PurchaseServices | 1087 | 4h | Next queue |
| 4 | LaporanStockServices | 2406 | 6h | Defer (complex) |
| 5 | FinancialServices | 9512 | 8h | Defer (huge) |

---

## PowerShell One-Liner for Field Updates

If you want to automate just the field/constructor part (still need manual method wrapping):

```powershell
$file = "eSoft.Order\Services\OrderSalesServices.cs"
$content = Get-Content $file -Raw
$content = $content -replace 'private readonly DbContextOrder _context;', 'private readonly IDbContextFactory<DbContextOrder> _context;'
$content = $content -replace 'private readonly DbContextPiutang _contextAr;', 'private readonly IDbContextFactory<DbContextPiutang> _contextAr;'
$content = $content -replace 'private readonly DbContextPersediaan _contextIc;', 'private readonly IDbContextFactory<DbContextPersediaan> _contextIc;'
$content = $content -replace 'public OrderSalesServices\(DbContextOrder context, DbContextPiutang contextPiutang, DbContextPersediaan contextPersediaan\)', 'public OrderSalesServices(IDbContextFactory<DbContextOrder> context, IDbContextFactory<DbContextPiutang> contextPiutang, IDbContextFactory<DbContextPersediaan> contextPersidiaan)'
Set-Content -Path $file -Value $content -Encoding UTF8
Write-Host "Fields and constructor updated. Now wrap each method manually!"
```

**WARNING:** Still must manually wrap every method. Script can't do this reliably.

---

Good luck! Follow the pattern. You've got this. 💪
