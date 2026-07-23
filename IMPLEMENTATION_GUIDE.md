# Contoh Implementasi: Menambah AR/AP DownPayment ke BankTransaction

## 1. LANGKAH 1: Update BankTransactionView (eSoft.CashBank/View/BankTransactionView.cs)

```csharp
// File: eSoft.CashBank/View/BankTransactionView.cs

public class BankTransactionView
{
	// Existing properties...
	public DateTime Tanggal { get; set; }
	public string Description { get; set; }
	public decimal Amount { get; set; }
	public string Type { get; set; }  // "CR" / "DB"
	public string KodeBank { get; set; }
	public string SrcCode { get; set; }
	public string Target { get; set; }  // "AP", "AR", or blank

	// ✅ NEW PROPERTY: Explicit DP marker
	public string TransactionType { get; set; } = "PAYMENT";
	// Values: "PAYMENT" (default), "DOWNPAYMENT", "ADJUSTMENT"

	// Existing...
	public List<OutstandingDocView> OutstandingDocs { get; set; }
	public string PartyCode { get; set; }
	public string NoPrj { get; set; }
	public bool IsSelected { get; set; }
}
```

---

## 2. LANGKAH 2: Update SaveTransactionsAsync - Explicit DP Routing

```csharp
// File: eSoft.CashBank/Services/CashBankServices.cs
// Method: SaveTransactionsAsync (Update around line 1218+)

private string DeterminePaymentService(BankTransactionView trx, string targetCode)
{
	// Auto-detect DownPayment if:
	// 1. Explicitly marked as DOWNPAYMENT, OR
	// 2. No outstanding documents selected
	bool isDownPayment = trx.TransactionType == "DOWNPAYMENT"
					  || string.Equals(trx.TransactionType, "DOWNPAYMENT", 
									   StringComparison.OrdinalIgnoreCase)
					  || (trx.OutstandingDocs == null || trx.OutstandingDocs.Count == 0);

	if (string.Equals(targetCode, "AP", StringComparison.OrdinalIgnoreCase))
	{
		return isDownPayment
			? "eSoft.Hutang.Services.IPaymentApDpServices"
			: "eSoft.Hutang.Services.IPaymentApServices";
	}
	else if (string.Equals(targetCode, "AR", StringComparison.OrdinalIgnoreCase))
	{
		return isDownPayment
			? "eSoft.Piutang.Services.IPaymentArDpServices"
			: "eSoft.Piutang.Services.IPaymentArServices";
	}

	return null;
}

// In SaveTransactionsAsync, replace the AP routing section:
public async Task SaveTransactionsAsync(List<BankTransactionView> transactions, 
									   DateTime formDate, 
									   string kodeBank, 
									   string tambah, 
									   string kurang)
{
	// ... existing code ...

	foreach (var trx in apArTransactions)
	{
		try
		{
			var paymentDate = ResolveCsvPaymentDate(trx, formDate);
			var effectiveTarget = !string.IsNullOrEmpty(trx.Target)
				? trx.Target
				: (trx.SrcCode ?? string.Empty);

			if (string.Equals(effectiveTarget, "AP", StringComparison.OrdinalIgnoreCase))
			{
				// ✅ UPDATED: Get correct service (DP or Regular)
				string serviceName = DeterminePaymentService(trx, "AP");

				var apServiceType = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(a => a.GetTypesSafe())
					.FirstOrDefault(t => t.FullName == serviceName 
									  || t.FullName == serviceName.Replace("I", ""));

				if (apServiceType == null)
					throw new InvalidOperationException(
						$"AP payment service '{serviceName}' not found in loaded assemblies.");

				// ... rest of AP routing logic ...
			}
			else if (string.Equals(effectiveTarget, "AR", StringComparison.OrdinalIgnoreCase))
			{
				// ✅ UPDATED: Get correct service (DP or Regular)
				string serviceName = DeterminePaymentService(trx, "AR");

				var arServiceType = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(a => a.GetTypesSafe())
					.FirstOrDefault(t => t.FullName == serviceName 
									  || t.FullName == serviceName.Replace("I", ""));

				if (arServiceType == null)
					throw new InvalidOperationException(
						$"AR payment service '{serviceName}' not found in loaded assemblies.");

				// ... rest of AR routing logic ...
			}
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException(
				$"Failed to process payment for {effectiveTarget}: {ex.Message}", ex);
		}
	}

	// ... rest of method ...
}
```

