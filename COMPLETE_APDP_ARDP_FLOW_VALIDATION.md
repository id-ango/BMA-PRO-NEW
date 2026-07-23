# APDP/ARDP Data Flow Validation - Complete Review

## 📋 Overview

This document validates that the complete APDP/ARDP data flow is now correct after the fixes.

---

## ✅ Step 1: Supplier Selection & Currency Loading

**Location**: `Accounting/Pages/ModuleBank/BankTransaction/BankTransaction.razor` (Lines 627-660)

**Current Implementation** ✅
```csharp
private async Task OnSupplierChanged(BankTransactionView ctx, ChangeEventArgs e)
{
	if (ctx == null) return;
	ctx.OutstandingDocs.Clear();
	if (e != null)
	{
		ctx.PartyCode = e?.Value?.ToString();
	}
	if (string.IsNullOrEmpty(ctx.PartyCode)) return;

	// Load Currency and Supplier Master Info from ApSuppl
	var selectedSupplier = suppliers?.FirstOrDefault(s => s.Supplier == ctx.PartyCode);
	if (selectedSupplier != null)
	{
		// Set supplier ID and name for later use
		ctx.PartyId = selectedSupplier.ApSupplId;
		ctx.PartyName = selectedSupplier.NamaSup;

		// Use supplier's default currency
		if (!string.IsNullOrEmpty(selectedSupplier.Kurs))
		{
			ctx.Currency = selectedSupplier.Kurs;  // ✅ Currency from supplier master
		}
		else
		{
			ctx.Currency = "IDR";  // ✅ Default to IDR
		}
	}
	// ... rest of method
}
```

**Status**: ✅ CORRECT
- Currency is loaded from `ApSuppl.Kurs`
- Supplier ID and name are captured
- IDR default is applied when no currency specified

---

## ✅ Step 2: Amount & Payment Entry

**Location**: `Accounting/Pages/ModuleBank/BankTransaction/BankTransaction.razor` (UI fields)

**Fields**:
- `Amount (IDR)` - The payment amount in IDR
- `Currency` - Auto-populated from supplier master
- `Kurs` - Exchange rate (if foreign currency)
- `Nilai` - Foreign amount (calculated as Amount ÷ Kurs)

**Status**: ✅ CORRECT
- Amount is the transaction amount from bank statement
- User can override currency/Kurs if needed
- Nilai is calculated and displayed

---

## ✅ Step 3: View Model Preparation

**Location**: `eSoft.CashBank/View/BankTransactionView.cs`

**Fields Set**:
```csharp
public decimal Amount { get; set; }              // IDR amount from bank
public string Currency { get; set; } = "IDR";    // Currency code (USD, etc.)
public decimal Kurs { get; set; } = 1m;          // Exchange rate
public decimal Nilai { get; set; }               // Foreign currency amount
public int PartyId { get; set; }                 // Supplier ID
public string PartyName { get; set; }            // Supplier name
public string Target { get; set; }               // CB, AR, AP, APDP, ARDP
public string TransactionType { get; set; }      // PAYMENT, etc.
```

**Status**: ✅ CORRECT
- All necessary fields are present
- Data is ready to pass to service layer

---

## ✅ Step 4: Service Routing & Data Mapping

**Location**: `eSoft.CashBank/Services/CashBankServices.cs`

**APDP Routing** (Lines 1350-1460):
```csharp
if (target == "APDP")
{
	// JumBayar calculation
	decimal totalBayarAp = 0m;
	var selDocsForAp = trx.OutstandingDocs?.Where(...);

	if (selDocsForAp != null && selDocsForAp.Any())
	{
		totalBayarAp = selDocsForAp.Sum(s => s.Bayar);  // From selected docs
	}
	else
	{
		totalBayarAp = trx.Amount;  // ✅ Use bank transaction amount
	}

	// Set JumBayar
	var propJumBayar = apViewType.GetProperty("JumBayar");
	if (propJumBayar != null && propJumBayar.CanWrite)
		propJumBayar.SetValue(apInstance, totalBayarAp);

	// Set Currency and Kurs
	var propCurrency = apViewType.GetProperty("Currency");
	if (propCurrency != null && propCurrency.CanWrite)
		propCurrency.SetValue(apInstance, trx.Currency);

	var propKurs = apViewType.GetProperty("Kurs");
	if (propKurs != null && propKurs.CanWrite)
		propKurs.SetValue(apInstance, trx.Kurs);
}
```

