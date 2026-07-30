# ANALISIS: Delete Transaksi Penjualan dan Restore SO

## 🔴 MASALAH DITEMUKAN: **DELETE TRANSAKSI TIDAK RESTORE SO STATUS**

---

## 📊 FLOW SAAT INI

### **A. Saat CREATE Transaksi Penjualan dari SO**

```
AddTransOrderJual.razor (Line 307-325)
│
├─ User pilih SO (NoPrj)
│  └─ OnPI() → ambil semua detail SO
│
├─ User submit
│  └─ service.AddTransH(Transh, true)
│     ├─ Create OeTransH dengan NoPrj = SO nomor
│     ├─ Create OeTransDs dengan detail
│     └─ return transaksi jual
│
└─ serviceOrder.SaveOrderAktif(Transh.NoPrj)  ⚠️ Line 315
   └─ SO status → "3" (SELESAI/DITANDAI)

❌ RESULT: SO sekarang status "3", tidak bisa buat transaksi lagi
```

### **B. Saat DELETE Transaksi Penjualan**

```
TransJual.razor atau EditTransJual.razor
│
├─ User click Delete
│  └─ service.DelTransH(id)
│
└─ SalesCommandService.DelTransH() (Line 148-180)
   ├─ Ambil OeTransH dengan id
   ├─ Reverse inventory adjustment (Line 166)
   ├─ Reverse receivable (Line 170)
   ├─ Delete OeTransH (Line 173)
   ├─ Save changes ke semua context
   └─ return true

❌ RESULT: Transaksi jual dihapus, tapi SO status tetap "3" (TIDAK DI-RESTORE!)
		   SO tidak bisa digunakan untuk transaksi lagi
```

---

## 🔍 DETAIL MASALAH

### **1. Di AddTransOrderJual.razor (Line 315)**
```csharp
private void HandleValidSubmit()
{
	if (dblclick != 1) return;

	dblclick++;
	var newEdit = service.AddTransH(Transh, true);
	if (newEdit != null)
	{
		// ⚠️ SO DIBERI STATUS "3" (DITANDAI SELESAI)
		serviceOrder.SaveOrderAktif(Transh.NoPrj);  // ← MASALAH DI SINI

		navigationmanager.NavigateTo($"/ModulePrinting/PrintSuratJalan/{id}");
	}
}
```

### **2. Di OrderSalesServices.cs (Line 180-188)**
```csharp
public void SaveOrderAktif(string customer)
{
	_context.PoTransHs.Include(p => p.PoTransDs)
		.Where(x => x.NoLpb == customer).FirstOrDefault().Cek = "3";
	_context.SaveChanges();
}

// ⚠️ TIDAK ADA LOGIC UNTUK RESTORE KEMBALI KE "1"
// Ketika transaksi jual di-delete, SO tidak dikembalikan ke status "1"
```

### **3. Di SalesCommandService.cs (Line 148-180)**
```csharp
public async Task<bool> DelTransH(int id)
{
	using var scope = new TransactionScope(...);

	var existingTrans = _context.OeTransHs
		.Include(y => y.OeTransDs)
		.FirstOrDefault(x => x.OeTransHId == id);

	// ... reverse inventory & receivable ...

	_context.OeTransHs.Remove(existingTrans);
	await _context.SaveChangesAsync();

	// ❌ TIDAK ADA CODE UNTUK RESTORE SO STATUS!
	// Harusnya ada logic: 
	//   if (!string.IsNullOrEmpty(existingTrans.NoPrj))
	//   {
	//       var salesOrder = serviceOrder.GetOrderAktif(existingTrans.NoPrj);
	//       if (salesOrder != null && salesOrder.Cek == "3")
	//       {
	//           salesOrder.Cek = "1";  // Restore ke aktif
	//           _context.SaveChanges();
	//       }
	//   }

	return true;
}
```

---

## 🚨 SKENARIO PROBLEMATIK

```
Timeline:
┌────────────────────────────────────────────────────────────┐
│ 1. Create SO: Qty = 3, Status = "1" (aktif)              │
├────────────────────────────────────────────────────────────┤
│ 2. Create Transaksi Jual: Qty = 2 dari SO                │
│    → SO Status berubah: "1" → "3" (selesai)              │
├────────────────────────────────────────────────────────────┤
│ 3. Delete Transaksi Jual ❌ MASALAH!                     │
│    → SO Status TETAP "3" (tidak di-restore)              │
│    → User tidak bisa buat transaksi dari SO lagi         │
│    → Qty SO sisa = 1 TERBUANG                            │
├────────────────────────────────────────────────────────────┤
│ 4. Hasil AKHIR:                                           │
│    - SO ada, tapi tidak bisa dipakai (locked status "3") │
│    - Inventory sudah di-reverse (baik)                   │
│    - Tapi SO state tidak konsisten (buruk)               │
└────────────────────────────────────────────────────────────┘
```

---

## ✅ SOLUSI: RESTORE SO SETELAH DELETE TRANSAKSI

