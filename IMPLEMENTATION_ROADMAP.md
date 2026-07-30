# 🚀 IMPLEMENTATION ROADMAP: Sales Order Partial Fulfillment System

## 📊 SUMMARY: Issues yang Sudah Diidentifikasi

```
┌─────────────────────────────────────────────────────────┐
│ ISSUE 1: Penjualan Parsial SO (Multi-transaction)      │
│ Status: ❌ Belum supported                              │
│ Impact: HIGH - Core business requirement                │
│ Risk: MEDIUM                                             │
├─────────────────────────────────────────────────────────┤
│ ISSUE 2: Delete Transaksi Tidak Restore SO Status      │
│ Status: ❌ Bug critical                                 │
│ Impact: CRITICAL - Data integrity                       │
│ Risk: HIGH - Corrupt SO status                          │
├─────────────────────────────────────────────────────────┤
│ ISSUE 3: Edit SO Validation (Qty Decrease)              │
│ Status: ❌ Belum ada validasi                           │
│ Impact: MEDIUM - Prevent negative sisa qty              │
│ Risk: HIGH - Corrupt SO quantity                        │
├─────────────────────────────────────────────────────────┤
│ ISSUE 4: Delete SO Prevention (Has Transaction)        │
│ Status: ❌ Belum ada check                              │
│ Impact: MEDIUM - Prevent orphaned FK                    │
│ Risk: MEDIUM - Orphaned transaction                     │
└─────────────────────────────────────────────────────────┘
```

---

## 🎯 IMPLEMENTATION PRIORITY (Rekomendasi)

### **PHASE 1: BUG FIX (CRITICAL) - Week 1** 🔴

**Kerjakan duluan karena:**
- ✅ Fix critical data integrity bug
- ✅ Prevent data corruption
- ✅ Tidak dependent pada feature lain
- ✅ Simple implementation (1-2 file)

#### **P1.1: Fix Delete Transaksi Jual → Restore SO Status** (Day 1)
```
File to modify:
├── eSoft.Penjualan\Services\SalesCommandService.cs
│   └── Modify DelTransH() + add RestoreSalesOrderStatus()
├── eSoft.Order\Services\OrderSalesServices.cs
│   └── Add RestoreSalesOrderStatus()
└── eSoft.Order\Services\IOrderSalesServices.cs
	└── Add interface RestoreSalesOrderStatus()

Effort: 1-2 jam
Risk: LOW (isolated change)
Test: Delete transaksi → SO status restore ke "1"
```

**Alasan:**
- 🔴 **CRITICAL BUG**: Saat ini delete transaksi, SO status tetap "3" (locked)
- 💥 Bisa corrupt SO history selamanya
- 🔒 Dependency: Harus fixed sebelum implement partial fulfillment
- ✅ Walau belum ada partial, setidaknya restore logic sudah siap

#### **P1.2: Add Delete SO Prevention (Has Transaction Check)** (Day 1)
```
File to modify:
├── eSoft.Order\Services\OrderSalesServices.cs
│   ├── Add CanDeleteSalesOrder()
│   └── Modify DeleteSalesOrder() with validation
└── eSoft.Order\Services\IOrderSalesServices.cs
	├── Add CanDeleteSalesOrder() interface
	└── Add DeleteSalesOrder() interface

Effort: 1 jam
Risk: LOW
Test: Try delete SO with transaction → Error message
```

**Alasan:**
- 🔒 Prevent orphaned FK (transaction reference SO)
- 💥 Bisa crash application jika SO dihapus
- ✅ Support untuk partial fulfillment (SO tidak boleh delete)

---

### **PHASE 2: CORE FEATURE (HIGH) - Week 2** 🟠

**Kerjakan setelah PHASE 1 karena:**
- ✅ Needs foundation dari bug fixes
- ✅ Core business requirement
- ✅ Dependencies clear

#### **P2.1: Implement Partial Fulfillment Logic** (Day 3-4)
```
File to modify:
├── eSoft.Order\Services\OrderSalesServices.cs
│   ├── Modify SaveOrderAktif() → Smart status logic
│   ├── Add CalculateSalesOrderStatus()
│   └── Add TrackQtyTerjual()
├── eSoft.Penjualan\Services\SalesCommandService.cs
│   ├── Modify AddTransH() → Call smart SaveOrderAktif()
│   └── Modify DelTransH() → Call smart SaveOrderAktif()
└── eSoft.Penjualan\Model\OeTransD.cs (?)
	└── Check if need additional fields

Effort: 3-4 jam
Risk: MEDIUM (business logic)
Test: 
  - Create 2 partial transactions → SO partial status
  - Delete 1 transaction → SO still active
  - Create final transaction → SO complete
```