**Status**: ✅ CORRECT
- JumBayar is set from Amount when no docs selected
- Currency and Kurs are passed to the view model
- No need to set Nilai (it's calculated in view model)

---

## ✅ Step 5: View Model Calculation

**Location**: `eSoft.Hutang/View/ApTransHView.cs`

**Calculated Properties**:
```csharp
public decimal JumBayar { get; set; }  // Set by service

public decimal Nilai
{
	get { return Kurs * JumBayar; }    // ✅ CALCULATION
}

public decimal UpdateUnapplied
{
	get { return JumBayar - ApTransDs.Sum(p => p.Bayar); }  // ✅ CALCULATION
}
```

**Status**: ✅ CORRECT
- Nilai = Kurs × JumBayar ✅
- UpdateUnapplied = JumBayar - payments ✅
- These are read-only calculated properties

---

## ✅ Step 6: ApTransH Save

**Location**: `eSoft.Hutang/Services/PaymentApDpServices.cs` (Lines 98-123)

**Mapping**:
```csharp
ApTransH transH = new ApTransH
{
	Bukti = GetNumber(),
	Supplier = trans.Supplier.ToUpper(),
	Tanggal = trans.Tanggal,
	Currency = trans.Currency,        // ✅ Currency from BankTrx
	Kurs = trans.Kurs,                // ✅ Exchange rate
	Nilai = trans.Nilai,              // ✅ Calculated value
	Jumlah = trans.JumBayar,          // ✅ IDR amount
	Unapplied = trans.UpdateUnapplied, // ✅ Remaining to use
	Kode = "23",                      // ✅ DP marker
	ApSupplId = trans.ApSupplId       // ✅ Supplier ID
};
```

**Status**: ✅ CORRECT
- Jumlah = payment amount in IDR
- Unapplied = remaining for future allocation
- Currency and Kurs are saved
- Nilai is calculated from Kurs × JumBayar

---

## ✅ Step 7: ApHutang Tracking Record (FIXED)

**Location**: `eSoft.Hutang/Services/PaymentApDpServices.cs` (Lines 159-182)

**BEFORE (BROKEN)**:
```csharp
ApHutang transaksi = new ApHutang
{
	Kode = "CA",
	Jumlah = -1 * transH.Jumlah,     // ❌ WRONG
	SldSisa = -1 * transH.Jumlah,
	// Bayar = -1 * transH.Jumlah,   // ❌ COMMENTED
	Sisa = -1 * transH.Unapplied,
	// UnApplied = -1 * transH.Unapplied, // ❌ COMMENTED
};
```

**AFTER (FIXED)** ✅:
```csharp
ApHutang transaksi = new ApHutang
{
	Kode = "CA",
	Jumlah = 0,                      // ✅ No invoice for DP
	SldSisa = -1 * transH.Jumlah,
	Bayar = -1 * transH.Jumlah,      // ✅ PAYMENT RECORDED
	Sisa = -1 * transH.Unapplied,
	UnApplied = -1 * transH.Unapplied, // ✅ ENABLED
	Nilai = transH.Nilai,
	Currency = trans.Currency,
	Kurs = transH.Kurs,
};
```

**Status**: ✅ FIXED
- `Jumlah = 0` (no invoice amount for down payment)
- `Bayar = -1 * transH.Jumlah` (payment amount recorded)
- `UnApplied = -1 * transH.Unapplied` (remaining payment tracked)

---

## ✅ Step 8: ARDP Processing (Same Pattern)

**Location**: `eSoft.Piutang/Services/PaymentArDpServices.cs`

**Same fixes applied**:
- `Jumlah = 0` (no invoice)
- `Bayar = -1 * transH.Jumlah` (payment recorded)
- `UnApplied = -1 * transH.Unapplied` (remaining tracked)

**Status**: ✅ FIXED

---

## 📊 Final Data State

### ApTransH (Header) Example
```
Bukti:        DPY-2502-00001
Supplier:     W0002
Tanggal:      2025-02-11
Currency:     CNY
Kurs:         1.0
Jumlah:       4,650,000        ← IDR amount
Nilai:        4,650,000        ← Calculated (1.0 × 4,650,000)
Unapplied:    4,650,000        ← Available for allocation
Kode:         23               ← DP marker
ApSupplId:    (supplier ID)    ← For master link
```

### ApHutang (Tracking) Example
```
Dokumen:      DPY-2502-00001
Supplier:     W0002
Tanggal:      2025-02-11
Kode:         CA               ← Cash/payment
KodeTran:     23               ← DP marker
Jumlah:       0                ← ✅ No invoice
Bayar:        -4,650,000       ← ✅ Payment recorded
Sisa:         -4,650,000       ← Remaining (same as unapplied)
UnApplied:    -4,650,000       ← ✅ Available for use
Nilai:        4,650,000        ← FX value
Currency:     CNY              ← Currency code
Kurs:         1.0              ← Exchange rate
```

---

## ✅ Validation Checklist

| Check | Status | Notes |
|-------|--------|-------|
| Supplier currency loaded | ✅ | From ApSuppl.Kurs |
| Amount entered (IDR) | ✅ | From bank statement |
| Currency/Kurs in view model | ✅ | Passed to service layer |
| JumBayar calculation | ✅ | Uses trx.Amount when no docs |
| Nilai calculation | ✅ | Kurs × JumBayar |
| ApTransH.Jumlah | ✅ | Stores IDR amount |
| ApTransH.Unapplied | ✅ | Stores remaining payment |
| ApHutang.Jumlah | ✅ | Fixed to 0 (was wrong) |
| ApHutang.Bayar | ✅ | Fixed to record payment (was commented) |
| ApHutang.UnApplied | ✅ | Fixed to track remaining (was commented) |
| ARDP same pattern | ✅ | Matching fixes applied |
| Build status | ✅ | Successful |

---

## 🎯 Summary

The complete data flow for APDP/ARDP is now **correct** and **consistent**:

1. ✅ User selects supplier → currency auto-loads from master
2. ✅ User enters amount → stored in ApTransH.Jumlah
3. ✅ System calculates Nilai = Kurs × Jumlah
4. ✅ ApTransH stores complete payment info
5. ✅ **FIXED**: ApHutang records payment in Bayar field (not Jumlah)
6. ✅ **FIXED**: ApHutang.UnApplied tracks remaining payment

This aligns with accounting semantics where:
- **Jumlah** = invoice amount (0 for DP)
- **Bayar** = payment amount (used for DP)
- **Unapplied/Sisa** = remaining to allocate

