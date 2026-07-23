# Data Flow & Integration Diagram

## 1. AR DownPayment (ARDP) Flow

```
┌─────────────────────────────────────────────────────────────────────┐
│                    ARDP Payment Creation Flow                        │
└─────────────────────────────────────────────────────────────────────┘

INPUT: ArTransHView (dari UI/BankTransaction)
├── Customer: "CUST001"
├── Tanggal: 2024-01-15
├── JumBayar: 5,000,000 (prepayment amount)
├── KdBank: "BCA"            ← Bank link
└── KdBank: Optional

	   ↓

┌──────────────────────────────────────────────┐
│  PaymentArDpServices.AddTransH()             │
└──────────────────────────────────────────────┘

	Generate DocNo: UMY-240113-00001

	   ↓

   CREATE IN ArTransH
   ┌─────────────────────────────────────────────────┐
   │ ArTransHId: Auto                               │
   │ Bukti: UMY-240113-00001                        │
   │ Customer: CUST001                             │
   │ Tanggal: 2024-01-15                           │
   │ Jumlah: 5,000,000                             │
   │ Unapplied: 5,000,000 (ready for future inv)   │
   │ Kode: "13" ← Down Payment Marker              │
   │ KdBank: BCA                                   │
   └─────────────────────────────────────────────────┘
			  ↓        ↓        ↓

   CREATE:              UPDATE:            CREATE:
   ArPiutng (Aging)    ArCust (Balance)   CbTransH (Bank)
   ┌────────────────┐  ┌───────────────┐  ┌─────────────────────┐
   │ Dokumen:       │  │ Piutang:      │  │ DocNo: same         │
   │ UMY-240113-... │  │ -= 5,000,000  │  │ KodeBank: BCA       │
   │ Jumlah:        │  │               │  │ Tanggal: same       │
   │ -5,000,000     │  │ Customer:     │  │ Saldo: 5,000,000    │
   │ Kode: CA       │  │ CUST001       │  │ CbTransD[]:          │
   │ Sisa:          │  └───────────────┘  │   SrcCode: "AR"    │
   │ -5,000,000     │                      │   Terima: 5,000,000│
   │ KodeTran: 13   │                      │   Jumlah: 5,000,000│
   └────────────────┘                      └─────────────────────┘
												 ↓
										   UPDATE CbBanks
										   ├── Saldo += 5,000,000
										   └── KSaldo += 0

OUTPUT: ArTransH object returned
```

---

## 2. AP DownPayment (APDP) Flow - With Multi-Currency

```
┌─────────────────────────────────────────────────────────────────────┐
│                    APDP Payment Creation Flow                       │
│                    (Support Multi-Currency)                         │
└─────────────────────────────────────────────────────────────────────┘

INPUT: ApTransHView
├── Supplier: "SUPP001"
├── Tanggal: 2024-01-15
├── Currency: "USD"              ← Multi-currency support!
├── Kurs: "15,500"              ← Exchange rate
├── Nilai: 300.00               ← Amount in foreign currency
├── JumBayar: 4,650,000         ← Amount in IDR (300 * 15,500)
└── KdBank: "MANDIRI"

	   ↓

┌──────────────────────────────────────────────────────┐
│  PaymentApDpServices.AddTransH()                     │
└──────────────────────────────────────────────────────┘

	Generate DocNo: DPY-245XX-00001

	   ↓

   CREATE IN ApTransH
   ┌──────────────────────────────────────────────────────┐
   │ ApTransHId: Auto                                    │
   │ Bukti: DPY-245XX-00001                             │
   │ Supplier: SUPP001                                  │
   │ Tanggal: 2024-01-15                                │
   │ Currency: USD                                      │
   │ Kurs: 15,500                                       │
   │ Nilai: 300.00                                      │
   │ Jumlah: 4,650,000                                  │
   │ Unapplied: 4,650,000                               │
   │ Kode: "23" ← Down Payment Marker                   │
   │ KdBank: MANDIRI                                    │
   └──────────────────────────────────────────────────────┘
			  ↓         ↓         ↓

   CREATE:               UPDATE:          CREATE:
   ApHutang (Aging)     ApSuppl (Bal)   CbTransH (Bank)
   ┌────────────────┐   ┌─────────────┐┌──────────────────────────┐
   │ Dokumen:       │   │ Hutang:     ││ DocNo: same              │
   │ DPY-245XX-...  │   │ -= 4,650,000││ KodeBank: MANDIRI        │
   │ Jumlah:        │   │             ││ Tanggal: same            │
   │ -4,650,000     │   │ Supplier:   ││ Kurs: bank.Kurs (IDR)   │
   │ Currency: USD  │   │ SUPP001     ││ Saldo: -300 (USD)        │
   │ Nilai: -300    │   └─────────────┘│ KSaldo: -4,650,000 (IDR) │
   │ Kode: CA       │                  │ CbTransDs[]:             │
   │ Sisa:          │                  │   SrcCode: "AP"          │
   │ -4,650,000     │                  │   KTerima: 4,650,000    │
   │ KodeTran: 23   │                  │   Terima: 300 (USD)     │
   └────────────────┘                  │   KValue: 15,500        │
										└──────────────────────────┘
												 ↓
										UPDATE CbBanks
										├── Saldo -= 300
										└── KSaldo -= 4,650,000
```

