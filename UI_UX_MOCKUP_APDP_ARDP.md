# 🎨 UI/UX MOCKUP - APDP & ARDP dengan Currency Input

## CURRENT STATE (Apa yang ada sekarang)

```
┌─ BANK TRANSACTION TABLE ──────────────────────────────────────┐
│                                                               │
│ [✓] Target [CB  ▼] Dup [ ] Date [Field] [Description...]     │
│                                                               │
│                Customer Supplier                 Auto Alokasi  │
│                                                               │
│ Form menampilkan:                                            │
│ ├─ Target dropdown: CB, AR, AP (tidak ada ARDP, APDP)       │
│ ├─ Currency field: ❌ TIDAK ADA                              │
│ ├─ Kurs field: ❌ TIDAK ADA                                  │
│ └─ Sistem tidak tahu apakah ini DP atau regular payment     │
│                                                               │
└───────────────────────────────────────────────────────────────┘
```

---

## PROPOSED STATE - DENGAN APDP & ARDP + CURRENCY SUPPORT

### Kondisi 1: Target = "CB" (Cash Bank)

```
┌─ BANK TRANSACTION TABLE ──────────────────────────────────────┐
│                                                               │
│ [✓] Target [CB ▼]     Dup [ ] Date [Field] [Amt] [Saldo]     │
│                                                               │
│ ← Currency fields: ❌ HIDDEN (transaction bank murni)        │
│                                                               │
└───────────────────────────────────────────────────────────────┘
```

### Kondisi 2: Target = "AR" (A/R Regular)

```
┌─ BANK TRANSACTION DETAIL ─────────────────────────────────────┐
│                                                               │
│ [✓] Target [AR ▼]  Dup [ ] Date [Field] [Description...]     │
│         ▼ EXPAND                                              │
│ ╔═══════════════════════════════════════════════════════════╗│
│ ║                                                           ║│
│ ║  Customer    [▼ Cust-001 Delta Trading]                 ║│
│ ║  Transaction Type:  [Regular Payment ▼]                 ║│
│ ║                                                           ║│
│ ║  Currency: ❌ HIDDEN (AR tidak support multi-currency)   ║│
│ ║  Kurs: ❌ HIDDEN                                         ║│
│ ║                                                           ║│
│ ║  Outstanding Docs:                                      ║│
│ ║  ┌─ INV-001   Balance: 10,000,000  [Iși Bayar] [Iști Disc]║│
│ ║  ├─ INV-002   Balance: 5,000,000   [Iști Bayar] [Iști Disc]║│
│ ║                                                           ║│
│ ║  [Auto Alokasi]                                         ║│
│ ║                                                           ║│
│ ╚═══════════════════════════════════════════════════════════╝│
│                                                               │
└───────────────────────────────────────────────────────────────┘
```

### Kondisi 3: Target = "ARDP" (A/R Down Payment)

```
┌─ BANK TRANSACTION DETAIL ─────────────────────────────────────┐
│                                                               │
│ [✓] Target [ARDP ▼]  Dup [ ] Date [Field] [Description...]   │
│         ▼ EXPAND                                              │
│ ╔═══════════════════════════════════════════════════════════╗│
│ ║                                                           ║│
│ ║  Customer    [▼ Cust-001 Delta Trading]                 ║│
│ ║  Transaction Type: [Down Payment ▼] (auto)              ║│
│ ║                                                           ║│
│ ║  Currency: ❌ HIDDEN                                     ║│
│ ║  Kurs: ❌ HIDDEN                                         ║│
│ ║                                                           ║│
│ ║  Catatan: Ini pembayaran uang muka (prepayment)         ║│
│ ║  Akan disimpan sebagai Kode "13"                        ║│
│ ║                                                           ║│
│ ║  Outstanding Docs: ❌ EMPTY (tidak ada invoices yet)    ║│
│ ║                                                           ║│
│ ║  [Auto Alokasi]                                         ║│
│ ║                                                           ║│
│ ╚═══════════════════════════════════════════════════════════╝│
│                                                               │
└───────────────────────────────────────────────────────────────┘
```

### Kondisi 4: Target = "AP" (A/P Regular Payment)

```
┌─ BANK TRANSACTION DETAIL ─────────────────────────────────────┐
│                                                               │
│ [✓] Target [AP ▼]   Dup [ ] Date [Field] [Description...]    │
│         ▼ EXPAND                                              │
│ ╔═══════════════════════════════════════════════════════════╗│
│ ║                                                           ║│
│ ║  Supplier    [▼ SUPP-001 Global Parts Inc]              ║│
│ ║  Transaction Type: [Regular Payment ▼]                   ║│
│ ║                                                           ║│
│ ║  ┌─ CURRENCY & KURS SECTION ──────────────────────────┐ ║│
│ ║  │  (Optional - hanya jika invoice dalam valuta asing) │ ║│
│ ║  │                                                      │ ║│
│ ║  │  Currency:  [USD      ]  Kurs: [15500.00      ]    │ ║│
│ ║  │  Amount:    [300      ] = IDR 4,650,000       │ ║│
│ ║  │                        (Nilai dalam currency asing)   │ ║│
│ ║  └──────────────────────────────────────────────────────┘ ║│
│ ║                                                           ║│
│ ║  Outstanding Docs (in Foreign Currency):                ║│
│ ║  ┌─ PO-2024-001  USD 300     [Iști Bayar] [Iști Disc]  ║│
│ ║  ├─ PO-2024-002  USD 200     [Iști Bayar] [Iști Disc]  ║│
│ ║                                                           ║│
│ ║  [Auto Alokasi]                                         ║│
│ ║                                                           ║│
│ ╚═══════════════════════════════════════════════════════════╝│
│                                                               │
└───────────────────────────────────────────────────────────────┘
```

