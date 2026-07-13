# 📊 PANDUAN MEMBACA SHEET "PREDIKSI PO & READINESS"

Sheet ini membantu management membuat keputusan tentang prioritas PO berdasarkan impact terhadap SO yang siap dikirim.

## 🔑 LOGIKA UTAMA

**Prediksi ini dihitung berdasarkan:**
1. **Stock saat ini**: Qty yang ada di inventory hari ini
2. **PI/PO urutan tanggal**: Mulai dari tanggal awal (oldest first)
3. **Alokasi sequensial per SO**: SO dengan tanggal order lebih awal dapat prioritas stock lebih dulu
   - Contoh: Jika stock IT-001 = 60, SO-001 butuh 30, SO-002 butuh 50
   - Maka SO-001 dapat 30, SO-002 dapat 30 (sisa 0) → SO-001 ready, SO-002 still pending
4. **Cumulative stock**: Stock bertambah seiring PO yang datang dalam urutan tanggal

---

## ✅ APA ITU "SO READY"?

**SO dinyatakan READY ketika:**
- Semua item yang dipesan sudah tersedia (qty cukup)
- Berdasarkan stock sekarang **PLUS** cumulative stock dari PO yang akan datang (up to this PO date)
- **Urutan SO**: Diproses berdasarkan **tanggal SO dari awal** (oldest first)

**CONTOH REAL:**
```
Stock saat ini:
- IT-001: 20 qty
- IT-002: 10 qty

Daftar SO (urutan tanggal):
- SO-001 (10/01): butuh IT-001 (30), IT-002 (20) → Need 30 + 20
- SO-002 (11/01): butuh IT-001 (50) → Need 50
- SO-003 (12/01): butuh IT-002 (15) → Need 15

PO yang akan datang (urutan tanggal):
- PI-2024-01 (15/01): IT-001 (60), IT-002 (40)

PREDIKSI:
1. Sekarang (qty sekarang saja):
   Stock = IT-001: 20, IT-002: 10
   - SO-001: butuh 30 IT-001 (hanya ada 20) → PENDING
   - SO-002: butuh 50 IT-001 (hanya ada 20) → PENDING
   - SO-003: butuh 15 IT-002 (hanya ada 10) → PENDING

2. Setelah PI-2024-01 datang (qty sekarang + PO):
   Stock = IT-001: 80 (20+60), IT-002: 50 (10+40)
   Alokasi sequensial:
   - SO-001 (earliest): dapat 30 IT-001 (dari 80, sisa 50), dapat 20 IT-002 (dari 50, sisa 30) 
     → SO-001 READY ✓
   - SO-002: dapat 50 IT-001 (dari 50 sisa, sisa 0) 
     → SO-002 READY ✓
   - SO-003: dapat 15 IT-002 (dari 30 sisa) 
     → SO-003 READY ✓

   Hasil: Setelah PI-2024-01 datang, 3 SO akan siap kirim!
```

---

## 🎯 4 SECTION UTAMA

### **SECTION 1: RINGKASAN CEPAT** (Quick Overview)
```
Total SO Aktif:      15        → Berapa banyak SO sedang berjalan
SO Sudah Ready:       5  ✓ Hijau → SO yang sudah lengkap (siap kirim)
SO Masih Pending:    10  ✗ Merah → SO yang masih menunggu stock
Total PO Aktif:       8        → Ada 8 PO yang sedang diproses
```

**Cara pakai:**
- Jika "SO Masih Pending" banyak → Prioritaskan PO untuk mengurangi pending
- Jika "SO Sudah Ready" tinggi → Supply chain sudah baik

---

### **SECTION 2: RANKING PO BERDASARKAN IMPACT** (Prioritas PO)
Tabel dengan 6 kolom:

| Ranking | No PO | No PI | SO akan Ready | % dari Total SO | Keterangan |
|---------|-------|-------|---------------|-----------------|-----------|
| 1 | PO-001 | PI-2024-01 | **8** | **53.3%** | ⭐ PALING PENTING |
| 2 | PO-002 | PI-2024-02 | 5 | 33.3% | 🔥 Prioritas tinggi |
| 3 | PO-003 | PI-2024-03 | 3 | 20.0% | 🔥 Prioritas tinggi |
| 4 | PO-004 | PI-2024-04 | 1 | 6.7% | Medium priority |

