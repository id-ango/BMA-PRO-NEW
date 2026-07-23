# 💻 CODE SCAFFOLDS - Copy-Paste Ready

Gunakan scaffolds ini untuk accelerate implementation. Adjust sesuai existing code style.

---

## 1️⃣ BankTransactionView.cs - NEW PROPERTIES

**Location:** `eSoft.CashBank/View/BankTransactionView.cs`  
**After:** `Line 28` (after `OutstandingDocs` property)

```csharp
// ============== NEW FIELDS FOR APDP/ARDP SUPPORT ==============

/// <summary>
/// Currency code (USD, EUR, SGD, etc.) for multi-currency transactions
/// Only used for AP/APDP transactions. Defaults to "IDR"
/// </summary>
public string Currency { get; set; } = "IDR";

/// <summary>
/// Exchange rate (e.g., 15500 = 1 USD = 15,500 IDR)
/// Only used for AP/APDP with foreign currency. Defaults to 1 (IDR only)
/// </summary>
public decimal Kurs { get; set; } = 1m;

/// <summary>
/// Amount in foreign currency (e.g., 300 USD)
/// Calculated from Amount / Kurs when needed
/// </summary>
public decimal Nilai { get; set; }

/// <summary>
/// Transaction type: "PAYMENT" or "DOWNPAYMENT"
/// Used to distinguish regular payments from down payments
/// Visible for AR/AP, auto-set for ARDP/APDP
/// </summary>
public string TransactionType { get; set; } = "PAYMENT";

// ============================================================
```

---

## 2️⃣ BankTransaction.razor - TARGET DROPDOWN

**Location:** `Accounting/Pages/ModuleBank/BankTransaction/BankTransaction.razor`  
**Replace:** Lines ~260-264 (the InputSelect with CB, AP, AR options)

**FIND:**
```razor
<InputSelect class="form-control form-control-sm" @bind-Value="ctx.Target">
	<option value="CB">CB</option>
	<option value="AP">AP</option>
	<option value="AR">AR</option>
</InputSelect>
```

**REPLACE WITH:**
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

## 3️⃣ BankTransaction.razor - ADD CURRENCY/KURS UI

**Location:** `Accounting/Pages/ModuleBank/BankTransaction/BankTransaction.razor`  
**After:** The detail row that shows Customer/Supplier selector (around line 327)

**ADD THIS NEW SECTION:**

```razor
<!-- ==================== TRANSACTION TYPE SELECTOR ==================== -->
<!-- Shows for AR and AP (not for ARDP or APDP which auto-set to DP) -->
@if (ctx.Target == "AR" || ctx.Target == "AP")
{
	<div style="display:flex; flex-wrap:wrap; align-items:center; gap:8px; margin-bottom:8px;">
		<label style="font-size:0.78rem; font-weight:600; color:#6c757d; text-transform:uppercase; margin:0; min-width:140px;">
			Transaction Type:
		</label>
		<select class="form-control form-control-sm" style="max-width:250px;" @bind="ctx.TransactionType">
			<option value="PAYMENT">Regular Payment</option>
			<option value="DOWNPAYMENT">Down Payment (DP)</option>
		</select>
		<small style="color:#6c757d; font-size:0.75rem;">
			DP = No invoices yet (prepayment) | Regular = Against existing invoices
		</small>
	</div>
}

<!-- ==================== CURRENCY & KURS SECTION ==================== -->
<!-- Only shown for AP and APDP (not for CB, AR, ARDP) -->
@if (ctx.Target == "AP" || ctx.Target == "APDP")
{
	<div style="border-top: 1px solid #dee2e6; border-bottom: 1px solid #dee2e6; padding: 10px; margin-bottom:8px; background-color: #f8f9fa;">

		<!-- Currency & Kurs Row 1 -->
		<div style="display:flex; flex-wrap:wrap; align-items:center; gap:8px; margin-bottom:8px;">

			<label style="font-size:0.78rem; font-weight:600; color:#6c757d; text-transform:uppercase; margin:0; min-width:100px;">
				Currency @if (ctx.Target == "APDP") { <span style="color:red;">*</span> }
			</label>

			<input type="text" 
				   class="form-control form-control-sm" 
				   placeholder="USD, EUR, SGD, etc." 
				   style="max-width:100px;" 
				   @bind="ctx.Currency" />

			<label style="font-size:0.78rem; font-weight:600; color:#6c757d; text-transform:uppercase; margin:0; margin-left:12px;">
				Kurs (Exchange Rate) @if (ctx.Target == "APDP") { <span style="color:red;">*</span> }
			</label>

			<input type="number" 
				   class="form-control form-control-sm" 
				   placeholder="15500" 
				   style="max-width:150px;" 
				   @bind="ctx.Kurs" 
				   @bind:event="oninput" 
				   step="0.01" />

			@if (ctx.Kurs > 1m && !string.IsNullOrEmpty(ctx.Currency))
			{
				<small style="color:#198754; font-weight:600;">
					1 @ctx.Currency = @(ctx.Kurs.ToString("N2")) IDR
				</small>
			}
			else if (ctx.Target == "APDP")
			{
				<small style="color:#dc3545; font-weight:600;">
					⚠️ Required for APDP
				</small>
			}
			else
			{
				<small style="color:#6c757d;">
					(Leave empty or 1 for IDR only)
				</small>
			}
		</div>

		<!-- Foreign Amount Row 2 (Optional) -->
		<div style="display:flex; flex-wrap:wrap; align-items:center; gap:8px;">

			<label style="font-size:0.78rem; font-weight:600; color:#6c757d; text-transform:uppercase; margin:0; min-width:100px;">
				Amount (@(ctx.Currency ?? "Foreign"))
			</label>

			<input type="number" 
				   class="form-control form-control-sm" 
				   placeholder="300" 
				   style="max-width:150px;" 
				   @bind="ctx.Nilai" 
				   @bind:event="oninput" 
				   step="0.01" />

			@if (ctx.Nilai > 0 && ctx.Kurs > 1m)
			{
				var idrAmount = ctx.Nilai * ctx.Kurs;
				<small style="color:#198754; font-weight:600;">
					= IDR @(idrAmount.ToString("N2"))
				</small>
			}
			else if (string.IsNullOrEmpty(ctx.Currency))
			{
				<small style="color:#6c757d;">
					(Optional - only if foreign currency)
				</small>
			}
		</div>
	</div>
}
```

