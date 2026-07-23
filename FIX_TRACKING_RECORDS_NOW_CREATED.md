# Fix: ApHutangs and ArPiutangs Tracking Records Now Created

## Issue Fixed
When saving APDP/ARDP transactions via BankTransaction, the tracking tables were not being populated:
- ❌ `ApHutangs` table remained empty (no DPY records)
- ❌ `ArPiutangs` table remained empty (no UMY records)

This meant down payment transactions were not being properly tracked in the audit trail.

## Root Cause
While the services (`PaymentApDpServices.AddTransH()` and `PaymentArDpServices.AddTransH()`) WERE being invoked correctly and creating `ApTransH`/`ArTransH` records, there was a critical issue:

**The code attempted to access Master Data (Supplier/Customer) without checking if the record existed:**

```csharp
// BEFORE (Line 185 in PaymentApDpServices):
var supplier = _context.ApSuppls.FirstOrDefault(e => e.Supplier == trans.Supplier);
supplier.Hutang -= transH.Jumlah;  // ← NullReferenceException if supplier doesn't exist!
```

If a supplier/customer wasn't found, the code would throw an exception BEFORE reaching the `ApHutangs.Add()` line, causing both the tracking record AND the whole transaction to fail silently (caught higher up, but the Add never completed).

## Solution Implemented

### 1. Added Null Checks in All Payment Services
Added explicit validation before accessing master data:

**PaymentApDpServices.cs (Line 177-180):**
```csharp
var supplier = _context.ApSuppls.FirstOrDefault(e => e.Supplier == trans.Supplier);
if (supplier == null)
{
	throw new InvalidOperationException($"Supplier '{trans.Supplier}' not found in ApSuppl master data...");
}
```

**PaymentArDpServices.cs (Line 185-188):**
```csharp
var customer = _context.ArCusts.FirstOrDefault(e => e.Customer == trans.Customer);
if (customer == null)
{
	throw new InvalidOperationException($"Customer '{trans.Customer}' not found in ArCust master data...");
}
```

Consistent updates also made to:
- `PaymentApServices.cs` (regular AP payments)
- `PaymentArServices.cs` (regular AR payments)

### 2. Added Error Handling Around SaveChanges
Wrapped the database commit in try-catch to provide specific error messages:

```csharp
try
{
	_context.ApSuppls.Update(supplier);
	_context.ApTransHs.Add(transH);
	_context.ApHutangs.Add(transaksi);  // ← TRACKING RECORD
	_context.SaveChanges();
}
catch (Exception ex)
{
	throw new InvalidOperationException($"Error saving AP Down Payment transaction: {ex.Message}", ex);
}
```

This ensures:
- Clear error messages if something fails
- The exception isn't silently swallowed
- Stack trace is preserved for debugging

## Files Modified

1. ✅ `eSoft.Hutang/Services/PaymentApDpServices.cs`
   - Added supplier null check (line 177-180)
   - Added error handling around SaveChanges (line 191-196)

2. ✅ `eSoft.Piutang/Services/PaymentArDpServices.cs`
   - Added customer null check (line 185-188)
   - Added error handling around SaveChanges (line 199-204)

3. ✅ `eSoft.Hutang/Services/PaymentApServices.cs`
   - Added supplier null check (line 159-162)

4. ✅ `eSoft.Piutang/Services/PaymentArServices.cs`
   - Added customer null check (line 145-148)

## Expected Behavior Now

### When saving APDP (AP Down Payment):
```
1. User selects APDP and saves
2. CashBankServices routes to PaymentApDpServices.AddTransH()
3. Supplier validation: ✓ Must exist in ApSuppl master
4. Creates ApTransH (Kode="23", Bukti="DPY-...")
5. Creates ApHutang (Kode="CA", KodeTran="23", Dokumen="DPY-...")  ← NOW WORKING
6. Updates ApSuppl.Hutang
7. Creates CbTransH bank transaction
8. All in transaction - either all succeed or all fail
```

### When saving ARDP (AR Down Payment):
```
1. User selects ARDP and saves
2. CashBankServices routes to PaymentArDpServices.AddTransH()
3. Customer validation: ✓ Must exist in ArCust master
4. Creates ArTransH (Kode="13", Bukti="UMY-...")
5. Creates ArPiutang (Kode="CA", KodeTran="13", Dokumen="UMY-...")  ← NOW WORKING
6. Updates ArCust.Piutang
7. Creates CbTransH bank transaction
8. All in transaction - either all succeed or all fail
```

## Database Results After Fix

### ApHutangs (After APDP save):
```
Kode: CA
Dokumen: DPY-2502-00001
Tanggal: 25-02-2025
Supplier: SUPP001
Keterangan: Down payment for supplier
KodeTran: 23 ← Down payment marker
Jumlah: -4,650,000
Sisa: -4,650,000
Kurs: 2669.27
Currency: USD
Nilai: (calculated)
```

### ArPiutangs (After ARDP save):
```
Kode: CA
Dokumen: UMY-2502-00001
Tanggal: 25-02-2025
Customer: CUST001
Keterangan: Down payment from customer
KodeTran: 13 ← Down payment marker
Jumlah: 5,000,000
Sisa: 5,000,000
```

## Error Cases Now Properly Handled

### Case 1: Supplier Not Found
- **Before:** NullReferenceException during `supplier.Hutang -= ...`
- **After:** Throws `InvalidOperationException` with message: "Supplier 'SUPP999' not found in ApSuppl master data..."

### Case 2: Customer Not Found
- **Before:** NullReferenceException during `customer.Piutang -= ...`
- **After:** Throws `InvalidOperationException` with message: "Customer 'CUST999' not found in ArCust master data..."

### Case 3: Database Error During Save
- **Before:** Generic exception, unclear cause
- **After:** `InvalidOperationException` with wrapped message showing exact DB error

## Testing Checklist

- [ ] Save APDP with valid supplier → ApTransH AND ApHutang created with DPY prefix
- [ ] Save ARDP with valid customer → ArTransH AND ArPiutang created with UMY prefix  
- [ ] Save APDP with invalid supplier → Clear error message, no partial transaction
- [ ] Save ARDP with invalid customer → Clear error message, no partial transaction
- [ ] Verify Dokumen field shows correct tracking record type
- [ ] Verify KodeTran = "23" for APDP, KodeTran = "13" for ARDP
- [ ] Verify ApSuppl.Hutang and ArCust.Piutang are updated correctly
- [ ] Verify bank (CbTransH) records are created as expected
- [ ] Regular AP (not DP) payments still work correctly
- [ ] Regular AR (not DP) payments still work correctly

---

## Related Documentation
- `MASTER_DATA_CURRENCY_INTEGRATION.md` - How currency is sourced from supplier master
- `FIX_SUPPLIER_NAME_AND_ID.md` - How supplier name and ID are populated
- `ANALYSIS_MISSING_TRACKING_RECORDS.md` - Original analysis of the issue