**Alasan:**
- ✅ **Foundation**: Bug fixes sudah solid
- ✅ **Clear requirement**: User bisa jual 2 dari 3, sisa 1 untuk transaksi berikutnya
- ✅ **Data ready**: Model sudah punya QtyTerima field
- ✅ **Service ready**: SaveOrderAktif() siap dimodify

**Smart Logic yang akan diimplementasikan:**
```csharp
public void SaveOrderAktifSmart(string noLpb)
{
	var salesOrder = GetOrderAktif(noLpb);
	var totalQtyTerjual = GetTotalQtyTerjual(noLpb);  // Sum dari OeTransDs

	if (totalQtyTerjual >= salesOrder.TotalQty)
	{
		salesOrder.Cek = "3";  // Selesai 100%
	}
	else if (totalQtyTerjual > 0)
	{
		salesOrder.Cek = "1";  // Partial (masih bisa transaksi)
		salesOrder.QtyTerima = totalQtyTerjual;  // Track
	}
	else
	{
		salesOrder.Cek = "1";  // Back to active
		salesOrder.QtyTerima = 0;
	}

	_context.SaveChanges();
}
```

#### **P2.2: Add Edit SO Validation (Qty Decrease Prevention)** (Day 5)
```
File to modify:
├── eSoft.Order\Services\OrderSalesServices.cs
│   ├── Add ValidateEditSalesOrderQty()
│   └── Modify EditSalesOrder() with validation
├── eSoft.Order\Services\IOrderSalesServices.cs
│   └── Add ValidateEditSalesOrderQty() interface
└── Accounting\Pages\ModuleJual\OrderJual\EditSalesOrder.razor
	└── Add validation warning (jika ada transaksi)

Effort: 2-3 jam
Risk: MEDIUM (validation logic)
Test:
  - Edit SO qty ↑ (3→5) → Success ✅
  - Edit SO qty ↓ (3→1) with transaction → Blocked ❌
  - Show warning message to user
```

**Alasan:**
- ✅ **Prevent**: SO qty decrease saat sudah ada transaksi
- ✅ **Safe**: Hanya allow increase (add more order)
- ✅ **UX friendly**: Clear warning message

---

### **PHASE 3: UI ENHANCEMENT (MEDIUM) - Week 2-3** 🟡

**Kerjakan setelah P2 karena:**
- ✅ User-facing feature
- ✅ Need business logic ready
- ✅ Can be done in parallel dengan P2

#### **P3.1: Add Editable Qty Column in AddTransOrderJual** (Day 4-5)
```
File to modify:
├── Accounting\Pages\ModuleJual\TransJual\AddTransOrderJual.razor
│   ├── Change Qty column → InputNumber (editable)
│   ├── Add validation: Qty ≤ QtyTersedia
│   ├── Show sisa qty after edit
│   └── Add clear error message
├── eSoft.Penjualan\View\OeTransDView.cs
│   ├── Add QtyTersedia (calculated)
│   └── Add QtyBo tracking
└── eSoft.Penjualan\View\OeTransHView.cs
	└── Update validation logic

Effort: 2-3 jam
Risk: LOW (UI only, validation separate)
Test:
  - Select SO with 3 items
  - Edit qty: 1,1,1 (partial) → Show sisa
  - Edit qty: 2,2,2 (over) → Error
  - Submit → Create transaksi dengan qty custom
```

**Alasan:**
- ✅ **UX**: User bisa customize qty per item
- ✅ **Clear**: Show available qty vs selected qty
- ✅ **Safe**: Validation prevent invalid input

---

## 📋 IMPLEMENTATION SCHEDULE

```
┌────────────────────────────────────────────────────┐
│ WEEK 1: PHASE 1 (Bug Fixes)                       │
├────────────────────────────────────────────────────┤
│ Day 1 (Monday):                                    │
│  ├─ P1.1: Fix Delete → Restore (2 jam)            │
│  └─ P1.2: Delete Prevention (1 jam)               │
│                                                    │
│ Day 2-3 (Tuesday-Wednesday):                      │
│  ├─ Testing & Bug fixes                           │
│  └─ Code review                                    │
│                                                    │
│ Status: Ready to deploy (Hot Fix)                 │
└────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────┐
│ WEEK 2: PHASE 2 (Core Feature)                    │
├────────────────────────────────────────────────────┤
│ Day 3 (Thursday) - Parallel dengan P3:            │
│  └─ P2.1: Partial Fulfillment Logic (4 jam)       │
│                                                    │
│ Day 4-5 (Friday-Saturday):                        │
│  ├─ P2.2: Edit SO Validation (3 jam)              │
│  ├─ P3.1: Editable Qty UI (3 jam) [Parallel]     │
│  └─ Integration testing                           │
│                                                    │
│ Status: Ready to test                             │
└────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────┐
│ WEEK 3: Final Testing & Polish                    │
├────────────────────────────────────────────────────┤
│  ├─ End-to-end testing                            │
│  ├─ Edge case testing                             │
│  ├─ Documentation                                 │
│  └─ UAT preparation                               │
│                                                    │
│ Status: Ready for production                      │
└────────────────────────────────────────────────────┘
```