---

## 4️⃣ CashBankServices.cs - HELPER METHOD

**Location:** `eSoft.CashBank/Services/CashBankServices.cs`  
**Where:** Add this method somewhere before `SaveTransactionsAsync()`, around line 1170

```csharp
/// <summary>
/// Determines which payment service to use based on target and transaction type
/// </summary>
/// <param name="target">Target type: CB, AR, ARDP, AP, APDP</param>
/// <param name="transactionType">PAYMENT or DOWNPAYMENT</param>
/// <returns>Service interface name or null for direct bank posting</returns>
private string DeterminePaymentService(string target, string transactionType)
{
	if (string.IsNullOrEmpty(target))
		return null;

	// ========== AP/APDP Services ==========
	if (target.Equals("APDP", StringComparison.OrdinalIgnoreCase))
	{
		return "eSoft.Hutang.Services.IPaymentApDpServices";
	}

	if (target.Equals("AP", StringComparison.OrdinalIgnoreCase))
	{
		// Check if explicitly set to DOWNPAYMENT
		if (transactionType != null && 
			transactionType.Equals("DOWNPAYMENT", StringComparison.OrdinalIgnoreCase))
		{
			return "eSoft.Hutang.Services.IPaymentApDpServices";
		}
		// Default is regular AP payment
		return "eSoft.Hutang.Services.IPaymentApServices";
	}

	// ========== AR/ARDP Services ==========
	if (target.Equals("ARDP", StringComparison.OrdinalIgnoreCase))
	{
		return "eSoft.Piutang.Services.IPaymentArDpServices";
	}

	if (target.Equals("AR", StringComparison.OrdinalIgnoreCase))
	{
		// Check if explicitly set to DOWNPAYMENT
		if (transactionType != null &&
			transactionType.Equals("DOWNPAYMENT", StringComparison.OrdinalIgnoreCase))
		{
			return "eSoft.Piutang.Services.IPaymentArDpServices";
		}
		// Default is regular AR payment
		return "eSoft.Piutang.Services.IPaymentArServices";
	}

	// ========== CB or unknown ==========
	return null;  // Direct bank transaction
}
```

---

## 5️⃣ CashBankServices.cs - UPDATE SaveTransactionsAsync

**Location:** `eSoft.CashBank/Services/CashBankServices.cs`  
**Find:** The line around 1218 that says `if (string.Equals(effectiveTarget, "AP", StringComparison.OrdinalIgnoreCase))`

**Replace the entire AP/AR detection & service selection block with:**

