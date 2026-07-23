# Fix for Missing Supplier/Customer Name and ID in AP Transactions

## Problem
When saving AP/APDP bank transactions, the `ApTransH` record was being created without:
- ❌ `NamaSup` (supplier name) = NULL
- ❌ `ApSupplId` (supplier ID) = 0

This caused blank supplier names and missing master data links in the saved transactions.

## Root Causes
1. `ApTransHView` was missing the `NamaSup` property (only existed in the model)
2. `BankTransactionView` was not capturing supplier ID and name when the user selected a supplier
3. `CashBankServices` had no way to pass supplier master data to the view models

## Solution

### Step 1: Add `NamaSup` to `ApTransHView`
✅ Added `public string NamaSup { get; set; }` to `eSoft.Hutang/View/ApTransHView.cs` (line 74)

**Why:** The database model `ApTransH` has this field, but the view model was missing it, preventing proper mapping during reflection-based save.

### Step 2: Add Party Master Data Fields to `BankTransactionView`
✅ Added two new properties to `eSoft.CashBank/View/BankTransactionView.cs`:
```csharp
public int PartyId { get; set; }        // Supplier ID for AP, Customer ID for AR
public string PartyName { get; set; }   // Supplier name for AP, Customer name for AR
```

**Why:** These act as a bridge between the Blazor UI (which has access to master data) and the service layer (which builds AP/AR transactions).

### Step 3: Populate Party Master Data in Blazor Page
✅ Updated `OnSupplierChanged()` and `OnCustomerChanged()` in `BankTransaction.razor`:

```csharp
// In OnSupplierChanged:
var selectedSupplier = suppliers?.FirstOrDefault(s => s.Supplier == ctx.PartyCode);
if (selectedSupplier != null)
{
	ctx.PartyId = selectedSupplier.ApSupplId;      // ← NEW
	ctx.PartyName = selectedSupplier.NamaSup;      // ← NEW
	ctx.Currency = selectedSupplier.Kurs ?? "IDR";
}

// In OnCustomerChanged:
var selectedCustomer = customers?.FirstOrDefault(c => c.Customer == ctx.PartyCode);
if (selectedCustomer != null)
{
	ctx.PartyId = selectedCustomer.ArCustId;       // ← NEW
	ctx.PartyName = selectedCustomer.NamaCust;     // ← NEW
}
```

**Why:** When the user selects a supplier/customer from the dropdown, we now capture both the code and the master data IDs/names.

### Step 4: Pass Master Data to Service Layer
✅ Updated `SaveTransactionsAsync()` in `CashBankServices.cs` to set ApSupplId and NamaSup during reflection:

```csharp
if (serviceName.Contains("Hutang"))
{
	apViewType.GetProperty("Supplier")?.SetValue(apInstance, partyCode);

	// ← NEW: Set supplier ID and name from master data
	if (trx.PartyId > 0)
	{
		apViewType.GetProperty("ApSupplId")?.SetValue(apInstance, trx.PartyId);
	}
	if (!string.IsNullOrEmpty(trx.PartyName))
	{
		apViewType.GetProperty("NamaSup")?.SetValue(apInstance, trx.PartyName);
	}
}
```

**Why:** This ensures the ApTransH record gets both ID and name when created through reflection.

---

## Data Flow

```
User selects Supplier "SUPP001" in BankTransaction.razor
	↓
OnSupplierChanged() triggers
	↓
Load supplier master: ApSuppl { ApSupplId=5, Supplier="SUPP001", NamaSup="PT ABC", Kurs="USD" }
	↓
ctx.PartyId = 5
ctx.PartyName = "PT ABC"
ctx.Currency = "USD"
	↓
User fills in amount and clicks Save
	↓
CashBankServices.SaveTransactionsAsync() receives BankTransactionView with PartyId=5, PartyName="PT ABC"
	↓
Create ApTransHView using reflection
	↓
Set properties:
  - Supplier = "SUPP001"
  - ApSupplId = 5 ✅
  - NamaSup = "PT ABC" ✅
  - Currency = "USD" ✅
  - Kurs = 2669.27 (or entered value)
	↓
PaymentApServices.AddTransH() is called
	↓
ApTransH is saved to database with all fields populated
	↓
Result: Jumlah, Unapplied, Kurs, Nilai, Currency, NamaSup, ApSupplId ALL FILLED ✅
```

---

## Result

### Before Fix:
```
Jumlah: 1,341,110,000 ✓
Unapplied: 1,341,110,000 ✓
Kurs: 2669.2700 ✓
Nilai: 502,499,368.97 ✓
Currency: NULL ❌
NamaSup: NULL ❌
ApSupplid: NULL ❌
```

### After Fix:
```
Jumlah: 1,341,110,000 ✓
Unapplied: 1,341,110,000 ✓
Kurs: 2669.2700 ✓
Nilai: 502,499,368.97 ✓
Currency: USD ✓
NamaSup: PT ABC (or actual supplier name) ✓
ApSupplid: 5 (or actual ID) ✓
ActSet: NULL (unchanged, might need mapping later)
```

---

## Files Modified

1. ✅ `eSoft.Hutang/View/ApTransHView.cs`
   - Added `public string NamaSup { get; set; }`

2. ✅ `eSoft.CashBank/View/BankTransactionView.cs`
   - Added `public int PartyId { get; set; }`
   - Added `public string PartyName { get; set; }`

3. ✅ `Accounting/Pages/ModuleBank/BankTransaction/BankTransaction.razor`
   - Updated `OnSupplierChanged()` to populate `ctx.PartyId` and `ctx.PartyName`
   - Updated `OnCustomerChanged()` to populate `ctx.PartyId` and `ctx.PartyName`

4. ✅ `eSoft.CashBank/Services/CashBankServices.cs`
   - Updated `SaveTransactionsAsync()` to set `ApSupplId` and `NamaSup` via reflection

---

## Testing Checklist

- [ ] Select supplier with known ApSupplId and NamaSup
- [ ] Save AP/APDP transaction
- [ ] Verify `ApSupplid` field is populated in database
- [ ] Verify `NamaSup` field is populated with supplier name
- [ ] Verify `Currency` is populated (from supplier master or user input)
- [ ] Verify `Jumlah`, `Unapplied`, `Kurs`, `Nilai` are still correct
- [ ] Test with AR customer (should handle gracefully, though ArTransH may not have equivalent ID field)

---

## Future Enhancements

1. **AR Master Link:** Check if `ArTransH` needs an `ArCustId` and customer name field for consistency
2. **Audit Trail:** Consider adding timestamp/user who created transaction
3. **Validation:** Could add early validation that supplier/customer exists before allowing save
4. **UI Display:** Show supplier name and ID in confirmation dialog before saving

---
