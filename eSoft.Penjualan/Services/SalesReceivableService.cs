using System.Linq;
using eSoft.Penjualan.Model;
using eSoft.Piutang.Data;
using eSoft.Piutang.Model;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Penjualan.Services
{
    public class SalesReceivableService : ISalesReceivableService
    {
        private readonly IDbContextFactory<DbContextPiutang> _contextAr;

        public SalesReceivableService(IDbContextFactory<DbContextPiutang> contextAr)
        {
            _contextAr = contextAr;
        }

        public bool HasSettlement(string documentNo)
        {
            using var contextAr = CreatePiutangContext();

            return contextAr.ArPiutngs.Any(x => x.Dokumen == documentNo && x.Bayar > 0);
        }

        public void ApplySaleReceivable(OeTransH transH, bool nonPiutang)
        {
            if (nonPiutang)
            {
                return;
            }

            using var contextAr = CreatePiutangContext();

            var customer = GetCustomer(contextAr, transH.Customer);
            if (customer == null)
            {
                return;
            }

            var piutang = new ArPiutng
            {
                Kode = "OE",
                Dokumen = transH.NoLpb,
                Tanggal = transH.Tanggal,
                Salesman = transH.Salesman,
                DueDate = transH.JthTempo,
                Customer = transH.Customer,
                Keterangan = transH.Keterangan,
                Jumlah = transH.Jumlah,
                Sisa = transH.Jumlah,
                SldSisa = transH.Jumlah,
                KodeTran = transH.Kode
            };

            contextAr.ArPiutngs.Add(piutang);
            customer.Piutang += transH.Jumlah;
            contextAr.ArCusts.Update(customer);
        }

        public void ApplyReturnReceivable(OeTransH transH)
        {
            using var contextAr = CreatePiutangContext();

            var customer = GetCustomer(contextAr, transH.Customer);
            if (customer == null)
            {
                return;
            }

            var piutang = new ArPiutng
            {
                Kode = "OE",
                Dokumen = transH.NoLpb,
                Tanggal = transH.Tanggal,
                DueDate = transH.Tanggal.AddDays(customer.Termin),
                Customer = transH.Customer,
                Keterangan = transH.Keterangan,
                Jumlah = -1 * transH.Jumlah,
                Sisa = -1 * transH.Jumlah,
                SldSisa = -1 * transH.Jumlah,
                KodeTran = transH.Kode
            };

            contextAr.ArPiutngs.Add(piutang);
            customer.Piutang -= transH.Jumlah;
            contextAr.ArCusts.Update(customer);
        }

        public void ReverseExistingReceivable(OeTransH existingTrans)
        {
            if (existingTrans.Cek != "1")
            {
                return;
            }

            using var contextAr = CreatePiutangContext();

            var customer = GetCustomer(contextAr, existingTrans.Customer);
            if (customer != null)
            {
                customer.Piutang -= existingTrans.Jumlah;
                contextAr.ArCusts.Update(customer);
            }

            var piutang = contextAr.ArPiutngs.FirstOrDefault(x => x.Dokumen == existingTrans.NoLpb);
            if (piutang != null)
            {
                contextAr.ArPiutngs.Remove(piutang);
            }
        }

        public void ReverseExistingReceivableForEdit(OeTransH existingTrans)
        {
            using var contextAr = CreatePiutangContext();

            var customer = GetCustomer(contextAr, existingTrans.Customer);
            if (customer != null)
            {
                if (existingTrans.Kode == "94")
                {
                    customer.Piutang -= existingTrans.Jumlah;
                }
                else
                {
                    customer.Piutang += existingTrans.Jumlah;
                }

                contextAr.ArCusts.Update(customer);
            }

            var piutang = contextAr.ArPiutngs.FirstOrDefault(x => x.Dokumen == existingTrans.NoLpb && x.Bayar == 0);
            if (piutang != null)
            {
                contextAr.ArPiutngs.Remove(piutang);
            }
        }

        public void ApplyEditedReceivable(OeTransH transH, bool nonPiutang)
        {
            if (nonPiutang)
            {
                return;
            }

            using var contextAr = CreatePiutangContext();

            var customer = GetCustomer(contextAr, transH.Customer);
            if (customer == null)
            {
                return;
            }

            var signedAmount = transH.Kode == "94" ? transH.Jumlah : -1 * transH.Jumlah;

            var piutang = new ArPiutng
            {
                Kode = "OE",
                Dokumen = transH.NoLpb,
                Tanggal = transH.Tanggal,
                DueDate = transH.JthTempo,
                Customer = transH.Customer,
                Salesman = transH.Salesman,
                Keterangan = transH.Keterangan,
                Jumlah = signedAmount,
                Sisa = signedAmount,
                SldSisa = signedAmount,
                KodeTran = transH.Kode
            };

            contextAr.ArPiutngs.Add(piutang);

            if (transH.Kode == "94")
            {
                customer.Piutang += transH.Jumlah;
            }
            else
            {
                customer.Piutang -= transH.Jumlah;
            }

            contextAr.ArCusts.Update(customer);
        }

        private ArCust GetCustomer(DbContextPiutang contextAr, string customerCode)
        {
            return contextAr.ArCusts.FirstOrDefault(x => x.Customer == customerCode);
        }

        private DbContextPiutang CreatePiutangContext()
        {
            return _contextAr.CreateDbContext();
        }
    }
}