---

## 3. BankTransaction SaveTransactionsAsync Integration

```
┌──────────────────────────────────────────────────────────────────┐
│  SaveTransactionsAsync() - Complete Flow                         │
└──────────────────────────────────────────────────────────────────┘

INPUT: List<BankTransactionView> + Date + KodeBank
		   ↓
	Filter (IsSelected == true)
		   ↓
	Group by Tanggal.Date
		   ↓
	For each group:
	┌───────────────────────────────────────────────────────────┐
	│                                                           │
	│  ┌─────────────────────────────────────────────────────┐ │
	│  │ Routing Logic (Line 1196-1199):                    │ │
	│  │                                                     │ │
	│  │ apArTransactions = where (                          │ │
	│  │   Target == "AP" OR "AR"                           │ │
	│  │   fallback: SrcCode == "AP" OR "AR"                │ │
	│  │ )                                                   │ │
	│  │                                                     │ │
	│  │ cashBankTransactions = all others                  │ │
	│  └─────────────────────────────────────────────────────┘ │
	│           ↓
	│    Branch A: AP/AR Transactions (via Services)
	│           
	│    For each apArTransaction:
	│    ┌──────────────────────────────────────┐
	│    │ if Target == "AP":                   │
	│    │  ├── Load: PaymentApServices        │
	│    │  │       (or PaymentApDpServices?   │
	│    │  │        - detection logic TBD)   │
	│    │  ├── Create: ApTransHView instance  │
	│    │  ├── Set properties via Reflection: │
	│    │  │   • Tanggal                      │
	│    │  │   • KdBank                       │
	│    │  │   • Supplier                     │
	│    │  │   • Keterangan                   │
	│    │  │   • ApTransDs (details)          │
	│    │  ├── Call: AddTransH() via Reflection
	│    │  │   [Service will create:          │
	│    │  │    - ApTransH                    │
	│    │  │    - ApHutang                    │
	│    │  │    - CbTransH (auto)             │
	│    │  │    - Update ApSuppl & CbBanks]   │
	│    │  │                                  │
	│    └──────────────────────────────────────┘
	│         Same logic for AR (Target == "AR")
	│
	│    Branch B: Cash/Bank Transactions
	│    
	│    ┌──────────────────────────────────────┐
	│    │ For remaining (non-AP/AR):          │
	│    │                                     │
	│    │ Create CbTransH directly:           │
	│    │  ├── Generate DocNo                 │
	│    │  ├── Create CbTransD items          │
	│    │  ├── Add to _context                │
	│    │  └── SaveChanges()                  │
	│    │                                     │
	│    └──────────────────────────────────────┘
	│
	└───────────────────────────────────────────────────────────┘
		   ↓
	ALL DATABASES UPDATED:
	✓ eSoft.Piutang (ArTransH, ArPiutng, ArCust) - if AR
	✓ eSoft.Hutang (ApTransH, ApHutang, ApSuppl) - if AP
	✓ eSoft.CashBank (CbTransH, CbTransD, CbBanks) - all
```

---

## 4. Data Relationships

