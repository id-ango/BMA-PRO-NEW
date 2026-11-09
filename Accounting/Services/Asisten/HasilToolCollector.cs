namespace Accounting.Services.Asisten;

public sealed record TabelHasil(
    string Judul,
    IReadOnlyList<string> Kolom,
    IReadOnlyList<IReadOnlyList<string>> Baris);

public sealed class HasilToolCollector
{
    private readonly List<TabelHasil> _tabel = new();
    private readonly object _lock = new();

    public void Tambah(TabelHasil tabel)
    {
        lock (_lock)
        {
            _tabel.Add(tabel);
        }
    }

    public List<TabelHasil> Ambil()
    {
        lock (_lock)
        {
            return _tabel.ToList();
        }
    }

    public void Bersihkan()
    {
        lock (_lock)
        {
            _tabel.Clear();
        }
    }
}
