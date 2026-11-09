using System.ComponentModel;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Services.Asisten;

public sealed record HasilPenjualan(
    DateTime Dari,
    DateTime Sampai,
    DateTime DariSebelumnya,
    DateTime SampaiSebelumnya,
    decimal Jumlah,
    decimal JumlahSebelumnya,
    decimal? PerubahanPersen);

public sealed partial class BosTools
{
    [Description("Bandingkan jumlah penjualan bersih periode tanggal yyyy-MM-dd dengan periode sebelumnya yang sama panjang. Nilai menggunakan Jumlah kode 94 dikurangi kode 95; persentase tidak tersedia jika periode sebelumnya nol atau negatif.")]
    public async Task<HasilPenjualan> GetPenjualan(string dari, string sampai, CancellationToken cancellationToken = default)
    {
        var awal = ParseTanggal(dari);
        var akhir = ParseTanggal(sampai);
        if (akhir < awal)
        {
            throw new ArgumentException("Tanggal akhir harus sama dengan atau sesudah tanggal awal.");
        }

        var lamaHari = (akhir - awal).Days + 1;
        DateTime awalSebelumnya;
        DateTime akhirEksklusif;
        try
        {
            awalSebelumnya = awal.AddDays(-lamaHari);
            akhirEksklusif = akhir.AddDays(1);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new ArgumentException("Rentang tanggal tidak dapat dibandingkan.");
        }

        await using var db = _factory.CreateJualContext();
        var transaksi = db.OeTransHs.AsNoTracking().Where(x => x.Kode == "94" || x.Kode == "95");
        var jumlah = await transaksi.Where(x => x.Tanggal >= awal && x.Tanggal < akhirEksklusif)
            .SumAsync(x => (decimal?)(x.Kode == "94" ? x.Jumlah : -x.Jumlah), cancellationToken) ?? 0m;
        var jumlahSebelumnya = await transaksi.Where(x => x.Tanggal >= awalSebelumnya && x.Tanggal < awal)
            .SumAsync(x => (decimal?)(x.Kode == "94" ? x.Jumlah : -x.Jumlah), cancellationToken) ?? 0m;
        var perubahan = HitungPerubahan(jumlah, jumlahSebelumnya);
        var hasil = new HasilPenjualan(awal, akhir, awalSebelumnya, awal.AddDays(-1), jumlah, jumlahSebelumnya, perubahan);

        _collector.Tambah(new TabelHasil(
            "Perbandingan penjualan bersih (Jumlah penjualan dikurangi retur)",
            new[] { "Periode", "Dari", "Sampai", "Jumlah" },
            new List<IReadOnlyList<string>>
            {
                new[] { "Dipilih", FormatTanggal(awal), FormatTanggal(akhir), FormatRupiah(jumlah) },
                new[] { "Sebelumnya", FormatTanggal(awalSebelumnya), FormatTanggal(awal.AddDays(-1)), FormatRupiah(jumlahSebelumnya) },
                new[] { "Perubahan (%)", "", "", perubahan.HasValue ? FormatAngka(perubahan.Value) + "%" : "Tidak tersedia (periode sebelumnya nol atau negatif)" }
            }));
        return hasil;
    }

    private static DateTime ParseTanggal(string nilai)
    {
        if (!DateTime.TryParseExact(nilai, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var tanggal))
        {
            throw new ArgumentException("Tanggal harus menggunakan format yyyy-MM-dd.");
        }
        return tanggal.Date;
    }

    private static string FormatTanggal(DateTime tanggal) => tanggal.ToString("dd MMM yyyy", Indonesia);

    private static string FormatRupiah(decimal nilai) => "Rp " + FormatAngka(nilai);

    private static decimal? HitungPerubahan(decimal jumlah, decimal sebelumnya) =>
        sebelumnya > 0 ? Math.Round((jumlah - sebelumnya) / sebelumnya * 100m, 2, MidpointRounding.AwayFromZero) : null;
}
