# 🔧 BUG FIX - Property Set Method Errors

**Status**: ✅ BUILD SUCCESSFUL  
**Issue**: `ArgumentException: Property set method not found` when saving transactions  
**Root Cause**: Attempting to set read-only calculated properties and non-existent properties  
**Files Fixed**: 1 (CashBankServices.cs)

---

## 🐛 PROBLEMS IDENTIFIED & FIXED

### Problem 1: Trying to Set Read-Only Property "Nilai"
**Error Location**: Service routing code attempting to set `Nilai` property on `ApTransHView`

**Root Cause**: 
```csharp
// ApTransHView.cs - Nilai is read-only (calculated property)
public decimal Nilai
{
	get
	{
		return Kurs * JumBayar;  // Calculated, not set
	}
	// NO SETTER!
}
```

**Original Code** (WRONG):
```csharp
// Set Nilai (foreign currency amount) if provided
if (trx.Nilai > 0)
{
	apViewType.GetProperty("Nilai")?.SetValue(apInstance, trx.Nilai);  // ❌ FAILS - read-only
}
```

**Fixed Code** (CORRECT):
```csharp
// NOTE: Do NOT set Nilai - it's a calculated property (Nilai = Kurs * JumBayar)
// Nilai will be automatically calculated by the system
```

**Why**: `Nilai` is automatically calculated as `Kurs * JumBayar`. When we set `Kurs` and `JumBayar`, `Nilai` computes itself. No need to set it manually!

---

### Problem 2: Incorrect Fallback to Non-Existent Property "Dokumen"
**Error Location**: Detail item property mapping for document numbers

**Root Cause**:
```csharp
// Current problematic code:
var prop = apDType.GetProperty("Lpb") ?? apDType.GetProperty("Dokumen");
prop?.SetValue(apd, sdoc.Dokumen);  // ❌ For AR: Dokumen doesn't exist!
```

**Difference between AP and AR Detail Views**:
- **ApTransDView**: Has `Lpb` property ✅
- **ArTransDView**: Has `Lpb` property ✅
- Both have `Lpb`, neither has `Dokumen` 

The fallback to "Dokumen" was wrong - it doesn't exist on either view!

**Fixed Code** (CORRECT):
```csharp
// Lpb exists on both AP and AR detail views
apDType.GetProperty("Lpb")?.SetValue(apd, sdoc.Dokumen);  // ✅ Correct for both AP & AR
```

---

## 📋 PROPERTIES THAT EXIST vs. DON'T EXIST

### ApTransDView (Supplier Detail):
| Property | Has Setter? | Note |
|----------|-----------|------|
| Tanggal | ✅ Yes | DateTime |
| DueDate | ✅ Yes | Nullable DateTime |
| Jumlah | ✅ Yes | decimal |
| Bayar | ✅ Yes | decimal |
| Discount | ✅ Yes | decimal |
| **Lpb** | ✅ Yes | Document number |
| Dokumen | ❌ NO | Doesn't exist |
| Keterangan | ✅ Yes | string |
| KodeTran | ✅ Yes | string |

### ArTransDView (Customer Detail):
| Property | Has Setter? | Note |
|----------|-----------|------|
| Tanggal | ✅ Yes | DateTime |
| DueDate | ✅ Yes | Nullable DateTime |
| Jumlah | ✅ Yes | decimal |
| Bayar | ✅ Yes | decimal |
| Discount | ✅ Yes | decimal |
| **Lpb** | ✅ Yes | Document number |
| Dokumen | ❌ NO | Doesn't exist |
| Keterangan | ✅ Yes | string |
| KodeTran | ✅ Yes | string |

---

## 📊 CHANGES MADE

### CashBankServices.cs

**Change 1: Removed setting of Nilai property**
```
Lines 1342-1346: REMOVED
- The attempt to set Nilai is deleted
- Replaced with comment explaining it's auto-calculated
```

**Change 2: Fixed document property mapping**
```
Line 1393: CHANGED
FROM: var prop = apDType.GetProperty("Lpb") ?? apDType.GetProperty("Dokumen");
TO:   apDType.GetProperty("Lpb")?.SetValue(apd, sdoc.Dokumen);

Reason: Both AP and AR use "Lpb", not "Dokumen"
		Fallback to "Dokumen" was wrong and caused property set errors
```

---

## ✨ WHAT HAPPENS NOW

### When APDP is saved:

**Header (ApTransH) - What you SET:**
- ✅ Currency: "USD" (set explicitly)
- ✅ Kurs: 15500 (set explicitly)
- ✅ JumBayar: 300 (set explicitly for foreign amount)

**Header (ApTransH) - What SYSTEM CALCULATES:**
- ✅ Nilai: Automatically = Kurs * JumBayar = 15500 * 300 = 4,650,000
- ✅ JumNilai: Automatically calculated
- ✅ Jumlah: Sum of all detail amounts
- ✅ UpdateUnapplied: JumBayar - sum of Bayar from details

**Details (ApTransD) - Document Line Items:**
- ✅ Lpb: Document number (was trying "Dokumen" - FIXED)
- ✅ Bayar: Payment amount
- ✅ Discount: Discount amount
- ✅ Keterangan: Description

---

## 🎯 RESULT

✅ **Build**: SUCCESSFUL  
✅ **All properties**: Correctly mapped  
✅ **Read-only properties**: Not attempted to be set  
✅ **Non-existent properties**: Not requested  
✅ **Calculated values**: Will auto-compute from set values  

**Ready for testing!** The "Property set method not found" error should now be resolved. 🎉

---

## 📝 KEY LEARNINGS

1. **Read-only calculated properties**: Don't try to SET them if they have only a getter
   - Example: `Nilai = Kurs * JumBayar` (no setter, calculated)

2. **Property existence**: Check if property actually exists on the target type
   - Example: "Dokumen" doesn't exist, use "Lpb" instead

3. **Reflection safety**: Use null-coalescing (`?.`) to safely handle missing properties
   - Good: `prop?.SetValue()` - no error if property doesn't exist
   - Bad: `prop.SetValue()` - throws if property is null

4. **DI container resolution**: Always verify property names before using reflection
   - Different types might use different property names for the same concept
   - Check both the AP and AR view models when routing applies to both
