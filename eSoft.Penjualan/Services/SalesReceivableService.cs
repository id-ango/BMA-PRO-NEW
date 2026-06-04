using System.Linq;
using eSoft.Penjualan.Model;
using eSoft.Piutang.Data;
using eSoft.Piutang.Model;

namespace eSoft.Penjualan.Services
{
    public class SalesReceivableService : ISalesReceivableService
    {
        private readonly DbContextPiutang _contextAr;

        public SalesReceivableService(DbContextPiutang contextAr)
        {
            _contextAr = contextAr;
        }

        public bool HasSettlement(string documentNo)
        {
            return _contextAr.ArPiutngs.Any(x => x.Dokumen == documentNo && x.Bayar > 0);
        }

        public void ApplySaleReceivable(OeTransH transH, bool nonPiutang)
        {
            if (nonPiutang)
            {
                return;
            }

            var customer = GetCustomer(transH.Customer);
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

            _contextAr.ArPiutngs.Add(piutang);
            customer.Piutang += transH.Jumlah;
            _contextAr.ArCusts.Update(customer);
        }

        public void ApplyReturnReceivable(OeTransH transH)
        {
            var customer = GetCustomer(transH.Customer);
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

            _contextAr.ArPiutngs.Add(piutang);
            customer.Piutang -= transH.Jumlah;
            _contextAr.ArCusts.Update(customer);
        }

        public void ReverseExistingReceivable(OeTransH existingTrans)
        {
            if (existingTrans.Cek != "1")
            {
                return;
            }

            var customer = GetCustomer(existingTrans.Customer);
            if (customer != null)
            {
                customer.Piutang -= existingTrans.Jumlah;
                _contextAr.ArCusts.Update(customer);
            }

            var piutang = _contextAr.ArPiutngs.FirstOrDefault(x => x.Dokumen == existingTrans.NoLpb);
            if (piutang != null)
            {
                _contextAr.ArPiutngs.Remove(piutang);
                _contextAr.Entry(piutang).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
        }

        public void ReverseExistingReceivableForEdit(OeTransH existingTrans)
        {
            var customer = GetCustomer(existingTrans.Customer);
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

                _contextAr.ArCusts.Update(customer);
            }

            var piutang = _contextAr.ArPiutngs.FirstOrDefault(x => x.Dokumen == existingTrans.NoLpb && x.Bayar == 0);
            if (piutang != null)
            {
                _contextAr.ArPiutngs.Remove(piutang);
                _contextAr.Entry(piutang).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
        }

        public void ApplyEditedReceivable(OeTransH transH, bool nonPiutang)
        {
            if (nonPiutang)
            {
                return;
            }

            var customer = GetCustomer(transH.Customer);
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

            _contextAr.ArPiutngs.Add(piutang);

            if (transH.Kode == "94")
            {
                customer.Piutang += transH.Jumlah;
            }
            else
            {
                customer.Piutang -= transH.Jumlah;
            }

            _contextAr.ArCusts.Update(customer);
        }

        private ArCust GetCustomer(string customerCode)
        {
            return _contextAr.ArCusts.FirstOrDefault(x => x.Customer == customerCode);
        }
    }
}
