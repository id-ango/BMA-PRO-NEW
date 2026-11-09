using System.ComponentModel;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Services.Asisten;

public sealed record StokMenipisItem(
    string Sku,
    string Nama,
    string Satuan,
    decimal Stok,
    decimal StokMinimum,
    decimal RataJualPerMinggu,
    bool Nonaktif);

public sealed record MinimumNolItem(
    string Sku,
    string Nama,
    string Satuan,
    decimal Stok,
    bool Nonaktif);

public sealed record HasilStokMenipis(
    string PeriodeDari,
    string PeriodeSampai,
    int LeadTimeBulan,
    int LeadTimeHari,
    int JumlahStokMenipis,
    IReadOnlyList<StokMenipisItem> StokMenipis,
    int JumlahMinimumNol,
    IReadOnlyList<MinimumNolItem> MinimumNol);

public sealed partial class BosTools
{
    private const int BatasBaris = 20;
    private static readonly CultureInfo Indonesia = CultureInfo.GetCultureInfo("id-ID");
    private readonly BmaReadOnlyContextFactory _factory;
    private readonly HasilToolCollector _collector;
    private readonly IConfiguration _configuration;

    public BosTools(BmaReadOnlyContextFactory factory, HasilToolCollector collector, IConfiguration configuration)
    {
        _factory = factory;
        _collector = collector;
        _configuration = configuration;
    }

    [Description("Tampilkan maksimal 20 barang dengan stok di bawah minimum, rata-rata penjualan mingguan selama 28 hari termasuk hari ini, dan lead time. Barang dengan minimum nol dilaporkan terpisah. Penjualan hanya mencakup kode 94 dan tidak mengurangi retur.")]
    public async Task<HasilStokMenipis> GetStokMenipis(CancellationToken cancellationToken = default)
    {
        var hariIni = DateTime.Today;
        var akhirEksklusif = hariIni.AddDays(1);
        var awal = akhirEksklusif.AddDays(-28);
        var leadTimeBulan = _configuration.GetValue<int?>("AI:LeadTimeMonths") ?? 3;
        if (leadTimeBulan < 0)
        {
            throw new InvalidOperationException("AI:LeadTimeMonths tidak boleh negatif.");
        }

        var leadTimeHari = (hariIni.AddMonths(leadTimeBulan) - hariIni).Days;
        await using var persediaan = _factory.CreatePersediaanContext();
        var stokQuery = persediaan.IcItems.AsNoTracking()
            .Where(item => item.MinQty != 0 && item.Qty < item.MinQty);
        var minimumNolQuery = persediaan.IcItems.AsNoTracking()
            .Where(item => item.MinQty == 0);

        var jumlahStokMenipis = await stokQuery.CountAsync(cancellationToken);
        var stok = await stokQuery.OrderBy(item => item.ItemCode).ThenBy(item => item.IcItemId)
            .Take(BatasBaris)
            .Select(item => new { item.ItemCode, item.NamaItem, item.Satuan, item.Qty, item.MinQty, item.Disabled })
            .ToListAsync(cancellationToken);
        var jumlahMinimumNol = await minimumNolQuery.CountAsync(cancellationToken);
        var minimumNolData = await minimumNolQuery.OrderBy(item => item.ItemCode).ThenBy(item => item.IcItemId)
            .Take(BatasBaris)
            .Select(item => new { item.ItemCode, item.NamaItem, item.Satuan, item.Qty, item.Disabled })
            .ToListAsync(cancellationToken);

        var stokMenipis = new List<StokMenipisItem>();
        if (stok.Count > 0)
        {
            await using var penjualan = _factory.CreateJualContext();
            foreach (var item in stok)
            {
                var jumlahJual = await penjualan.OeTransDs.AsNoTracking()
                    .Where(jual => jual.ItemCode == item.ItemCode && jual.Kode == "94"
                        && jual.Tanggal >= awal && jual.Tanggal < akhirEksklusif)
                    .SumAsync(jual => (decimal?)jual.Qty, cancellationToken) ?? 0m;

                stokMenipis.Add(new StokMenipisItem(
                    item.ItemCode, item.NamaItem, item.Satuan, item.Qty, item.MinQty,
                    jumlahJual / 4m, item.Disabled));
            }
        }

        var minimumNol = minimumNolData.Select(item => new MinimumNolItem(
            item.ItemCode, item.NamaItem, item.Satuan, item.Qty, item.Disabled)).ToList();
        var hasil = new HasilStokMenipis(
            awal.ToString("dd MMM yyyy", Indonesia),
            hariIni.ToString("dd MMM yyyy", Indonesia),
            leadTimeBulan, leadTimeHari, jumlahStokMenipis, stokMenipis,
            jumlahMinimumNol, minimumNol);

        _collector.Tambah(new TabelHasil(
            $"Stok menipis ({jumlahStokMenipis.ToString("N0", Indonesia)} barang; maksimal {BatasBaris})",
            new[] { "Kode", "Nama", "Satuan", "Stok", "Minimum", "Rata-rata jual/minggu", "Lead time (hari)", "Status" },
            stokMenipis.Select(item => (IReadOnlyList<string>)new[]
            {
                item.Sku, item.Nama, item.Satuan, FormatAngka(item.Stok), FormatAngka(item.StokMinimum),
                FormatAngka(item.RataJualPerMinggu), leadTimeHari.ToString("N0", Indonesia),
                item.Nonaktif ? "Nonaktif" : "Aktif"
            }).ToList()));
        _collector.Tambah(new TabelHasil(
            $"Minimum stok nol ({jumlahMinimumNol.ToString("N0", Indonesia)} barang; maksimal {BatasBaris})",
            new[] { "Kode", "Nama", "Satuan", "Stok", "Status" },
            minimumNol.Select(item => (IReadOnlyList<string>)new[]
            {
                item.Sku, item.Nama, item.Satuan, FormatAngka(item.Stok),
                item.Nonaktif ? "Nonaktif" : "Aktif"
            }).ToList()));

        return hasil;
    }

    private static string FormatAngka(decimal angka) => angka.ToString("#,##0.######", Indonesia);
}
