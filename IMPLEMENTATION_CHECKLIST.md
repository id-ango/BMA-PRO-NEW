# ✅ IMPLEMENTATION CHECKLIST - APDP & ARDP dengan Multi-Currency

## 📋 PHASE 1: PREPARATION (Before Coding)

- [ ] Read `SOLUSI_APDP_ARDP_MULTI_CURRENCY.md` (complete understanding)
- [ ] Review `UI_UX_MOCKUP_APDP_ARDP.md` (visual reference)
- [ ] Check current `BankTransactionView.cs` structure
- [ ] Backup current code
- [ ] Create feature branch: `feature/apdp-ardp-multi-currency`

---

## 📝 PHASE 2: MODEL CHANGES (Step 1)

**File: `eSoft.CashBank/View/BankTransactionView.cs`**

- [ ] Add `public string Currency { get; set; } = "IDR";`
- [ ] Add `public decimal Kurs { get; set; } = 1m;`
- [ ] Add `public decimal Nilai { get; set; }`
- [ ] Add `public string TransactionType { get; set; } = "PAYMENT";`
- [ ] Verify OutstandingDocView already has `Kurs` property
- [ ] Build & verify no compilation errors

### Code Changes:
```csharp
// Add these properties to BankTransactionView:
public string Currency { get; set; } = "IDR";
public decimal Kurs { get; set; } = 1m;
public decimal Nilai { get; set; }
public string TransactionType { get; set; } = "PAYMENT";
```

---

## 🎨 PHASE 3: UI CHANGES (Step 2-3)

**File: `Accounting/Pages/ModuleBank/BankTransaction/BankTransaction.razor`**

### Part A: Update Target Dropdown (Line ~260-264)

- [ ] Find `<InputSelect class="form-control form-control-sm" @bind-Value="ctx.Target">`
- [ ] Change options:
  - [ ] Add `<option value="ARDP">ARDP (A/R Down Payment)</option>`
  - [ ] Add `<option value="APDP">APDP (A/P Down Payment)</option>`
- [ ] Update text labels for clarity (CB, AR, ARDP, AP, APDP)

### Part B: Add Currency/Kurs UI Section

Find the section: `@if (ctx.Target == "AR" || ctx.Target == "AP")`

- [ ] Keep existing Customer/Supplier selector logic
- [ ] Add TransactionType selector dropdown:
  ```razor
  @if (ctx.Target == "AR" || ctx.Target == "AP")
  {
	  <!-- TransactionType selector -->
  }
  ```
- [ ] Add Currency/Kurs section (after TransactionType):
  ```razor
  @if (ctx.Target == "AP" || ctx.Target == "APDP")
  {
	  <!-- Currency, Kurs, Nilai inputs -->
  }
  ```

### Part C: Visibility Logic

- [ ] Currency section should be:
  - [ ] ✅ VISIBLE when Target = "AP" or "APDP"
  - [ ] ❌ HIDDEN when Target = "CB", "AR", "ARDP"
- [ ] TransactionType should be:
  - [ ] ✅ VISIBLE when Target = "AR" or "AP"
  - [ ] ❌ HIDDEN when Target = "CB", "ARDP", "APDP" (auto-set)
- [ ] Build & test in Browser (DevTools F12 to verify element visibility)

---

## 💻 PHASE 4: SERVICE LOGIC CHANGES (Step 4)

**File: `eSoft.CashBank/Services/CashBankServices.cs`**

### Part A: Add Helper Method

- [ ] Add `DeterminePaymentService()` method around line 1180:
```csharp
private string DeterminePaymentService(string target, string transactionType)
{
	if (target.Equals("APDP", StringComparison.OrdinalIgnoreCase))
		return "eSoft.Hutang.Services.IPaymentApDpServices";

	if (target.Equals("AP", StringComparison.OrdinalIgnoreCase) &&
		transactionType?.Equals("DOWNPAYMENT", StringComparison.OrdinalIgnoreCase) == true)
		return "eSoft.Hutang.Services.IPaymentApDpServices";

	if (target.Equals("AP", StringComparison.OrdinalIgnoreCase))
		return "eSoft.Hutang.Services.IPaymentApServices";

	if (target.Equals("ARDP", StringComparison.OrdinalIgnoreCase))
		return "eSoft.Piutang.Services.IPaymentArDpServices";

	if (target.Equals("AR", StringComparison.OrdinalIgnoreCase) &&
		transactionType?.Equals("DOWNPAYMENT", StringComparison.OrdinalIgnoreCase) == true)
		return "eSoft.Piutang.Services.IPaymentArDpServices";

	if (target.Equals("AR", StringComparison.OrdinalIgnoreCase))
		return "eSoft.Piutang.Services.IPaymentArServices";

	return null;
}
```

### Part B: Update SaveTransactionsAsync

