using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Services.Asisten;

public sealed record KasBankItem(
    string Dokumen,
    DateTime Tanggal,
    string Bank,
    string Referensi,
    string Keterangan,
    string KeteranganDetailCocok,
    string MataUang,
    decimal? Terima,
    decimal? Bayar);

public sealed record KasBankTotalMataUang(string MataUang, int JumlahDokumen, decimal? TotalMasuk, decimal? TotalKeluar);

public sealed record HasilKasBank(
    int JumlahDokumen,
    int JumlahKelompokMataUang,
    IReadOnlyList<KasBankTotalMataUang> TotalPerMataUang,
    IReadOnlyList<KasBankItem> Transaksi);

public sealed partial class BosTools
{
    [Description("Cari kata dalam keterangan header atau detail kas/bank tanpa membedakan huruf besar dan kecil. Hitung dokumen dan total Terima/Bayar semua detail berdasarkan mata uang Kurs di header. Dokumen tanpa kode mata uang tidak ditotal. Tampilkan maksimal 20 dokumen terbaru; tanggal opsional yyyy-MM-dd.")]
    public async Task<HasilKasBank> CariKasBank(string kata, string dari = null, string sampai = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(kata))
        {
            throw new ArgumentException("Kata pencarian wajib diisi.");
        }
        kata = kata.Trim();
        var awal = string.IsNullOrWhiteSpace(dari) ? (DateTime?)null : ParseTanggal(dari);
        var akhir = string.IsNullOrWhiteSpace(sampai) ? (DateTime?)null : ParseTanggal(sampai);
        if (awal.HasValue && akhir.HasValue && awal.Value > akhir.Value)
        {
            throw new ArgumentException("Tanggal akhir harus sama dengan atau sesudah tanggal awal.");
        }
        DateTime? akhirEksklusif = null;
        if (akhir.HasValue)
        {
            try
            {
                akhirEksklusif = akhir.Value.AddDays(1);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException("Tanggal akhir tidak dapat digunakan.");
            }
        }

        var pola = BuatPolaLike(kata);
        await using var db = _factory.CreateBankContext();
        var dokumen = db.CbTransHs.AsNoTracking().Where(h =>
            (h.Keterangan != null && EF.Functions.Like(EF.Functions.Collate(h.Keterangan, "Latin1_General_100_CI_AS"), pola, "\\")) ||
            h.CbTransDs.Any(d => d.Keterangan != null && EF.Functions.Like(EF.Functions.Collate(d.Keterangan, "Latin1_General_100_CI_AS"), pola, "\\")));
        if (awal.HasValue)
        {
            dokumen = dokumen.Where(h => h.Tanggal >= awal.Value);
        }
        if (akhirEksklusif.HasValue)
        {
            dokumen = dokumen.Where(h => h.Tanggal < akhirEksklusif.Value);
        }