---

## 3. LANGKAH 3: Update BankTransaction.razor UI

```razor
@* File: Accounting/Pages/ModuleBank/BankTransaction/BankTransaction.razor *@

<EditForm Model="@bankTransaction">
	<div class="form-group">
		<label>Bank:</label>
		<select class="form-control" @bind="bankTransaction.KodeBank">
			<option value="">-- Select Bank --</option>
			@foreach (var bank in Banks)
			{
				<option value="@bank.KodeBank">@bank.NmBank</option>
			}
		</select>
	</div>

	<div class="form-group">
		<label>Amount:</label>
		<input type="number" class="form-control" @bind="bankTransaction.Amount" />
	</div>

	<div class="form-group">
		<label>Target (Optional):</label>
		<select class="form-control" @bind="bankTransaction.Target">
			<option value="">-- Bank Transaction --</option>
			<option value="AP">Accounts Payable (AP)</option>
			<option value="AR">Accounts Receivable (AR)</option>
		</select>
	</div>

	<!-- ✅ NEW: Explicit Transaction Type Selector -->
	<div class="form-group">
		<label>Transaction Type:</label>
		<select class="form-control" @bind="bankTransaction.TransactionType">
			<option value="PAYMENT">Regular Payment</option>
			<option value="DOWNPAYMENT">Down Payment</option>
			<option value="ADJUSTMENT">Adjustment</option>
		</select>
		<small class="form-text text-muted">
			<strong>Note:</strong>
			<ul>
				<li><strong>Regular Payment:</strong> Payment against specific invoices</li>
				<li><strong>Down Payment:</strong> Prepayment before invoice received (DP)</li>
				<li><strong>Adjustment:</strong> Other cash movements</li>
			</ul>
		</small>
	</div>

	<div class="form-group">
		<label>Party Code (Customer/Supplier):</label>
		<input type="text" class="form-control" @bind="bankTransaction.PartyCode" 
			   placeholder="Auto-filled from description if empty" />
	</div>

	<div class="form-group">
		<label>Description:</label>
		<input type="text" class="form-control" @bind="bankTransaction.Description" />
	</div>

	<button type="button" class="btn btn-primary" @onclick="SaveTransaction">
		Save
	</button>
</EditForm>

@code {
	private BankTransactionView bankTransaction = new();
	private List<CbBank> Banks = new();

	// ... existing code ...

	private async Task SaveTransaction()
	{
		try
		{
			// Validation
			if (string.IsNullOrEmpty(bankTransaction.KodeBank))
			{
				// Show error
				return;
			}

			if (string.Equals(bankTransaction.Target, "AP", StringComparison.OrdinalIgnoreCase) ||
				string.Equals(bankTransaction.Target, "AR", StringComparison.OrdinalIgnoreCase))
			{
				if (string.IsNullOrEmpty(bankTransaction.PartyCode))
				{
					// Show error: Party Code required for AP/AR
					return;
				}

				if (string.IsNullOrEmpty(bankTransaction.TransactionType))
				{
					bankTransaction.TransactionType = "PAYMENT";
				}
			}

			// Call service with selected transactions
			await CashBankService.SaveTransactionsAsync(
				new List<BankTransactionView> { bankTransaction },
				DateTime.Now,
				bankTransaction.KodeBank,
				"tambah",
				"kurang"
			);

			// Success! Show message
		}
		catch (Exception ex)
		{
			// Show error: ex.Message
		}
	}
}
```

---

## 4. LANGKAH 4: Modifikasi atau Validasi Service Registration (Startup/Program.cs)

```csharp
// File: Program.cs or Startup.cs
// Ensure both regular and DP services are registered

public void ConfigureServices(IServiceCollection services)
{
	// ... existing registrations ...

	// ✅ Register AR Payment Services
	services.AddScoped<IPaymentArServices, PaymentArServices>();
	services.AddScoped<IPaymentArDpServices, PaymentArDpServices>();  // ← DP Service

	// ✅ Register AP Payment Services
	services.AddScoped<IPaymentApServices, PaymentApServices>();
	services.AddScoped<IPaymentApDpServices, PaymentApDpServices>();  // ← DP Service

	// ✅ Register CashBank Service (already exists)
	services.AddScoped<ICashBankServices, CashBankServices>();

	// ✅ Ensure reflection can find them
	services.AddScoped<IServiceProvider>(_ => services.BuildServiceProvider());
}
```

