using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using eSoft.Penjualan.Data;
using eSoft.Penjualan.Model;
using eSoft.Penjualan.View;
using eSoft.Piutang.Data;
using eSoft.Piutang.Model;
using eSoft.Persediaan.Data;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Penjualan.Services
{
    public class SalesCommandService : ISalesCommandService
    {
        private static readonly TransactionOptions SalesTransactionOptions = new()
        {
            IsolationLevel = IsolationLevel.Serializable,
            Timeout = TransactionManager.MaximumTimeout
        };

        private readonly IDbContextFactory<DbContextJual> _context;
        private readonly IDbContextFactory<DbContextPiutang> _contextAr;
        private readonly IDbContextFactory<DbContextPersediaan> _contextIc;
        private readonly ISalesDocumentNumberService _salesDocumentNumberService;
        private readonly ISalesDetailFactory _salesDetailFactory;
        private readonly ISalesInventoryAdjustmentService _salesInventoryAdjustmentService;
        private readonly ISalesReceivableService _salesReceivableService;
        private readonly ISalesQueryService _salesQueryService;

        public SalesCommandService(
            IDbContextFactory<DbContextJual> context,
            IDbContextFactory<DbContextPiutang> contextPiutang,
            IDbContextFactory<DbContextPersediaan> contextPersediaan,
            ISalesDocumentNumberService salesDocumentNumberService,
            ISalesDetailFactory salesDetailFactory,
            ISalesInventoryAdjustmentService salesInventoryAdjustmentService,
            ISalesReceivableService salesReceivableService,
            ISalesQueryService salesQueryService)
        {
            _context = context;
            _contextAr = contextPiutang;
            _contextIc = contextPersediaan;
            _salesDocumentNumberService = salesDocumentNumberService;
            _salesDetailFactory = salesDetailFactory;
            _salesInventoryAdjustmentService = salesInventoryAdjustmentService;
            _salesReceivableService = salesReceivableService;
            _salesQueryService = salesQueryService;
        }

        public OeTransH AddTransH(OeTransHView trans, bool pajak)
        {
            using var scope = new TransactionScope(TransactionScopeOption.Required, SalesTransactionOptions, TransactionScopeAsyncFlowOption.Enabled);
            using var context = _context.CreateDbContext();
            using var contextAr = _contextAr.CreateDbContext();
            using var contextIc = _contextIc.CreateDbContext();

            var noLpb = pajak ? _salesDocumentNumberService.GetNumberTax() : _salesDocumentNumberService.GetNumber();

            OeTransH transH = CreateTransHeader(trans, noLpb, "94", pajak, trans.NamaCust, trans.NonPiutang ? "" : "1");

            _salesInventoryAdjustmentService.ApplyDetailsForCode(trans.OeTransDs, "94");

            context.OeTransHs.Add(transH);

            if (!trans.NonPiutang)
            {
                _salesReceivableService.ApplySaleReceivable(transH, trans.NonPiutang);
            }

            context.SaveChanges();
            contextAr.SaveChanges();
            contextIc.SaveChanges();

            var tempTrans = _salesQueryService.GetOeTransDokumen(transH.NoLpb);

            scope.Complete();

            return tempTrans;
        }

        public OeTransH AddTransHRetur(OeTransHView trans, bool pajak)
        {
            using var scope = new TransactionScope(TransactionScopeOption.Required, SalesTransactionOptions, TransactionScopeAsyncFlowOption.Enabled);
            using var context = _context.CreateDbContext();
            using var contextAr = _contextAr.CreateDbContext();
            using var contextIc = _contextIc.CreateDbContext();

            var noLpb = pajak ? _salesDocumentNumberService.GetNumberTaxRetur() : _salesDocumentNumberService.GetNumberRetur();

            OeTransH transH = CreateTransHeader(trans, noLpb, "95", pajak, GetCustomerName(trans.Customer), "1");

            _salesInventoryAdjustmentService.ApplyDetailsForCode(trans.OeTransDs, "95");

            context.OeTransHs.Add(transH);
            _salesReceivableService.ApplyReturnReceivable(transH);

            context.SaveChanges();
            contextAr.SaveChanges();
            contextIc.SaveChanges();

            var tempTrans = _salesQueryService.GetOeTransDokumen(transH.NoLpb);

            scope.Complete();

            return tempTrans;
        }

        public async Task<bool> DelTransH(int id)
        {
            using var scope = new TransactionScope(TransactionScopeOption.Required, SalesTransactionOptions, TransactionScopeAsyncFlowOption.Enabled);
            using var context = _context.CreateDbContext();
            using var contextAr = _contextAr.CreateDbContext();
            using var contextIc = _contextIc.CreateDbContext();

            var existingTrans = context.OeTransHs
                .Include(y => y.OeTransDs)
                .FirstOrDefault(x => x.OeTransHId == id);

            if (existingTrans == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(existingTrans.Cek) && _salesReceivableService.HasSettlement(existingTrans.NoLpb))
            {
                return false;
            }

            _salesInventoryAdjustmentService.ReverseDetails(existingTrans.OeTransDs, existingTrans.Kode);

            if (existingTrans.Cek == "1")
            {
                _salesReceivableService.ReverseExistingReceivable(existingTrans);
            }

            context.OeTransHs.Remove(existingTrans);
            await context.SaveChangesAsync();
            await contextAr.SaveChangesAsync();
            await contextIc.SaveChangesAsync();

            scope.Complete();
            return true;
        }

        public bool EditTransH(OeTransHView trans)
        {
            using var context = _context.CreateDbContext();
            
            var existingTrans = context.OeTransHs
                .Include(h => h.OeTransDs)
                .FirstOrDefault(x => x.NoLpb == trans.NoLpb);

            if (existingTrans == null)
            {
                Console.WriteLine("Data transaksi tidak ditemukan.");
                return false;
            }

            if (!string.IsNullOrEmpty(existingTrans.Cek) && _salesReceivableService.HasSettlement(existingTrans.NoLpb))
            {
                Console.WriteLine("Transaksi tidak dapat diubah/hapus karena sudah ada pelunasan.");
                return false;
            }

            try
            {
                using var scope = new TransactionScope(TransactionScopeOption.Required, SalesTransactionOptions, TransactionScopeAsyncFlowOption.Enabled);
                using var contextAr = _contextAr.CreateDbContext();
                using var contextIc = _contextIc.CreateDbContext();

                _salesInventoryAdjustmentService.ReverseDetails(existingTrans.OeTransDs, existingTrans.Kode);

                _salesReceivableService.ReverseExistingReceivableForEdit(existingTrans);
                context.OeTransHs.Remove(existingTrans);

                OeTransH transH = CreateTransHeader(
                    trans,
                    trans.NoLpb,
                    existingTrans.Kode,
                    trans.Pajak,
                    GetCustomerName(trans.Customer),
                    trans.NonPiutang ? "" : "1",
                    trans.Kurir);

                _salesInventoryAdjustmentService.ApplyDetailsForCode(trans.OeTransDs, existingTrans.Kode);

                if (!trans.NonPiutang)
                {
                    _salesReceivableService.ApplyEditedReceivable(transH, trans.NonPiutang);
                }

                context.OeTransHs.Add(transH);
                contextAr.SaveChanges();
                contextIc.SaveChanges();
                context.SaveChanges();

                scope.Complete();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("caught exception" + e.Message);
                return false;
            }
        }

        private ArCust GetCustomerId(string id)
        {
            using var contextAr = _contextAr.CreateDbContext();
            return contextAr.ArCusts.FirstOrDefault(x => x.Customer == id);
        }

        private OeTransH CreateTransHeader(
            OeTransHView trans,
            string noLpb,
            string kode,
            bool pajak,
            string? namaCust,
            string cek,
            string? kurir = null)
        {
            return new OeTransH
            {
                NoLpb = noLpb,
                Customer = NormalizeCustomerCode(trans.Customer),
                NamaCust = namaCust,
                AlamatKirim = trans.AlamatKirim,
                Tanggal = trans.Tanggal,
                JthTempo = trans.JthTempo,
                Keterangan = trans.Keterangan,
                NoPrj = trans.NoPrj,
                Salesman = trans.Salesman,
                Jumlah = trans.Jumlah,
                Discount = trans.Discount,
                Ongkos = trans.Ongkos,
                Ppn = trans.Ppn,
                PpnPersen = trans.PpnPersen,
                TtlJumlah = trans.TtlJumlah,
                DPayment = trans.DPayment,
                Tagihan = trans.Tagihan,
                TotalQty = trans.TotalQty,
                Kode = kode,
                Cek = cek,
                Pajak = pajak,
                Kurir = kurir,
                OeTransDs = _salesDetailFactory.CreateDetails(trans, noLpb, kode)
            };
        }

        private static string NormalizeCustomerCode(string customer)
        {
            return customer?.ToUpper() ?? string.Empty;
        }

        private string? GetCustomerName(string customer)
        {
            return GetCustomerId(NormalizeCustomerCode(customer))?.NamaLengkap;
        }

        public bool CekPiutang(OeTransH trans)
        {
            using var contextAr = _contextAr.CreateDbContext();
            var cekFirst = contextAr.ArPiutngs.Where(x => x.Dokumen == trans.NoLpb && x.Sisa == 0).FirstOrDefault();
            return cekFirst != null;
        }
    }
}
