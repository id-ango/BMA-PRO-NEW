# STEP 3: Edit SO Validation Test Scenarios

## Implementation Summary

**Feature:** Prevent SO (Sales Order) quantity decreases when transactions already exist.

**Files Modified:**
1. `eSoft.Order\Services\IOrderSalesServices.cs` - Added interface method `ValidateEditSalesOrderQty`
2. `eSoft.Order\Services\OrderSalesServices.cs` - Implemented validation logic
3. `Accounting\Pages\ModuleJual\OrderJual\EditTransOrderJual.razor` - Integrated validation in edit flow

---

## Test Scenarios

### Test 1: Increase SO Quantity ✅ (SHOULD ALLOW)
**Scenario:** SO has 5 qty, user edits to 10 qty
- **Precondition:** SO exists with TotalQty = 5
- **Action:** Edit SO qty to 10
- **Expected Result:** 
  - ✅ Edit allowed
  - ✅ No validation error
  - ✅ SO updated to 10 qty
- **Validation Logic:** `newQty (10) > currentQty (5)` → Allow

---

### Test 2: No Change in SO Quantity ✅ (SHOULD ALLOW)
**Scenario:** SO has 5 qty, user edits to 5 qty (no change)
- **Precondition:** SO exists with TotalQty = 5
- **Action:** Edit SO with same qty (5)
- **Expected Result:**
  - ✅ Edit allowed
  - ✅ No validation error
  - ✅ SO updated successfully
- **Validation Logic:** `newQty (5) == currentQty (5)` → Allow

---

### Test 3: Decrease SO Quantity Without Any Sales ✅ (SHOULD ALLOW)
**Scenario:** SO has 5 qty with no sales yet, user edits to 3 qty
- **Precondition:** 
  - SO exists with TotalQty = 5
  - QtyTerima = 0 (no sales transactions)
- **Action:** Edit SO qty to 3
- **Expected Result:**
  - ✅ Edit allowed
  - ✅ No validation error
  - ✅ SO updated to 3 qty
- **Validation Logic:** `QtyTerima (0) == 0` → No sales exist → Allow decrease

---

### Test 4: Decrease SO Below Already-Sold Quantity ❌ (SHOULD BLOCK)
**Scenario:** SO has 5 qty, 3 already sold (QtyTerima=3), user tries to edit to 2 qty
- **Precondition:**
  - SO exists with TotalQty = 5
  - QtyTerima = 3 (3 units already sold)
  - NoPrj = "SO-2024-001" (PO/PI reference)
- **Action:** Edit SO qty to 2
- **Expected Result:**
  - ❌ Edit BLOCKED
  - ❌ Error message shown: 
	```
	Tidak dapat mengurangi qty SO menjadi 2. Sudah ada penjualan sebesar 3 qty. 
	Minimum qty yang diizinkan adalah 3.
	```
  - ❌ SO NOT updated
- **Validation Logic:** `remainingAfterDecrease (2 - 3 = -1) < 0` → Block with error message

---

### Test 5: Decrease SO to Exact Sold Quantity ✅ (SHOULD ALLOW)
**Scenario:** SO has 5 qty, 3 already sold (QtyTerima=3), user edits to 3 qty
- **Precondition:**
  - SO exists with TotalQty = 5
  - QtyTerima = 3 (3 units already sold)
- **Action:** Edit SO qty to 3 (exact match)
- **Expected Result:**
  - ✅ Edit allowed
  - ✅ No validation error
  - ✅ SO updated to 3 qty
  - ✅ Next sales cannot be created (already at max)
- **Validation Logic:** `remainingAfterDecrease (3 - 3 = 0) >= 0` → Allow

---

### Test 6: Multiple Decreases (Progressive Partial Sales) ✅
**Scenario:** SO 10 qty → Sell 3 → Edit to 7 → Sell 3 more → Edit to 5 → Sell 1
- **Precondition:** SO with 10 qty
- **Step 1:** Sell 3 items (QtyTerima = 3)
- **Step 2:** Edit SO to 7 qty (7 ≥ 3, OK)
- **Step 3:** Sell 3 more items (QtyTerima = 6)
- **Step 4:** Edit SO to 5 qty (5 < 6, BLOCKED)
- **Expected Result:**
  - ✅ Steps 1-3 succeed
  - ❌ Step 4 blocked with error: `Tidak dapat mengurangi qty SO menjadi 5... Minimum qty yang diizinkan adalah 6.`
- **Validation Logic:** Follows Test 4 pattern for final edit

---

### Test 7: UI Error Display
**Scenario:** Validation error should be displayed in modal dialog
- **Precondition:** Test 4 scenario
- **Expected Result:**
  - ❌ ModalDialogComponent shows with:
	- Title: "Alert"
	- Text: The error message from ValidateEditSalesOrderQty
	- TombolSave: false (read-only acknowledgment)
  - ✅ User can close dialog and fix edit
- **Verification:** ValidationMessage field is dynamically bound to ModalDialogComponent Text property

---

## Edge Cases

### Edge Case 1: SO Not Found
**Condition:** NoPrj references non-existent SO
- **Expected:** Error message: `Sales Order tidak ditemukan.`

### Edge Case 2: Concurrent Sales
**Condition:** Between validation and save, another transaction sells qty
- **Current Behavior:** No locking; last validation result applies
- **Note:** This is acceptable since SO qties can exceed sold by design (partial fulfillment)

### Edge Case 3: Zero Quantity SO
**Condition:** SO has 0 qty (shouldn't happen normally)
- **Expected:** Allow increase or no change; block any decrease

---

## Rollback / Undo

If validation blocks an edit:
1. User sees error dialog
2. User clicks "OK" to close dialog
3. Edit form remains open with values unchanged
4. User can adjust quantities and retry
5. NO data is saved (transactional safety)

---

## Summary Matrix

| Test | Scenario | newQty vs currentQty | QtyTerima | Result | Reason |
|------|----------|---------------------|-----------|--------|--------|
| 1 | Increase | 10 > 5 | Any | ✅ Allow | Increase always allowed |
| 2 | No change | 5 = 5 | Any | ✅ Allow | No change always allowed |
| 3 | Decrease, no sales | 3 < 5 | 0 | ✅ Allow | No committed qty |
| 4 | Decrease below sold | 2 < 5 | 3 | ❌ Block | newQty < QtyTerima |
| 5 | Decrease to sold | 3 < 5 | 3 | ✅ Allow | newQty = QtyTerima (boundary) |
| 6 | Progressive sales | Multi-step | Variable | Mixed | See scenario details |
| 7 | UI Display | Test 4 | 3 | ❌ Show Error | Modal dialog validation |

---

## Integration Notes

**Service Layer:**
- `ValidateEditSalesOrderQty` uses existing `DbContextOrder` and `PoTransHs` (no circular dependency)
- Returns tuple: `(bool canEdit, string message)` for clear true/false + reason

**Blazor Page:**
- `EditTransOrderJual.razor` calls validation before `service.EditTransH(Transh)`
- Stores original and current qty for comparison
- ModalDialogComponent displays validation errors dynamically
- No changes if validation fails (safe undo)

**Backward Compatibility:**
- Original `EditTransH` service method unchanged
- Validation is separate layer, can be disabled if needed
- Existing SO edit functionality not affected for qty increases

---

## Next Steps

After STEP 3 validation is verified:
- **STEP 4:** Delete SO Prevention (check for existing transactions before delete)
- **P3.1:** Add Editable Qty Column in AddTransOrderJual (UI enhancement)
- **Delete Restore:** Final check on `SalesCommandService.DelTransH()` to restore SO status