---

## 5. LANGKAH 5: Test Case Examples

```csharp
// File: [Test Project]/CashBankServiceTests.cs

[TestClass]
public class CashBankServiceDownPaymentTests
{
	private CashBankServices _cashBankService;
	private IPaymentArDpServices _arDpService;
	private IPaymentApDpServices _apDpService;
	private DbContextBank _contextBank;

	[TestInitialize]
	public void Setup()
	{
		// ... setup mocks and services ...
	}

	[TestMethod]
	public async Task SaveTransactionsAsync_WithARDownPayment_CreatesArTransH()
	{
		// Arrange
		var transactions = new List<BankTransactionView>
		{
			new BankTransactionView
			{
				Tanggal = DateTime.Now,
				Amount = 5000000,
				KodeBank = "BCA",
				Target = "AR",
				TransactionType = "DOWNPAYMENT",  // ← Key!
				PartyCode = "CUST001",
				Description = "Prepayment from Customer 001",
				SrcCode = "AR",
				IsSelected = true
			}
		};

		// Act
		await _cashBankService.SaveTransactionsAsync(
			transactions, 
			DateTime.Now, 
			"BCA", 
			"tambah", 
			"kurang"
		);

		// Assert
		var arTransH = _context.ArTransHs.Where(x => x.Kode == "13").FirstOrDefault();
		Assert.IsNotNull(arTransH);
		Assert.AreEqual("CUST001", arTransH.Customer);
		Assert.AreEqual(5000000, arTransH.Jumlah);
		Assert.AreEqual("13", arTransH.Kode);  // DownPayment marker

		// Verify bank transaction created
		var cbTransH = _contextBank.CbTransHs
			.FirstOrDefault(x => x.SrcCode == "AR" && x.DocNo == arTransH.Bukti);
		Assert.IsNotNull(cbTransH);
	}

	[TestMethod]
	public async Task SaveTransactionsAsync_WithAPDownPayment_MultiCurrency_CreatesApTransH()
	{
		// Arrange
		var transactions = new List<BankTransactionView>
		{
			new BankTransactionView
			{
				Tanggal = DateTime.Now,
				Amount = 4650000,
				KodeBank = "MANDIRI",
				Target = "AP",
				TransactionType = "DOWNPAYMENT",  // ← Key!
				PartyCode = "SUPP001",
				Description = "Prepayment in USD",
				SrcCode = "AP",
				IsSelected = true
				// Note: Currency/Kurs info would come from ApTransHView
			}
		};

		// Act
		await _cashBankService.SaveTransactionsAsync(
			transactions,
			DateTime.Now,
			"MANDIRI",
			"tambah",
			"kurang"
		);

		// Assert
		var apTransH = _context.ApTransHs
			.Where(x => x.Kode == "23")  // DownPayment marker
			.FirstOrDefault();
		Assert.IsNotNull(apTransH);
		Assert.AreEqual("SUPP001", apTransH.Supplier);
		Assert.AreEqual(4650000, apTransH.Jumlah);
		Assert.AreEqual("23", apTransH.Kode);

		// Verify multi-currency support
		Assert.IsNotNull(apTransH.Currency);
		Assert.IsTrue(apTransH.Kurs > 0);
	}

	[TestMethod]
	public async Task SaveTransactionsAsync_WithoutDocuments_AutoDetectsDownPayment()
	{
		// Arrange: Transaction with NO OutstandingDocs (should auto-detect DP)
		var transactions = new List<BankTransactionView>
		{
			new BankTransactionView
			{
				Tanggal = DateTime.Now,
				Amount = 5000000,
				KodeBank = "BCA",
				Target = "AR",
				TransactionType = "PAYMENT",  // ← Set to PAYMENT
				PartyCode = "CUST002",
				Description = "Auto-detect DP (no docs)",
				SrcCode = "AR",
				IsSelected = true,
				OutstandingDocs = null  // ← No documents = DP!
			}
		};

		// Act
		await _cashBankService.SaveTransactionsAsync(
			transactions,
			DateTime.Now,
			"BCA",
			"tambah",
			"kurang"
		);

		// Assert: Should still create ArTransH with Kode="13" (DP)
		var arTransH = _context.ArTransHs
			.Where(x => x.Kode == "13" && x.Customer == "CUST002")
			.FirstOrDefault();
		Assert.IsNotNull(arTransH);  // Auto-detected as DP
	}
}
```

