# Comparison Matrix: AR DP vs AP DP vs Regular Bank Transaction

## 1. FEATURE COMPARISON TABLE

```
┌─────────────────────────────┬──────────────────┬──────────────────┬──────────────────┐
│ FEATURE / ASPECT            │  AR DownPayment  │  AP DownPayment  │ Regular Bank Trx │
├─────────────────────────────┼──────────────────┼──────────────────┼──────────────────┤
│ Service Class               │ PaymentArDpSvc   │ PaymentApDpSvc   │ CashBankServices │
│ Header Table                │ ArTransH         │ ApTransH         │ CbTransH         │
│ Aging/Sisa Table            │ ArPiutng         │ ApHutang         │ -                │
│ Master Table                │ ArCust           │ ApSuppl          │ CbBanks          │
│ Transaction Code (Kode)     │ "13"             │ "23"             │ -                │
│ Aging Code                  │ "CA"             │ "CA"             │ -                │
│ DocNo Format                │ UMY-yy2MM-nnnnn │ DPY-yy5MM-nnnnn │ custm (flexible) │
│ Currency Support            │ ❌ No           │ ✅ Yes          │ ✅ Yes           │
│ Multi-amount (Asing+IDR)   │ ❌ No           │ ✅ Yes          │ ✅ Yes           │
│ Outstanding Doc Selection   │ ❌ No           │ ✅ Yes          │ Optional         │
│ Auto Bank Mirror (CbTransH) │ ✅ Yes (AR)     │ ✅ Yes (AP)     │ ✅ Yes           │
│ Party Type                  │ Customer         │ Supplier         │ N/A              │
│ Balance Field               │ Piutang          │ Hutang           │ Saldo            │
│ Can Apply to Invoice        │ ✅ Yes          │ ✅ Yes          │ N/A              │
│ Entry Point                 │ AR Module        │ AP Module        │ Bank Module      │
│ Alternative Entry           │ BankTransaction  │ BankTransaction  │ BankTransaction  │
└─────────────────────────────┴──────────────────┴──────────────────┴──────────────────┘
```

---

## 2. DATABASE TABLE CREATION MATRIX

```
┌──────────────────────────────────────────────────────────────────────────┐
│                     WHICH TABLES ARE CREATED?                           │
├──────────────────────┬──────────────────┬──────────────────┬────────────┤
│ Database / Table     │ AR DP            │ AP DP            │ Bank Only  │
├──────────────────────┼──────────────────┼──────────────────┼────────────┤
│ eSoft.Piutang:       │                  │                  │            │
│ ├─ ArTransH          │ ✅ Created       │ ❌ No            │ ❌ No      │
│ ├─ ArTransD          │ Optional         │ ❌ No            │ ❌ No      │
│ ├─ ArPiutng (Aging)  │ ✅ Created       │ ❌ No            │ ❌ No      │
│ └─ ArCust (Updated)  │ ✅ Piutang -= X  │ ❌ No            │ ❌ No      │
│                      │                  │                  │            │
│ eSoft.Hutang:        │                  │                  │            │
│ ├─ ApTransH          │ ❌ No            │ ✅ Created       │ ❌ No      │
│ ├─ ApTransD          │ ❌ No            │ Optional         │ ❌ No      │
│ ├─ ApHutang (Aging)  │ ❌ No            │ ✅ Created       │ ❌ No      │
│ └─ ApSuppl (Updated) │ ❌ No            │ ✅ Hutang -= X   │ ❌ No      │
│                      │                  │                  │            │
│ eSoft.CashBank:      │                  │                  │            │
│ ├─ CbTransH          │ ✅ Auto mirror   │ ✅ Auto mirror   │ ✅ Created │
│ ├─ CbTransD          │ ✅ Created       │ ✅ Created       │ ✅ Created │
│ └─ CbBanks (Updated) │ ✅ Saldo += X    │ ✅ Saldo -= X    │ ✅ Updated │
│                      │    KSaldo += 0   │    KSaldo ±= X   │ ✅ Updated │
└──────────────────────┴──────────────────┴──────────────────┴────────────┘

KEY: ✅ = Created/Modified, ❌ = Not involved, Optional = Conditional
```