### **Opsi A: Restore ke "1" (Aktif) - SIMPLE**
```csharp
// Di SalesCommandService.DelTransH(), tambahkan:

public async Task<bool> DelTransH(int id)
{
	using var scope = new TransactionScope(...);

	var existingTrans = _context.OeTransHs
		.Include(y => y.OeTransDs)
		.FirstOrDefault(x => x.OeTransHId == id);

	if (existingTrans == null)
		return false;

	if (!string.IsNullOrEmpty(existingTrans.Cek) && 
		_salesReceivableService.HasSettlement(existingTrans.NoLpb))
		return false;

	// ✅ RESTORE SO STATUS sebelum delete transaksi
	if (!string.IsNullOrEmpty(existingTrans.NoPrj))
	{
		var salesOrder = _context.PoTransHs
			.FirstOrDefault(x => x.NoLpb == existingTrans.NoPrj);

		if (salesOrder != null && salesOrder.Cek == "3")
		{
			// Restore SO ke status "1" (aktif, bisa transaksi lagi)
			salesOrder.Cek = "1";
			_context.PoTransHs.Update(salesOrder);
		}
	}

	// Reverse inventory & receivable
	_salesInventoryAdjustmentService.ReverseDetails(existingTrans.OeTransDs, existingTrans.Kode);

	if (existingTrans.Cek == "1")
	{
		_salesReceivableService.ReverseExistingReceivable(existingTrans);
	}

	_context.OeTransHs.Remove(existingTrans);
	await _context.SaveChangesAsync();

	scope.Complete();
	return true;
}
```

### **Opsi B: Restore dengan Smart Logic - BETTER**
```csharp
// Buat method baru di OrderSalesServices:

public void RestoreSalesOrderStatus(string noLpb)
{
	var salesOrder = _context.PoTransHs
		.Include(x => x.PoTransDs)
		.FirstOrDefault(x => x.NoLpb == noLpb);

	if (salesOrder == null)
		return;

	// Hitung total qty yang sudah dijual dari SO ini
	var totalQtyTerjual = _context.OeTransDs  // OeTransH reference ke SO
		.Where(x => x.NoLpb == noLpb)
		.Sum(x => x.Qty);

	// Jika tidak ada transaksi jual lagi → restore ke "1" (aktif)
	if (totalQtyTerjual == 0)
	{
		salesOrder.Cek = "1";  // Aktif lagi
	}
	else if (totalQtyTerjual < salesOrder.TotalQty)
	{
		// Masih ada sisa → tetap "1" (partial)
		salesOrder.Cek = "1";
	}
	// else jika totalQtyTerjual >= TotalQty → tetap "3" (selesai)

	_context.PoTransHs.Update(salesOrder);
	_context.SaveChanges();
}

// Panggil dari DelTransH:
if (!string.IsNullOrEmpty(existingTrans.NoPrj))
{
	RestoreSalesOrderStatus(existingTrans.NoPrj);
}
```

---

## 📋 IMPLEMENTASI CHECKLIST

| File | Perubahan | Priority |
|------|-----------|----------|
| **SalesCommandService.cs** | Tambah restore SO logic di DelTransH() | 🔴 HIGH |
| **OrderSalesServices.cs** | Tambah method RestoreSalesOrderStatus() | 🔴 HIGH |
| **IOrderSalesServices.cs** | Tambah interface RestoreSalesOrderStatus() | 🟡 MEDIUM |
| **AddTransOrderJual.razor** | Optional: Show warning jika SO status | 🟡 MEDIUM |

---

## 🎯 REKOMENDASI

**Gunakan Opsi B (Smart Logic)** karena:
- ✅ Lebih fleksibel untuk partial fulfillment
- ✅ Tracking QtyTerjual yang akurat
- ✅ SO status selalu konsisten dengan reality
- ✅ Support untuk transaksi parsial berulang

**Urutan implementasi:**
1. ✏️ Tambah method `RestoreSalesOrderStatus()` di OrderSalesServices
2. ✏️ Modify `DelTransH()` di SalesCommandService untuk call restore
3. ✏️ Test: Delete transaksi jual → SO status kembali "1"
4. ✏️ Test: Create transaksi baru dari SO → harus work

---

## 🧪 TEST CASE

```
Test 1: Delete Transaksi Partial
┌─────────────────────────────────────────────┐
│ SO: Qty = 3, Status = "1"                   │
├─────────────────────────────────────────────┤
│ Create Transaksi Jual: 2 unit               │
│ → SO Status = "3" (selesai)                 │
├─────────────────────────────────────────────┤
│ Delete Transaksi Jual ✅                   │
│ → SO Status = "1" (aktif lagi)             │
│ → Can create new transaksi dari SO? YES ✅│
└─────────────────────────────────────────────┘

Test 2: Delete Transaksi Partial, Check QtyTerjual
┌─────────────────────────────────────────────┐
│ SO: Qty = 5, Status = "1"                   │
├─────────────────────────────────────────────┤
│ Create Transaksi Jual 1: 2 unit             │
│ Create Transaksi Jual 2: 2 unit             │
│ → SO Status = "3" (selesai)                 │
├─────────────────────────────────────────────┤
│ Delete Transaksi Jual 2 ✅                 │
│ → Total dijual = 2, Sisa = 3                │
│ → SO Status = "1" (aktif, partial)         │
│ → Can create new transaksi untuk 3 unit? YES✅
└─────────────────────────────────────────────┘
```

---

Mau saya implement code-nya sekarang?

