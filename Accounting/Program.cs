using Accounting.Areas.Identity;
using Accounting.Data;
using Accounting.Services;

using eSoft.Asset.Data;
using eSoft.Asset.Services;
using eSoft.CashBank.Data;
using eSoft.CashBank.Services;
using eSoft.Company.Data;
using eSoft.Company.Services;
using eSoft.Hutang.Data;
using eSoft.Hutang.Services;
using eSoft.LaporanStock.Services;
using eSoft.Ledger.Data;
using eSoft.Ledger.Services;
using eSoft.Order.Data;
using eSoft.Order.Services;
using eSoft.Pembelian.Data;
using eSoft.Pembelian.Services;
using eSoft.Penjualan.Data;
using eSoft.Penjualan.Services;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Services;
using eSoft.Piutang.Data;
using eSoft.Piutang.Services;
using eSoft.Financial.Data;
using eSoft.Financial.View;
using eSoft.Financial.Services;
using eSoft.TestRun.Data;
using eSoft.TestRun.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;



var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.Limits.MaxRequestBodySize = long.MaxValue; // Set the maximum file size here
//});

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
 //   options.SignIn.RequireConfirmedAccount = true;
     options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();



builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
// builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

builder.Services.AddDbContextFactory<DbContextBank>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));
builder.Services.AddScoped<DbContextBank>(sp => sp.GetRequiredService<IDbContextFactory<DbContextBank>>().CreateDbContext());

builder.Services.AddDbContextFactory<DbContextLedger>(options =>
       options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));
builder.Services.AddScoped<DbContextLedger>(sp => sp.GetRequiredService<IDbContextFactory<DbContextLedger>>().CreateDbContext());

builder.Services.AddDbContextFactory<DbContextPiutang>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting").CommandTimeout(180)));
builder.Services.AddScoped<DbContextPiutang>(sp => sp.GetRequiredService<IDbContextFactory<DbContextPiutang>>().CreateDbContext());

builder.Services.AddDbContextFactory<DbContextHutang>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));
builder.Services.AddScoped<DbContextHutang>(sp => sp.GetRequiredService<IDbContextFactory<DbContextHutang>>().CreateDbContext());

builder.Services.AddDbContextFactory<DbContextPersediaan>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));
builder.Services.AddScoped<DbContextPersediaan>(sp => sp.GetRequiredService<IDbContextFactory<DbContextPersediaan>>().CreateDbContext());

builder.Services.AddDbContextFactory<DbContextBeli>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));
builder.Services.AddScoped<DbContextBeli>(sp => sp.GetRequiredService<IDbContextFactory<DbContextBeli>>().CreateDbContext());

builder.Services.AddDbContextFactory<DbContextJual>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));
builder.Services.AddScoped<DbContextJual>(sp => sp.GetRequiredService<IDbContextFactory<DbContextJual>>().CreateDbContext());

builder.Services.AddDbContextFactory<DbContextOrder>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));
builder.Services.AddScoped<DbContextOrder>(sp => sp.GetRequiredService<IDbContextFactory<DbContextOrder>>().CreateDbContext());

builder.Services.AddDbContextFactory<DbContextCompany>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));
builder.Services.AddScoped<DbContextCompany>(sp => sp.GetRequiredService<IDbContextFactory<DbContextCompany>>().CreateDbContext());

builder.Services.AddDbContextFactory<DbContextAssets>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));
builder.Services.AddScoped<DbContextAssets>(sp => sp.GetRequiredService<IDbContextFactory<DbContextAssets>>().CreateDbContext());

builder.Services.AddDbContextFactory<DbContextFinancial>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));
builder.Services.AddScoped<DbContextFinancial>(sp => sp.GetRequiredService<IDbContextFactory<DbContextFinancial>>().CreateDbContext());
// builder.Services.AddDbContext<DbContextTestRun>(options =>
//      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));

builder.Services.AddScoped<ICashBankServices, CashBankServices>();
builder.Services.AddScoped<ILedgerServices, LedgerServices>();
builder.Services.AddScoped<IReceivableServices, ReceivableServices>();
builder.Services.AddScoped<IPaymentArServices, PaymentArServices>();
builder.Services.AddScoped<IPaymentArDpServices, PaymentArDpServices>();
builder.Services.AddScoped<IPayableServices, PayableServices>();
builder.Services.AddScoped<IPaymentApServices, PaymentApServices>();
builder.Services.AddScoped<IPaymentApDpServices, PaymentApDpServices>();
builder.Services.AddScoped<IInventoryServices, InventoryServices>();
builder.Services.AddScoped<IIcAdjustServices, IcAdjustServices>();
builder.Services.AddScoped<IPurchaseServices, PurchaseServices>();
builder.Services.AddScoped<ISalesDocumentNumberService, SalesDocumentNumberService>();
builder.Services.AddScoped<ISalesDetailFactory, SalesDetailFactory>();
builder.Services.AddScoped<ISalesInventoryAdjustmentService, SalesInventoryAdjustmentService>();
builder.Services.AddScoped<ISalesReceivableService, SalesReceivableService>();
builder.Services.AddScoped<ISalesmanMasterService, SalesmanMasterService>();
builder.Services.AddScoped<IKurirMasterService, KurirMasterService>();
builder.Services.AddScoped<ISalesReportService, SalesReportService>();
builder.Services.AddScoped<ISalesQueryService, SalesQueryService>();
builder.Services.AddScoped<ISalesCommandService, SalesCommandService>();
builder.Services.AddScoped<ISalesServices, SalesServices>();
builder.Services.AddScoped<IOrderPurchaseServices, OrderPurchaseServices>();
builder.Services.AddScoped<ILaporanStockServices, LaporanStockServices>();
builder.Services.AddScoped<ICompanyServices, CompanyServices>();
builder.Services.AddScoped<IAdministrationServices, AdministrationServices>();
builder.Services.AddScoped<IExcelServices, ExcelServices>();
builder.Services.AddScoped<IAssetServices, AssetServices>();
builder.Services.AddScoped<IOrderSalesServices, OrderSalesServices>();
builder.Services.AddScoped<IFinancialServices, FinancialServices>();
// builder.Services.AddTransient<ITestRunServices, TestRunServices>();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();


app.UseStaticFiles();

app.UseRouting();

 app.UseAuthentication();
 app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
//app.MapBlazorHub(configureOptions: options =>
//{
//    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
//});
app.MapFallbackToPage("/_Host");

app.Run();

//endpoints.MapBlazorHub(configureOptions: options =>
//{
//    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
//});

