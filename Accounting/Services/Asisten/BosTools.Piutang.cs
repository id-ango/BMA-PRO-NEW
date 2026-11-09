using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Services.Asisten;

public sealed record PiutangItem(
    string Customer,
    string NamaCustomer,
    string Dokumen,
    string Kode,
    string KodeTransaksi,
    DateTime Tanggal,
    DateTime? JatuhTempo,
    decimal Sisa,
    int? HariTerlambat);

public sealed record HasilPiutangJatuhTempo(
    int JumlahJatuhTempo,
    decimal TotalJatuhTempo,
    IReadOnlyList<PiutangItem> JatuhTempo,
    int JumlahTanpaJatuhTempo,
    decimal TotalTanpaJatuhTempo,
    IReadOnlyList<PiutangItem> TanpaJatuhTempo);

public sealed partial class BosTools
{
    [Description("Tampilkan maksimal 20 dokumen piutang bersaldo positif yang sudah lewat tanggal jatuh tempo, diurutkan berdasarkan sisa terbesar. Dokumen bersaldo positif tanpa tanggal jatuh tempo dilaporkan terpisah.")]
    public async Task<HasilPiutangJatuhTempo> GetPiutangJatuhTempo(CancellationToken cancellationToken = default)
    {
        var hariIni = DateTime.Today;
        await using var db = _factory.CreatePiutangContext();
        var piutang = db.ArPiutngs.AsNoTracking().Where(x => x.Sisa > 0);
        var lewat = piutang.Where(x => x.DueDate.HasValue && x.DueDate.Value < hariIni);
        var tanpaTanggal = piutang.Where(x => !x.DueDate.HasValue);

        var jumlahLewat = await lewat.CountAsync(cancellationToken);
        var totalLewat = await lewat.SumAsync(x => (decimal?)x.Sisa, cancellationToken) ?? 0m;
        var jumlahTanpaTanggal = await tanpaTanggal.CountAsync(cancellationToken);
        var totalTanpaTanggal = await tanpaTanggal.SumAsync(x => (decimal?)x.Sisa, cancellationToken) ?? 0m;

        var dataLewat = await lewat.OrderByDescending(x => x.Sisa).ThenBy(x => x.ArPiutngId)
            .Take(BatasBaris)
            .Select(x => new
            {
                x.Customer, x.Dokumen, x.Kode, x.KodeTran, x.Tanggal, x.DueDate, x.Sisa,
                Nama = db.ArCusts.AsNoTracking().Where(c => c.Customer == x.Customer).Select(c => c.NamaCust).FirstOrDefault()
            }).ToListAsync(cancellationToken);
        var dataTanpaTanggal = await tanpaTanggal.OrderByDescending(x => x.Sisa).ThenBy(x => x.ArPiutngId)
            .Take(BatasBaris)
            .Select(x => new
            {
                x.Customer, x.Dokumen, x.Kode, x.KodeTran, x.Tanggal, x.DueDate, x.Sisa,
                Nama = db.ArCusts.AsNoTracking().Where(c => c.Customer == x.Customer).Select(c => c.NamaCust).FirstOrDefault()
            }).ToListAsync(cancellationToken);

        var hasilLewat = dataLewat.Select(x => new PiutangItem(
            x.Customer, x.Nama, x.Dokumen, x.Kode, x.KodeTran, x.Tanggal, x.DueDate, x.Sisa,
            (hariIni - x.DueDate.Value.Date).Days)).ToList();
        var hasilTanpaTanggal = dataTanpaTanggal.Select(x => new PiutangItem(
            x.Customer, x.Nama, x.Dokumen, x.Kode, x.KodeTran, x.Tanggal, null, x.Sisa, null)).ToList();
        var hasil = new HasilPiutangJatuhTempo(jumlahLewat, totalLewat, hasilLewat,
            jumlahTanpaTanggal, totalTanpaTanggal, hasilTanpaTanggal);

        _collector.Tambah(new TabelHasil(
            $"Piutang lewat jatuh tempo: {jumlahLewat.ToString("N0", Indonesia)} dokumen; total sisa {FormatRupiah(totalLewat)}; maksimal {BatasBaris} baris",
            new[] { "Customer", "Nama", "Dokumen", "Kode", "Kode transaksi", "Tanggal", "Jatuh tempo", "Sisa", "Hari terlambat" },
            hasilLewat.Select(x => (IReadOnlyList<string>)new[]
            {
                x.Customer, x.NamaCustomer ?? "Tidak tersedia", x.Dokumen, x.Kode, x.KodeTransaksi,
                FormatTanggal(x.Tanggal), FormatTanggal(x.JatuhTempo.Value), FormatRupiah(x.Sisa),
                x.HariTerlambat.Value.ToString("N0", Indonesia)
            }).ToList()));
        _collector.Tambah(new TabelHasil(
            $"Piutang tanpa tanggal jatuh tempo: {jumlahTanpaTanggal.ToString("N0", Indonesia)} dokumen; total sisa {FormatRupiah(totalTanpaTanggal)}; maksimal {BatasBaris} baris",
            new[] { "Customer", "Nama", "Dokumen", "Kode", "Kode transaksi", "Tanggal", "Sisa" },
            hasilTanpaTanggal.Select(x => (IReadOnlyList<string>)new[]
            {
                x.Customer, x.NamaCustomer ?? "Tidak tersedia", x.Dokumen, x.Kode, x.KodeTransaksi,
                FormatTanggal(x.Tanggal), FormatRupiah(x.Sisa)
            }).ToList()));
        return hasil;
    }
}
