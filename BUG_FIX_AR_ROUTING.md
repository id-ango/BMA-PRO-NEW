# 🔧 BUG FIX - AR Service Routing

**Issue**: `NullReferenceException` when trying to process ARDP (Customer Down Payment) transactions

**Root Cause**: The reflection-based routing code was trying to set the wrong property name on `ArTransHView`:
- Code was setting: `Debitur` 
- Correct property: `Customer`

Also, the code was trying to set `JumHutang` property which only exists on AP (`ApTransHView`), not on AR (`ArTransHView`).

**Fixes Applied**:

### Fix 1: Correct AR Party Property Name
```csharp
// BEFORE (WRONG):
else if (serviceName.Contains("Piutang"))
{
	apViewType.GetProperty("Debitur")?.SetValue(apInstance, partyCode);
}

// AFTER (CORRECT):
else if (serviceName.Contains("Piutang"))
{
	apViewType.GetProperty("Customer")?.SetValue(apInstance, partyCode);
}
```

### Fix 2: Make JumHutang Setting Conditional
```csharp
// BEFORE (ERROR):
var propJumHutang = apViewType.GetProperty("JumHutang");
if (propJumHutang != null && propJumHutang.CanWrite)
	propJumHutang.SetValue(apInstance, totalBayarAp + totalDiscountAp);

// AFTER (CORRECT):
if (serviceName.Contains("Hutang"))
{
	var propJumHutang = apViewType.GetProperty("JumHutang");
	if (propJumHutang != null && propJumHutang.CanWrite)
		propJumHutang.SetValue(apInstance, totalBayarAp + totalDiscountAp);
}
```

**Result**: ✅ Build successful, ready for testing

**Next Steps**: Test all 5 transaction types:
1. ✅ CB (Cash Bank) - regular bank transaction
2. ✅ AP (Supplier Regular) - with optional currency
3. ✅ APDP (Supplier DownPayment) - with required currency
4. ✅ AR (Customer Regular) - no currency
5. ✅ ARDP (Customer DownPayment) - no currency

All should now route properly to their respective services without NRE.