---

## 3. DATA FLOW SEQUENCE

```
┌────────────────────────────────────────────────────────────────┐
│                    DATA FLOW COMPARISON                        │
└────────────────────────────────────────────────────────────────┘

AR DOWNPAYMENT FLOW:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

User Input (BankTransaction)
	└─ Target: "AR"
	└─ Customer: CUST001
	└─ Amount: 5,000,000
		 │
		 ↓
SaveTransactionsAsync()
	└─ Detect Target == "AR"
		 │
		 ↓
PaymentArDpServices.AddTransH()
	├─ Generate: UMY-240115-00001
	├─ Create ArTransH (Kode="13")
	├─ Create ArPiutng (Kode="CA", Sisa=-5M)
	├─ Update ArCust (Piutang -= 5M)
	├─ Save to eSoft.Piutang
	│
	└─ Create CbTransH (mirror)
	   ├─ DocNo: UMY-240115-00001
	   ├─ SrcCode: "AR"
	   ├─ Terima: 5,000,000
	   ├─ Create CbTransD
	   └─ Update CbBanks (Saldo += 5M, KSaldo += 0)

		 ↓
RESULT ✅
	├─ ArTransH: 1 row
	├─ ArPiutng: 1 row
	├─ ArCust: 1 updated
	└─ CbTransH: 1 row (with CbTransD)


AP DOWNPAYMENT FLOW:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

User Input (BankTransaction)
	└─ Target: "AP"
	└─ Supplier: SUPP001
	└─ Amount: 300 USD = 4,650,000 IDR (@ 15,500)
		 │
		 ↓
SaveTransactionsAsync()
	└─ Detect Target == "AP"
		 │
		 ↓
PaymentApDpServices.AddTransH()
	├─ Generate: DPY-24501-00001
	├─ Create ApTransH (Kode="23", Currency="USD", Kurs=15500, Nilai=300)
	├─ Create ApHutang (Kode="CA", Sisa=-4,650,000)
	├─ Update ApSuppl (Hutang -= 4,650,000)
	├─ Save to eSoft.Hutang
	│
	└─ Create CbTransH (mirror, DUAL-AMOUNT)
	   ├─ DocNo: DPY-24501-00001
	   ├─ SrcCode: "AP"
	   ├─ Saldo: -300 (USD)
	   ├─ KSaldo: -4,650,000 (IDR)
	   ├─ Create CbTransD with:
	   │  ├─ KTerima: 4,650,000 (IDR returned)
	   │  ├─ Terima: 300 (USD)
	   │  └─ KValue: 15,500
	   └─ Update CbBanks (Saldo -= 300, KSaldo -= 4,650,000)

		 ↓
RESULT ✅
	├─ ApTransH: 1 row (with Currency info)
	├─ ApHutang: 1 row
	├─ ApSuppl: 1 updated
	└─ CbTransH: 1 row (with dual-amount CbTransD)


REGULAR BANK TRANSACTION FLOW:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

User Input (BankTransaction)
	└─ Target: (blank)
	└─ Amount: 2,500,000
		 │
		 ↓
SaveTransactionsAsync()
	└─ Detect Target == blank (not AP/AR)
		 │
		 ↓
Create CbTransH directly
	├─ Generate: BT-240115-0001
	├─ Create CbTransH
	├─ Create CbTransD
	├─ Save to eSoft.CashBank
	└─ Update CbBanks (Saldo ±= 2,500,000)

		 ↓
RESULT ✅
	├─ CbTransH: 1 row (default bank transaction)
	└─ CbBanks: 1 updated

	❌ NO ArTransH, ArPiutng, ApTransH, ApHutang created
```

