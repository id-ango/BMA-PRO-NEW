# 🧪 TESTING GUIDE - BankTransaction APDP/ARDP Implementation

## ✅ Build Status
- **Status**: BUILD SUCCESSFUL ✅
- **Files Modified**: 3
- **Bug Fixes**: 2 (AR property mapping, JumHutang property)
- **Ready for Testing**: YES

---

## 📋 TEST SCENARIOS

### Scenario 1: CB (Cash Bank) - Regular Bank Entry
**Purpose**: Verify that regular bank transactions work without any changes

**Steps**:
1. Open BankTransaction page
2. Click a transaction row
3. **Target dropdown**: Select "CB (Bank)"
4. Verify:
   - ❌ No Currency field shown
   - ❌ No Kurs field shown
   - ❌ No Party selector shown (CB doesn't need it)
5. Save transaction
6. **Expected Result**: 
   - Transaction saved as direct cash bank entry
   - NO call to PaymentArServices or PaymentApServices
   - Balance reflected in CbBanks immediately

---

### Scenario 2: AP (Supplier Regular Payment)
**Purpose**: Verify AP regular payments with optional currency

**Steps**:
1. Open BankTransaction page
2. Click a transaction row
3. **Target dropdown**: Select "AP (Regular)"
4. **Supplier selector**: Pick a supplier (e.g., SUPP001)
5. Verify:
   - ✅ Currency field shown (optional)
   - ✅ Kurs field shown (optional)
   - ✅ Document selector shown (select 1+ outstanding docs)
   - ✅ TransactionType selector NOT shown (implicit: PAYMENT)
6. **Test 2A - Regular IDR Payment**:
   - Leave Currency empty (or "IDR")
   - Leave Kurs as 1
   - Select documents
   - Save
   - **Expected**: PaymentApServices.AddTransH() called, no currency data saved
7. **Test 2B - Optional Foreign Currency**:
   - Currency: "USD"
   - Kurs: "15500"
   - Should display: "1 USD = 15,500.00 IDR"
   - Amount: 1,000,000 IDR (or equivalent)
   - Save
   - **Expected**: PaymentApServices.AddTransH() called with Currency="USD", Kurs=15500
   - Check ApTransH: Currency="USD", Kurs=15500

---

### Scenario 3: AP + Down Payment Selection
**Purpose**: Verify that AP with explicit Down Payment selection routes to APDP service

**Steps**:
1. Open BankTransaction page
2. Click a transaction row
3. **Target dropdown**: Select "AP (Regular)"
4. **Supplier selector**: Pick a supplier
5. **TransactionType selector**: Should show with options (check if PAYMENT/DOWNPAYMENT present)
6. ⚠️ **Note**: Current implementation may not show TransactionType selector for AP
   - If NOT shown: AP assumes PAYMENT by default
   - If shown: You can select DOWNPAYMENT to route to APDP service
7. Save
8. **Expected**: Routes to appropriate service (PaymentApServices or PaymentApDpServices)

---

### Scenario 4: APDP (Supplier Down Payment) - Foreign Currency
**Purpose**: Verify APDP with mandatory currency/kurs

**Steps**:
1. Open BankTransaction page
2. Click a transaction row
3. **Target dropdown**: Select "APDP (DP)" ⭐ **Explicit APDP type**
4. **Supplier selector**: Pick a supplier (e.g., SUPP002)
5. **Amount**: Enter 4,650,000 IDR
6. Verify UI:
   - ✅ Currency field shown with **RED ASTERISK** (REQUIRED)
   - ✅ Kurs field shown with **RED ASTERISK** (REQUIRED)
   - ✅ No document selector (not applicable for DP)
7. **Test with Empty Currency** (should fail validation):
   - Leave Currency empty
   - Try to save
   - **Expected**: Validation error: "Currency is required for APDP"
8. **Test Valid APDP Entry**:
   - Currency: "USD"
   - Kurs: "15500"
   - Should display live: "1 USD = 15,500.00 IDR"
   - Foreign Amount Nilai: "300" (optional, calculated as 4,650,000 / 15500)
   - Save
9. **Expected Results**:
   - ✅ PaymentApDpServices.AddTransH() called
   - ✅ ApTransH created with:
	 - Bukti: "DPY-2502XX-00001" (DP prefix)
	 - Kode: "23" (DP marker)
	 - Currency: "USD"
	 - Kurs: 15500
	 - Nilai: 300
	 - Jumlah: 4,650,000
   - ✅ CbTransH mirror created with:
	 - DocNo: "DPY-2502XX-00001"
	 - Saldo: -300 (USD balance)
	 - KSaldo: -4,650,000 (IDR balance)
   - ✅ ApSuppl.Hutang updated: -4,650,000
   - ✅ CbBanks.Saldo updated: -4,650,000

---

### Scenario 5: AR (Customer Regular Payment)
**Purpose**: Verify AR regular payments (no currency)

**Steps**:
1. Open BankTransaction page
2. Click a transaction row
3. **Target dropdown**: Select "AR (Regular)"
4. **Customer selector**: Pick a customer (e.g., CUST001)
5. Verify:
   - ❌ Currency field HIDDEN (AR doesn't support multicurrency)
   - ❌ Kurs field HIDDEN
   - ✅ Document selector shown
   - ✅ TransactionType selector shown (or not, depending on implementation)
6. Select outstanding invoices
7. Save
8. **Expected Results**:
   - ✅ PaymentArServices.AddTransH() called (for PAYMENT)
   - ✅ ArTransH created (normal AR payment processing)
   - ✅ No currency fields saved (AR fields are NULL)

---

### Scenario 6: ARDP (Customer Down Payment)
**Purpose**: Verify ARDP customer down payment (no currency)

**Steps**:
1. Open BankTransaction page
2. Click a transaction row
3. **Target dropdown**: Select "ARDP (DP)" ⭐ **Explicit ARDP type**
4. **Customer selector**: Pick a customer (e.g., CUST002)
5. Verify:
   - ❌ Currency field HIDDEN (AR never uses currency)
   - ❌ Kurs field HIDDEN
   - ❌ Document selector hidden (not applicable for DP)
   - ❌ TransactionType selector hidden (always DP, explicit in Target)
6. **Amount**: Enter 5,000,000 IDR
7. Save
8. **Expected Results**:
   - ✅ PaymentArDpServices.AddTransH() called (not PaymentArServices)
   - ✅ ArTransH created with:
	 - Bukti: "UMY-2502XX-00001" (DP prefix for AR)
	 - Kode: "13" (DP marker)
	 - Customer: CUST002
	 - Jumlah: 5,000,000
   - ✅ Currency fields: NULL (not applicable)
   - ✅ CbBanks updated with 5,000,000 IDR

---

## 🔍 DATABASE VERIFICATION QUERIES

After running tests, verify data in SQL:

### Check AP Regular Payment (Scenario 2):
```sql
SELECT Bukti, Supplier, Currency, Kurs, Nilai, Jumlah, Kode 
FROM ApTransH 
WHERE Supplier = 'SUPP001' AND Kode <> '23' 
ORDER BY Bukti DESC LIMIT 1;
```
**Expected**: Kode='24' or similar (not '23' which is DP marker)

### Check APDP Down Payment (Scenario 4):
```sql
SELECT Bukti, Supplier, Currency, Kurs, Nilai, Jumlah, Kode 
FROM ApTransH 
WHERE Supplier = 'SUPP002' AND Kode = '23' 
ORDER BY Bukti DESC LIMIT 1;
```
**Expected**: 
- Kode = '23' (DP marker)
- Currency = 'USD'
- Kurs = 15500
- Nilai = 300
- Jumlah = 4,650,000

### Check AP Mirror in Bank (Scenario 4):
```sql
SELECT h.DocNo, h.Saldo, h.KSaldo, d.Terima, d.KTerima, d.KValue, d.SrcCode
FROM CbTransH h
JOIN CbTransD d ON h.CbTransHId = d.CbTransHId
WHERE h.DocNo LIKE 'DPY-%' 
ORDER BY h.DocNo DESC LIMIT 1;
```
**Expected**:
- DocNo = 'DPY-2502XX-00001'
- Saldo = -300 (foreign currency)
- KSaldo = -4,650,000 (IDR)
- Terima = 300
- KTerima = 4,650,000
- KValue = 15500
- SrcCode = 'AP'

### Check AR Down Payment (Scenario 6):
```sql
SELECT Bukti, Customer, Jumlah, Kode, Currency
FROM ArTransH 
WHERE Customer = 'CUST002' AND Kode = '13' 
ORDER BY Bukti DESC LIMIT 1;
```
**Expected**:
- Kode = '13' (DP marker)
- Currency = NULL (AR doesn't use currency)
- Bukvi prefix = 'UMY-'

---

## ⚠️ KNOWN LIMITATIONS & NOTES

1. **AR Never Supports Currency**: Even if you try to force Currency in the DB, AR services ignore it. This is by design.
2. **AP Currency is Optional**: Regular AP payments can have currency, but it's not required.
3. **APDP Currency is Required**: Only APDP explicitly requires currency/kurs input.
4. **TransactionType Selector**: Current implementation may not show this on the UI. This can be added later if needed for explicit AP/APDP DP selection within AP target.
5. **Document Selection**: 
   - AR/AP show document selector (outstanding invoices/POs)
   - ARDP/APDP do NOT show documents (pure DP entry, no doc allocation)

---

## 🐛 TROUBLESHOOTING

### If you see NullReferenceException on ARDP:
- **Status**: ✅ FIXED (see BUG_FIX_AR_ROUTING.md)
- This was due to wrong property name mapping ("Debitur" → "Customer")

### If Currency field shows for AR/ARDP:
- **Expected Behavior**: Currency should be HIDDEN for AR/ARDP
- **Fix**: Check BankTransaction.razor conditional logic for Target=="AR" or Target=="ARDP"

### If APDP validation doesn't require Currency:
- **Expected**: Currency field should show red asterisk and validation should fail if empty
- **Fix**: Verify BankTransaction.razor has proper validation attributes on Currency input

### If Kurs value doesn't populate:
- **Possible Cause**: Kurs field might not be property bound correctly
- **Check**: View model BankTransactionView.cs has `decimal Kurs { get; set; } = 1m;`

---

## ✅ SUCCESS CRITERIA

All 5 scenarios should pass without errors:
- ✅ Scenario 1: CB processes correctly
- ✅ Scenario 2: AP with optional currency works
- ✅ Scenario 3: AP+DP combo routes correctly
- ✅ Scenario 4: APDP with required currency/kurs works, data persists
- ✅ Scenario 5: AR processes without currency
- ✅ Scenario 6: ARDP processes without currency

If all pass: **Implementation is complete and production-ready!**

---

**Last Updated**: Post-implementation  
**Build Status**: ✅ Successful  
**Test Status**: Ready to run