### Kondisi 5: Target = "APDP" (A/P Down Payment)

```
┌─ BANK TRANSACTION DETAIL ─────────────────────────────────────┐
│                                                               │
│ [✓] Target [APDP ▼]  Dup [ ] Date [Field] [Description...]   │
│         ▼ EXPAND                                              │
│ ╔═══════════════════════════════════════════════════════════╗│
│ ║                                                           ║│
│ ║  Supplier    [▼ SUPP-001 Global Parts Inc]              ║│
│ ║  Transaction Type: [Down Payment ▼] (auto)              ║│
│ ║                                                           ║│
│ ║  ┌─ CURRENCY & KURS SECTION (REQUIRED) ─────────────────┐ ║│
│ ║  │  Currency:  [USD        ] * REQUIRED               │ ║│
│ ║  │  Kurs:      [15500.00    ] * REQUIRED               │ ║│
│ ║  │                                                      │ ║│
│ ║  │  Amount (Foreign): [300        ]                    │ ║│
│ ║  │  ⚠ Amount (IDR):   4,650,000 (calculated)           │ ║│
│ ║  │                                                      │ ║│
│ ║  │  1 USD = 15,500 IDR                                │ ║│
│ ║  └──────────────────────────────────────────────────────┘ ║│
│ ║                                                           ║│
│ ║  Catatan:                                               ║│
│ ║  ✓ Ini pembayaran uang muka (prepayment)              ║│
│ ║  ✓ Akan disimpan sebagai Kode "23"                    ║│
│ ║  ✓ Mendukung multi-currency:                          ║│
│ ║    - Storage: Amount (USD), Kurs, Nilai             ║│
│ ║    - Dapat di-apply ke invoice future               ║│
│ ║                                                           ║│
│ ║  Outstanding Docs: ❌ EMPTY (prepayment, belum ada invoice) ║│
│ ║                                                           ║│
│ ║  [AUTO ALOKASI (disabled untuk DP)]                   ║│
│ ║                                                           ║│
│ ╚═══════════════════════════════════════════════════════════╝│
│                                                               │
└───────────────────────────────────────────────────────────────┘
```

---

## 🔄 CURRENCY FIELD VISIBILITY LOGIC

```
┌─────────────────────────────────────┐
│ User selects Target                │
├─────────────────────────────────────┤
│                                      │
│  if (Target == "CB")                 │
│  ├─ Currency Fields: ❌ HIDDEN        │
│  └─ TransactionType: ❌ HIDDEN        │
│                                      │
│  if (Target == "AR")                 │
│  ├─ Currency Fields: ❌ HIDDEN        │
│  └─ TransactionType: ✅ VISIBLE       │
│                                      │
│  if (Target == "ARDP")               │
│  ├─ Currency Fields: ❌ HIDDEN        │
│  └─ TransactionType: Auto (DP)      │
│                                      │
│  if (Target == "AP")                 │
│  ├─ Currency Fields: ✅ VISIBLE       │
│  │  Availability: OPTIONAL           │
│  │  Required: ONLY if Kurs > 1      │
│  │                                   │
│  └─ TransactionType: ✅ VISIBLE       │
│                                      │
│  if (Target == "APDP")               │
│  ├─ Currency Fields: ✅ VISIBLE       │
│  │  Availability: REQUIRED *         │
│  │  Validation: Kurs & Currency     │
│  │             minimum required      │
│  │                                   │
│  └─ TransactionType: Auto (DP)      │
│                                      │
└─────────────────────────────────────┘
```

---

## INPUT VALIDATION LOGIC

### Untuk Target = "APDP" (DOWN PAYMENT)

```
┌─────────────────────────────────────┐
│ APDP VALIDATION CHECKLIST           │
├─────────────────────────────────────┤
│                                      │
│ [✓] Supplier selected               │
│ [✓] Currency tidak kosong           │
│ [✓] Kurs > 1 (untuk foreign curr)   │
│ [✓] Amount (IDR) > 0                │
│ [?] Nilai (foreign) = Amount/Kurs   │
│     (calculated, not required input)│
│                                      │
│ ❌ VALIDATION ERROR JIKA:            │
│ • Currency kosong saat Target=APDP │
│ • Kurs <= 1 untuk valuta asing     │
│ • Amount IDR tidak sesuai kalkulasi │
│ • No supplier selected              │
│                                      │
└─────────────────────────────────────┘
```

