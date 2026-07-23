# Fix: APDP/ARDP Payment Recording - Step by Step

## 📋 Summary

Fixed the data flow for APDP (AP Down Payment) and ARDP (AR Down Payment) payment recording. The issue was that payment amounts were being recorded in the wrong database field.

---

## 🔧 What Was Fixed

### Problem
When saving APDP/ARDP transactions:
- **Jumlah** field was storing the payment amount (WRONG - this is for invoice amounts)
- **Bayar** field was commented out and not recording the payment (WRONG - this should record the payment)

### Solution
For down payment tracking records (ApHutang and ArPiutng):
- **Jumlah** = 0 (no invoice amount involved in down payment)
- **Bayar** = -1 × payment amount (payment is recorded here)
- **UnApplied** = -1 × remaining payment amount

---

## 📝 Files Changed

### 1. **eSoft.Hutang/Services/PaymentApDpServices.cs** (Lines 159-182)

**Before:**
```csharp
var transaksi = new ApHutang
{
	Kode = "CA",
	Dokumen = transH.Bukti,
	Jumlah = -1 * transH.Jumlah,           // ❌ WRONG - invoice amount
	SldSisa = -1 * transH.Jumlah,
	// Bayar = -1 * transH.Jumlah,         // ❌ COMMENTED OUT - payment not recorded
	// UnApplied = -1 * transH.Unapplied,  // ❌ COMMENTED OUT
	Sisa = -1 * transH.Unapplied,
	...
};
```

**After:**
```csharp
var transaksi = new ApHutang
{
	Kode = "CA",
	Dokumen = transH.Bukti,
	Jumlah = 0,                              // ✅ No invoice amount
	SldSisa = -1 * transH.Jumlah,
	Bayar = -1 * transH.Jumlah,              // ✅ PAYMENT RECORDED
	UnApplied = -1 * transH.Unapplied,       // ✅ ENABLED
	Sisa = -1 * transH.Unapplied,
	...
};
```

### 2. **eSoft.Piutang/Services/PaymentArDpServices.cs** (Lines 158-180)

**Before:**
```csharp
var transaksi = new ArPiutng
{
	Kode = "CA",
	Dokumen = transH.Bukti,
	Jumlah = -1 * transH.Jumlah,           // ❌ WRONG - invoice amount
	SldSisa = -1 * transH.Jumlah,
	Discount = 0,
	Sisa = -1 * transH.Unapplied,
	// Bayar = -1 * transH.Jumlah,         // ❌ COMMENTED OUT
	// UnApplied = -1 * transH.Unapplied,  // ❌ COMMENTED OUT
	...
};
```

**After:**
```csharp
var transaksi = new ArPiutng
{
	Kode = "CA",
	Dokumen = transH.Bukti,
	Jumlah = 0,                              // ✅ No invoice amount
	SldSisa = -1 * transH.Jumlah,
	Bayar = -1 * transH.Jumlah,              // ✅ PAYMENT RECORDED
	UnApplied = -1 * transH.Unapplied,       // ✅ ENABLED
	Sisa = -1 * transH.Unapplied,
	...
};
```

---

## 📊 Data Flow After Fix

### Before (BROKEN):
```
BankTransaction (IDR 4,650,000)
	↓
ApTransH
├─ Jumlah: 4,650,000 ✅ correct
├─ Unapplied: 4,650,000 ✅ correct
└─ Nilai: 300 × 15,500 = 4,650,000 ✅ correct

ApHutang (tracking)
├─ Jumlah: -4,650,000 ❌ WRONG (invoice amount field)
├─ Bayar: empty ❌ WRONG (payment not recorded)
├─ Sisa: -4,650,000 ✅ correct
└─ UnApplied: empty ❌ WRONG (not tracked)
```

### After (CORRECT):
```
BankTransaction (IDR 4,650,000)
	↓
ApTransH
├─ Jumlah: 4,650,000 ✅ correct
├─ Unapplied: 4,650,000 ✅ correct
└─ Nilai: 300 × 15,500 = 4,650,000 ✅ correct

ApHutang (tracking)
├─ Jumlah: 0 ✅ correct (no invoice)
├─ Bayar: -4,650,000 ✅ PAYMENT RECORDED
├─ Sisa: -4,650,000 ✅ remaining payment
└─ UnApplied: -4,650,000 ✅ tracked
```

---

## ✅ Verification

- **Build Status**: ✅ Successful
- **Changes Applied**: 
  - PaymentApDpServices.cs ✅
  - PaymentArDpServices.cs ✅
- **Related Components** (already correct):
  - CashBankServices.cs: JumBayar calculation ✅
  - ApTransHView.cs: Nilai calculation ✅
  - BankTransaction.razor: Supplier currency loading ✅

---

## 🎯 Impact

### APDP (AP Down Payment)
When user creates down payment to supplier:
- ✅ Currency is auto-loaded from supplier master data
- ✅ Amount is saved to ApTransH.Jumlah
- ✅ Nilai is calculated as Kurs × Jumlah
- ✅ ApHutang tracking correctly records payment in Bayar field

### ARDP (AR Down Payment)
When user receives down payment from customer:
- ✅ Amount is saved to ArTransH.Jumlah
- ✅ ArPiutang tracking correctly records payment in Bayar field

---

## 🔍 Test Scenario

**Example: APDP to Supplier Wuhan (CNY currency)**

```
User Actions:
1. Select Target: APDP
2. Select Supplier: [W0002] WUHAN ZHU HO (CNY)
   → Currency auto-set to: CNY
   → Supplier ID: automatically loaded
3. Enter Amount (IDR): 4,650,000

Expected Result in Database:

ApTransH:
├─ Bukti: DPY-2502-00001
├─ Supplier: W0002
├─ Jumlah: 4,650,000
├─ Unapplied: 4,650,000
├─ Currency: CNY
├─ Kurs: 1.0 (or exchange rate)
└─ Nilai: 4,650,000

ApHutang:
├─ Dokumen: DPY-2502-00001
├─ Supplier: W0002
├─ Jumlah: 0          ← No invoice
├─ Bayar: -4,650,000  ← Payment recorded
├─ Sisa: -4,650,000   ← Remaining to use
├─ UnApplied: -4,650,000
├─ Nilai: 4,650,000
└─ Currency: CNY
```

---

## 📌 Notes

- Currency code is sourced from `ApSuppl.Kurs` field (supplier master)
- For AR/ARDP, currency is always IDR (no master currency field exists)
- The fix aligns with accounting semantics: down payments should be tracked as payments (Bayar), not invoices (Jumlah)
- Build verified and successful on .NET 10

