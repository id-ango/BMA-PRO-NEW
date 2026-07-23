# 🎨 DECISION MATRIX - Kapan Isi Kurs?

## QUICK REFERENCE TABLE

```
┌─────────────┬──────────────────┬──────────────┬─────────────────┐
│   Target    │   Currency       │     Kurs     │   Service       │
│             │   Field Shown?   │   Required?  │   Called        │
├─────────────┼──────────────────┼──────────────┼─────────────────┤
│             │                  │              │                 │
│    CB       │  ❌ NO           │  ➖ N/A      │  CbTransH       │
│ (Cash Bank) │  (Hidden)        │  (Not used)  │  (direct)       │
│             │                  │              │                 │
├─────────────┼──────────────────┼──────────────┼─────────────────┤
│             │                  │   Optional   │                 │
│    AR       │  ❌ NO           │  (Show if    │  PaymentAr      │
│ (Payment)   │  (Hidden)        │   user       │  Services       │
│             │                  │   enters)    │                 │
│             │                  │              │                 │
├─────────────┼──────────────────┼──────────────┼─────────────────┤
│             │                  │              │                 │
│   ARDP      │  ❌ NO           │  ❌ NO       │  PaymentAr      │
│ (Down Pay)  │  (Hidden)        │  (Not used)  │  DpServices     │
│             │                  │              │                 │
├─────────────┼──────────────────┼──────────────┼─────────────────┤
│             │                  │   Optional   │                 │
│    AP       │  ✅ YES          │  (Show if    │  PaymentAp      │
│ (Payment)   │  (Optional)      │   Kurs>1)    │  Services OR    │
│             │                  │              │  PaymentAp      │
│             │                  │              │  DpServices     │
│             │                  │              │  (if DP type)   │
│             │                  │              │                 │
├─────────────┼──────────────────┼──────────────┼─────────────────┤
│             │                  │              │                 │
│   APDP      │  ✅ YES          │  ✅ YES      │  PaymentAp      │
│ (Down Pay)  │  (Required)      │  (Required)  │  DpServices     │
│             │                  │              │                 │
└─────────────┴──────────────────┴──────────────┴─────────────────┘
```

---

## DETAILED DECISION TREE

```
User selects Target in BankTransaction
│
├─────────────────────────────────────────────────────────────────
│ Decision Point 1: Is it Cash Bank only (CB)?
├─────────────────────────────────────────────────────────────────
│
├─ YES (Target="CB")
│  │
│  ├─ Currency Field:      ❌ HIDDEN
│  ├─ Kurs Input:          ❌ HIDDEN
│  ├─ TransactionType:     ❌ HIDDEN
│  └─ Result: Direct bank transaction (no AP/AR processing)
│
└─ NO: Continue to next decision
   │
   ├────────────────────────────────────────────────────────────
   │ Decision Point 2: Is it AR (Receivable)?
   ├────────────────────────────────────────────────────────────
   │
   ├─ YES (Target="AR" OR Target="ARDP")
   │  │
   │  ├─ Currency Field:    ❌ HIDDEN (AR tidak support)
   │  ├─ Kurs Input:        ❌ HIDDEN
   │  │
   │  └─ Decision Point 2a: Is it explicit DP (ARDP)?
   │     │
   │     ├─ YES (Target="ARDP")
   │     │  ├─ TransactionType: Auto-set to "DOWNPAYMENT"
   │     │  └─ Service: PaymentArDpServices (kode "13")
   │     │
   │     └─ NO (Target="AR") - show TransactionType selector
   │        │
   │        ├─ User selects: "PAYMENT"
   │        │  └─ Service: PaymentArServices (regular)
   │        │
   │        └─ User selects: "DOWNPAYMENT"
   │           └─ Service: PaymentArDpServices (kode "13")
   │
   └─ NO: Is it AP (Payable)?
	  │
	  ├─ YES (Target="AP" OR Target="APDP")
	  │  │
	  │  ├─ Currency Field:    ✅ SHOWN (editable)
	  │  │
	  │  ├─ Decision Point 3: Is it explicit DP (APDP)?
	  │  │  │
	  │  │  ├─ YES (Target="APDP")
	  │  │  │  │
	  │  │  │  ├─ Kurs Input:        ✅ VISIBLE & REQUIRED
	  │  │  │  ├─ Currency Required: ✅ YES (validation)
	  │  │  │  ├─ TransactionType:   Auto-set to "DOWNPAYMENT"
	  │  │  │  └─ Service:           PaymentApDpServices (kode "23")
	  │  │  │
	  │  │  └─ NO (Target="AP") - show TransactionType selector
	  │  │     │
	  │  │     ├─ Kurs Input: ✅ SHOWN (optional)
	  │  │     │  (only need if currency foreign)
	  │  │     │
	  │  │     ├─ User selects: "PAYMENT"
	  │  │     │  ├─ Currency:  Optional (use if foreign PO)
	  │  │     │  ├─ Kurs:      Optional (use if currency set)
	  │  │     │  └─ Service:   PaymentApServices (regular)
	  │  │     │
	  │  │     └─ User selects: "DOWNPAYMENT"
	  │  │        ├─ Currency:  Optional (but usually needed)
	  │  │        ├─ Kurs:      Optional (but usually needed)
	  │  │        └─ Service:   PaymentApDpServices (kode "23")
	  │  │
	  │  └─ End (AP processed)
	  │
	  └─ NO: Unknown target
		 └─ Show validation error
```