```csharp
// ========== DETERMINE TARGET & SERVICE ==========
var effectiveTarget = !string.IsNullOrEmpty(trx.Target)
	? trx.Target
	: (trx.SrcCode ?? string.Empty);

// Determine which service to use (Regular vs DP)
string serviceName = DeterminePaymentService(
	effectiveTarget, 
	trx.TransactionType ?? "PAYMENT"
);

// Skip if service not determined (e.g., CB or unknown)
if (string.IsNullOrEmpty(serviceName))
	continue;

try
{
	var apViewType = TypeResolver.GetTypeFromAssembly(
		effectiveTarget.StartsWith("AP", StringComparison.OrdinalIgnoreCase)
			? "eSoft.Hutang.View.ApTransHView"
			: "eSoft.Piutang.View.ArTransHView"
	);

	if (apViewType == null)
		continue;

	// ========== BUILD VIEW INSTANCE ==========
	var apInstance = Activator.CreateInstance(apViewType);

	// Set common properties
	apViewType.GetProperty("KdBank")?.SetValue(apInstance, kodeBank);
	apViewType.GetProperty("NoRef")?.SetValue(apInstance, trx.SrcCode);
	apViewType.GetProperty("Tanggal")?.SetValue(apInstance, formDate);
	apViewType.GetProperty("Keterangan")?.SetValue(apInstance, trx.Description);
	apViewType.GetProperty("Jumlah")?.SetValue(apInstance, trx.Amount);

	// ========== NEW: SET CURRENCY/KURS FOR AP ==========
	if (effectiveTarget.StartsWith("AP", StringComparison.OrdinalIgnoreCase))
	{
		// Only set if we have currency data
		if (!string.IsNullOrEmpty(trx.Currency))
		{
			apViewType.GetProperty("Currency")?.SetValue(apInstance, trx.Currency);
		}

		// Set Kurs (default to 1 if not provided)
		decimal kurs = trx.Kurs > 1m ? trx.Kurs : 1m;
		apViewType.GetProperty("Kurs")?.SetValue(apInstance, kurs);

		// Set Nilai (foreign currency amount) if provided
		if (trx.Nilai > 0)
		{
			apViewType.GetProperty("Nilai")?.SetValue(apInstance, trx.Nilai);
		}
	}

	// Set party code (Supplier for AP, Customer for AR)
	apViewType.GetProperty("Supplier")?.SetValue(apInstance, 
		effectiveTarget.StartsWith("AP", StringComparison.OrdinalIgnoreCase)
			? trx.PartyCode 
			: null);

	apViewType.GetProperty("Debitur")?.SetValue(apInstance,
		effectiveTarget.StartsWith("AR", StringComparison.OrdinalIgnoreCase)
			? trx.PartyCode
			: null);

	// ========== INVOKE SERVICE ==========
	var serviceType = _serviceProvider.GetService(Type.GetType(serviceName));

	if (serviceType == null)
		continue;

	var addTransMethod = serviceType.GetType()
		.GetMethod("AddTransH", new[] { apViewType });

	if (addTransMethod != null)
	{
		var result = addTransMethod.Invoke(serviceType, new[] { apInstance });
		savedResults.Add($"OK: {trx.SrcCode} processed via {serviceName}");
	}
}
catch (Exception ex)
{
	throw new InvalidOperationException(
		$"Error processing {effectiveTarget} transaction: {ex.Message}", ex);
}
```

---

## 6️⃣ UNIT TEST EXAMPLE

**Location:** Your test project  
**Purpose:** Verify DeterminePaymentService works correctly

```csharp
using Xunit;
using eSoft.CashBank.Services;

public class CashBankServicesTests
{
	private readonly CashBankServices _service;

	public CashBankServicesTests()
	{
		_service = new CashBankServices(/* dependencies */);
	}

	[Theory]
	[InlineData("APDP", "DOWNPAYMENT", "eSoft.Hutang.Services.IPaymentApDpServices")]
	[InlineData("APDP", "PAYMENT", "eSoft.Hutang.Services.IPaymentApDpServices")]
	[InlineData("AP", "DOWNPAYMENT", "eSoft.Hutang.Services.IPaymentApDpServices")]
	[InlineData("AP", "PAYMENT", "eSoft.Hutang.Services.IPaymentApServices")]
	[InlineData("ARDP", "DOWNPAYMENT", "eSoft.Piutang.Services.IPaymentArDpServices")]
	[InlineData("ARDP", "PAYMENT", "eSoft.Piutang.Services.IPaymentArDpServices")]
	[InlineData("AR", "DOWNPAYMENT", "eSoft.Piutang.Services.IPaymentArDpServices")]
	[InlineData("AR", "PAYMENT", "eSoft.Piutang.Services.IPaymentArServices")]
	[InlineData("CB", "PAYMENT", null)]
	public void DeterminePaymentService_ReturnsCorrectService(
		string target, 
		string transactionType, 
		string expectedService)
	{
		// Arrange & Act
		var result = _service.DeterminePaymentService(target, transactionType);

		// Assert
		Assert.Equal(expectedService, result);
	}

	[Fact]
	public void DeterminePaymentService_WithNullTarget_ReturnsNull()
	{
		// Arrange, Act & Assert
		Assert.Null(_service.DeterminePaymentService(null, "PAYMENT"));
	}
}
```

