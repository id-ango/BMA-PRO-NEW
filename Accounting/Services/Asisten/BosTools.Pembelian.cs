using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Services.Asisten;

public sealed record SaranPembelianItem(
    string Sku,
    string Nama,
    string Satuan,
    decimal Stok,
    decimal RataJualPerMinggu,
    int LeadTimeBulan,
    int LeadTimeHari,
    int BufferHari,
    decimal SaranJumlah,
    bool Nonaktif);

public sealed record HasilRekomendasiPembelian(string Status, SaranPembelianItem Item);

public sealed partial class BosTools
{
    [Description("Hitung saran jumlah pesan untuk satu kode barang berdasarkan rata-rata penjualan kode 94 selama 28 hari, lead time bulan kalender dan buffer konfigurasi. PO dan SO yang masih terbuka tidak diperhitungkan.")]
    public async Task<HasilRekomendasiPembelian> GetRekomendasiPembelian(string sku, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new ArgumentException("Kode barang wajib diisi.");
        }
        sku = sku.Trim();
        var hariIni = DateTime.Today;
        var akhirEksklusif = hariIni.AddDays(1);
        var awal = akhirEksklusif.AddDays(-28);
        var leadTimeBulan = _configuration.GetValue<int?>("AI:LeadTimeMonths") ?? 3;
        var bufferHari = _configuration.GetValue<int?>("AI:BufferDays") ?? 14;
        if (leadTimeBulan < 0 || bufferHari < 0)
        {
            throw new InvalidOperationException("Konfigurasi lead time atau buffer tidak valid.");
        }
        var leadTimeHari = (hariIni.AddMonths(leadTimeBulan) - hariIni).Days;

        await using var persediaan = _factory.CreatePersediaanContext();
        var items = await persediaan.IcItems.AsNoTracking().Where(x => x.ItemCode == sku)
            .OrderBy(x => x.IcItemId).Take(2)
            .Select(x => new { x.ItemCode, x.NamaItem, x.Satuan, x.Qty, x.Disabled })
            .ToListAsync(cancellationToken);
        if (items.Count != 1)
        {
            var status = items.Count == 0 ? "Kode barang tidak ditemukan." : "Kode barang tidak unik; rekomendasi tidak dapat dihitung.";
            _collector.Tambah(new TabelHasil("Rekomendasi pembelian", new[] { "Kode barang", "Hasil" },
                new List<IReadOnlyList<string>> { new[] { sku, status } }));
            return new HasilRekomendasiPembelian(status, null);
        }

        var item = items[0];
        await using var penjualan = _factory.CreateJualContext();
        var jumlahJual = await penjualan.OeTransDs.AsNoTracking()
            .Where(x => x.ItemCode == item.ItemCode && x.Kode == "94" && x.Tanggal >= awal && x.Tanggal < akhirEksklusif)
            .SumAsync(x => (decimal?)x.Qty, cancellationToken) ?? 0m;
        var rataJual = jumlahJual / 4m;
        var saranJumlah = HitungSaranPembelian(rataJual, leadTimeHari, bufferHari, item.Qty);
        var saran = new SaranPembelianItem(item.ItemCode, item.NamaItem, item.Satuan, item.Qty,
            rataJual, leadTimeBulan, leadTimeHari, bufferHari, saranJumlah, item.Disabled);
        _collector.Tambah(new TabelHasil(
            $"Saran pembelian (penjualan {FormatTanggal(awal)}–{FormatTanggal(hariIni)}; PO/SO terbuka belum diperhitungkan)",
            new[] { "Kode", "Nama", "Satuan", "Stok", "Rata-rata jual/minggu", "Lead time (hari)", "Buffer (hari)", "Saran jumlah", "Status" },
            new List<IReadOnlyList<string>>
            {
                new[] { saran.Sku, saran.Nama, saran.Satuan, FormatAngka(saran.Stok), FormatAngka(saran.RataJualPerMinggu),
                    leadTimeHari.ToString("N0", Indonesia), bufferHari.ToString("N0", Indonesia), FormatAngka(saran.SaranJumlah),
                    saran.Nonaktif ? "Nonaktif" : "Aktif" }
            }));
        return new HasilRekomendasiPembelian("Tersedia", saran);
    }

    private static decimal HitungSaranPembelian(decimal rataJual, int leadTimeHari, int bufferHari, decimal stok) =>
        Math.Max(0m, Math.Ceiling(rataJual * ((decimal)leadTimeHari + bufferHari) / 7m - stok));
}