---

## 6. LANGKAH 6: Database Verification Queries

```sql
-- Verify AR DownPayment created
SELECT 
	t.ArTransHId,
	t.Bukti,
	t.Customer,
	t.Tanggal,
	t.Jumlah,
	t.Unapplied,
	t.Kode,
	t.KdBank
FROM ArTransHs t
WHERE t.Kode = '13'  -- DownPayment marker
ORDER BY t.Tanggal DESC;

-- Verify AP DownPayment created (with currency)
SELECT 
	t.ApTransHId,
	t.Bukti,
	t.Supplier,
	t.Tanggal,
	t.Currency,
	t.Kurs,
	t.Nilai,
	t.Jumlah,
	t.Unapplied,
	t.Kode,
	t.KdBank
FROM ApTransHs t
WHERE t.Kode = '23'  -- DownPayment marker
ORDER BY t.Tanggal DESC;

-- Verify Bank Transaction created for AR DP
SELECT 
	h.CbTransHId,
	h.DocNo,
	h.KodeBank,
	h.Tanggal,
	h.Saldo,
	h.KSaldo,
	d.SrcCode,
	d.Keterangan,
	d.Terima,
	d.Bayar,
	d.Jumlah
FROM CbTransHs h
LEFT JOIN CbTransDs d ON h.CbTransHId = d.CbTransHId
WHERE h.DocNo LIKE 'UMY-%'  -- AR DP format
ORDER BY h.Tanggal DESC;

-- Verify Bank Transaction created for AP DP
SELECT 
	h.CbTransHId,
	h.DocNo,
	h.KodeBank,
	h.Tanggal,
	h.Saldo,
	h.KSaldo,
	d.SrcCode,
	d.Keterangan,
	d.Terima,
	d.Bayar,
	d.KTerima,
	d.KBayar,
	d.Jumlah,
	d.KJumlah
FROM CbTransHs h
LEFT JOIN CbTransDs d ON h.CbTransHId = d.CbTransHId
WHERE h.DocNo LIKE 'DPY-%'  -- AP DP format
ORDER BY h.Tanggal DESC;

-- Verify Customer balance updated (AR DP)
SELECT 
	Customer,
	NamaCust,
	Piutang
FROM ArCusts
WHERE Piutang <> 0
ORDER BY Piutang DESC;

-- Verify Supplier balance updated (AP DP)
SELECT 
	Supplier,
	NamaSup,
	Hutang
FROM ApSuppls
WHERE Hutang <> 0
ORDER BY Hutang DESC;

-- Verify Bank balance updated
SELECT 
	KodeBank,
	NmBank,
	Saldo,
	KSaldo
FROM CbBanks
ORDER BY KodeBank;
```

---

## 7. Implementation Checklist

- [ ] Step 1: Update `BankTransactionView.cs` - Add `TransactionType` property
- [ ] Step 2: Update `SaveTransactionsAsync()` - Add `DeterminePaymentService()` method
- [ ] Step 3: Update `BankTransaction.razor` - Add UI for TransactionType
- [ ] Step 4: Verify DI registration in `Program.cs`
- [ ] Step 5: Write and run unit tests
- [ ] Step 6: Test in development environment
- [ ] Step 7: Run database verification queries
- [ ] Step 8: Document in wiki/knowledge base

---

## 8. Known Considerations

### ✅ Advantages:
1. **Explicit routing** - Clear which service is called
2. **Auto-detection** - Falls back to smart logic if not specified
3. **Backward compatible** - Existing code continues to work
4. **Multi-currency** - AP DP supports foreign currency

### ⚠️ Considerations:
1. **Service availability** - Both services must be registered in DI
2. **Error handling** - Add proper error messages for missing services
3. **Concurrency** - Each service manages its own transaction (no cross-context issues)
4. **Audit trail** - Ensure Kode field is consistent ("13" for AR DP, "23" for AP DP)

---

**Document Version:** 1.0
**Last Updated:** 2024
**Status:** Ready for Implementation