---

## 4. PAYMENT APPLICATION MATRIX

```
┌────────────────────────────────────────────────────────────────┐
│        CAN DOWN PAYMENT BE APPLIED TO FUTURE INVOICE?         │
└────────────────────────────────────────────────────────────────┘

AR DownPayment Flow:
━━━━━━━━━━━━━━━━━━━

Step 1: Create DP (as shown above)
		ArTransH (Kode="13") created with Unapplied = 5,000,000

Step 2: When Invoice arrives
		Sales (Invoice) creates ArTransH (Kode="11" or similar)
		with Piutang amount = 7,000,000

Step 3: Payment/Collection
		User applies DP against Invoice:
		├─ 5,000,000 from DP (Unapplied)
		├─ 2,000,000 from new payment
		└─ Invoice fully paid

RESULT: ArPiutng aging shows:
		├─ Original DP: Applied ✓
		└─ Invoice: Fully collected ✓


AP DownPayment Flow:
━━━━━━━━━━━━━━━━━━━

Step 1: Create DP (as shown above)
		ApTransH (Kode="23") created with Unapplied = 4,650,000 IDR + 300 USD

Step 2: When PO/Invoice arrives
		Purchase (Hutang) creates ApTransH (Kode="21" or similar)
		with Hutang amount = USD 400 = 6,200,000 IDR (@ 15,500)

Step 3: Payment against Invoice
		User applies DP against Hutang:
		├─ 300 USD (4,650,000 IDR) from DP (Unapplied)
		├─ 100 USD (1,550,000 IDR) from new payment
		└─ Invoice fully paid

RESULT: ApHutang aging shows:
		├─ Original DP: Applied ✓
		└─ Invoice: Fully paid ✓
```

---

## 5. CURRENCY HANDLING COMPARISON

```
┌────────────────────────────────────────────────────────────────┐
│           MULTI-CURRENCY SUPPORT IN DETAIL                    │
└────────────────────────────────────────────────────────────────┘

AR DownPayment (NO Currency Support):
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Input:  5,000,000 (IDR only)
		└─ No Currency field

ArTransH fields:
├─ Jumlah:    5,000,000
├─ Currency:  (not stored)
├─ Kurs:      (not stored)
└─ Nilai:     (not stored)

CbTransH created:
├─ Saldo:     5,000,000
├─ KSaldo:    0 (or bank default)
└─ CbTransD:
   ├─ Terima: 5,000,000
   └─ KTerima: 0


AP DownPayment (FULL Currency Support):
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Input:  300 USD @ Exchange Rate 15,500
		├─ Currency:  "USD"
		├─ Nilai:     300 (foreign amount)
		├─ Kurs:      15,500 (exchange rate)
		└─ JumBayar:  4,650,000 (IDR equivalent)

ApTransH fields:
├─ Jumlah:     4,650,000 (IDR)
├─ Currency:   "USD" ← Stored!
├─ Kurs:       15,500 ← Stored!
└─ Nilai:      300 ← Foreign amount stored!

CbTransH created (DUAL-AMOUNT):
├─ Saldo:      -300 (Foreign currency balance)
├─ KSaldo:     -4,650,000 (IDR balance)
└─ CbTransD:  
   ├─ Terima:  300 (Foreign)
   ├─ KTerima: 4,650,000 (IDR)
   ├─ Bayar:   0
   ├─ KBayar:  0
   ├─ KValue:  15,500 ← Exchange rate stored!
   └─ Kurs:    (bank rate)

RESULT: Can track USD position separately!
		├─ USD reserve: 300
		└─ IDR cost: 4,650,000


Regular Bank Transaction (Flexible):
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Input:  Transaction amount + optional currency

CbTransH created:
├─ Saldo:      ±X (base currency)
├─ KSaldo:     ±Y (optional foreign)
└─ CbTransD:
   ├─ Terima:  X
   ├─ KTerima: Y
   └─ Jumlah:  X

RESULT: Simple or dual-amount as needed
```