---

## KURS VISIBILITY RULES (PSEUDO-CODE)

```javascript
// Show/Hide Currency & Kurs Fields
function ShowCurrencyFields(target) {
	return target === "AP" || target === "APDP";
}

// Is Currency Input REQUIRED?
function IsCurrencyRequired(target) {
	return target === "APDP";  // Only APDP requires
}

// Is Kurs Input REQUIRED?
function IsKursRequired(target) {
	return target === "APDP";  // Only APDP requires
}

// Show TransactionType Selector
function ShowTransactionType(target) {
	return target === "AR" || target === "AP";
}

// Auto-set TransactionType
function AutoSetTransactionType(target) {
	if (target === "APDP" || target === "ARDP") {
		return "DOWNPAYMENT";  // Auto-set, cannot change
	}
	return null;  // User selects manually
}

// Determine Service
function GetServiceName(target, transactionType) {
	switch(target.toUpperCase()) {
		case "APDP":
			return "IPaymentApDpServices";

		case "AP":
			return transactionType === "DOWNPAYMENT" 
				? "IPaymentApDpServices" 
				: "IPaymentApServices";

		case "ARDP":
			return "IPaymentArDpServices";

		case "AR":
			return transactionType === "DOWNPAYMENT"
				? "IPaymentArDpServices"
				: "IPaymentArServices";

		case "CB":
		default:
			return null;  // Direct bank transaction
	}
}

// Validate before save
function ValidateTransaction(transaction) {
	const { target, currency, kurs, transactionType } = transaction;

	// APDP = MUST have currency & kurs
	if (target === "APDP") {
		if (!currency || currency.trim() === "") 
			return { valid: false, error: "Currency required for APDP" };
		if (kurs <= 1) 
			return { valid: false, error: "Kurs must be > 1 for APDP" };
	}

	// AR/ARDP = Should NOT have currency
	if ((target === "AR" || target === "ARDP") && kurs > 1) {
		console.warn("Currency input for AR/ARDP will be ignored");
	}

	// AP = Optional currency (show warning if partial)
	if (target === "AP" && currency && kurs <= 1) {
		console.warn("Currency set but Kurs=1; treating as IDR");
	}

	return { valid: true };
}
```

---

## VISUAL STATE MATRIX

```
┌──────────────────────────────────────────────────────────────┐
│ Target=CB              Target=AR              Target=ARDP      │
│ (Cash Bank)            (A/R Payment)          (A/R Down Pay)    │
├──────────────────────────────────────────────────────────────┤
│                                                                 │
│ Customer Selector: ❌  Customer: [✓]        Customer: [✓]      │
│ Currency: ❌ HIDDEN    Currency: ❌ HIDDEN   Currency: ❌ HIDDEN│
│ Kurs: ❌ HIDDEN        Kurs: ❌ HIDDEN       Kurs: ❌ HIDDEN    │
│ TxnType: ❌ HIDDEN     TxnType: [Payment ▼] TxnType: Hidden    │
│                        └─ DOWNPAYMENT ▼                       │
│                                                                 │
│ Result: Direct         Result:                Result:           │
│ bank entry             ├─ PAYMENT            ├─ DOWNPAYMENT    │
│ (CbTransH)             │  (regular)           │  (Kode=13)      │
│                        │                      │                 │
│                        └─ DOWNPAYMENT        └─ Creates:       │
│                           (Kode=13)            ├─ ArTransH     │
│                                                ├─ ArPiutng     │
│                                                ├─ ArCust       │
│                                                └─ CbTransH     │
└──────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│ Target=AP              Target=APDP                            │
│ (A/P Payment)          (A/P Down Pay)                          │
├──────────────────────────────────────────────────────────────┤
│                                                                 │
│ Supplier: [✓]          Supplier: [✓]                          │
│ Currency: [optional]   Currency: [REQUIRED] ⚠️                 │
│ Kurs: [optional]       Kurs: [REQUIRED] ⚠️                     │
│ TxnType: [Payment ▼]   TxnType: Hidden                         │
│ └─ DOWNPAYMENT ▼       └─ Auto = DOWNPAYMENT                  │
│                                                                 │
│ Result:                Result:                                 │
│ ├─ PAYMENT             │                                       │
│ │  (regular)           ├─ DOWNPAYMENT                          │
│ │  ├─if Kurs>1         │  (Kode=23)                            │
│ │  │  multi-currency   │  ✅ Stores:                           │
│ │  └─else              │     ├─ Currency                       │
│ │     IDR only         │     ├─ Kurs                           │
│ │                      │     ├─ Nilai                          │
│ └─ DOWNPAYMENT         │                                       │
│    (Kode=23)           └─ Creates:                             │
│    if Kurs>1:            ├─ ApTransH (with currency)          │
│    Multi-currency        ├─ ApHutang                           │
│                          ├─ ApSuppl                            │
│                          └─ CbTransH (dual-amount)             │
│                                                                 │
└──────────────────────────────────────────────────────────────┘
```

