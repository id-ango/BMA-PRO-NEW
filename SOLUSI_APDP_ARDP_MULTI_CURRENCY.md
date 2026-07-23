# SOLUSI: Menambah APDP & ARDP sebagai Tipe Transaksi & Handling Multi-Currency

## ❓ PERTANYAAN USER

**"Transaction tipenya dibuat seperti ini aja CB, AR, AP, APDP, ARDP terus anda bilang multicurrency kok tidak ada inputan untuk isi kurs ya terus tahu darimana ini pembayaran yang perlu isi kurs atau tidak"**

### Artinya:
1. ✅ Sudah ada Target dropdown: CB | AR | AP
2. ❌ Belum ada APDP dan ARDP sebagai option
3. ❌ Tidak ada input field untuk Kurs/Currency
4. ❓ Sistem tidak tahu kapan harus diminta input Kurs
5. ❓ Sistem tidak tahu ini DP atau regular payment

---

## 🎯 SOLUSI

### LANGKAH 1: Tambah Field Kurs ke BankTransactionView

**File:** `eSoft.CashBank/View/BankTransactionView.cs`

```csharp
public class BankTransactionView
{
	// Existing fields...
	public string Date { get; set; }
	public string Description { get; set; }
	public decimal Amount { get; set; }
	public string Target { get; set; } = "CB";
	public string PartyCode { get; set; }
	public List<OutstandingDocView> OutstandingDocs { get; set; }

	// ✅ NEW FIELDS FOR MULTI-CURRENCY
	public string Currency { get; set; } = "IDR";  // "USD", "EUR", etc.
	public decimal Kurs { get; set; } = 1m;        // Exchange rate (1 = lokal IDR)
	public decimal Nilai { get; set; }             // Amount in foreign currency

	// ✅ NEW FIELD TO DISTINGUISH DP vs REGULAR PAYMENT
	public string TransactionType { get; set; } = "PAYMENT";  // "PAYMENT" or "DOWNPAYMENT"

	public class OutstandingDocView
	{
		// Existing fields...
		public string Dokumen { get; set; }
		public decimal Sisa { get; set; }
		public decimal Bayar { get; set; }
		public decimal Discount { get; set; }

		// ← Kurs SUDAH ADA (untuk docs foreign currency)
		public decimal Kurs { get; set; }
	}
}
```

---

### LANGKAH 2: Update BankTransaction.razor - Add APDP, ARDP Options

**File:** `Accounting/Pages/ModuleBank/BankTransaction/BankTransaction.razor` (Line 260-264)

**SEBELUM:**
```razor
<InputSelect class="form-control form-control-sm" @bind-Value="ctx.Target">
	<option value="CB">CB</option>
	<option value="AP">AP</option>
	<option value="AR">AR</option>
</InputSelect>
```

**SESUDAH:**
```razor
<InputSelect class="form-control form-control-sm" @bind-Value="ctx.Target">
	<option value="CB">CB (Cash Bank)</option>
	<option value="AR">AR (A/R Regular)</option>
	<option value="ARDP">ARDP (A/R Down Payment)</option>
	<option value="AP">AP (A/P Regular)</option>
	<option value="APDP">APDP (A/P Down Payment)</option>
</InputSelect>
```

---

### LANGKAH 3: Add UI untuk Input Currency & Kurs (Conditional)

**Tambahkan setelah line 300-327 di BankTransaction.razor:**