**Cara baca:**
- **Ranking 1** (⭐ PALING PENTING): Setelah PO-001 datang, 8 SO akan ready (53.3% dari total 15 SO)
  - **HARUS DIPRIORITASKAN DULUAN** ← Banyak impact
  - **Penting**: Angka "8 SO ready" sudah memperhitungkan alokasi sequensial
    - Contoh: Jika stock terbatas, SO dengan tanggal order lebih awal mendapat prioritas
    - Maka hasil "8 ready" = SO yang paling awal yang akan ready terlebih dahulu
- **Ranking 2-3** (🔥): Prioritas tinggi, cukup banyak impact
- **Ranking 4+**: Rendah prioritas

**Contoh keputusan:**
```
Andai supplier hanya bisa kirim 2 PO minggu ini:
→ Prioritas PO-001 (8 SO ready) + PO-002 (5 SO ready) = 13 SO siap kirim
→ Jangan PO-001 + PO-004 = hanya 9 SO siap kirim (lebih sedikit)

Catatan: Dari 8 SO ready untuk PO-001, SO-001 s/d SO-008 adalah urutan 
SO dengan tanggal order paling awal. SO yang datang kemudian akan melihat
sisa stock setelah SO sebelumnya dapat bagian mereka.
```

---

### **SECTION 3: ITEM YANG PALING CRITICAL** (Bottleneck Items)
Item apa yang paling dibutuhkan banyak SO?

| Item Code | Nama Item | Qty Diminta | Stock Saat Ini | PO Direncanakan | Kekurangan |
|-----------|-----------|-------------|----------------|-----------------|-----------|
| IT-001 | Bearing A | **100** | 20 | 50 | **30** |
| IT-002 | Cable B | 80 | 10 | 60 | **10** |

**Cara baca:**
- **Qty Diminta**: Total qty dari semua SO yang masih pending untuk item ini
- **Stock Saat Ini**: Stock hari ini
- **PO Direncanakan**: Total qty dari semua PO yang sedang diproses
- **Kekurangan**: Setelah stock + PO, masih kurang berapa?

**Contoh:**
```
Bearing A: 
- 100 qty diminta (dari 8 SO)
- Stock sekarang: 20
- PO yang akan datang: 50
- Masih kurang: 30 qty

→ Artinya: Bahkan dengan PO yang akan datang, Bearing A masih kurang 30 unit
→ Action: Cek apakah ada PO lain yang include Bearing A, atau pesan tambahan
```

---

### **SECTION 4: DETAIL PREDIKSI PER PO** (Analisa Detail)

Ini adalah section TERPENTING untuk planning. Setiap PO ditunjukkan dengan detail:

#### **4.1 PO Header** (Informasi Umum PO)
```
┌─────────────────────────────────────────────────────────────────┐
│ PO: PO-001 │ PI: PI-2024-01 │ Tanggal: 15/01/2024 │ Supplier: PT-ABC │
└─────────────────────────────────────────────────────────────────┘
```

**Info:**
- `PO: PO-001` → Nomor PO
- `PI: PI-2024-01` → Nomor Project/PI yang memesan
- `Tanggal: 15/01/2024` → Estimasi tiba PO
- `Supplier: PT-ABC` → Siapa suppliernya

**Urutan**: PO diurutkan berdasarkan **TANGGAL** (paling awal duluan)
→ Ini penting untuk planning: "Minggu depan PO ini akan datang, dan itu berdampak ke SO ini"

---

#### **4.2 Items dalam PO** (Apa saja barangnya)
```
Items dalam PO (3):
ItemCode  | NamaItem    | Qty | Satuan
----------|-------------|-----|-------
IT-001    | Bearing A   | 50  | Pcs
IT-003    | Motor B     | 10  | Unit
IT-005    | Cable C     | 100 | Meter
```