Find line ~1218: `if (string.Equals(effectiveTarget, "AP", StringComparison.OrdinalIgnoreCase))`

- [ ] Replace service selection logic:
  ```csharp
  string serviceName = DeterminePaymentService(effectiveTarget, trx.TransactionType);
  ```

- [ ] Add Currency/Kurs property assignment (around line 1243):
  ```csharp
  if (effectiveTarget.StartsWith("AP", StringComparison.OrdinalIgnoreCase))
  {
	  apViewType.GetProperty("Currency")?.SetValue(apInstance, trx.Currency ?? "IDR");
	  apViewType.GetProperty("Kurs")?.SetValue(apInstance, trx.Kurs > 1m ? trx.Kurs : 1m);
	  apViewType.GetProperty("Nilai")?.SetValue(apInstance, trx.Nilai);
  }
  ```

- [ ] Similar logic for AR (around line 1350):
  ```csharp
  // AR doesn't need Currency/Kurs, but set defaults if needed
  ```

- [ ] Build & verify ServiceProvider can resolve new service names

---

## 🧪 PHASE 5: TESTING (Step X)

### Test 1: Model Properties
- [ ] Create BankTransactionView instance
- [ ] Verify `Currency`, `Kurs`, `Nilai`, `TransactionType` are accessible
- [ ] Verify defaults work (Currency="IDR", Kurs=1m, TransactionType="PAYMENT")

### Test 2: UI Visibility (Manual)
- [ ] Open BankTransaction.razor in Browser
- [ ] Select Target = "CB" → Currency field ❌ NOT visible
- [ ] Select Target = "AR" → Currency field ❌ NOT visible, TransactionType ✅ visible
- [ ] Select Target = "ARDP" → Currency field ❌ NOT visible
- [ ] Select Target = "AP" → Currency field ✅ visible, TransactionType ✅ visible
- [ ] Select Target = "APDP" → Currency field ✅ visible, TransactionType auto-set

### Test 3: Currency Calculation
- [ ] Input APDP transaction:
  - [ ] Supplier: SUPP001
  - [ ] Amount (IDR): 4,650,000
  - [ ] Currency: USD
  - [ ] Kurs: 15,500
  - [ ] Nilai (optional): 300
  - [ ] Verify: 300 × 15,500 = 4,650,000 ✓

### Test 4: Service Routing
**Key test:** Verify correct service is called

- [ ] AP + PAYMENT → PaymentApServices called
- [ ] AP + DOWNPAYMENT → PaymentApDpServices called
- [ ] APDP → PaymentApDpServices called
- [ ] AR + PAYMENT → PaymentArServices called
- [ ] AR + DOWNPAYMENT → PaymentArDpServices called
- [ ] ARDP → PaymentArDpServices called

**How to verify:**
```csharp
// Add logging in SaveTransactionsAsync:
var serviceName = DeterminePaymentService(effectiveTarget, trx.TransactionType);
System.Diagnostics.Debug.WriteLine($"Service selected: {serviceName}");
// Check in Debug Output window
```

### Test 5: Database Verification (SQL)

**After saving APDP transaction:**
```sql
-- Verify ApTransH created with currency info
SELECT ApTransHId, Bukti, Supplier, Currency, Kurs, Nilai, Jumlah, Kode
FROM ApTransHs
WHERE Kode = '23'  -- DP marker
ORDER BY ApTransHId DESC LIMIT 1;

-- Expected result:
-- Bukti: DPY-2502XX-00001
-- Currency: USD
-- Kurs: 15500.00
-- Nilai: 300.00
-- Jumlah: 4650000
-- Kode: 23

-- Verify ApHutang created
SELECT * FROM ApHutangs
WHERE Dokumen = 'DPY-2502XX-00001'
ORDER BY ApHutangId DESC LIMIT 1;

-- Verify CbTransH created (bank mirror)
SELECT * FROM CbTransHs
WHERE DocNo = 'DPY-2502XX-00001'
ORDER BY CbTransHId DESC LIMIT 1;
```

### Test 6: Multi-Currency Edge Cases
- [ ] Test Kurs = 1 (should be treated as IDR, no multi-currency)
- [ ] Test Kurs = 0 (should default to 1)
- [ ] Test Kurs = 15500.5678 (decimal precision)
- [ ] Test Amount = 0 (edge case)
- [ ] Test Nilai empty (should be optional for UI, calculated in service)

### Test 7: Validation
- [ ] APDP without Currency → Should show validation error
- [ ] APDP with Kurs <= 1 → Should allow (or show warning)
- [ ] AP with OutstandingDocs → Should work normally
- [ ] ARDP with Currency input → Should be ignored (hidden)

---

## 📊 PHASE 6: DATA VERIFICATION

**After successful save, run these queries:**