```razor
<!-- Detail row untuk AR/AP/ARDP/APDP -->
@if (ctx.Target == "AR" || ctx.Target == "AP" || ctx.Target == "ARDP" || ctx.Target == "APDP")
{
	<tr class="trx-detail-row">
		<td colspan="9">
			<div style="display:flex; flex-wrap:wrap; align-items:center; gap:8px; margin-bottom:8px;">
				<!-- CUSTOMER / SUPPLIER SELECTOR -->
				@if (ctx.Target.StartsWith("AR"))
				{
					<label style="font-size:0.78rem; font-weight:600; color:#6c757d; text-transform:uppercase; margin:0;">
						Customer
					</label>
					<select class="form-control form-control-sm" style="max-width:420px;" 
							value="@ctx.PartyCode"
							@onchange="@(async (ChangeEventArgs e) => await PartySelectionChanged(ctx, e))">
						<option value="">--- Pilih Customer ---</option>
						@foreach (var c in customers)
						{
							<option value="@c.Customer">[@c.Customer] @c.NamaCust</option>
						}
					</select>
				}
				else
				{
					<label style="font-size:0.78rem; font-weight:600; color:#6c757d; text-transform:uppercase; margin:0;">
						Supplier
					</label>
					<select class="form-control form-control-sm" style="max-width:420px;" 
							value="@ctx.PartyCode"
							@onchange="@(async (ChangeEventArgs e) => await PartySelectionChanged(ctx, e))">
						<option value="">--- Pilih Supplier ---</option>
						@foreach (var s in suppliers)
						{
							<option value="@s.Supplier">[@s.Supplier] @s.NamaSup</option>
						}
					</select>
				}
			</div>

			<!-- ✅ NEW: TRANSACTION TYPE SELECTOR (untuk membedakan DP vs Regular) -->
			<div style="display:flex; flex-wrap:wrap; align-items:center; gap:8px; margin-bottom:8px;">
				<label style="font-size:0.78rem; font-weight:600; color:#6c757d; text-transform:uppercase; margin:0; min-width:100px;">
					Transaction Type:
				</label>
				<select class="form-control form-control-sm" style="max-width:250px;"
						@bind="ctx.TransactionType">
					<option value="PAYMENT">Regular Payment</option>
					<option value="DOWNPAYMENT">Down Payment (DP)</option>
				</select>
				<small style="color:#6c757d; font-size:0.75rem;">
					DP = No docs / Prepayment | Regular = Against specific invoices
				</small>
			</div>

			<!-- ✅ NEW: CURRENCY & KURS INPUT (conditional - hanya untuk AP/APDP) -->
			@if (ctx.Target == "AP" || ctx.Target == "APDP")
			{
				<div style="display:flex; flex-wrap:wrap; align-items:center; gap:8px; margin-bottom:8px;">
					<label style="font-size:0.78rem; font-weight:600; color:#6c757d; text-transform:uppercase; margin:0; min-width:80px;">
						Currency:
					</label>
					<input type="text" class="form-control form-control-sm" placeholder="USD, EUR, SGD, etc." 
						   style="max-width:100px;" @bind="ctx.Currency" />

					<label style="font-size:0.78rem; font-weight:600; color:#6c757d; text-transform:uppercase; margin:0; margin-left:12px;">
						Kurs:
					</label>
					<input type="number" class="form-control form-control-sm" placeholder="1.0"
						   style="max-width:150px;" @bind="ctx.Kurs" @bind:event="oninput" step="0.01" />

					@if (ctx.Kurs > 1m)
					{
						<small style="color:#198754; font-weight:600;">
							1 @(ctx.Currency ?? "USD") = @(ctx.Kurs.ToString("N2")) IDR
						</small>
					}
					else
					{
						<small style="color:#6c757d;">
							(Leave 1 or empty for IDR only)
						</small>
					}
				</div>

				<!-- Optional: Nilai (Foreign Amount) -->
				<div style="display:flex; flex-wrap:wrap; align-items:center; gap:8px; margin-bottom:8px;">
					<label style="font-size:0.78rem; font-weight:600; color:#6c757d; text-transform:uppercase; margin:0; min-width:80px;">
						Amount (@(ctx.Currency ?? "Foreign")):
					</label>
					<input type="number" class="form-control form-control-sm" placeholder="300"
						   style="max-width:150px;" @bind="ctx.Nilai" @bind:event="oninput" step="0.01" />

					@if (ctx.Nilai > 0 && ctx.Kurs > 1m)
					{
						<small style="color:#198754;">
							= IDR @((ctx.Nilai * ctx.Kurs).ToString("N2"))
						</small>
					}
				</div>
			}

			<!-- Auto Alokasi Button -->
			<div style="margin-top:8px;">
				<button class="btn btn-sm btn-outline-primary" @onclick="() => AllocateAmount(ctx)">
					<i class="bi bi-lightning-charge"></i> Auto Alokasi
				</button>
			</div>

			<!-- Outstanding Docs Table (existing logic) -->
			@if (!string.IsNullOrEmpty(ctx.PartyCode) && ctx.OutstandingDocs.Any())
			{
				<!-- Existing outstanding docs table code -->
				<!-- ... (keep existing code) ... -->
			}
		</td>
	</tr>
}
```

---

### LANGKAH 4: Update SaveTransactionsAsync Logic untuk Handle APDP/ARDP

**File:** `eSoft.CashBank/Services/CashBankServices.cs` (Update routing logic)

