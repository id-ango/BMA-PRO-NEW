using eSoft.CashBank.Data;
using eSoft.Penjualan.Data;
using eSoft.Persediaan.Data;
using eSoft.Piutang.Data;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Services.Asisten;

public sealed class BmaReadOnlyContextFactory
{
    private readonly IConfiguration _configuration;

    public BmaReadOnlyContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbContextPersediaan CreatePersediaanContext()
    {
        var options = new DbContextOptionsBuilder<DbContextPersediaan>()
            .UseSqlServer(GetConnectionString())
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;
        return new DbContextPersediaan(options);
    }

    public DbContextJual CreateJualContext()
    {
        var options = new DbContextOptionsBuilder<DbContextJual>()
            .UseSqlServer(GetConnectionString())
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;
        return new DbContextJual(options);
    }

    public DbContextPiutang CreatePiutangContext()
    {
        var options = new DbContextOptionsBuilder<DbContextPiutang>()
            .UseSqlServer(GetConnectionString())
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;
        return new DbContextPiutang(options);
    }

    public DbContextBank CreateBankContext()
    {
        var options = new DbContextOptionsBuilder<DbContextBank>()
            .UseSqlServer(GetConnectionString())
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;
        return new DbContextBank(options);
    }

    private string GetConnectionString()
    {
        var connectionString = _configuration.GetConnectionString("BmaReadOnly");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("ConnectionStrings:BmaReadOnly belum dikonfigurasi.");
        }

        return connectionString;
    }
}
