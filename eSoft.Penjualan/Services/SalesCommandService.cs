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

        private readonly DbContextJual _context;
        private readonly DbContextPiutang _contextAr;
        private readonly DbContextPersediaan _contextIc;
        private readonly ISalesDocumentNumberService _salesDocumentNumberService;
        private readonly ISalesDetailFactory _salesDetailFactory;
        private readonly ISalesInventoryAdjustmentService _salesInventoryAdjustmentService;
        private readonly ISalesReceivableService _salesReceivableService;
        private readonly ISalesQueryService _salesQueryService;

        public SalesCommandService(
            DbContextJual context,
            DbContextPiutang contextPiutang,
            DbContextPersediaan contextPersediaan,
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

            var noLpb = pajak ? _salesDocumentNumberService.GetNumberTax() : _salesDocumentNumberService.GetNumber();

            OeTransH transH = new OeTransH
            {
                NoLpb = noLpb,
                Customer = trans.Customer.ToUpper(),
                NamaCust = trans.NamaCust,
                AlamatKirim = trans.AlamatKirim,
                Tanggal = trans.Tanggal,
                JthTempo = trans.JthTempo,
                Keterangan = trans.Keterangan,
                NoPrj = trans.NoPrj,
                Salesman = trans.Salesman,
                Jumlah = trans.Jumlah,
                Ongkos = trans.Ongkos,
                Ppn = trans.Ppn,
                Discount = trans.Discount,
                PpnPersen = trans.PpnPersen,
                TtlJumlah = trans.TtlJumlah,
                DPayment = trans.DPayment,
                Tagihan = trans.Tagihan,
                TotalQty = trans.TotalQty,
                Kode = "94",
                Cek = trans.NonPiutang ? "" : "1",
                Pajak = pajak,
                OeTransDs = _salesDetailFactory.CreateDetails(trans, noLpb, "94")
            };

            _salesInventoryAdjustmentService.ApplyDetailsForCode(trans.OeTransDs, "94");

            _context.OeTransHs.Add(transH);

            if (!trans.NonPiutang)
            {
                _salesReceivableService.ApplySaleReceivable(transH, trans.NonPiutang);
            }

            _context.SaveChanges();
            _contextAr.SaveChanges();
            _contextIc.SaveChanges();

            var tempTrans = _salesQueryService.GetOeTransDokumen(transH.NoLpb);

            scope.Complete();

            return tempTrans;
        }

        public OeTransH AddTransHRetur(OeTransHView trans, bool pajak)
        {
            using var scope = new TransactionScope(TransactionScopeOption.Required, SalesTransactionOptions, TransactionScopeAsyncFlowOption.Enabled);

            var noLpb = pajak ? _salesDocumentNumberService.GetNumberTaxRetur() : _salesDocumentNumberService.GetNumberRetur();

            OeTransH transH = new OeTransH
            {
                NoLpb = noLpb,
                Customer = trans.Customer.ToUpper(),
                NamaCust = GetCustomerId(trans.Customer.ToUpper())?.NamaLengkap,
                Tanggal = trans.Tanggal,
                Keterangan = trans.Keterangan,
                AlamatKirim = trans.AlamatKirim,
                Jumlah = trans.Jumlah,
                Ongkos = trans.Ongkos,
                Ppn = trans.Ppn,
                PpnPersen = trans.PpnPersen,
                TtlJumlah = trans.TtlJumlah,
                DPayment = trans.DPayment,
                Tagihan = trans.Tagihan,
                TotalQty = trans.TotalQty,
                Kode = "95",
                Cek = "1",
                Pajak = pajak,
                OeTransDs = _salesDetailFactory.CreateDetails(trans, noLpb, "95")
            };

            _salesInventoryAdjustmentService.ApplyDetailsForCode(trans.OeTransDs, "95");

            _context.OeTransHs.Add(transH);
            _salesReceivableService.ApplyReturnReceivable(transH);

            _context.SaveChanges();
            _contextAr.SaveChanges();
            _contextIc.SaveChanges();

            var tempTrans = _salesQueryService.GetOeTransDokumen(transH.NoLpb);

            scope.Complete();

            return tempTrans;
        }

        public async Task<bool> DelTransH(int id)
        {
            using var scope = new TransactionScope(TransactionScopeOption.Required, SalesTransactionOptions, TransactionScopeAsyncFlowOption.Enabled);

            var existingTrans = _context.OeTransHs
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

            _context.OeTransHs.Remove(existingTrans);
            await _context.SaveChangesAsync();
            await _contextAr.SaveChangesAsync();
            await _contextIc.SaveChangesAsync();

            scope.Complete();
            return true;
        }

        public bool EditTransH(OeTransHView trans)
        {
            var existingTrans = _context.OeTransHs
                .Include(h => h.OeTransDs)
                .FirstOrDefault(x => x.NoLpb == trans.NoLpb);

            if (existingTrans == null)
            {
                Console.WriteLine("Data transaksi tidak ditemukan.");
                return false;
            }

            if (string.IsNullOrEmpty(existingTrans.Cek) == false && _salesReceivableService.HasSettlement(existingTrans.NoLpb))
            {
                Console.WriteLine("Transaksi tidak dapat diubah/hapus karena sudah ada pelunasan.");
                return false;
            }

            try
            {
                using var scope = new TransactionScope(TransactionScopeOption.Required, SalesTransactionOptions, TransactionScopeAsyncFlowOption.Enabled);

                _salesInventoryAdjustmentService.ReverseDetails(existingTrans.OeTransDs, existingTrans.Kode);

                _salesReceivableService.ReverseExistingReceivableForEdit(existingTrans);
                _context.OeTransHs.Remove(existingTrans);

                OeTransH transH = new OeTransH
                {
                    NoLpb = trans.NoLpb,
                    Customer = trans.Customer.ToUpper(),
                    NamaCust = GetCustomerId(trans.Customer.ToUpper())?.NamaLengkap,
                    Salesman = trans.Salesman,
                    Tanggal = trans.Tanggal,
                    JthTempo = trans.JthTempo,
                    Keterangan = trans.Keterangan,
                    AlamatKirim = trans.AlamatKirim,
                    NoPrj = trans.NoPrj,
                    Jumlah = trans.Jumlah,
                    Discount = trans.Discount,
                    Ongkos = trans.Ongkos,
                    Ppn = trans.Ppn,
                    PpnPersen = trans.PpnPersen,
                    TtlJumlah = trans.TtlJumlah,
                    DPayment = trans.DPayment,
                    Tagihan = trans.Tagihan,
                    TotalQty = trans.TotalQty,
                    Kode = existingTrans.Kode,
                    Cek = trans.NonPiutang ? "" : "1",
                    Pajak = trans.Pajak,
                    Kurir = trans.Kurir,
                    OeTransDs = _salesDetailFactory.CreateDetails(trans, trans.NoLpb, existingTrans.Kode)
                };

                _salesInventoryAdjustmentService.ApplyDetailsForCode(trans.OeTransDs, existingTrans.Kode);

                if (!trans.NonPiutang)
                {
                    _salesReceivableService.ApplyEditedReceivable(transH, trans.NonPiutang);
                }

                _context.OeTransHs.Add(transH);
                _contextAr.SaveChanges();
                _contextIc.SaveChanges();
                _context.SaveChanges();

                scope.Complete();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("caught exception" + e.Message);
                return false;
            }
        }

        public bool CekPiutang(OeTransH trans)
        {
            var cekFirst = _contextAr.ArPiutngs.Where(x => x.Dokumen == trans.NoLpb && x.Sisa == 0).FirstOrDefault();
            return cekFirst != null;
        }

        private ArCust GetCustomerId(string id)
        {
            return _contextAr.ArCusts.Where(x => x.Customer == id).FirstOrDefault();
        }
    }
}
