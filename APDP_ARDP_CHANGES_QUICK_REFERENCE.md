# Quick Reference: APDP/ARDP Changes

## Changes Summary

Two files were modified to fix APDP/ARDP payment recording:

### 1. PaymentApDpServices.cs
**File**: `eSoft.Hutang/Services/PaymentApDpServices.cs` (Lines 159-182)

**Change**:
```csharp
// Old:
Jumlah = -1 * transH.Jumlah,    // WRONG
// Bayar = -1 * transH.Jumlah,  // COMMENTED OUT

// New:
Jumlah = 0,                      // ✅ No invoice for DP
Bayar = -1 * transH.Jumlah,      // ✅ Payment recorded
UnApplied = -1 * transH.Unapplied, // ✅ Enabled
```

### 2. PaymentArDpServices.cs
**File**: `eSoft.Piutang/Services/PaymentArDpServices.cs` (Lines 158-180)

**Change**:
```csharp
// Old:
Jumlah = -1 * transH.Jumlah,    // WRONG
// Bayar = -1 * transH.Jumlah,  // COMMENTED OUT

// New:
Jumlah = 0,                      // ✅ No invoice for DP
Bayar = -1 * transH.Jumlah,      // ✅ Payment recorded
UnApplied = -1 * transH.Unapplied, // ✅ Enabled
```

---

## What Did This Fix?

### Before
- ApHutang.Jumlah = payment amount ❌ (wrong field)
- ApHutang.Bayar = empty ❌ (not recorded)
- Payment not properly tracked

### After
- ApHutang.Jumlah = 0 ✅ (no invoice for DP)
- ApHutang.Bayar = payment amount ✅ (correct field)
- Payment properly recorded and tracked

---

## Data Already Working (No Changes)

✅ Supplier currency loading (OnSupplierChanged)
✅ Amount field mapping (CashBankServices)
✅ Nilai calculation (ApTransHView.cs)
✅ JumBayar calculation (CashBankServices)
✅ ApTransH.Jumlah saving (PaymentApDpServices)
✅ ApTransH.Unapplied saving (PaymentApDpServices)

---

## Test Example: APDP Invoice

```
Admin Transaction → Bank Transaction Screen

1. Target: APDP (selected)
2. Supplier: WUHAN ZHU HO (CNY) [auto-sets currency to CNY]
3. Amount (IDR): 4,650,000
4. Currency: CNY [auto-filled]
5. Kurs: 1.0 [or whatever exchange rate]
6. Save

Result in Database:

ApTransH:
  - Jumlah: 4,650,000 ✅
  - Unapplied: 4,650,000 ✅
  - Nilai: 4,650,000 ✅
  - Currency: CNY ✅
  - Kurs: 1.0 ✅

ApHutang:
  - Jumlah: 0 ✅ (no invoice)
  - Bayar: -4,650,000 ✅ (payment recorded)
  - UnApplied: -4,650,000 ✅ (available)
```

---

## Build Status

✅ **Build Successful**

All changes compile without errors and the solution builds successfully.

---

## Related Files (No Changes Needed)

- `BankTransaction.razor` - Already loads supplier currency ✅
- `CashBankServices.cs` - Already calculates JumBayar correctly ✅
- `BankTransactionView.cs` - Already has all needed fields ✅
- `ApTransHView.cs` - Already has Nilai calculation ✅
- `ArTransHView.cs` - Already correct ✅

---

## Key Takeaway

**Down payments should record in payment fields (Bayar), not invoice fields (Jumlah).**

This semantic fix ensures the accounting system properly tracks down payment amounts and allows future allocation to actual invoices.

