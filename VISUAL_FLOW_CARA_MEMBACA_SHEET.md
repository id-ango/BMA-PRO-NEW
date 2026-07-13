# 📊 VISUAL FLOW - CARA MEMBACA SHEET "PREDIKSI PO & READINESS"

## ALUR MEMBACA DAN PENGAMBILAN KEPUTUSAN

```
┌─────────────────────────────────────────────────────────────────────────────┐
│ START: Anda perlu membuat keputusan PO prioritas minggu depan               │
└─────────────────────────────────────────────────────────────────────────────┘
									  │
									  ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│ STEP 1: BACA SECTION 1 (RINGKASAN)                                          │
│ ┌─────────────────────────────────────────────────────────────────────────┐ │
│ │ Total SO Aktif:        15 SO                                             │ │
│ │ SO Sudah Ready:         5 SO (✓ 33%)                                     │ │
│ │ SO Masih Pending:      10 SO (✗ 67%)  ← BANYAK, HARUS GERAK CEPAT!      │ │
│ │ Total PO Aktif:         8 PO                                             │ │
│ └─────────────────────────────────────────────────────────────────────────┘ │
│                                                                              │
│ KEPUTUSAN: "Urgent! Harus prioritaskan PO untuk reduce pending"             │
└─────────────────────────────────────────────────────────────────────────────┘
									  │
									  ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│ STEP 2: BACA SECTION 2 (RANKING PO)                                         │
│ ┌─────────────────────────────────────────────────────────────────────────┐ │
│ │  Ranking  No PO  No PI        SO akan Ready  %Coverage  Prioritas      │ │
│ │  ───────────────────────────────────────────────────────────────────   │ │
│ │    1     PO-001  PI-2024-01  ▲▲▲▲▲▲▲▲ (8)     53.3%     ⭐ PENTING!    │ │
│ │    2     PO-002  PI-2024-02  ▲▲▲▲▲ (5)        33.3%     🔥 Tinggi      │ │
│ │    3     PO-003  PI-2024-03  ▲▲▲ (3)          20.0%     🔥 Tinggi      │ │
│ │    4     PO-004  PI-2024-04  ▲ (1)             6.7%     Medium         │ │
│ │    5     PO-005  PI-2024-05  ▲ (1)             6.7%     Low            │ │
│ └─────────────────────────────────────────────────────────────────────────┘ │
│                                                                              │
│ INSIGHT: PO-001 paling penting (8 SO siap), diikuti PO-002 (5 SO)          │
└─────────────────────────────────────────────────────────────────────────────┘
									  │
									  ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│ STEP 3: BACA SECTION 3 (CRITICAL ITEMS)                                     │
│ ┌─────────────────────────────────────────────────────────────────────────┐ │
│ │ ItemCode  Nama Item  Qty Diminta  Stock Sekarang  PO Plan  Kekurangan  │ │
│ │ ───────────────────────────────────────────────────────────────────── │ │
│ │ IT-001    Bearing A  [100]        [20]            [50]     ⚠️ 30      │ │
│ │ IT-002    Cable B    [80]         [10]            [60]     ⚠️ 10      │ │
│ │ IT-003    Motor C    [60]         [30]            [40]     ⚠️ 0       │ │
│ └─────────────────────────────────────────────────────────────────────────┘ │
│                                                                              │
│ BOTTLENECK: Item IT-001 (Bearing A) paling critical → perlu prioritas       │
└─────────────────────────────────────────────────────────────────────────────┘
									  │
									  ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│ STEP 4: CROSS CHECK - Apakah Item Critical ada di PO-001?                   │
│                                                                              │
│ SECTION 4: Lihat PO-001                                                     │
│ ┌─────────────────────────────────────────────────────────────────────────┐ │
│ │ PO: PO-001 | PI: PI-2024-01 | Tanggal: 15/01/2024 | Supplier: PT-ABC  │ │
│ │                                                                         │ │
│ │ Items dalam PO (3):                                                     │ │
│ │   • IT-001 (Bearing A)  - 50 qty   ✓ INCLUDE! (dari SECTION 3)        │ │
│ │   • IT-003 (Motor C)    - 10 qty                                        │ │
│ │   • IT-005 (Cable D)    - 100 qty                                       │ │
│ │                                                                         │ │
│ │ CONCLUSION: PO-001 PENTING ✓ (include critical item IT-001)            │ │
│ └─────────────────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────────────┘
									  │
									  ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│ STEP 5: DETAIL IMPACT - SO AKAN READY                                       │
│                                                                              │
│ SECTION 4.3: SO yang akan SIAP KIRIM setelah PO-001                        │
│ ┌─────────────────────────────────────────────────────────────────────────┐ │
│ │ ✓ SO AKAN READY (3)                                                     │ │
│ │ ┌─────────────────────────────────────────────────────────────────────┐ │ │
│ │ │ No SO   Customer      Tgl       PI      Item Selesai              │ │ │
│ │ │ ─────────────────────────────────────────────────────────────── │ │ │
│ │ │ SO-001  Customer A  10/01/24  PI-001  IT-001, IT-003 ✓         │ │ │
│ │ │ SO-002  Customer B  11/01/24  PI-002  IT-001, IT-005 ✓         │ │ │
│ │ │ SO-003  Customer C  12/01/24  PI-003  IT-003, IT-005 ✓         │ │ │
│ │ └─────────────────────────────────────────────────────────────────┘ │ │
│ │                                                                         │ │
│ │ = 3 SO akan lengkap & siap kirim setelah PO-001 datang                │ │
│ └─────────────────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────────────┘
									  │
									  ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│ STEP 6: MASIH PENDING - SO MANA YANG DAPAT ITEM DARI PO TAPI BELUM LENGKAP  │
│                                                                              │
│ SECTION 4.4: SO yang masih PENDING (butuh PO lain)                         │
│ ┌─────────────────────────────────────────────────────────────────────────┐ │
│ │ ◐ SO MASIH PENDING (5)                                                  │ │
│ │ ┌─────────────────────────────────────────────────────────────────────┐ │ │
│ │ │ NO SO   Customer    Tgl       PI     Item Selesai ✓  Item Kurang ✗│ │ │
│ │ │ ───────────────────────────────────────────────────────────────  │ │ │
│ │ │                                                                   │ │ │
│ │ │ SO-004  Cust D    13/01/24  PI-004  IT-001        IT-002, IT-007│ │ │
│ │ │        (dapat 1 item, butuh      (masih butuh 2 items)           │ │ │
│ │ │         IT-001 dari PO-001)                                      │ │ │
│ │ │                                                                   │ │ │
│ │ │ SO-005  Cust E    14/01/24  PI-005  IT-003, IT-005 IT-004, IT-006│ │ │
│ │ │        (dapat 2 items dari                                       │ │ │
│ │ │         PO-001, tapi masih      (masih butuh 2 items)            │ │ │
│ │ │         kurang 2)                                                │ │ │
│ │ │                                                                   │ │ │
│ │ │ SO-006  Cust F    15/01/24  PI-006  -             IT-001, IT-002,│ │ │
│ │ │        (tidak dapat yang                            IT-003       │ │ │
│ │ │         dari PO-001)                                             │ │ │
│ │ └─────────────────────────────────────────────────────────────────┘ │ │
│ │                                                                         │ │
│ │ ACTION: Cari PO yg include IT-002, IT-004, IT-006, IT-007              │ │
│ └─────────────────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────────────┘
									  │
									  ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│ STEP 7: BUAT KEPUTUSAN                                                       │
│                                                                              │
│ SKENARIO: Supplier bisa kirim 2 PO minggu ini, pilih mana?                 │
│                                                                              │
│ ├─ OPSI A: PO-001 + PO-002                                                 │
│ │  ├─ Ready SO: 8 + 5 = 13 SO ✓✓✓ TERBAIK!                                │
│ │  ├─ Bearing A gap: -50 (dapat 50 dari PO-001)                           │
│ │  ├─ Cable B gap: -40 (dapat 60, butuh 80)                               │
│ │  └─ RECOMMENDATION: PILIH INI                                            │
│ │                                                                          │
│ ├─ OPSI B: PO-001 + PO-003                                                │
│ │  ├─ Ready SO: 8 + 3 = 11 SO                                             │
│ │  ├─ Bearing A gap: -70 (dapat 70 dari 2 PO)                            │
│ │  └─ Tapi Cable B tidak berkurang                                        │
│ │                                                                          │
│ └─ OPSI C: PO-002 + PO-003                                                │
│    ├─ Ready SO: 5 + 3 = 8 SO ✗ (paling sedikit)                           │
│    └─ TIDAK RECOMMENDED                                                    │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
									  │
									  ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│ ✅ FINAL DECISION                                                            │
│                                                                              │
│ PRIORITAS MINGGU DEPAN: PO-001 + PO-002                                    │
│                                                                              │
│ HASIL:                                                                       │
│  • 13 SO akan siap kirim (reduce pending dari 10 menjadi tinggal 2!)  │
│  • Bearing A gap -50 (dari critical 30, menjadi cukup)                    │
│  • Cable B gap -40 (dari critical 10, menjadi lebih baik)                 │
│  • Supplier PT-ABC & PT-XYZ bisa dikontak sesuai tanggal                  │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## ALTERNATIVE: QUICK SUMMARY TABLE

Jika ingin lebih cepat, gunakan tabel ini:

```
┌────────────────────────────────────────────────────────────────────────────┐
│                    SHEET PREDIKSI PO & READINESS                           │
│                          QUICK REFERENCE                                   │
├────────────────────────────────────────────────────────────────────────────┤
│                                                                            │
│ SECTION | Gunakan Untuk...              | Info Kunci | Tindakan         │
│ ───────┼────────────────────────────────┼────────────┼─────────────────│
│    1   | Lihat urgency                  | % pending  | 67% → URGENT! │
│    2   | Pilih PO ranking               | Rank 1-3   | Focus ke top 3  │
│    3   | Cari bottleneck item           | Critical   | Include di PO?   │
│    4.1 | Info PO (tanggal, supplier)    | Timeline   | Plan komunikasi │
│    4.2 | Barang apa di PO ini           | Item list  | Cross cek items │
│    4.3 | SO akan ready                  | # Ready SO | Impact measure  │
│    4.4 | SO masih pending, kurang apa   | Missing    | PO mana next?   │
│                                                                            │
└────────────────────────────────────────────────────────────────────────────┘
```

---

## INTERPRETASI WARNA

```
🟩 HIJAU = READY
   ├─ SO akan lengkap & siap kirim
   └─ Item yang akan selesai dari PO ini

  🔵 BIRU = HEADER / INFO
   ├─ Nomor PO, PI, Tanggal, Supplier
   └─ Informasi umum