### Untuk Target = "AP + TransactionType = DOWNPAYMENT"

```
┌─────────────────────────────────────┐
│ AP (PAYMENT) VALIDATION             │
├─────────────────────────────────────┤
│                                      │
│ [✓] Supplier selected               │
│ [?] Currency (OPTIONAL)             │
│ [?] Kurs (OPTIONAL)                 │
│ [✓] Outstanding Docs selected       │
│     (MINIMUM 1 doc required)        │
│                                      │
│ ❌ VALIDATION ERROR JIKA:            │
│ • No supplier selected              │
│ • No outstanding docs selected      │
│ • No payment allocated to docs      │
│                                      │
└─────────────────────────────────────┘
```

---

## FORM FIELD STRUCTURE (HTML/CSS)

```html
<!-- Conditional Display CSS -->
<style>
	/* Show currency fields hanya untuk AP & APDP -->
	.currency-section {
		display: none;
	}

	.target-ap .currency-section,
	.target-apdp .currency-section {
		display: flex;
	}

	/* AR & ARDP tidak ada currency -->
	.target-ar .currency-section,
	.target-ardp .currency-section {
		display: none;
	}

	/* APDP: currency required, marked with red */
	.target-apdp .currency-required::after {
		content: " *";
		color: red;
		font-weight: bold;
	}

	/* AP: currency optional, not marked */
	.target-ap .currency-optional {
		font-weight: normal;
	}
</style>

<!-- Form Template -->
<div class="trx-detail-row" [class.target-cb]="ctx.Target=='CB'"
							[class.target-ar]="ctx.Target=='AR'"
							[class.target-ardp]="ctx.Target=='ARDP'"
							[class.target-ap]="ctx.Target=='AP'"
							[class.target-apdp]="ctx.Target=='APDP'">

	<div class="form-section">
		<label>Target:</label>
		<select @bind="ctx.Target">
			<option>CB</option>
			<option>AR</option>
			<option>ARDP</option>
			<option>AP</option>
			<option>APDP</option>
		</select>
	</div>

	<!-- Conditional: Party Selector -->
	@if (ctx.Target.StartsWith("AR"))
	{
		<div class="form-section">
			<label>Customer:</label>
			<select @bind="ctx.PartyCode">...</select>
		</div>
	}
	else if (ctx.Target.StartsWith("AP"))
	{
		<div class="form-section">
			<label>Supplier:</label>
			<select @bind="ctx.PartyCode">...</select>
		</div>
	}

	<!-- Conditional: TransactionType -->
	@if (ctx.Target == "AR" || ctx.Target == "AP")
	{
		<div class="form-section">
			<label>Transaction Type:</label>
			<select @bind="ctx.TransactionType">
				<option>PAYMENT</option>
				<option>DOWNPAYMENT</option>
			</select>
		</div>
	}

	<!-- Conditional: Currency & Kurs (AP & APDP only) -->
	<div class="currency-section">
		<div class="form-group">
			<label class="currency-required">Currency:</label>  <!-- APDP: required -->
			<input type="text" @bind="ctx.Currency" placeholder="USD, EUR, SGD..." />
		</div>

		<div class="form-group">
			<label>Kurs:</label>
			<input type="number" @bind="ctx.Kurs" 
				   placeholder="15500" step="0.01" />
		</div>

		<div class="form-group">
			<label>Amount (@ctx.Currency):</label>
			<input type="number" @bind="ctx.Nilai"
				   placeholder="300" step="0.01" />

			@if (ctx.Nilai > 0 && ctx.Kurs > 1m)
			{
				<small>= IDR @((ctx.Nilai * ctx.Kurs).ToString("N2"))</small>
			}
		</div>
	</div>
</div>
```

---

## 📊 COMPARISON TABLE - SEBELUM vs SESUDAH

| Aspek | Sebelum | Sesudah |
|-------|---------|---------|
| **Target Options** | CB, AR, AP | CB, AR, ARDP, AP, APDP |
| **Currency Field** | ❌ Tidak ada | ✅ Show/hide conditional |
| **Kurs Input** | ❌ Tidak ada | ✅ Show/hide conditional |
| **TransactionType** | ❌ Tidak ada | ✅ Untuk AR & AP |
| **ARDP Support** | ❌ Tidak ada | ✅ Ada (tanpa currency) |
| **APDP Support** | ❌ Tidak ada | ✅ Ada (dengan currency) |
| **User knows apakah DP?** | ❌ Tidak jelas | ✅ Jelas (explicit type) |
| **System routing** | ❓ Implicit | ✅ Explicit |

---

**Status:** ✅ UI/UX Design Complete  
**Next:** Implementasi sesuai checklist di SOLUSI_APDP_ARDP_MULTI_CURRENCY.md