**Cara pakai:**
- Lihat item apa yang ada di PO ini
- Bandingkan dengan SECTION 3 (Critical Items) 
- Jika ada item critical di sini, PO ini penting!

---

#### **4.3 ✓ SO AKAN READY (Hijau)** - SO yang akan lengkap

```
✓ SO AKAN READY (3)

No SO    | Customer    | Tgl      | PI/Project | Keterangan     | Item Selesai
---------|-------------|----------|----------|----------------|--------
SO-001   | Customer A  | 10/01/24 | PI-001   | Rumah Besar    | IT-001, IT-003
SO-002   | Customer B  | 11/01/24 | PI-002   | Proyek Pabrik  | IT-001, IT-005
SO-003   | Customer C  | 12/01/24 | PI-003   | Renovasi Mall  | IT-003, IT-005
```

**Cara baca:**
- Jika PO-001 datang, **3 SO ini akan lengkap** dan siap dikirim!
  - **Urutan SO**: SO-001, SO-002, SO-003 adalah urutan dari tanggal order paling awal
  - SO-001 dapat prioritas stock terlebih dahulu (karena order paling awal)
  - SO-002 dapat sisa stock setelah SO-001 dapat bagian mereka
  - SO-003 dapat sisa stock setelah SO-001 dan SO-002
- Kolom "Item Selesai" menunjukkan item dari PO ini yang akan membuat SO lengkap

**PENTING - Urutan Alokasi:**
```
Contoh dengan stock terbatas:
- Stock sekarang: IT-001 = 60 qty
- SO-001 (10/01): butuh IT-001 (30) → dapat 30 (dari 60) → READY
- SO-002 (11/01): butuh IT-001 (50) → dapat 30 (sisa 60-30) → PENDING (kurang 20)
- SO-003 (12/01): butuh IT-001 (40) → dapat 0 (tidak ada sisa) → PENDING

→ Jadi SO-001 yang paling awal akan READY duluan,  SO-002 dan SO-003 masih pending
   meski mereka juga order IT-001 dari PO ini
```

**Contoh analisa:**
```
Dari SECTION 2: PO-001 akan membuat 8 SO ready
Lihat di SECTION 4.3: Ada 3 SO yang akan lengkap dari PO-001

Ini bisa berarti:
- Dari 15 SO total, 8 akan ready setelah PO-001
- Di SECTION 4.3 ditampilkan 3 SO dari mereka yang detail
- Ada 5 SO lain yang sudah ready sebelumnya (berbeda PO atau dari stock awal)
- Atau ada SO lain yang akan ready tapi tidak ditampilkan di detail ini

Lihat SECTION 4.4 untuk SO mana saja yang masih PENDING
```

---

#### **4.4 ◐ SO MASIH PENDING (Merah)** - SO yang belum lengkap

```
◐ SO MASIH PENDING (5)

No SO    | Customer    | Tgl      | PI/Project | Item Selesai (✓ Hijau)          | Item Masih Kurang (✗ Merah)
---------|-------------|----------|----------|--------------------------------|------------------------
SO-004   | Customer D  | 13/01/24 | PI-004   | IT-001                         | IT-002, IT-007
SO-005   | Customer E  | 14/01/24 | PI-005   | IT-003, IT-005                 | IT-004, IT-006
SO-006   | Customer F  | 15/01/24 | PI-006   | -                              | IT-001, IT-002, IT-003
SO-007   | Customer G  | 16/01/24 | PI-007   | IT-001                         | IT-008
SO-008   | Customer H  | 17/01/24 | PI-008   | -                              | Semua item
```

**Cara baca:**
- **Item Selesai (✓ Hijau)**: Item dari PO ini yang akan diterima SO, tapi masih belum lengkap
  - Ini adalah progress dari SO setelah PO ini datang
  - SO ini masih menunggu PO lain untuk item yang masih kurang
- **Item Masih Kurang (✗ Merah)**: Item apa yang still missing