---

## 🎯 QUICK ANSWER: "INI APA DULU?"

**URUTAN YANG HARUS DIKERJAKAN:**

```
┌─────────────────────────────────────────────────────────┐
│ STEP 1: FIX DELETE BUG (PALING URGENT) ❌→✅           │
│                                                         │
│ Masalah: Delete transaksi tidak restore SO status      │
│ File: SalesCommandService.cs + OrderSalesServices.cs   │
│ Waktu: 2 jam                                           │
│ Reason: CRITICAL BUG - Bisa corrupt data!             │
│         Harus fixed dulu sebelum partial feature       │
│         Walau belum ada partial, restore logic sudah   │
│         siap di-reuse                                 │
├─────────────────────────────────────────────────────────┤
│ STEP 2: SMART SAVE ORDER STATUS (FOUNDATION) 🟠       │
│                                                         │
│ Masalah: SaveOrderAktif() langsung tandai selesai     │
│ File: OrderSalesServices.cs                           │
│ Waktu: 3-4 jam                                         │
│ Reason: Foundation untuk partial fulfillment          │
│         Update logic: check total qty vs sisa qty      │
│         Jika partial → status "1", jika full → "3"    │
├─────────────────────────────────────────────────────────┤
│ STEP 3: ADD EDITABLE QTY UI (FEATURE) 🟡              │
│                                                         │
│ Masalah: User tidak bisa edit qty, harus ambil semua  │
│ File: AddTransOrderJual.razor                         │
│ Waktu: 2-3 jam                                         │
│ Reason: Enable user untuk pilih qty custom            │
│         UI dengan validation                          │
│         Depend pada STEP 2 logic                      │
├─────────────────────────────────────────────────────────┤
│ STEP 4: EDIT SO VALIDATION (PROTECTION) 🟡            │
│                                                         │
│ Masalah: User bisa edit SO qty jadi negative          │
│ File: OrderSalesServices.cs + EditSalesOrder.razor    │
│ Waktu: 2-3 jam                                         │
│ Reason: Protect SO from invalid edits                 │
│         Only allow qty increase                       │
│         Show warning message                         │
├─────────────────────────────────────────────────────────┤
│ STEP 5: DELETE SO PREVENTION (PROTECTION) 🟡          │
│                                                         │
│ Masalah: User bisa delete SO dengan transaksi aktif   │
│ File: OrderSalesServices.cs                           │
│ Waktu: 1 jam                                          │
│ Reason: Prevent orphaned FK + data corruption         │
│         Check if has any transaction reference        │
└─────────────────────────────────────────────────────────┘

TOTAL: ~13-15 jam (2 hari kerja penuh)
```

---

## 🚦 TRAFFIC LIGHT DECISION

| Prioritas | Yang Dikerjakan | Alasan |
|-----------|-----------------|--------|
| 🔴 **DO FIRST** | Fix Delete → Restore | Critical bug, prevent data corruption |
| 🔴 **DO FIRST** | Smart SaveOrderAktif() | Foundation untuk semua feature |
| 🟡 **DO SECOND** | Editable Qty UI | Enable partial order entry |
| 🟡 **DO SECOND** | Edit SO Validation | Protect from invalid edits |
| 🟡 **DO SECOND** | Delete SO Prevention | Protect from orphaned FK |

---

## ✅ VALIDATION CHECKLIST (Sebelum Mulai)

- [ ] Backup database sebelum changes
- [ ] Create feature branch: `feature/partial-fulfillment`
- [ ] All tests di-create sebelum implement code
- [ ] Code review checklist siap

---

## NEXT ACTION

**PILIH SALAH SATU:**

1. **👉 "Mulai STEP 1: Fix Delete Bug"**
   - Saya implement code sekarang
   - Paling urgent & simple

2. **"Mulai STEP 2: Smart SaveOrderAktif()"**
   - Saya implement foundation logic
   - Dependency untuk STEP 3

3. **"Mulai STEP 3: Editable Qty UI"**
   - Saya implement user interface
   - Most visible untuk user

4. **"Buat master plan yang lebih detail"**
   - Detailed implementation plan untuk setiap step
   - Test cases untuk setiap step

**Rekomendasi saya: Mulai dari STEP 1 (Fix Delete Bug) karena:**
- ✅ Paling critical
- ✅ Paling simple (low risk)
- ✅ Foundation untuk yang lain
- ✅ Bisa di-deploy as hotfix

Mau saya mulai?