        var jumlah = await dokumen.CountAsync(cancellationToken);
        var jumlahKelompok = await dokumen
            .Select(h => h.Kurs == null ? "" : h.Kurs.Trim().ToUpper()).Distinct().CountAsync(cancellationToken);
        var kelompok = await dokumen.GroupBy(h => h.Kurs == null ? "" : h.Kurs.Trim().ToUpper())
            .Select(g => new { MataUang = g.Key, Jumlah = g.Count() })
            .OrderBy(x => x.MataUang).Take(BatasBaris).ToListAsync(cancellationToken);
        var kodeMataUang = kelompok.Where(x => x.MataUang != "").Select(x => x.MataUang).ToArray();
        var totals = new Dictionary<string, (decimal Masuk, decimal Keluar)>();
        if (kodeMataUang.Length > 0)
        {
            var detail = db.CbTransDs.AsNoTracking().Where(d =>
                dokumen.Select(h => h.CbTransHId).Contains(d.CbTransHId)
                && d.CbTransH.Kurs != null && kodeMataUang.Contains(d.CbTransH.Kurs.Trim().ToUpper()));
            var totalData = await detail.GroupBy(d => d.CbTransH.Kurs.Trim().ToUpper())
                .Select(g => new { MataUang = g.Key, Masuk = g.Sum(d => d.Terima), Keluar = g.Sum(d => d.Bayar) })
                .ToListAsync(cancellationToken);
            totals = totalData.ToDictionary(x => x.MataUang, x => (x.Masuk, x.Keluar));
        }
        var totalPerMataUang = kelompok.Select(x =>
        {
            if (x.MataUang == "")
            {
                return new KasBankTotalMataUang(null, x.Jumlah, null, null);
            }
            var total = totals.GetValueOrDefault(x.MataUang);
            return new KasBankTotalMataUang(x.MataUang, x.Jumlah, total.Masuk, total.Keluar);
        }).ToList();
        var data = await dokumen.OrderByDescending(h => h.Tanggal).ThenByDescending(h => h.CbTransHId)
            .Take(BatasBaris)
            .Select(h => new
            {
                h.DocNo, h.Tanggal, h.KodeBank, h.Refno, h.Keterangan,
                MataUang = h.Kurs == null ? "" : h.Kurs.Trim().ToUpper(),
                DetailCocok = h.CbTransDs.Where(d => d.Keterangan != null && EF.Functions.Like(EF.Functions.Collate(d.Keterangan, "Latin1_General_100_CI_AS"), pola, "\\"))
                    .OrderBy(d => d.CbTransDId).Select(d => d.Keterangan).FirstOrDefault(),
                Masuk = h.CbTransDs.Sum(d => (decimal?)d.Terima) ?? 0m,
                Keluar = h.CbTransDs.Sum(d => (decimal?)d.Bayar) ?? 0m
            }).ToListAsync(cancellationToken);
        var transaksi = data.Select(x =>
        {
            var mataUang = string.IsNullOrEmpty(x.MataUang) ? null : x.MataUang;
            return new KasBankItem(x.DocNo, x.Tanggal, x.KodeBank, x.Refno, x.Keterangan, x.DetailCocok,
                mataUang, mataUang is null ? null : x.Masuk, mataUang is null ? null : x.Keluar);
        }).ToList();
        _collector.Tambah(new TabelHasil($"Ringkasan kas/bank: {jumlah.ToString("N0", Indonesia)} dokumen; {jumlahKelompok.ToString("N0", Indonesia)} kelompok mata uang; maksimal {BatasBaris} kelompok",
            new[] { "Mata uang header", "Dokumen cocok", "Total masuk", "Total keluar" },
            totalPerMataUang.Select(x => (IReadOnlyList<string>)new[]
            {
                x.MataUang ?? "Tidak diketahui", x.JumlahDokumen.ToString("N0", Indonesia),
                FormatNilaiKasBank(x.MataUang, x.TotalMasuk), FormatNilaiKasBank(x.MataUang, x.TotalKeluar)
            }).ToList()));
        _collector.Tambah(new TabelHasil($"Dokumen kas/bank terbaru (maksimal {BatasBaris} baris; transfer antarbank muncul per dokumen)",
            new[] { "Tanggal", "Dokumen", "Bank", "Referensi", "Keterangan header", "Contoh detail cocok", "Mata uang header", "Masuk", "Keluar" },
            transaksi.Select(x => (IReadOnlyList<string>)new[]
            {
                FormatTanggal(x.Tanggal), x.Dokumen, x.Bank, x.Referensi, x.Keterangan, x.KeteranganDetailCocok,
                x.MataUang ?? "Tidak diketahui", FormatNilaiKasBank(x.MataUang, x.Terima), FormatNilaiKasBank(x.MataUang, x.Bayar)
            }).ToList()));
        return new HasilKasBank(jumlah, jumlahKelompok, totalPerMataUang, transaksi);
    }

    private static string FormatNilaiKasBank(string mataUang, decimal? nilai) =>
        !nilai.HasValue ? "Tidak tersedia (mata uang tidak diketahui)"
        : mataUang == "IDR" ? FormatRupiah(nilai.Value) : FormatAngka(nilai.Value);

    private static string BuatPolaLike(string kata) =>
        "%" + kata.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_").Replace("[", "\\[") + "%";
}