---

## 7️⃣ DATABASE VALIDATION SQL

Run after saving APDP transaction to verify data:

```sql
-- ========== Check ApTransH (Header) ==========
SELECT 
	ApTransHId,
	Bukti,
	Supplier,
	Currency,
	Kurs,
	Nilai,
	Jumlah,
	Kode,
	Tanggal
FROM ApTransHs
WHERE Kode = '23'  -- DP marker
  AND Bukti LIKE 'DPY-%'  -- DP doc prefix
ORDER BY ApTransHId DESC
LIMIT 1;

-- Expected output:
-- Bukti: DPY-2501XX-00001
-- Currency: USD
-- Kurs: 15500.00
-- Nilai: 300.00
-- Jumlah: 4650000
-- Kode: 23

-- ========== Check ApHutang (Aging) ==========
SELECT 
	ApHutangId,
	Dokumen,
	Tgl,
	Supplier,
	Kode,
	Sisa,
	Kurs
FROM ApHutangs
WHERE Dokumen LIKE 'DPY-%'
ORDER BY ApHutangId DESC
LIMIT 1;

-- Expected:
-- Dokumen: DPY-2501XX-00001
-- Kode: CA (aging)
-- Sisa: Should sync with ApTransH.Jumlah

-- ========== Check CbTransH (Bank Mirror) ==========
SELECT 
	CbTransHId,
	Tgl,
	DocNo,
	KdBank,
	SrcCode,
	Saldo,
	KSaldo
FROM CbTransHs
WHERE DocNo LIKE 'DPY-%'
ORDER BY CbTransHId DESC
LIMIT 1;

-- Expected:
-- DocNo: DPY-2501XX-00001
-- SrcCode: AP
-- Saldo: -300 (or -300 USD)
-- KSaldo: -4650000

-- ========== Check ApSuppl (Master Balance) ==========
SELECT 
	Supplier,
	NamaSup,
	Hutang,
	UpdateDate
FROM ApSuppls
WHERE Supplier = 'SUPP001'
ORDER BY UpdateDate DESC
LIMIT 1;

-- Expected:
-- Hutang: Should decrease by 4,650,000
```

---

## 📋 VALIDATION RULES (C#)

Add these validation methods if needed:

```csharp
public bool ValidateBankTransaction(BankTransactionView transaction)
{
	// APDP requires Currency & Kurs
	if (transaction.Target == "APDP")
	{
		if (string.IsNullOrWhiteSpace(transaction.Currency))
		{
			throw new ValidationException("Currency is required for APDP");
		}

		if (transaction.Kurs <= 1m)
		{
			throw new ValidationException("Kurs must be > 1 for APDP");
		}
	}

	// AP with foreign currency should have Kurs > 1
	if (transaction.Target == "AP" && 
		!string.IsNullOrEmpty(transaction.Currency) && 
		!transaction.Currency.Equals("IDR", StringComparison.OrdinalIgnoreCase))
	{
		if (transaction.Kurs <= 1m)
		{
			throw new ValidationException("Kurs should be > 1 for foreign currency");
		}
	}

	// AR/ARDP should NOT have currency set
	if ((transaction.Target == "AR" || transaction.Target == "ARDP") && 
		transaction.Kurs > 1m)
	{
		LogWarning($"Currency input for {transaction.Target} will be ignored");
	}

	// Amount validation
	if (transaction.Amount <= 0)
	{
		throw new ValidationException("Amount must be > 0");
	}

	return true;
}
```

---

## 🎬 COPY-PASTE CHECKLIST

- [ ] Copy #1: BankTransactionView.cs - New properties
- [ ] Copy #2: BankTransaction.razor - Target dropdown
- [ ] Copy #3: BankTransaction.razor - Currency/Kurs UI  
- [ ] Copy #4: CashBankServices.cs - DeterminePaymentService method
- [ ] Copy #5: CashBankServices.cs - Update SaveTransactionsAsync
- [ ] Copy #6: Unit test - DeterminePaymentService tests
- [ ] Copy #7: SQL validation queries
- [ ] Adjust styling to match existing code
- [ ] Run build to verify compilation
- [ ] Test with sample data

---

**Status:** ✅ Copy-Paste Ready  
**Tested:** Pseudo-code (adapt to exact class names in your codebase)  
**Version:** 1.0