---

## 6. INVOICE MATCHING / OUTSTANDING DOCS

```
┌────────────────────────────────────────────────────────────────┐
│         OUTSTANDING DOCUMENTS / INVOICE SELECTION             │
└────────────────────────────────────────────────────────────────┘

AR DownPayment:
━━━━━━━━━━━━━━

OutstandingDocs: ❌ NOT supported
├─ Reason: DP is prepayment (no invoice yet)
├─ Later: Can be applied during invoice payment
└─ State: Stored in Unapplied field of ArTransH


AP DownPayment:
━━━━━━━━━━━━━━

OutstandingDocs: ✅ FULLY supported
├─ User can select POs/Invoices to partially pay
├─ Amount split between selected invcs
└─ Each document has:
   ├─ Bayar: payment amount
   ├─ Discount: discount amount
   └─ Sisa: remaining balance

FORMAT: List<OutstandingDocView> from CSV or UI
		├─ DocNo: PO number
		├─ Tanggal: invoice date
		├─ Jumlah: invoice total
		├─ Bayar: amount to pay (<=Jumlah)
		├─ Discount: discount (<=Jumlah)
		└─ Sisa: Jumlah - Bayar - Discount


Regular Bank Transaction:
━━━━━━━━━━━━━━━━━━━━━━━━

OutstandingDocs: ✅ OPTIONAL (for AP/AR only)
├─ If specified: Used to match invoice
├─ If empty: General transaction
└─ Not required for plain bank transactions


PROCESSING LOGIC IN SaveTransactionsAsync:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

For AP DP:
```csharp
var selDocs = trx.OutstandingDocs
	?.Where(d => d.IsSelected || d.Bayar > 0 || d.Discount > 0)
	.ToList();

if (selDocs != null && selDocs.Any()) {
	// User selected specific docs
	totalBayar = selDocs.Sum(s => s.Bayar);
	totalDiscount = selDocs.Sum(s => s.Discount);
} else {
	// No docs selected = prepayment (DP mode!)
	totalBayar = trx.Amount;
	totalDiscount = 0;
}
```
```

---

## 7. WHEN TO USE EACH TYPE

```
┌────────────────────────────────────────────────────────────────┐
│           DECISION TREE: WHICH TRANSACTION TYPE?              │
└────────────────────────────────────────────────────────────────┘

START: User wants to record a bank transaction

Q1: Is it related to AP/AR?
├─ NO → Use Regular Bank Transaction ✓
└─ YES → Q2

Q2: Payment against what?
├─ KNOWN INVOICES/POs → Q3
└─ PREPAYMENT (no docs, or docs unknown) → Q4

Q3: Is it full/partial against selected docs?
├─ YES → Use AP/AR PAYMENT (Regular)
└─ PARTIALLY or NO DOCS → Q4

Q4: What party?
├─ CUSTOMER (prepayment) → Use AR DownPayment ✓
├─ SUPPLIER (prepayment) → Use AP DownPayment
│    └─ If multi-currency? → ✅ Full support!
└─ OTHER → Use Regular Bank Transaction

RESULTS:
┌─────────────────────┬─────────────────────────┐
│ SCENARIO            │ RECOMMENDED TYPE        │
├─────────────────────┼─────────────────────────┤
│ Bayar Invoice AP    │ PaymentApServices       │
│ Bayar Invoice AR    │ PaymentArServices       │
│ Prepay Supplier     │ PaymentApDpServices ✓   │
│ Prepay Customer     │ PaymentArDpServices ✓   │
│ Prepay (USD)        │ PaymentApDpServices ✅  │
│ Bank receipts       │ Regular Bank Trans      │
│ Bank transfers      │ Regular Bank Trans      │
│ Reconciliation      │ Regular Bank Trans      │
└─────────────────────┴─────────────────────────┘
```