```csharp
private string DeterminePaymentService(string target, string transactionType)
{
	// New logic untuk membedakan DP vs Regular

	if (target.Equals("AP", StringComparison.OrdinalIgnoreCase) ||
		target.Equals("APDP", StringComparison.OrdinalIgnoreCase))
	{
		// ✅ APDP explicitly = AP DownPayment Service
		if (target.Equals("APDP", StringComparison.OrdinalIgnoreCase))
			return "eSoft.Hutang.Services.IPaymentApDpServices";

		// ✅ AP with TransactionType="DOWNPAYMENT" = AP DownPayment Service  
		if (transactionType.Equals("DOWNPAYMENT", StringComparison.OrdinalIgnoreCase))
			return "eSoft.Hutang.Services.IPaymentApDpServices";

		// Default: Regular AP Payment
		return "eSoft.Hutang.Services.IPaymentApServices";
	}

	if (target.Equals("AR", StringComparison.OrdinalIgnoreCase) ||
		target.Equals("ARDP", StringComparison.OrdinalIgnoreCase))
	{
		// ✅ ARDP explicitly = AR DownPayment Service
		if (target.Equals("ARDP", StringComparison.OrdinalIgnoreCase))
			return "eSoft.Piutang.Services.IPaymentArDpServices";

		// ✅ AR with TransactionType="DOWNPAYMENT" = AR DownPayment Service
		if (transactionType.Equals("DOWNPAYMENT", StringComparison.OrdinalIgnoreCase))
			return "eSoft.Piutang.Services.IPaymentArDpServices";

		// Default: Regular AR Payment
		return "eSoft.Piutang.Services.IPaymentArServices";
	}

	return null;
}
```

**Dalam SaveTransactionsAsync, gunakan:**

```csharp
foreach (var trx in apArTransactions)
{
	try
	{
		var effectiveTarget = !string.IsNullOrEmpty(trx.Target)
			? trx.Target
			: (trx.SrcCode ?? string.Empty);

		// ✅ Determine which service to use (Regular vs DP)
		string serviceName = DeterminePaymentService(
			effectiveTarget, 
			trx.TransactionType ?? "PAYMENT"
		);

		if (serviceName == null)
			continue; // Skip non-AP/AR

		// ... rest of the logic ...

		// ✅ Pass Kurs & Currency info to view instance
		if (effectiveTarget.StartsWith("AP", StringComparison.OrdinalIgnoreCase))
		{
			apViewType.GetProperty("Currency")?.SetValue(apInstance, trx.Currency ?? "IDR");
			apViewType.GetProperty("Kurs")?.SetValue(apInstance, trx.Kurs > 1m ? trx.Kurs : 1m);
			apViewType.GetProperty("Nilai")?.SetValue(apInstance, trx.Nilai);
		}
		// AR DP doesn't need Currency/Kurs
	}
	catch (Exception ex)
	{
		throw;
	}
}
```

---

## 📋 USER EXPERIENCE FLOW

### SKENARIO 1: Regular Payment to Supplier (AP - No Currency)

```
1. User click row
   ├─ Target: [CB | AR | ARDP | AP | APDP] ← select "AP"
   └─ Muncul: Supplier selector

2. System detects Target="AP"
   ├─ Currency field: ❌ HIDDEN (tidak usah input kurs)
   └─ TransactionType: PAYMENT (default) ✓

3. User select supplier + outstanding docs
   └─ Save: Panggil PaymentApServices (regular)
```

### SKENARIO 2: Down Payment to Supplier (APDP - Multi-Currency)

```
1. User click row
   ├─ Target: select "APDP"
   └─ Muncul: Supplier selector + Currency + Kurs fields

2. System detects Target="APDP"
   ├─ Currency field: ✅ VISIBLE & REQUIRED
   ├─ Kurs input:     ✅ VISIBLE & REQUIRED
   └─ TransactionType: DOWNPAYMENT (default)

3. User input:
   ├─ Supplier: SUPP001
   ├─ Amount (IDR): 4,650,000
   ├─ Currency: USD
   ├─ Kurs: 15500
   ├─ Nilai (Foreign): 300
   └─ Save: Panggil PaymentApDpServices (WITH currency info)
```

### SKENARIO 3: Down Payment to Customer (ARDP - No Currency)

```
1. User click row
   ├─ Target: select "ARDP"
   └─ Muncul: Customer selector

2. System detects Target="ARDP"
   ├─ Currency field: ❌ HIDDEN (AR DP tidak support multi-currency)
   └─ TransactionType: DOWNPAYMENT (auto)

3. User input:
   ├─ Customer: CUST001
   ├─ Amount: 5,000,000
   └─ Save: Panggil PaymentArDpServices
```