**PENTING - Urutan Alokasi:**
```
SO-004 vs SO-005 vs SO-006 di PO yang sama:
- PO ini punya: IT-001 (50), IT-003 (100), IT-005 (80)

Alokasi sequensial berdasarkan tanggal SO:
1. SO-004 (13/01, urutan awal) dapat prioritas duluan:
   - Butuh IT-001 (30) → dapat 30 (dari 50) → sisa stock: IT-001 (20)
   - → Item Selesai: IT-001

2. SO-005 (14/01, urutan berikutnya) dapat sisa stock:
   - Butuh IT-003 (100) → dapat 100 (dari 100) → sisa: 0
   - Butuh IT-005 (80) → dapat 80 (dari 80) → sisa: 0
   - → Item Selesai: IT-003, IT-005

3. SO-006 (15/01, urutan paling akhir) dapat sisa stock:
   - Butuh IT-001 (30) → dapat 20 (sisa dari SO-004) → kurang 10
   - Butuh IT-002 (-) → tidak ada di PO ini
   - → Item Selesai: -, Item Masih Kurang: IT-001, IT-002, IT-003
```

**Contoh analisa:**
```
SO-004 (Customer D):
- Dari PO-001, akan dapat: IT-001 (1 item) ✓
- Tapi masih kurang: IT-002, IT-007 (2 items) ✗
- → Ini SO masih pending, tunggu PO lain yang include IT-002 dan IT-007
- → Karena urutan awalnya (13/01), dia dapat prioritas stock terlebih dahulu

SO-006 (Customer F):
- Dari PO-001, tidak dapat item apapun (-)
- Masih kurang semua: IT-001, IT-002, IT-003 ✗
- → Karena urutan paling akhir (15/01), stock sudah habis oleh SO-004 dan SO-005
- → Harus tunggu PO lain

SO-008 (Customer H):
- Dari PO-001, tidak dapat item apapun (-)
- Masih kurang semua items
- → Harus tunggu banyak PO
```


---

## ❓ FAQ: MENGAPA SO A READY TAPI SO B MASIH PENDING?

### **Pertanyaan:** 
"Kenapa SO-001 dan SO-002 dua-duanya butuh IT-001 dari PO yang sama, tapi SO-001 ready sedangkan SO-002 masih pending?"

### **Jawab:**
Ini karena **URUTAN TANGGAL**. Stock dialokasikan berdasarkan siapa yang pesan lebih dulu (tanggal SO), bukan siapa PO-nya.

**Contoh:**
```
Stock sekarang: IT-001 = 0
PO-001: IT-001 = 50 qty

SO-001 (pesan 01/01): butuh IT-001 (30)
SO-002 (pesan 02/01): butuh IT-001 (50)

Setelah PO-001 datang, alokasi:
1. SO-001 (lebih awal): dapat 30 (dari 50) → READY ✓
2. SO-002 (lebih lambat): dapat 20 (sisa 50-30) → PENDING ✗ (kurang 30)

→ Meskipun item sama dan dari PO sama, SO yang pesan lebih awal mendapat prioritas!
```

### **Ini mirip dengan:**
- **FIFO (First In First Out)** di warehouse: barang lebih awal yang diproses duluan
- **Antrian pelayanan**: yang datang lebih dulu dilayani lebih dulu

---

## 💡 CONTOH KASUS ANALISA MANAGEMENT

### **Skenario: Minggu Depan Pilih Prioritas PO**

**Data ada di Sheet:**
```
SECTION 1: 15 SO total, 10 masih pending
SECTION 2: 
- Ranking 1: PO-001 (akan siapkan 8 SO)
- Ranking 2: PO-002 (akan siapkan 5 SO)
- Ranking 3: PO-003 (akan siapkan 3 SO)

SECTION 3: 
- Critical: Bearing A (kurang 30), Cable B (kurang 10)

SECTION 4: 
PO-001 items: Bearing A (50), Motor B (10), Cable C (100)
PO-002 items: Cable B (60), Gear D (20)
PO-003 items: Bearing A (20), Pipe E (30)
```