```
┌─────────────────────────────────────────────────────────────┐
│            BankTransaction (Input Source)                  │
│                                                             │
│ • Tanggal: 2024-01-15                                     │
│ • Amount: 5,000,000                                       │
│ • DocNo: AUTO (from SaveTransactionsAsync)               │
│ • Target: "AP" or "AR" ← ROUTING KEY!                   │
│ • SrcCode: Fallback if Target empty                      │
│ • PartyCode: Customer/Supplier code                      │
│ • Description: Narrative                                 │
│ • KodeBank: BCA, MANDIRI, etc.                          │
└──────────┬──────────────────────────────────────┬─────────┘
		   │                                      │
		   ├─ "AP" → PaymentApDpServices          ├─ "AR" → PaymentArDpServices
		   │                                      │
	  ┌────v────────────────────┐           ┌────v────────────────────┐
	  │    Hutang DB            │           │    Piutang DB           │
	  ├────────────────────────┤           ├────────────────────────┤
	  │ ApTransH                │           │ ArTransH               │
	  │ ├─ Bukti: DPY-245...   │           │ ├─ Bukti: UMY-240...   │
	  │ ├─ Supplier            │           │ ├─ Customer            │
	  │ ├─ Kode: "23" (DP)     │           │ ├─ Kode: "13" (DP)     │
	  │ ├─ Currency (support)  │           │ ├─ No Currency         │
	  │ └─ Jumlah, Unapplied   │           │ └─ Jumlah, Unapplied   │
	  │                         │           │                        │
	  │ ApHutang (Aging)        │           │ ArPiutng (Aging)       │
	  │ ├─ Kode: "CA"           │           │ ├─ Kode: "CA"          │
	  │ └─ Sisa: -Jumlah        │           │ └─ Sisa: -Jumlah       │
	  │                         │           │                        │
	  │ ApSuppl (Master)        │           │ ArCust (Master)        │
	  │ └─ Hutang -= Jumlah     │           │ └─ Piutang -= Jumlah   │
	  └────┬────────────────────┘           └────┬────────────────────┘
		   │                                     │
		   └──────────────┬──────────────────────┘
						  │
						 (Both also create:)
						  │
					┌─────v──────────────┐
					│  CashBank DB       │
					├────────────────────┤
					│ CbTransH           │
					│ ├─ DocNo: same     │
					│ ├─ SrcCode: AP/AR  │
					│ ├─ Kodebank        │
					│ └─ Saldo/KSaldo    │
					│                    │
					│ CbTransD           │
					│ └─ Terima/Bayar    │
					│                    │
					│ CbBanks (Master)   │
					│ └─ Saldo ±=        │
					│    KSaldo ±=       │
					└────────────────────┘
```

---

## 5. Key Decision Points

```
BankTransaction.razor (UI)
		│
		├─ User fills form:
		│  ├─ Amount
		│  ├─ Date
		│  ├─ Bank
		│  ├─ Party (Customer/Supplier)
		│  └─ Target: [Select] AP / AR / (blank=Bank)
		│
		└─ SaveChanges() → SaveTransactionsAsync()
			   │
			   └─ If Target == "AP" or SrcCode == "AP":
				  │
				  ├─ Q: Is this a DOWN PAYMENT?
				  │  ├─ YES: Call PaymentApDpServices.AddTransH()
				  │  │  (currently: depends on reflection & service availability)
				  │  │
				  │  └─ NO: Call PaymentApServices.AddTransH()
				  │     (for regular invoice payment)
				  │
				  └─ [IMPROVEMENT NEEDED]
					 Make this routing EXPLICIT

			   └─ Same for "AR"

				  └─ Similar logic: ARDP vs AR regular
```

---

## 6. Current Implementation Status

### ✅ What's Already Working:

```
BankTransaction.razor
	↓
SaveTransactionsAsync()
	├─ Detects Target == "AP" or "AR"
	├─ Dynamically loads service via Reflection
	├─ Creates view instances
	├─ Calls AddTransH()
	└─ RESULT: Works for both DP and regular payments!
		(But user/dev must know which service is loaded at runtime)
```

### ⚠️ What's Implicit / Needs Clarification:

```
1. Service Selection Logic:
   - Currently: Loads "PaymentApServices" OR "PaymentApDpServices"
   - Mechanism: Reflection on type names
   - PROBLEM: Which one gets loaded?

   Answer: Depends on AppDomain.CurrentDomain.GetAssemblies()
		   and DI container registration

2. Down Payment Detection:
   - Currently: User must set Target="AP" or "AR"
   - No explicit marker: "this is a DP" vs "regular payment"
   - FIX: Add TransactionType field or auto-detect logic

3. OutstandingDocs:
   - AP/AR support document selection (invoice picking)
   - DP = no documents selected (prepayment)
   - OPPORTUNITY: Auto-detect DP based on empty docs
```

---

## 7. Recommended Enhancement

```
┌───────────────────────────────────────────────────────┐
│  Proposed: Explicit DP Routing                        │
└───────────────────────────────────────────────────────┘

Modify: SaveTransactionsAsync()

BEFORE:
	if (effectiveTarget == "AP") {
		apService = _serviceProvider.GetService(apType);
		apService.GetType().GetMethod("AddTransH").Invoke(...);
	}

AFTER:
	if (effectiveTarget == "AP") {
		// Detect if this is DownPayment
		bool isDownPayment = trx.TransactionType == "DOWNPAYMENT"
						  || (trx.OutstandingDocs?.Count == 0);

		string serviceName = isDownPayment
			? "eSoft.Hutang.Services.IPaymentApDpServices"
			: "eSoft.Hutang.Services.IPaymentApServices";

		apService = _serviceProvider.GetService(reflectionLoadType(serviceName));
		apService.GetType().GetMethod("AddTransH").Invoke(...);
	}

BENEFIT:
	• Explicit routing based on business logic
	• Traceable in code reviews
	• Self-documenting
	• Easier to debug
```

---

**End of Flow Diagram**
