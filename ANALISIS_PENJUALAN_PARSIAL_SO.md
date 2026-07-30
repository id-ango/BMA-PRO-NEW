# Analisis Sistem Penjualan Parsial dari Sales Order (SO)

## Pertanyaan User
Apakah sistem sudah mendukung penjualan parsial dari Sales Order? Saat ini, apakah SO harus lengkap (100%) baru bisa dibuat transaksi penjualan, atau bisa sebagian item saja?

---

## STATUS SISTEM SAAT INI: ❌ BELUM MENDUKUNG PENJUALAN PARSIAL

### 🔴 Masalah Utama

**Saat ini sistem menggunakan model ALL-OR-NOTHING (All Items Required):**

1. **Ketika SO dipilih di AddTransOrderJual.razor (line 416-449):**
   - Semua item dari SO **otomatis terambil penuh** dengan `item.Qty` (qty order)
   - Tidak ada opsi untuk mengurangi qty per item
   - User hanya bisa edit form header (diskon, ongkos, PPN), bukan qty detail

   ```csharp
   public void OnPI(ChangeEventArgs args)
   {
	   transaksi = serviceOrder.GetOrderAktif(args.Value.ToString());
	   // ... ambil data header ...

	   Transh.OeTransDs.Clear();
	   if (transaksi != null)
	   {
		   foreach (var item in transaksi.PoTransDs)
		   {
			   Transh.OeTransDs.Add(new OeTransDView()
			   {
				   // ⚠️ SEMUA QTY DIAMBIL PENUH, TIDAK BISA PARTIAL
				   Qty = item.Qty,  // Qty dari SO, tanpa opsi edit
				   // ... field lain ...
			   });
		   }
	   }
   }
   ```

2. **SO status berubah menjadi "SELESAI" setelah transaksi dibuat (line 315):**
   ```csharp
   private void HandleValidSubmit()
   {
	   var newEdit = service.AddTransH(Transh, true);
	   if (newEdit != null)
	   {
		   // ⚠️ SO status langsung jadi "3" (selesai), tidak bisa dibuat transaksi lagi
		   serviceOrder.SaveOrderAktif(Transh.NoPrj);
	   }
   }
   ```

   Di OrderSalesServices.cs (line 180):
   ```csharp
   public void SaveOrderAktif(string customer)
   {
	   // Status "3" = SO selesai/ditandai, tidak bisa dibuat transaksi parsial lagi
	   _context.PoTransHs.Include(p => p.PoTransDs)
		   .Where(x => x.NoLpb == customer).FirstOrDefault().Cek = "3";
	   _context.SaveChanges();
   }
   ```

---

## STRUKTUR DATA SAAT INI

### 📊 Model di Order (PoTransH & PoTransD - Sales Order)
```
PoTransH (Header)
├── PoTransHId
├── NoLpb (nomor SO)
├── NoPrj (project/customer)
├── Vendor (dari misnaming, seharusnya Customer)
├── NamaVendor (nama customer)
├── TotalQty ✅ (field untuk total qty SO)
├── QtyTerima ⚠️ (ada field ini, tapi TIDAK DIGUNAKAN untuk partial)
├── Status → Cek field dengan value:
│   ├── "1" = Aktif (bisa dijual)
│   ├── "3" = Selesai (ditandai SO sudah selesai)
└── PoTransDs[] (detail items)
	├── Qty (quantity order)
	├── QtyBo (quantity back order)
	└── ... field lain ...

OeTransH (Header Penjualan)
├── OeTransHId
├── NoLpb (nomor transaksi jual, reference ke SO)
├── NoPrj (customer)
├── TotalQty ✅ (total qty yang dijual)
├── QtyTerima ⚠️ (ada field ini, tapi TIDAK DIGUNAKAN)
└── OeTransDs[] (detail items)
	├── Qty (quantity yang dijual)
	└── ... field lain ...
```

---

## KESIMPULAN ANALISIS STRUKTUR

### ✅ STRUKTUR SUDAH SIAP untuk Parsial
Model **SUDAH MEMILIKI FIELD yang mendukung:**
- `QtyTerima` di PoTransH & OeTransH
- `QtyBo` (back order) di detail
- Status tracking dengan field `Cek`

### ❌ LOGIC MASIH BELUM SIAP
**Masalah:**
1. **Tidak ada input field qty di UI** - User tidak bisa mengedit qty individual
2. **SaveOrderAktif() langsung tandai selesai** - Tidak ada validasi partial
3. **Tidak ada tracking QtyTerima** - Field ada tapi tidak digunakan
4. **Tidak ada mekanisme untuk transaksi parsial berulang**

---

## REKOMENDASI IMPLEMENTASI PENJUALAN PARSIAL

Untuk mendukung penjualan parsial dari SO, diperlukan perubahan di:

### 1️⃣ **AddTransOrderJual.razor** (UI)
   - ✏️ Tambah kolom "Edit Qty" di tabel detail
   - ✏️ Validation: Qty penjualan ≤ Qty tersedia di SO
   - ✏️ Hitung sisa qty yang belum dijual

### 2️⃣ **OeTransDView.cs** (View Model)
   - ✏️ Tambah field `QtyTersedia` untuk validasi
   - ✏️ Tambah field `QtyBo` (back order tracking)

### 3️⃣ **OrderSalesServices.cs** (Business Logic)
   - ✏️ Ubah `SaveOrderAktif()` untuk cek apakah qty penuh/parsial:
	 ```
	 if (totalQtyJual >= totalQtySO) {
		 status = "3"  // Selesai
	 } else {
		 status = "1"  // Tetap aktif, bisa transaksi lagi
	 }
	 ```
   - ✏️ Update `QtyTerima` dengan qty yang sudah dijual
   - ✏️ Hitung sisa qty untuk transaksi berikutnya

### 4️⃣ **SalesServices.cs** (Penjualan Service)
   - ✏️ Validasi qty tidak melebihi SO
   - ✏️ Update status SO berdasarkan qty yang sudah terjual

### 5️⃣ **Database** (jika perlu)
   - ✏️ Migrasi: Pastikan QtyTerima ter-initialize dengan baik
   - ✏️ Backup before changes

---

## RINGKASAN JAWABAN

| Aspek | Status | Keterangan |
|-------|--------|-----------|
| **Sistem sekarang** | ❌ Mengharuskan Qty Penuh | SO harus lengkap 100% sebelum bisa dijual |
| **Struktur Data** | ✅ Sudah Siap | Field QtyTerima, QtyBo, Status sudah ada |
| **Logic/Service** | ❌ Belum Siap | SaveOrderAktif() langsung tandai selesai tanpa cek parsial |
| **UI/Component** | ❌ Belum Siap | Tidak ada input qty di AddTransOrderJual |
| **Upaya Perubahan** | 🟡 Medium | ~4-5 file perlu diubah, moderate complexity |

---

## NEXT STEPS

Apakah Anda ingin saya:
1. ✏️ **Membuat implementasi penjualan parsial SO** dengan perubahan code?
2. 📋 **Membuat test case** untuk validasi penjualan parsial?
3. 🔍 **Analisis lebih detail** bagian tertentu?
4. 🚀 **Modifikasi SaveOrderAktif()** untuk support parsial?

