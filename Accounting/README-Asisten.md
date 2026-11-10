# Asisten Bos

## Tahap 4

Halaman `/asisten` menyediakan tool stok menipis, penjualan, piutang jatuh tempo, rekomendasi pembelian, dan pencarian kas/bank. Query menggunakan connection string `BmaReadOnly` jika tersedia. Jika kosong dan `AI:UseDefaultConnection` bernilai `true`, query menggunakan `DefaultConnection`; pendaftaran DbContext modul lain tidak diubah. Setiap daftar dibatasi maksimal 20 baris, sedangkan total dan jumlah dokumen mencakup semua hasil yang cocok.

Stok dibandingkan dengan `MinQty` yang tidak nol. Barang dengan `MinQty` nol dihitung dan ditampilkan terpisah. Masing-masing daftar dibatasi 20 baris, diurutkan berdasarkan kode barang, dan menyertakan status aktif/nonaktif. Rata-rata penjualan per minggu adalah jumlah `Qty` kode `94` selama 28 hari kalender termasuk hari ini, dibagi empat; retur tidak dikurangi. Lead time dihitung dari tanggal hari ini sampai tanggal yang sama setelah `AI:LeadTimeMonths` bulan kalender, dengan default tiga bulan.

- **Penjualan:** jumlah header kode `94` dikurangi kode `95`; kedua batas tanggal inklusif. Periode sebelumnya sama panjang. Persentase perubahan tidak tersedia jika total sebelumnya nol atau negatif.
- **Piutang:** hanya `Sisa` positif; tanggal sebelum hari ini dinyatakan lewat jatuh tempo. Saldo positif tanpa `DueDate` ditampilkan terpisah, bukan diberi tanggal perkiraan. Saldo negatif tidak termasuk.
- **Saran pembelian:** menggunakan rumus rata-rata mingguan dikali lead time dan buffer, dikurangi stok lalu dibulatkan ke atas dan dibatasi minimal nol. `AI:BufferDays` default 14 hari. PO dan SO terbuka tidak diperhitungkan; saran perlu ditinjau sebelum membuat pesanan. Status barang nonaktif tetap ditampilkan.
- **Kas/bank:** pencarian kata literal pada keterangan header atau detail tidak membedakan huruf besar/kecil. Jumlah dihitung per dokumen; total masuk/keluar menggunakan `Terima`/`Bayar` dari semua detail dokumen yang cocok, dipisahkan menurut `CbTransH.Kurs` sesuai aturan yang dikonfirmasi pengguna. Maksimal 20 kelompok mata uang ditampilkan. Dokumen tanpa kode mata uang tetap dihitung dan ditampilkan, tetapi nilainya tidak ditotal atau ditampilkan sebagai total dokumen karena mata uangnya tidak diketahui. Transfer antarbank muncul sebagai dokumen masing-masing sisi sehingga total masuk dan keluar bukan ukuran arus kas eksternal.

Entitas penjualan dan piutang yang digunakan tidak menyimpan kode mata uang; pengguna telah mengonfirmasi bahwa nilai `Jumlah` penjualan dan `Sisa` piutang selalu IDR. Nilai tersebut ditampilkan dalam rupiah. Untuk kas/bank, kode mata uang diambil dari header dan nilai dengan kode `IDR` ditampilkan dalam rupiah.

**Verifikasi kas/bank sebelum digunakan:** pada sebagian alur input, `Terima`/`Bayar` dihitung dari `KTerima`/`KBayar` dikalikan `KValue`. Periksa transaksi nyata bersama pengguna apakah hasil konversi tersebut memang menggunakan mata uang `CbTransH.Kurs`; sampai terkonfirmasi, total kas/bank perlu ditinjau dan tidak boleh langsung dijadikan dasar keputusan.

## Konfigurasi development

Project `Accounting` sudah memiliki User Secrets ID. Dari direktori solusi, simpan nilai sebenarnya di user-secrets, bukan di repository:

```powershell
dotnet user-secrets set "ConnectionStrings:BmaReadOnly" "<connection-string-akun-read-only>" --project .\Accounting\Accounting.csproj
dotnet user-secrets set "AI:ApiKey" "<api-key>" --project .\Accounting\Accounting.csproj
dotnet user-secrets set "AI:Model" "<nama-model>" --project .\Accounting\Accounting.csproj
```

Jangan memasukkan nilai contoh secara harfiah. Perhatikan bahwa shell dapat menyimpan perintah dan nilainya dalam riwayat; gunakan mekanisme rahasia yang sesuai dengan lingkungan. Untuk production, gunakan secret store atau environment variables yang aman.

Konfigurasi saat ini mengaktifkan `AI:UseDefaultConnection` agar Asisten Bos menggunakan autentikasi Windows yang sama dengan aplikasi apabila `BmaReadOnly` belum diisi. Akun aplikasi dapat memiliki hak tulis; query tanpa tracking tidak membatasi hak akun tersebut. Untuk perlindungan database, siapkan akun khusus read-only seperti di bawah, isi `BmaReadOnly`, dan nonaktifkan `AI:UseDefaultConnection`.

`AI:Provider` saat ini mendukung `OpenAI`. Untuk endpoint yang kompatibel, atur `AI:Endpoint` secara opsional; endpoint harus menggunakan HTTPS, kecuali HTTP pada alamat lokal. Untuk provider lain, sesuaikan pembuatan `IChatClient` di `AsistenBosService`. `AI:Role` menentukan role yang boleh mengakses halaman dan menu; nilainya saat ini `Admin`, dan jika kosong akses ditolak. `AI:MaxToolCalls` dan `AI:TimeoutSeconds` membatasi setiap permintaan.

Pertanyaan dan riwayat percakapan dikirim ke provider AI. Tool mengirimkan hanya status hasil ke model; angka dan teks database tetap di tabel aplikasi. Ringkasan dibuat dari hasil tool, bukan klaim bebas model. Waktu UTC, pertanyaan, user, nama tool, dan durasi dicatat di log aplikasi; jangan memasukkan rahasia ke dalam pertanyaan. Jawaban Markdown tidak merender HTML mentah, link, atau gambar.

## Akun SQL Server read-only

Minta administrator database membuat login dan user khusus tanpa peran tulis. Contoh berikut hanya panduan, **jangan dijalankan secara otomatis**; sesuaikan nama database, skema, autentikasi, dan kata sandi dengan lingkungan. Lindungi kata sandi dan jangan simpan skrip yang berisi kata sandi sebenarnya di repository.

```sql
-- Di database master, oleh administrator:
CREATE LOGIN [BmaAsistenReadOnly] WITH PASSWORD = '<kata-sandi-kuat>';

-- Di database aplikasi, oleh administrator:
CREATE USER [BmaAsistenReadOnly] FOR LOGIN [BmaAsistenReadOnly];
GRANT SELECT ON OBJECT::[dbo].[IcItems] TO [BmaAsistenReadOnly];
GRANT SELECT ON OBJECT::[dbo].[OeTransDs] TO [BmaAsistenReadOnly];
GRANT SELECT ON OBJECT::[dbo].[OeTransHs] TO [BmaAsistenReadOnly];
GRANT SELECT ON OBJECT::[dbo].[ArPiutngs] TO [BmaAsistenReadOnly];
GRANT SELECT ON OBJECT::[dbo].[ArCusts] TO [BmaAsistenReadOnly];
GRANT SELECT ON OBJECT::[dbo].[CbTransHs] TO [BmaAsistenReadOnly];
GRANT SELECT ON OBJECT::[dbo].[CbTransDs] TO [BmaAsistenReadOnly];
```

Konfirmasi bahwa skema tabel memang `dbo` di database tujuan. Jangan berikan `db_datareader`, `db_datawriter`, `db_owner`, atau hak tulis lainnya; periksa juga hak yang diwarisi dari role lain. Pengaturan tanpa tracking di EF bukan pengganti izin database read-only.