---

## 8. INTEGRATION STATUS: CURRENT vs RECOMMENDED

```
┌────────────────────────────────────────────────────────────────┐
│          INTEGRATION IN BankTransaction (Current)             │
└────────────────────────────────────────────────────────────────┘

CURRENT STATE (Implicit):
━━━━━━━━━━━━━━━━━━━━━━━━

✓ AR DP: Integrated via reflection
✓ AP DP: Integrated via reflection
✓ Routing: Automatic via Target field
? Detection: Implicit (depends on service loaded)

CODE PATTERN:
```csharp
if (effectiveTarget == "AP") {
	// Load service dynamically
	var apServiceType = reflection.GetType("...PaymentApServices");
	var apService = _serviceProvider.GetService(apServiceType);
	var addMethod = apService.GetType().GetMethod("AddTransH");
	addMethod.Invoke(apService, new object[] { apInstance });
	// ← Works for both AP and AP DP!
	//   But which one was actually called?
}
```

? ISSUE: Cannot distinguish AP DP vs AP regular payment


RECOMMENDED STATE (Explicit):
━━━━━━━━━━━━━━━━━━━━━━━━━━━

✓ Add TransactionType field
✓ Add routing logic based on type
✓ Explicit service selection
✓ Clear documentation

CODE PATTERN:
```csharp
string serviceName = DeterminePaymentService(
	transactionType: trx.TransactionType,
	targetCode: effectiveTarget,
	hasOutstandingDocs: trx.OutstandingDocs?.Count > 0
);

// Result: 
// "IPaymentApDpServices" or "IPaymentApServices"
// "IPaymentArDpServices" or "IPaymentArServices"
```

BENEFIT:
✓ Self-documenting
✓ Traceable in logs
✓ Easier debugging
✓ Less "magic"
```

---

## 9. QUICK REFERENCE CARD

```
╔════════════════════════════════════════════════════════════════╗
║                    QUICK REFERENCE CARD                       ║
╠════════════════════════════════════════════════════════════════╣
║                                                                ║
║  AR DOWNPAYMENT               AP DOWNPAYMENT                 ║
║  ─────────────────────────    ─────────────────────────       ║
║  Service: PaymentArDpSvc      Service: PaymentApDpSvc        ║
║  Kode: "13"                   Kode: "23"                     ║
║  DocNo: UMY-yy2MM-nnnnn      DocNo: DPY-yy5MM-nnnnn        ║
║  Party: Customer              Party: Supplier                ║
║  Currency: NO                Currency: YES ✅                ║
║  Balance: Piutang -= X        Balance: Hutang -= X           ║
║  Mirror: CbTransH (SrcCode="AR")  Mirror: CbTransH (AP)     ║
║  Can apply to: Invoices later      Can apply to: PO/Inv     ║
║                                                                ║
║  ENTRY POINTS: (Both)                                         ║
║  ├─ Direct in AR Module                                       ║
║  ├─ Direct in AP Module                                       ║
║  └─ Via BankTransaction ← Target="AR" or "AP"               ║
║                                                                ║
║  DEFAULT BEHAVIOR:                                            ║
║  • If Target="AR" → ArDpServices.AddTransH()                 ║
║  • If Target="AP" → ApDpServices.AddTransH()                 ║
║    (unless OutstandingDocs selected → might be regular)       ║
║                                                                ║
║  ENHANCEMENT NEEDED:                                          ║
║  • Add TransactionType field (PAYMENT vs DOWNPAYMENT)        ║
║  • Explicit routing based on type                            ║
║  • Document in UI                                            ║
║                                                                ║
╚════════════════════════════════════════════════════════════════╝
```

---

**End of Comparison Matrix**  
**Version:** 1.0  
**Date:** 2024  
**Status:** ✅ Complete