**Management Decision:**
```
Q: Supplier bisa kirim 2 PO minggu ini, pilih mana?

Opsi A: PO-001 + PO-002
→ Ready SO: 8 + 5 = 13 SO siap kirim ✓✓✓ TERBAIK!
→ Bearing A kurang: -30 (masih kurang)
→ Cable B: dapat 60, butuh 100, jadi kurang 40 (berkurang drastis)

Opsi B: PO-001 + PO-003
→ Ready SO: 8 + 3 = 11 SO siap kirim
→ Bearing A kurang: -30 - 20 = -50 (sudah dapat 70 dari 2 PO, kurang 30)
→ Cable B: tidak ada 

Opsi C: PO-002 + PO-003
→ Ready SO: 5 + 3 = 8 SO siap kirim ✗ (paling sedikit)

KEPUTUSAN: Pilih Opsi A (PO-001 + PO-002)
Alasan: Paling banyak SO siap kirim (13 SO), dan Cable B yang critical bisa berkurang gap-nya
```

---

## 🎮 CHECKLIST UNTUK MEMBACA SHEET

1. **Baca SECTION 1** 
   - Berapa SO pending? Urgent kah?

2. **Lihat SECTION 2 Ranking 1**
   - PO nomer berapa yang paling penting?
   - Berapa banyak SO yang akan ready?

3. **Cek SECTION 3 Critical Items**
   - Item apa yang paling kritis/kurang?
   - Apakah item itu ada di PO yang akan datang?

4. **Detail SECTION 4 untuk setiap PO yang sedang dipertimbangkan**
   - Lihat PO Header (tanggal & supplier info)
   - Check SO AKAN READY (berapa banyak & SO mana)
   - Check SO MASIH PENDING (item apa yang masih missing)

5. **Bandingkan scenario**
   - Jika ambil PO A, berapa SO ready?
   - Jika ambil PO B, berapa SO ready?
   - Pilih yang maximize SO ready

---

## 🔍 TIPS BACA CEPAT

| Warna | Arti |
|-------|------|
| 🟩 Hijau (Ready) | SO akan lengkap & siap kirim |
| 🟥 Merah (Pending) | SO masih kurang, butuh PO lain |
| 🔵 Biru (Header) | Informasi PO |
| 🟨 Kuning (Highlight) | Ranking 1 - prioritas tertinggi |

---

## ❓ QUICK FAQ

**Q: Kenapa urutan PO berdasarkan tanggal, bukan ranking?**
A: Karena supplier datang berdasarkan urutan tanggal. Ranking 1 belum tentu yang datang paling duluan. Sheet ini membantu: "Minggu ini PO X akan datang (tanggal Y), dan akan membuat SO ini siap"

**Q: Bagaimana jika PO batal datang?**
A: SO yang ditampilkan di "AKAN READY" akan kembali menjadi Pending. Lihat SECTION 4.4 untuk tahu SO mana yang affected.

**Q: Total SO Ready di SECTION 1 vs SECTION 2 beda kenapa?**
A: SECTION 1 = SO yang SUDAH READY sekarang (tanpa tunggu PO)
   SECTION 2 = SO yang AKAN READY jika PO datang (prediksi)

**Q: Saya mau tahu SO mana yang paling urgent?**
A: Lihat SECTION 4.3 & 4.4 di PO yang ranking 1 → lihat SO mana yang paling awal tanggalnya

---

## 📌 KESIMPULAN

Sheet ini dirancang untuk membantu management menjawab:
- ✅ **PO mana yang paling urgent?** → Lihat SECTION 2 (Ranking)
- ✅ **Jika prioritas PO X, berapa SO siap?** → Lihat SECTION 4 (Detail per PO)
- ✅ **Item apa yang paling critical?** → Lihat SECTION 3 (Critical Items)
- ✅ **SO mana yang akan ready setelah PO Y?** → Lihat SECTION 4.3 (Ready)
- ✅ **SO mana yang masih kurang item apa?** → Lihat SECTION 4.4 (Pending + missing items)

**Gunakan informasi ini untuk membuat keputusan procurement & scheduling yang lebih baik!**
