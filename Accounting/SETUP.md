# Setup Koneksi Database

## Development (Lokal)

Connection string untuk development disimpan di `appsettings.Development.json` (tidak dikomit ke git).

Buat/edit file `Accounting/appsettings.Development.json` dengan isi:

```json
{
  "DetailedErrors": true,
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

Ganti `YOUR_SERVER` dan `YOUR_DB` sesuai konfigurasi lokal Anda.

### Alternatif: Menggunakan User Secrets

Project ini sudah dikonfigurasi dengan User Secrets ID: `aspnet-Accounting-C0DD8C5E-2FE3-4D23-B9F9-20334E339888`

Jalankan perintah berikut di folder `Accounting/`:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER;Database=YOUR_DB;Trusted_Connection=True;MultipleActiveResultSets=true"
```

## Production

Gunakan **Environment Variable** untuk menyimpan connection string di server production:

```
ConnectionStrings__DefaultConnection=Server=PROD_SERVER;Database=PROD_DB;...
```

Atau gunakan layanan seperti **Azure Key Vault** / **AWS Secrets Manager**.

## Catatan Keamanan

- Jangan pernah commit connection string yang berisi password ke repository
- File `appsettings.Development.json` sudah ditambahkan ke `.gitignore`
