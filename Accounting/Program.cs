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
builder.Services.AddMemoryCache();
// builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

builder.Services.AddDbContext<DbContextBank>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));

builder.Services.AddDbContext<DbContextLedger>(options =>
       options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));

builder.Services.AddDbContext<DbContextPiutang>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting").CommandTimeout(180)));

builder.Services.AddDbContext<DbContextHutang>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));

builder.Services.AddDbContext<DbContextPersediaan>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));

builder.Services.AddDbContext<DbContextBeli>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));

builder.Services.AddDbContext<DbContextJual>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));

builder.Services.AddDbContext<DbContextOrder>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));

builder.Services.AddDbContext<DbContextCompany>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));

builder.Services.AddDbContext<DbContextAssets>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));

builder.Services.AddDbContext<DbContextFinancial>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));
builder.Services.AddDbContext<DbContextTestRun>(options =>
      options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Accounting")));

builder.Services.AddTransient<ICashBankServices, CashBankServices>();
builder.Services.AddTransient<ILedgerServices, LedgerServices>();
builder.Services.AddTransient<IReceivableServices, ReceivableServices>();
builder.Services.AddTransient<IPaymentArServices, PaymentArServices>();
builder.Services.AddTransient<IPaymentArDpServices, PaymentArDpServices>();
builder.Services.AddTransient<IPayableServices, PayableServices>();
builder.Services.AddTransient<IPaymentApServices, PaymentApServices>();
builder.Services.AddTransient<IPaymentApDpServices, PaymentApDpServices>();
builder.Services.AddTransient<IInventoryServices, InventoryServices>();
builder.Services.AddTransient<IIcAdjustServices, IcAdjustServices>();
builder.Services.AddTransient<IPurchaseServices, PurchaseServices>();
builder.Services.AddTransient<ISalesDocumentNumberService, SalesDocumentNumberService>();
builder.Services.AddTransient<ISalesDetailFactory, SalesDetailFactory>();
builder.Services.AddTransient<ISalesInventoryAdjustmentService, SalesInventoryAdjustmentService>();
builder.Services.AddTransient<ISalesReceivableService, SalesReceivableService>();
builder.Services.AddTransient<ISalesmanMasterService, SalesmanMasterService>();
builder.Services.AddTransient<IKurirMasterService, KurirMasterService>();
builder.Services.AddTransient<ISalesReportService, SalesReportService>();
builder.Services.AddTransient<ISalesQueryService, SalesQueryService>();
builder.Services.AddTransient<ISalesCommandService, SalesCommandService>();
builder.Services.AddTransient<ISalesServices, SalesServices>();
builder.Services.AddTransient<IOrderPurchaseServices, OrderPurchaseServices>();
builder.Services.AddTransient<ILaporanStockServices, LaporanStockServices>();
builder.Services.AddTransient<ICompanyServices, CompanyServices>();
builder.Services.AddTransient<IAdministrationServices, AdministrationServices>();
builder.Services.AddTransient<IExcelServices, ExcelServices>();
builder.Services.AddTransient<IAssetServices, AssetServices>();
builder.Services.AddTransient<IOrderSalesServices, OrderSalesServices>();
builder.Services.AddTransient<IFinancialServices, FinancialServices>();
builder.Services.AddTransient<ITestRunServices, TestRunServices>();
builder.Services.AddScoped<IDashboardCacheService, DashboardCacheService>();


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