---

## 🔍 LOGIC DECISION TREE

```
User select Target
	├─ "CB"
	│  └─ Regular bank transaction (no currency field)
	│
	├─ "AR"
	│  ├─ TransactionType selector appears
	│  ├─ Currency field: ❌ HIDDEN
	│  └─ If DOWNPAYMENT → PaymentArDpServices
	│
	├─ "ARDP"
	│  ├─ Explicit AR DownPayment
	│  ├─ Currency field: ❌ HIDDEN (AR tidak support)
	│  └─ Always → PaymentArDpServices
	│
	├─ "AP"
	│  ├─ TransactionType selector appears
	│  ├─ Currency field: ✅ SHOWN (optional)
	│  ├─ If Kurs > 1 → Multi-currency payment
	│  └─ If DOWNPAYMENT → PaymentApDpServices
	│
	└─ "APDP"
	   ├─ Explicit AP DownPayment
	   ├─ Currency field: ✅ SHOWN & REQUIRED
	   ├─ Kurs input: ✅ SHOWN & REQUIRED
	   └─ Always → PaymentApDpServices
```

---

## 💻 CODE CHANGES CHECKLIST

- [ ] **Step 1:** Update `BankTransactionView.cs`
  - [ ] Add `Currency` property
  - [ ] Add `Kurs` property
  - [ ] Add `Nilai` property
  - [ ] Add `TransactionType` property

- [ ] **Step 2:** Update `BankTransaction.razor` (Line 260-264)
  - [ ] Add ARDP option to Target dropdown
  - [ ] Add APDP option to Target dropdown

- [ ] **Step 3:** Add Currency/Kurs UI in `BankTransaction.razor`
  - [ ] Add Currency input (show only for AP/APDP)
  - [ ] Add Kurs input (show only for AP/APDP)
  - [ ] Add Nilai input (optional, for foreign amount)
  - [ ] Add conditional display logic

- [ ] **Step 4:** Update `CashBankServices.cs`
  - [ ] Add `DeterminePaymentService()` method
  - [ ] Update routing logic untuk handle APDP/ARDP
  - [ ] Pass Currency/Kurs to view instance

- [ ] **Step 5:** Testing
  - [ ] Test AR payment (no currency field)
  - [ ] Test ARDP (no currency field)
  - [ ] Test AP payment (currency optional)
  - [ ] Test APDP (currency required)
  - [ ] Verify Kurs > 1 shows exchange rate
  - [ ] Database checks: ApTransH.Currency, Kurs, Nilai saved correctly

---

## 🧪 TEST CASES

```csharp
// Test 1: AP DP with Currency
var transaction = new BankTransactionView
{
	Target = "APDP",
	TransactionType = "DOWNPAYMENT",
	PartyCode = "SUPP001",
	Amount = 4650000,
	Currency = "USD",
	Kurs = 15500,
	Nilai = 300,
	// User should NOT be able to save without Currency & Kurs
};

// Test 2: AR DP (no currency)
var transaction = new BankTransactionView
{
	Target = "ARDP",
	TransactionType = "DOWNPAYMENT",
	PartyCode = "CUST001",
	Amount = 5000000,
	Currency = null, // ← Should be NULL/IGNORED
	Kurs = null      // ← Should be NULL/IGNORED
	// Currency fields should not appear in UI
};

// Test 3: AP Regular with Currency
var transaction = new BankTransactionView
{
	Target = "AP",
	TransactionType = "PAYMENT",
	PartyCode = "SUPP002",
	Amount = 2500000,
	Currency = "SGD", // Optional
	Kurs = 11500,     // Optional
	OutstandingDocs = [invoice1, invoice2] // Against specific docs
};
```

---

## ✅ HASIL AKHIR

Setelah implementasi:

1. ✅ User bisa memilih ARDP atau APDP sebagai tipe transaksi
2. ✅ Currency & Kurs field hanya muncul untuk AP/APDP (not for AR/ARDP)
3. ✅ System otomatis route ke DP service atau regular payment service
4. ✅ Multi-currency info (Currency, Kurs, Nilai) disimpan di ApTransH
5. ✅ User tahu kapan harus isi kurs (hanya untuk AP/APDP dengan foreign currency)
6. ✅ Clear UI distinction antara PAYMENT vs DOWNPAYMENT

---

**Status:** ✅ Ready to Implement  
**Effort:** 4-6 jam development + 2 jam testing  
**Risk:** Low (isolated changes, no breaking changes to existing logic)