---

## WHEN USER NEEDS TO INPUT KURS

### User MUST Input Kurs:
```
✅ Target = APDP
   └─ Because: Explicit downpayment with multi-currency support
	  └─ Field: REQUIRED, cannot save without it

✅ Target = AP + TransactionType = DOWNPAYMENT + Currency = "USD"
   └─ Because: Foreign currency DP payment needs exchange rate
	  └─ Field: REQUIRED for calculation
```

### User MAY Input Kurs:
```
🔹 Target = AP + TransactionType = PAYMENT + Currency = "USD"
   └─ Because: Foreign currency regular payment (against PO)
	  └─ Field: OPTIONAL but recommended
	  └─ Default: Kurs = 1 (if left empty)
```

### User SHOULD NOT Input Kurs:
```
❌ Target = CB
   └─ Because: No party/vendor involved, just bank
	  └─ Field: HIDDEN

❌ Target = AR or ARDP
   └─ Because: AR not support multi-currency
	  └─ Field: HIDDEN

❌ Target = AP/APDP + Currency not set
   └─ Because: No foreign currency = no exchange rate
	  └─ Field: User can leave empty (system uses 1)
```

---

## SYSTEM KNOWS WHEN TO ASK FOR KURS

```javascript
// Logic dalam UI:

if (target === "APDP") {
	// ALWAYS show & require
	showCurrencyField = true;
	requireCurrency = true;
	showKursField = true;
	requireKurs = true;
} 
else if (target === "AP") {
	// Show but optional
	showCurrencyField = true;
	requireCurrency = false;
	showKursField = true;
	requireKurs = false;  // Optional unless currency set
} 
else if (target === "AR" || target === "ARDP") {
	// Never show
	showCurrencyField = false;
	requireCurrency = false;
	showKursField = false;
	requireKurs = false;
} 
else if (target === "CB") {
	// Never show
	showCurrencyField = false;
	showKursField = false;
}

// Validation:
if (target === "APDP" && !currency) {
	validationError = "Currency is required for APDP";
}
if (target === "APDP" && kurs <= 1) {
	validationError = "Kurs must be > 1 for APDP";
}
if (target === "AP" && currency && kurs <= 1) {
	validationWarning = "Currency set but Kurs=1 (treating as IDR)";
}
```

---

## SUMMARY - QUICK ANSWERS

| Question | Answer |
|----------|--------|
| **Kapan harus isi Kurs?** | Ketika Target=APDP (selalu) atau AP dengan foreign currency (opsional) |
| **Kapan Currency field muncul?** | Target=AP atau Target=APDP |
| **Kapan TransactionType muncul?** | Target=AR atau Target=AP |
| **AR support multi-currency?** | ❌ Tidak (field hidden) |
| **AP support multi-currency?** | ✅ Ya (field shown, optional untuk regular, required untuk DP) |
| **Bagaimana system tahu?** | Berdasarkan Target selection — automatic logic di UI |
| **Kurs default?** | 1 (berarti IDR, tidak ada konversi) |
| **Nilai (foreign amount)?** | Optional input atau calculated (Nilai = Amount / Kurs) |

---

**Status:** ✅ Complete Decision Matrix  
**Use This:** As reference when coding the conditional display logic