🟥 MERAH = PENDING / WARNING
   ├─ SO yang belum lengkap
   ├─ Item yang masih kurang
   └─ Action needed: cari PO lain

🟨 KUNING = HIGHLIGHT PENTING
   ├─ Ranking 1 (paling penting)
   └─ Prioritas tertinggi untuk diprocure
```

---

## DECISION TREE

```
					Mulai Analisa
						 │
						 ▼
			  Ada berapa SO pending?
			  /            │            \
		   0-2           3-7            8+
			│              │              │
		  GOOD           MEDIUM         🚨URGENT
			│              │              │
			▼              ▼              ▼
	  Maintain           Review      Aktifkan
	  current PO         PO-2,3      Ranking 1-2
			│              │              │
			│              │              │
			└──────┬───────┴──────┬───────┘
				   │              │
				   ▼              ▼
		   Lihat Section 3:    Lihat Critical Items:
		   Critical Items?   Include di PO-1/2?
				 │              │
			  YES│              │YES
				 │              │
				 ▼              ▼
		   ✅ PO ini OK    ✅ Prioritas!
			 (include      Confirm
			  critical)    supplier
```

---

## CONTOH REAL: DARI DATA KE KEPUTUSAN

### Hari Senin Pagi

**Admin:** "Pak, berikut sheet prediksi PO untuk minggu depan"

### Step 1: Baca RINGKASAN
```
Total SO pending: 10
SO sudah ready: 5
```
→ **CEO:** "10 pending, itu banyak. Kita harus gerak cepat!"

### Step 2: Baca RANKING
```
Rank 1: PO-001 → 8 SO ready
Rank 2: PO-002 → 5 SO ready
```
→ **Operations Manager:** "Jadi kita harus prioritas PO-001 dan PO-002"

### Step 3: Cek CRITICAL ITEMS
```
Bearing A kekurangan 30
```
→ **Supply Chain**: "Bearing A di PO-001 ada 50qty, bagus"

### Step 4: Lihat DETAIL PO-001
```
PO-001 tiga SO ready, lima SO masih pending
```
→ **Warehouse Manager:** "Oke, siap terima PO-001 minggu depan"

### Step 5: Final Decision
```
Prioritas: PO-001 (datang 15 Jan) + PO-002 (datang 18 Jan)
Result: 13 SO siap, Bearing A OK, Cable B OK
```

→ **Semua pihak align, keputusan tepat!** ✅

---

## CHECKLIST: Sebelum Meeting Dengan Supplier

```
☐ Baca SECTION 1 - Apa urgency level? (% pending)
☐ Lihat SECTION 2 - PO mana yang Rank 1?
☐ Check SECTION 3 - Item apa yang critical?
☐ Detail SECTION 4 - Apakah critical items ada di PO ranking atas?
☐ Hitung - Jika PO A + B datang, berapa SO akan ready?
☐ Altspringer - Jika tidak bisa, pilih PO mana yang least impactful?
☐ Contact supplier - Berdasarkan ranking, hubungi supplier prioritas
☐ Confirm timeline - Kapan datang? Berapa banyak item?
☐ Share hasil ke team - Lihat SECTION 4 untuk tahu SO mana affected
```

---

## ⚡ TL;DR - BACA CEPAT (2 MENIT)

1. **SECTION 2** → Lihat #1 ranking PO
2. **SECTION 3** → Apakah item critical?
3. **SECTION 4** untuk PO #1 → Lihat SO AKAN READY
4. **Decision** → Confirm dengan supplier untuk prioritas

Selesai! 🎯