```sql
-- 1. Check ApTransH for APDP (Kode='23')
SELECT TOP 1 * FROM ApTransHs WHERE Kode = '23' ORDER BY ApTransHId DESC;

-- 2. Check ApHutang (aging/sisa)
SELECT TOP 1 * FROM ApHutangs 
WHERE Dokumen LIKE 'DPY-%' 
ORDER BY ApHutangId DESC;

-- 3. Check CbTransH (bank mirror)
SELECT TOP 1 h.*, d.* 
FROM CbTransHs h
LEFT JOIN CbTransDs d ON h.CbTransHId = d.CbTransHId
WHERE h.DocNo LIKE 'DPY-%'
ORDER BY h.CbTransHId DESC;

-- 4. Check ApSuppl balance updated
SELECT Supplier, NamaSup, Hutang FROM ApSuppls 
WHERE Supplier = 'SUPP001';

-- 5. Check CbBanks balance updated
SELECT KodeBank, NmBank, Saldo, KSaldo FROM CbBanks
WHERE KodeBank = 'MANDIRI';
```

---

## 🎯 PHASE 7: FINAL VALIDATION

- [ ] Build project → NO errors
- [ ] Run existing tests → ALL pass
- [ ] Create simple test transaction (APDP) → Save successful
- [ ] Check database → All affected tables updated correctly
- [ ] Check logs → No unexpected exceptions
- [ ] UI responsive → Looks good on desktop & tablet
- [ ] Currency field shows/hides correctly based on Target selection

---

## 🚀 PHASE 8: CODE REVIEW & CLEANUP

- [ ] Code review checklist:
  - [ ] No hardcoded strings (use constants)
  - [ ] Error handling added
  - [ ] Comments added for complex logic
  - [ ] Null checks included
  - [ ] Validation complete

- [ ] Remove debug logging (if added)
- [ ] Update XML documentation (if applicable)
- [ ] Check for TODOs or FIXMEs
- [ ] Verify naming conventions

---

## 📝 PHASE 9: DOCUMENTATION

- [ ] Update README or wiki with:
  - [ ] New Target options (ARDP, APDP)
  - [ ] Currency handling rules
  - [ ] When to use each type
  - [ ] Example screenshots

- [ ] Document API/Service changes (if external)

---

## 🎬 PHASE 10: DEPLOYMENT

- [ ] Merge feature branch to development
- [ ] Run full regression tests
- [ ] Deploy to staging environment
- [ ] QA testing by business team
- [ ] User documentation/training
- [ ] Deploy to production

---

## 📌 COMMON ISSUES & SOLUTIONS

| Issue | Solution |
|-------|----------|
| Currency field shows for AR | Check `@if (ctx.Target == "AP"...` condition |
| TransactionType not visible | Verify condition: `if (ctx.Target == "AR" \|\| ctx.Target == "AP")` |
| Service not found | Check DI registration in Program.cs |
| Kurs not saved to DB | Verify `apViewType.GetProperty("Kurs")?.SetValue()` |
| Currency empty in UI | Initialize default: `Currency = "IDR"` |
| Amount mismatch | Verify Nilai × Kurs = Amount calculation |

---

## ⏱️ TIME ESTIMATE

| Phase | Effort | Notes |
|-------|--------|-------|
| Phase 1: Prep | 30 min | Review & backup |
| Phase 2: Model | 15 min | 4 properties, no complexity |
| Phase 3: UI | 1-2 hrs | Conditional display, testing |
| Phase 4: Service | 1-2 hrs | Routing logic, property mapping |
| Phase 5: Testing | 2-3 hrs | Manual + SQL verification |
| Phase 6: Data Check | 30 min | Query results verification |
| Phase 7: Final | 30 min | Build, run tests |
| Phase 8: Review | 1 hr | Code quality |
| Phase 9: Docs | 1 hr | Wiki/README updates |
| **TOTAL** | **8-12 hours** | Including all testing |

---

## ✨ SUCCESS CRITERIA

- [x] All code compiles without errors
- [x] UI shows Currency field only for AP & APDP
- [x] TransactionType selector works for AR & AP
- [x] APDP transaction saves successfully
- [x] ApTransH contains Currency, Kurs, Nilai
- [x] CbTransH created with dual-amount (if Kurs > 1)
- [x] ApSuppl.Hutang decreased correctly
- [x] CbBanks balance updated correctly
- [x] All existing tests still pass
- [x] No breaking changes to existing functionality

---

**Ready to start?** Begin with Phase 2 ✅
**Questions?** Refer to SOLUSI_APDP_ARDP_MULTI_CURRENCY.md
**Visual reference?** Check UI_UX_MOCKUP_APDP_ARDP.md

---

**Document Version:** 1.0  
**Last Updated:** 2024  
**Status:** ✅ Ready
