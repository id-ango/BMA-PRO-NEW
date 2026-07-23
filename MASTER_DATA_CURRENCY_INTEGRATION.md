# Master Data Currency Integration for Bank Transactions

## Summary
Currency is now **automatically sourced from supplier master data** (`ApSuppl.Kurs`) instead of requiring manual input. This change ensures consistency with your existing master data and removes ambiguity about when currency is required.

---

## How It Works

### For AP (Regular Supplier Payment)
```
User selects Supplier
	↓
OnSupplierChanged() triggers
	↓
Load supplier master (ApSuppl)
	↓
Extract Currency from ApSuppl.Kurs
	↓
Set ctx.Currency (default: "IDR" if blank)
	↓
UI shows Currency as READ-ONLY (from master)
	↓
User can still enter Kurs manually
```

**Result:**
- Currency field appears but is **read-only** with label: "Currency (from Supplier)"
- Value comes directly from the selected supplier master record
- Kurs (exchange rate) remains editable—user may override if transaction uses a different rate

---

### For APDP (Down Payment to Supplier)
```
User selects Supplier
	↓
OnSupplierChanged() triggers (same as AP)
	↓
Load supplier currency (ApSuppl.Kurs)
	↓
Set ctx.Currency
	↓
UI shows Currency as EDITABLE (user can override)
	↓
User must enter Kurs (required for APDP)
```

**Result:**
- Currency field appears and is **editable** with red asterisk (required)
- Default value comes from supplier, but user can change it
- Kurs input is required and marked with red asterisk

---

### For AR/ARDP (Customer Payment)
```
User selects Customer
	↓
OnCustomerChanged() triggers
	↓
AR customers don't support multicurrency
	↓
Set ctx.Currency = "IDR"
	↓
Set ctx.Kurs = 1m
	↓
Currency UI remains HIDDEN
```

**Result:**
- No currency field shown for AR or ARDP
- Automatically locked to IDR (1:1 ratio)
- This matches current AR model behavior (ArCust has no Kurs field)

---

## Code Changes

### 1. OnSupplierChanged() [BankTransaction.razor]
**Added supplier master currency lookup:**

```csharp
// Load Currency from supplier master (ApSuppl.Kurs)
var selectedSupplier = suppliers?.FirstOrDefault(s => s.Supplier == ctx.PartyCode);
if (selectedSupplier != null && !string.IsNullOrEmpty(selectedSupplier.Kurs))
{
	// Use supplier's default currency (e.g., "USD", "EUR")
	ctx.Currency = selectedSupplier.Kurs;
}
else
{
	// Default to IDR if supplier has no currency specified
	ctx.Currency = "IDR";
}
```

---

### 2. OnCustomerChanged() [BankTransaction.razor]
**Enforced AR-only IDR currency:**

```csharp
// AR customers don't support multicurrency - always use IDR
ctx.Currency = "IDR";
ctx.Kurs = 1m;
```

---

### 3. Currency UI Display [BankTransaction.razor]
**Conditional display based on Target:**

- **AP:** `readonly` input showing supplier's currency + helper text "(from Supplier)"
- **APDP:** Editable input with red asterisk, allowing user override if needed
- **AR/ARDP:** Completely hidden (handled by OnCustomerChanged)

---

## Data Flow Examples

### Scenario 1: AP Payment (Regular)
```
1. User selects Supplier "SUPP001" (Master shows Kurs="USD")
2. OnSupplierChanged() loads ApSuppl record
3. ctx.Currency = "USD" (auto-filled, read-only)
4. Outstanding docs load with their exchange rates
5. User enters Jumlah (IDR amount to pay)
6. If Kurs > 1: show "1 USD = 15500.00 IDR"
7. Save → PaymentApServices (Currency & Kurs included)
```

### Scenario 2: APDP Payment (Down Payment)
```
1. User selects Supplier "SUPP001" (Master shows Kurs="EUR")
2. OnSupplierChanged() loads ApSuppl record
3. ctx.Currency = "EUR" (auto-filled, BUT editable)
4. User can change to different currency if DP uses different rate
5. User enters Kurs = "16500" (for EUR)
6. User enters Nilai = "200" (200 EUR)
7. Show "1 EUR = 16500.00 IDR"
8. Save → PaymentApDpServices (Currency="EUR", Kurs=16500)
```

### Scenario 3: ARDP Payment (To Customer)
```
1. User selects Customer "CUST001"
2. OnCustomerChanged() locks to IDR
3. Currency field: HIDDEN
4. ctx.Currency = "IDR", ctx.Kurs = 1m (system-only)
5. User enters Jumlah (IDR amount)
6. Save → PaymentArDpServices (no currency fields used)
```

---

## Benefits

✅ **Consistency:** Supplier currency always comes from master data  
✅ **Reduced Errors:** No manual currency entry for regular AP payments  
✅ **Clarity:** System automatically determines if currency is needed  
✅ **Flexibility:** APDP still allows user override if different rate applies  
✅ **Standards Alignment:** AR remains IDR-only (matches existing model)  

---

## Fallback Logic

| Situation | Currency Resolved To | Notes |
|-----------|---------------------|-------|
| AP: Supplier Kurs set | ApSuppl.Kurs value | Master-driven, read-only in UI |
| AP: Supplier Kurs blank | "IDR" | Default fallback |
| APDP: Supplier Kurs set | ApSuppl.Kurs value | Initial value, editable override allowed |
| APDP: Supplier Kurs blank | "IDR" | Default fallback, user can edit |
| AR / ARDP | "IDR" | Always locked, never shown in UI |

---

## Next Steps (Optional Enhancements)

1. **Validate Currency Code:** Add list of valid currency codes (ISO 4217) and validate against it
2. **Supplier Currency Editing:** Allow editing supplier master Kurs directly from BankTransaction UI
3. **Historical Rate Lookup:** Fetch historical exchange rates if Kurs field matches a known date
4. **AR Multicurrency (Future):** If ArCust master gets a Kurs field, AR payments can also support multicurrency

---

## Files Modified

- ✅ `Accounting/Pages/ModuleBank/BankTransaction/BankTransaction.razor`
  - `OnSupplierChanged()`: Added master data currency lookup
  - `OnCustomerChanged()`: Added AR-only IDR enforcement
  - Currency UI: Conditional display based on Target (read-only for AP, editable for APDP, hidden for AR)

---

## Testing Checklist

- [ ] Select AP supplier with Kurs="USD" → Currency displays as read-only "USD"
- [ ] Select AP supplier with blank Kurs → Currency displays as read-only "IDR"
- [ ] Select APDP supplier with Kurs="EUR" → Currency displays editable "EUR"
- [ ] Edit APDP currency to "SGD" → Override allowed, not read-only
- [ ] Select AR customer → Currency field completely hidden
- [ ] Save AP transaction → PaymentApServices receives Currency from master
- [ ] Save APDP transaction → PaymentApDpServices receives Currency (master or user override)
- [ ] Save ARDP transaction → No currency fields in service call

---
