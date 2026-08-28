using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Model;
using eSoft.Persediaan.View;
using eSoft.Penjualan.Data;
using eSoft.Penjualan.Model;
using eSoft.Penjualan.View;
using eSoft.Pembelian.Data;
using eSoft.Pembelian.Model;
using eSoft.Hutang.Data;
using eSoft.Piutang.Data;
using eSoft.Piutang.Model;
using eSoft.Piutang.View;
using System.Globalization;
using System.Net.NetworkInformation;
using eSoft.Order.Data;
using eSoft.Order.Model;
using eSoft.Order.View;
using eSoft.Persediaan.Services;
using Microsoft.EntityFrameworkCore;

using System.Security.Cryptography;
using eSoft.Hutang.Model;
using ClosedXML.Excel;
using System.IO;
using System.IO.Packaging;

namespace eSoft.LaporanStock.Services
{
    public class LaporanStockServices : ILaporanStockServices
    {
        private readonly DbContextPersediaan _context;
        private readonly DbContextBeli _contextIR;
        private readonly DbContextJual _contextOE;
        private readonly DbContextPiutang _contextAR;
        private readonly DbContextOrder _contextOR;
        private readonly DbContextHutang _contextAP;


        public LaporanStockServices(DbContextPersediaan context, DbContextBeli contextBeli, DbContextJual contextJual, DbContextPiutang contextPiutang, DbContextOrder contextOrder, DbContextHutang contextHutang)
        {
            _context = context;
            _contextIR = contextBeli;
            _contextOE = contextJual;
            _contextAR = contextPiutang;
            _contextOR = contextOrder;
            _contextAP = contextHutang;



        }

        private ApSuppl GetSupplierKode(string kode)
        {
            return _contextAP.ApSuppls.Where(x => x.Supplier == kode).FirstOrDefault();
        }

        public IcDiv GetIcDivKd(string id)
        {
            return _context.IcDivs.Where(x => x.Divisi == id).FirstOrDefault();
        }

        #region Laporan
        public List<IcStockCardView> CetakMutasi(DateTime Tanggal1, DateTime Tanggal2, string kodeBank)
        {
            //   decimal SldAwalJual = 0;
            //   decimal SldAwalBeli = 0;
            //   decimal SldAwalIC = 0;
            decimal SaldoAwal = 0;
            decimal CostAwal = 0;

            IcStockCardView stockCard = new();

            List<IcStockCardView> Transaksi = new List<IcStockCardView>();
            List<OeTransD> TransAwalJual = new List<OeTransD>();
            List<IrTransD> TransAwalBeli = new List<IrTransD>();
            List<IcTransD> TransAwalIC = new List<IcTransD>();

            // private IcItem IcSldAwal = new();
            List<OeTransH> transHOE = new List<OeTransH>();
            List<OeTransD> transDOE = new List<OeTransD>();
            List<IrTransH> transHIR = new List<IrTransH>();
            List<IrTransD> transDIR = new List<IrTransD>();
            List<IcTransH> transHIC = new List<IcTransH>();
            List<IcTransD> transDIC = new List<IcTransD>();

            var IcSldAwal = prosesStockAwal(Tanggal1, Tanggal2, kodeBank);

            #region barisdeleted
            //TransAwalJual = _contextOE.OeTransDs.Where(x => x.ItemCode == kodeBank && x.Tanggal.Date < Tanggal1.Date)
            //    .ToList();
            //TransAwalBeli = _contextIR.IrTransDs.Where(x => x.ItemCode == kodeBank && x.Tanggal.Date < Tanggal1.Date)
            //   .ToList();
            //TransAwalIC = _context.IcTransDs.Where(x => x.ItemCode == kodeBank && x.Tanggal.Date < Tanggal1.Date)
            //   .ToList();
            //IcSldAwal = _context.IcItems.Where(x => x.ItemCode == kodeBank).FirstOrDefault();

            //stockCard.ItemCode = kodeBank;
            //stockCard.Cost = IcSldAwal.CostAwal;
            //stockCard.Qty = IcSldAwal.Qty;


            //if (TransAwalJual != null)
            //{
            //    SldAwalJual = TransAwalJual.Sum(x => (x.Kode == "94" ? -1 * x.Qty : x.Qty));
            //}
            //else
            //{
            //    SldAwalJual = 0;
            //}

            //if (TransAwalBeli != null)
            //{
            //    SldAwalBeli = TransAwalBeli.Sum(x => (x.Kode == "82" ? x.Qty : -1 * x.Qty));
            //}
            //else
            //{
            //    SldAwalBeli = 0;
            //}

            //if (TransAwalIC != null)
            //{
            //    SldAwalIC = TransAwalIC.Sum(x => (x.Kode == "81" || x.Kode == "72" ? x.QtyShp : 0));
            //}
            //else
            //{
            //    SldAwalIC = 0;
            //}

            // SaldoAwal = IcSldAwal.SaldoAwal + SldAwalBeli + SldAwalIC + SldAwalJual;
            #endregion


            Transaksi.Add(new IcStockCardView
            {
                ItemCode = kodeBank,
                Tanggal = Tanggal1,
                Keterangan = "Saldo Awal",
                Qty = IcSldAwal.Qty,
                Cost = IcSldAwal.Cost,
                HrgCost = IcSldAwal.HrgNetto

            }
                );

            // Optimization: Filter by ItemCode BEFORE Include to reduce in-memory data
            transHIR = _contextIR.IrTransHs
                .Include(p => p.IrTransDs)
                .Where(x => x.Tanggal.Date >= Tanggal1.Date && x.Tanggal.Date <= Tanggal2.Date)
                .AsNoTracking()
                .ToList();
            transDIR = _contextIR.IrTransDs
                .Where(x => x.ItemCode == kodeBank && (x.Tanggal.Date >= Tanggal1.Date && x.Tanggal.Date <= Tanggal2.Date))
                .AsNoTracking()
                .ToList();

            if (transHIR != null && transDIR != null)
            {
                var Rincian1 = (from e in transHIR
                                join f in transDIR on e.IrTransHId equals f.IrTransHId
                                select new IcStockCardView()
                                {
                                    Kode = e.Kode,
                                    ItemCode = f.ItemCode,
                                    Keterangan = e.NamaSup + ", " + e.Keterangan,
                                    Tanggal = f.Tanggal.Date,
                                    Dokumen = e.NoLpb,
                                    Qty = (e.Kode == "82" ? f.Qty : -1 * f.Qty),
                                    Cost = f.JumDpp,
                                    HrgCost = f.Harga
                                }).ToList();

                Transaksi.AddRange(Rincian1);
            }

            transHIC = _context.IcTransHs
                .Include(p => p.IcTransDs)
                .Where(x => (x.Tanggal.Date >= Tanggal1.Date && x.Tanggal.Date <= Tanggal2.Date) && (x.Kode == "81" || x.Kode == "72"))
                .AsNoTracking()
                .ToList();
            transDIC = _context.IcTransDs
                .Where(x => x.ItemCode == kodeBank && (x.Tanggal.Date >= Tanggal1.Date && x.Tanggal.Date <= Tanggal2.Date))
                .AsNoTracking()
                .ToList();

            if (transHIC != null && transDIC != null)
            {
                var Rincian2 = (from e in transHIC
                                join f in transDIC on e.IcTransHId equals f.IcTransHId
                                select new IcStockCardView()
                                {
                                    Kode = e.Kode,
                                    ItemCode = f.ItemCode,
                                    Keterangan = e.Keterangan,
                                    Tanggal = f.Tanggal.Date,
                                    Dokumen = e.NoFaktur,
                                    Qty = f.QtyShp,
                                    Cost = f.Jumlah,
                                    HrgCost = f.Harga
                                }).ToList();

                Transaksi.AddRange(Rincian2);
            }

            transHOE = _contextOE.OeTransHs
                .Include(p => p.OeTransDs)
                .Where(x => x.Tanggal.Date >= Tanggal1.Date && x.Tanggal.Date <= Tanggal2.Date)
                .AsNoTracking()
                .ToList();
            transDOE = _contextOE.OeTransDs
                .Where(x => x.ItemCode == kodeBank && (x.Tanggal.Date >= Tanggal1.Date && x.Tanggal.Date <= Tanggal2.Date))
                .AsNoTracking()
                .ToList();

            if (transHOE != null && transDOE != null)
            {
                var Rincian3 = (from e in transHOE
                                join f in transDOE on e.OeTransHId equals f.OeTransHId
                                select new IcStockCardView()
                                {
                                    Kode = e.Kode,
                                    ItemCode = f.ItemCode,
                                    Keterangan = e.NamaCust + ", " + e.Keterangan,
                                    Tanggal = f.Tanggal.Date,
                                    Dokumen = e.NoLpb,
                                    Qty = (e.Kode == "94" ? -1 * f.Qty : f.Qty),
                                    Cost = (e.Kode == "94" ? -1 * f.Cost : f.Cost),
                                    HrgCost = f.HrgCost
                                }).ToList();

                Transaksi.AddRange(Rincian3);
            }

            Transaksi = Transaksi.OrderBy(x => x.Tanggal).ToList();

            SaldoAwal = 0;
            CostAwal = 0;

            var Transaksi3 = Transaksi.Select(i => { SaldoAwal += i.Qty; i.Jumlah = SaldoAwal; return i; }).ToList();
            var Transaksi4 = Transaksi3.Select(i => { CostAwal += i.Cost; i.CostJumlah = CostAwal; return i; }).ToList();
            //foreach (var trans in Transaksi)
            //{
            //    SaldoAwal = SaldoAwal + trans.Qty;
            //    trans.Jumlah = SaldoAwal;

            //}


            return Transaksi4;
        }
        #endregion

        #region prosesStock

        private IcItemView prosesStockAwal(DateTime Tanggal1, DateTime Tanggal2, string kodeBank)
        {

            IcItem MasterStock = _context.IcItems.Where(x => x.ItemCode == kodeBank).FirstOrDefault();
            //  List<IcAltItem> AltStock = _context.IcAltItems.ToList();
            List<OeTransD> TransJual = new List<OeTransD>();
            List<IrTransD> TransBeli = new List<IrTransD>();
            List<IcTransD> TransIC = new List<IcTransD>();
            List<IcStockCardView> TransAwal = new List<IcStockCardView>();



            TransJual = _contextOE.OeTransDs.Where(x => x.ItemCode == kodeBank && x.Tanggal.Date < Tanggal1.Date)
                .ToList();
            TransBeli = _contextIR.IrTransDs.Where(x => x.ItemCode == kodeBank && x.Tanggal.Date < Tanggal1.Date)
               .ToList();
            TransIC = _context.IcTransDs.Where(x => x.ItemCode == kodeBank && x.Tanggal.Date < Tanggal1.Date)
               .ToList();

            if (TransBeli != null)
            {
                foreach (var trans in TransBeli)
                {
                    TransAwal.Add(new IcStockCardView()
                    {
                        Tanggal = trans.Tanggal,
                        Kode = trans.Kode,
                        ItemCode = trans.ItemCode,
                        Dokumen = trans.NoLpb,
                        Qty = trans.Qty,
                        Jumlah = trans.JumDpp,
                        Lokasi = trans.Lokasi,
                        IcCardId = trans.IrTransDId

                    });
                }
            }

            if (TransIC != null)
            {
                foreach (var trans in TransIC)
                {
                    TransAwal.Add(new IcStockCardView()
                    {
                        Tanggal = trans.Tanggal,
                        Kode = trans.Kode,
                        ItemCode = trans.ItemCode,
                        Dokumen = trans.NoFaktur,
                        Qty = trans.QtyShp,
                        Jumlah = trans.Jumlah,
                        Lokasi = trans.Lokasi,
                        Lokasi2 = trans.Lokasi2,
                        IcCardId = trans.IcTransDId

                    });
                }
            }

            if (TransJual != null)
            {
                foreach (var trans in TransJual)
                {
                    TransAwal.Add(new IcStockCardView()
                    {
                        Tanggal = trans.Tanggal,
                        Kode = trans.Kode,
                        ItemCode = trans.ItemCode,
                        Dokumen = trans.NoLpb,
                        Qty = trans.Qty,
                        HrgJual = trans.Harga,
                        Jumlah = trans.Jumlah,
                        Lokasi = trans.Lokasi,
                        IcCardId = trans.OeTransDId

                    });
                }
            }

            IcItemView Awalitem = new();
            Awalitem.ItemCode = MasterStock.ItemCode;
            Awalitem.SaldoAwal = MasterStock.SaldoAwal;
            Awalitem.CostAwal = MasterStock.CostAwal;
            Awalitem.Harga = MasterStock.Harga;
            Awalitem.JnsBrng = (jnsBrng)MasterStock.JnsBrng;

            Awalitem.Qty = 0;
            Awalitem.Cost = 0;

            Awalitem.Qty = Awalitem.SaldoAwal;
            Awalitem.Cost = Awalitem.CostAwal;
            Awalitem.HrgNetto = (Awalitem.SaldoAwal != 0 ? Awalitem.CostAwal / Awalitem.SaldoAwal : Awalitem.Harga);

            foreach (var trans in TransAwal.OrderBy(x => x.Tanggal).ToList())
            {


                if (Awalitem.JnsBrng == jnsBrng.Stock)
                {

                    //if ((trans.Tanggal > item.TglPost || item.TglPost == null) && trans.Tanggal <= DateTime.Today.Date)
                    //{
                    switch (trans.Kode)
                    {
                        case "81":
                            Awalitem.Cost += trans.Jumlah;
                            Awalitem.Qty += trans.Qty;
                            Awalitem.HrgNetto = (Awalitem.Qty != 0 ? (Awalitem.Cost / Awalitem.Qty) : Awalitem.Harga);

                            break;

                        case "82":
                            Awalitem.Cost += trans.Jumlah;
                            Awalitem.Qty += trans.Qty;
                            Awalitem.HrgNetto = (Awalitem.Qty != 0 ? (Awalitem.Cost / Awalitem.Qty) : Awalitem.Harga);

                            break;

                        case "83":
                            Awalitem.Cost -= trans.Jumlah;
                            Awalitem.Qty -= trans.Qty;
                            Awalitem.HrgNetto = (Awalitem.Qty != 0 ? (Awalitem.Cost / Awalitem.Qty) : Awalitem.Harga);

                            break;

                        case "90":

                            break;

                        case "94":
                            //   trans.Jumlah = item.HrgNetto * trans.Qty;
                            //    trans.Harga = item.HrgNetto;
                            Awalitem.Cost -= Awalitem.HrgNetto * trans.Qty;
                            Awalitem.Qty -= trans.Qty;
                            if (Awalitem.HrgJual < trans.HrgJual)
                            {
                                Awalitem.HrgJual = trans.HrgJual;
                            }
                            ;
                            if (Awalitem.Qty < 0)
                                Awalitem.Cost = 0;
                            //  item.HrgNetto = (item.Qty != 0 ? (item.Cost / item.Qty) : item.Harga);
                            //    TransJual.Find(x => x.OeTransDId == trans.IcCardId).Cost = trans.Jumlah;
                            //    TransJual.Find(x => x.OeTransDId == trans.IcCardId).HrgCost = trans.Harga;


                            break;

                        case "95":
                            //     trans.Jumlah = item.HrgNetto * trans.Qty;

                            //  trans.Harga = item.HrgNetto;
                            Awalitem.Cost += Awalitem.HrgNetto * trans.Qty;
                            Awalitem.Qty += trans.Qty;
                            //if (item.Qty < 0)
                            //    item.Cost = 0;
                            Awalitem.HrgNetto = (Awalitem.Qty != 0 ? (Awalitem.Cost / Awalitem.Qty) : Awalitem.Harga);
                            //    trans.Harga = item.HrgNetto;
                            //    TransJual.Find(x => x.OeTransDId == trans.IcCardId).Cost = trans.Jumlah;
                            //    TransJual.Find(x => x.OeTransDId == trans.IcCardId).HrgCost = trans.Harga;


                            break;
                    }
                    //   }

                }
                //   _context.Update(item);
            }


            return Awalitem;

        }
        public async Task prosesStock()
        {

            List<IcItem> MasterStock = _context.IcItems.ToList();
            List<IcAltItem> AltStock = _context.IcAltItems.ToList();
            List<OeTransD> TransJual = new List<OeTransD>();
            List<IrTransD> TransBeli = new List<IrTransD>();
            List<IcTransD> TransIC = new List<IcTransD>();
            List<IcStockCardView> Transaksi = new List<IcStockCardView>();

            List<PoTransH> OrderTrans = _contextOR.PoTransHs.Where(x => x.Kode == "71").ToList();

            foreach (var order in OrderTrans)
            {
                order.Currency = GetSupplierKode(order.Vendor).Kurs;
                if (!string.IsNullOrEmpty(order.Currency))
                {
                    List<PoTransD> transd = _contextOR.PoTransDs.Where(x => x.PoTransHId == order.PoTransHId).ToList();
                    foreach (var transaksi in transd)
                    {
                        if (transaksi.Harga > 0)
                        {
                            MasterStock.Where(x => x.ItemCode == transaksi.ItemCode).FirstOrDefault().HrgUsd = transaksi.Harga;
                            MasterStock.Where(x => x.ItemCode == transaksi.ItemCode).FirstOrDefault().CurrencyCode = order.Currency;
                        }
                    }
                }
            }

            MasterStock.ForEach(i => { i.Qty = 0; i.Cost = 0; });
            AltStock.ForEach(i => { i.Qty = 0; i.Cost = 0; });

            MasterStock.ForEach(i => { i.Qty = i.SaldoAwal; i.Cost = i.CostAwal; });
            MasterStock.ForEach(i => i.HrgNetto = (i.SaldoAwal != 0 ? i.CostAwal / i.SaldoAwal : i.Harga));

            AltStock.ForEach(i => { i.Qty = i.SaldoAwal; i.Cost = i.CostAwal; });


            TransJual = _contextOE.OeTransDs.OrderBy(x => x.Tanggal)
                .ToList();
            TransBeli = _contextIR.IrTransDs.OrderBy(x => x.Tanggal)
               .ToList();
            TransIC = _context.IcTransDs.OrderBy(x => x.Tanggal)
               .ToList();

            foreach (var trans in TransBeli)
            {
                Transaksi.Add(new IcStockCardView()
                {
                    Tanggal = trans.Tanggal.Date,
                    Kode = trans.Kode,
                    ItemCode = trans.ItemCode,
                    Dokumen = trans.NoLpb,
                    Qty = trans.Qty,
                    Jumlah = trans.JumDpp,
                    Lokasi = trans.Lokasi,
                    IcCardId = trans.IrTransDId

                });
            }

            foreach (var trans in TransIC)
            {
                Transaksi.Add(new IcStockCardView()
                {
                    Tanggal = trans.Tanggal.Date,
                    Kode = trans.Kode,
                    ItemCode = trans.ItemCode,
                    Dokumen = trans.NoFaktur,
                    Qty = trans.QtyShp,
                    Jumlah = trans.Jumlah,
                    Lokasi = trans.Lokasi,
                    Lokasi2 = trans.Lokasi2,
                    IcCardId = trans.IcTransDId

                });
            }

            foreach (var trans in TransJual)
            {
                Transaksi.Add(new IcStockCardView()
                {
                    Tanggal = trans.Tanggal.Date,
                    Kode = trans.Kode,
                    ItemCode = trans.ItemCode,
                    Dokumen = trans.NoLpb,
                    Qty = trans.Qty,
                    HrgJual = trans.Harga,
                    Jumlah = trans.Jumlah,
                    Lokasi = trans.Lokasi,
                    IcCardId = trans.OeTransDId

                });
            }



            foreach (var trans in Transaksi.OrderBy(x => x.Tanggal).ToList())
            {
                IcItem item = MasterStock.Find(x => x.ItemCode == trans.ItemCode);
                IcAltItem cekLokasi1 = _context.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == trans.Lokasi).FirstOrDefault();
                if (cekLokasi1 == null)
                {
                    AltStock.Add(new IcAltItem()
                    {
                        ItemCode = item.ItemCode.ToUpper(),
                        NamaItem = item.NamaItem,
                        Satuan = item.Satuan,
                        Lokasi = trans.Lokasi,
                        Qty = 0
                    });


                }
                if (trans.Lokasi2 != null)
                {
                    IcAltItem cekLokasi2 = _context.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == trans.Lokasi2).FirstOrDefault();
                    if (cekLokasi1 == null)
                    {
                        AltStock.Add(new IcAltItem()
                        {
                            ItemCode = item.ItemCode.ToUpper(),
                            NamaItem = item.NamaItem,
                            Satuan = item.Satuan,
                            Lokasi = trans.Lokasi2,
                            Qty = 0
                        });


                    }
                }

                if (item.JnsBrng == 1)
                {

                    //if ((trans.Tanggal > item.TglPost || item.TglPost == null) && trans.Tanggal <= DateTime.Today.Date)
                    //{
                    switch (trans.Kode)
                    {
                        case "81":
                            MasterStock.Find(x => x.ItemCode == trans.ItemCode).Cost += trans.Jumlah;
                            MasterStock.Find(x => x.ItemCode == trans.ItemCode).Qty += trans.Qty;
                            MasterStock.Find(x => x.ItemCode == trans.ItemCode).HrgNetto = (MasterStock.Find(x => x.ItemCode == trans.ItemCode).Qty != 0 ? (MasterStock.Find(x => x.ItemCode == trans.ItemCode).Cost / MasterStock.Find(x => x.ItemCode == trans.ItemCode).Qty) : item.Harga);
                            AltStock.Find(x => x.ItemCode == trans.ItemCode && x.Lokasi == trans.Lokasi).Qty += trans.Qty;
                            break;

                        case "82":
                            MasterStock.Find(x => x.ItemCode == trans.ItemCode).Cost += trans.Jumlah;
                            MasterStock.Find(x => x.ItemCode == trans.ItemCode).Qty += trans.Qty;
                            MasterStock.Find(x => x.ItemCode == trans.ItemCode).HrgNetto = (MasterStock.Find(x => x.ItemCode == trans.ItemCode).Qty != 0 ? (MasterStock.Find(x => x.ItemCode == trans.ItemCode).Cost / MasterStock.Find(x => x.ItemCode == trans.ItemCode).Qty) : MasterStock.Find(x => x.ItemCode == trans.ItemCode).Harga);
                            AltStock.Find(x => x.ItemCode == trans.ItemCode && x.Lokasi == trans.Lokasi).Qty += trans.Qty;
                            break;

                        case "83":
                            MasterStock.Find(x => x.ItemCode == trans.ItemCode).Cost -= trans.Jumlah;
                            MasterStock.Find(x => x.ItemCode == trans.ItemCode).Qty -= trans.Qty;
                            MasterStock.Find(x => x.ItemCode == trans.ItemCode).HrgNetto = (MasterStock.Find(x => x.ItemCode == trans.ItemCode).Qty != 0 ? (MasterStock.Find(x => x.ItemCode == trans.ItemCode).Cost / MasterStock.Find(x => x.ItemCode == trans.ItemCode).Qty) : MasterStock.Find(x => x.ItemCode == trans.ItemCode).Harga);
                            AltStock.Find(x => x.ItemCode == trans.ItemCode && x.Lokasi == trans.Lokasi).Qty -= trans.Qty;
                            break;

                        case "90":
                            //   MasterStock.Find(x => x.ItemCode == trans.ItemCode).Cost += trans.Jumlah;
                            //  MasterStock.Find(x => x.ItemCode == trans.ItemCode).Qty += trans.Qty;
                            //  MasterStock.Find(x => x.ItemCode == trans.ItemCode).HrgNetto = (MasterStock.Find(x => x.ItemCode == trans.ItemCode).Qty != 0 ? (MasterStock.Find(x => x.ItemCode == trans.ItemCode).Cost / MasterStock.Find(x => x.ItemCode == trans.ItemCode).Qty) : item.Harga);
                            AltStock.Find(x => x.ItemCode == trans.ItemCode && x.Lokasi == trans.Lokasi).Qty -= trans.Qty;
                            AltStock.Find(x => x.ItemCode == trans.ItemCode && x.Lokasi == trans.Lokasi2).Qty += trans.Qty;
                            break;

                        case "94":
                            trans.Jumlah = MasterStock.Find(x => x.ItemCode == trans.ItemCode).HrgNetto * trans.Qty;
                            trans.Harga = MasterStock.Find(x => x.ItemCode == trans.ItemCode).HrgNetto;
                            MasterStock.Find(x => x.ItemCode == trans.ItemCode).Cost -= MasterStock.Find(x => x.ItemCode == trans.ItemCode).HrgNetto * trans.Qty;
                            MasterStock.Find(x => x.ItemCode == trans.ItemCode).Qty -= trans.Qty;
                            if (MasterStock.Find(x => x.ItemCode == trans.ItemCode).HrgJual < trans.HrgJual)
                            {
                                MasterStock.Find(x => x.ItemCode == trans.ItemCode).HrgJual = trans.HrgJual;
                            }
                            ;
                            if (MasterStock.Find(x => x.ItemCode == trans.ItemCode).Qty < 0)
                                MasterStock.Find(x => x.ItemCode == trans.ItemCode).Cost = 0;
                            //  item.HrgNetto = (item.Qty != 0 ? (item.Cost / item.Qty) : item.Harga);
                            TransJual.Find(x => x.OeTransDId == trans.IcCardId).Cost = trans.Jumlah;
                            TransJual.Find(x => x.OeTransDId == trans.IcCardId).HrgCost = trans.Harga;

                            AltStock.Find(x => x.ItemCode == trans.ItemCode && x.Lokasi == trans.Lokasi).Qty -= trans.Qty;
                            break;

                        case "95":
                            trans.Jumlah = MasterStock.Find(x => x.ItemCode == trans.ItemCode).HrgNetto * trans.Qty;

                            //  trans.Harga = item.HrgNetto;
                            MasterStock.Find(x => x.ItemCode == trans.ItemCode).Cost += MasterStock.Find(x => x.ItemCode == trans.ItemCode).HrgNetto * trans.Qty;
                            MasterStock.Find(x => x.ItemCode == trans.ItemCode).Qty += trans.Qty;
                            //if (item.Qty < 0)
                            //    item.Cost = 0;
                            MasterStock.Find(x => x.ItemCode == trans.ItemCode).HrgNetto = (MasterStock.Find(x => x.ItemCode == trans.ItemCode).Qty != 0 ? (MasterStock.Find(x => x.ItemCode == trans.ItemCode).Cost / item.Qty) : MasterStock.Find(x => x.ItemCode == trans.ItemCode).Harga);
                            trans.Harga = MasterStock.Find(x => x.ItemCode == trans.ItemCode).HrgNetto;
                            TransJual.Find(x => x.OeTransDId == trans.IcCardId).Cost = trans.Jumlah;
                            TransJual.Find(x => x.OeTransDId == trans.IcCardId).HrgCost = trans.Harga;

                            AltStock.Find(x => x.ItemCode == trans.ItemCode && x.Lokasi == trans.Lokasi).Qty += trans.Qty;
                            break;
                    }
                    //   }

                }
                //   _context.Update(item);
            }


            _context.UpdateRange(MasterStock);
            _context.UpdateRange(AltStock);

            _contextOE.UpdateRange(TransJual);

            await _context.SaveChangesAsync();
            await _contextOE.SaveChangesAsync();


            // return Transaksi;

        }
        #endregion

        #region prosessalespiutang
        public async Task ProsesSalesPiutang()
        {
            List<OeTransH> TransJual = _contextOE.OeTransHs
                .Where(x => !string.IsNullOrEmpty(x.Salesman))
                .ToList();
            List<ArPiutng> TransPiutang = _contextAR.ArPiutngs.ToList();

            foreach (OeTransH transH in TransJual)
            {
                // Find matching ArPiutng record with the same document number (NoLpb)
                var matchingPiutang = TransPiutang
                    .FirstOrDefault(p => p.Dokumen == transH.NoLpb);

                if (matchingPiutang != null)
                {
                    // Update the Salesman field in the matching ArPiutng record
                    matchingPiutang.Salesman = transH.Salesman;
                }
            }
            _contextAR.UpdateRange(TransPiutang);
            // Save all changes to the database in a single transaction
            await _contextAR.SaveChangesAsync();
        }
        #endregion

        #region RubahKodeItem
        public void ProsesRubahKodeItem(string Barang, string UbahItem)
        {


            _context.IcItems
                .Where(x => x.ItemCode == Barang).ToList()
                .ForEach(item => item.ItemCode = UbahItem);
            _context.IcAltItems
                .Where(x => x.ItemCode == Barang).ToList()
                .ForEach(item => item.ItemCode = UbahItem);
            _context.IcTransDs
               .Where(x => x.ItemCode == Barang).ToList()
               .ForEach(item => item.ItemCode = UbahItem);
            _contextOE.OeTransDs
                .Where(x => x.ItemCode == Barang).ToList()
                .ForEach(item => item.ItemCode = UbahItem);
            _contextIR.IrTransDs
                .Where(x => x.ItemCode == Barang).ToList()
                .ForEach(item => item.ItemCode = UbahItem);
            _contextOR.PoTransDs
                .Where(x => x.ItemCode == Barang).ToList()
                .ForEach(item => item.ItemCode = UbahItem);



            _context.SaveChanges();
            _contextOE.SaveChanges();
            _contextIR.SaveChanges();
            _contextOR.SaveChanges();

            // return Transaksi;

        }

        #endregion

        #region LaporanPembelian
        public List<IcItem> CetakItemSupplier(DateTime Tanggal1, DateTime Tanggal2)
        {
            List<IrTransH> irTransHs = new List<IrTransH>();
            List<IrTransD> irTransDs = new List<IrTransD>();
            List<IcItem> icItems = new List<IcItem>();


            irTransHs = _contextIR.IrTransHs.Include(p => p.IrTransDs).Where(x => x.Tanggal.Date >= Tanggal1.Date && x.Tanggal.Date <= Tanggal2.Date).ToList();
            foreach (var items in irTransHs)
            {

                foreach (var item in items.IrTransDs)
                {
                    if (icItems.Any(x => x.ItemCode == item.ItemCode))
                    {

                        ///  icItems.Find(x => x.ItemCode == item.ItemCode).Qty += item.Qty;
                        ///  icItems.Find(x => x.ItemCode == item.ItemCode).Cost += item.Jumlah;

                        icItems.Find(x => x.ItemCode == item.ItemCode).Qty += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                        icItems.Find(x => x.ItemCode == item.ItemCode).Cost += (item.Kode == "82" ? item.Jumlah : -1 * item.Jumlah);
                        //   icItems.Find(x => x.ItemCode == item.ItemCode).BefNetto += (item.Kode == "82" ? item.Jumlah : -1 * item.Jumlah);

                    }
                    else
                    {
                        icItems.Add(
                            new IcItem()
                            {
                                ItemCode = item.ItemCode,

                                Qty = (item.Kode == "82" ? item.Qty : -1 * item.Qty),
                                Cost = (item.Kode == "82" ? item.Jumlah : -1 * item.Jumlah)


                            });
                    }

                }

                foreach (var item in icItems)
                {
                    var produk = GetIcItemProduk(item.ItemCode);

                    item.NamaItem = produk.NamaItem;
                    item.Satuan = produk.Satuan;
                    item.HrgUsd = produk.HrgUsd;
                    item.HrgJual = produk.HrgJual;
                    item.Harga = item.Cost / item.Qty;
                    item.QtyPo = produk.Qty;
                    item.BefNetto = produk.Cost;
                }

            }
            return icItems;
        }

        #endregion

        public IcItem GetIcItemProduk(string itemKode)
        {
            return _context.IcItems.Where(x => x.ItemCode == itemKode).FirstOrDefault();
        }

        public OeKurir GetOeKurirProduk(string itemKode)
        {
            return _contextOE.OeKurirs.Where(x => x.Kurir == itemKode).FirstOrDefault();
        }

        public OeSalesman GetOeSalesProduk(string itemKode)
        {
            return _contextOE.OeSalesmans.Where(x => x.Salesman == itemKode).FirstOrDefault();
        }

        #region LaporanPenjualan
        public List<IcItem> CetakItemCustomer(DateTime Tanggal1, DateTime Tanggal2)
        {
            List<OeTransH> irTransHs = new List<OeTransH>();
            List<OeTransD> irTransDs = new List<OeTransD>();
            List<IcItem> icItems = new List<IcItem>();

            irTransHs = _contextOE.OeTransHs.Include(p => p.OeTransDs).Where(x => x.Tanggal.Date >= Tanggal1.Date && x.Tanggal.Date <= Tanggal2.Date).ToList();
            foreach (var items in irTransHs)
            {

                foreach (var item in items.OeTransDs)
                {
                    if (icItems.Any(x => x.ItemCode == item.ItemCode))
                    {
                        icItems.Find(x => x.ItemCode == item.ItemCode).Qty += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                        icItems.Find(x => x.ItemCode == item.ItemCode).Cost += (item.Kode == "94" ? item.Jumlah : -1 * item.Jumlah);
                        icItems.Find(x => x.ItemCode == item.ItemCode).BefNetto += (item.Kode == "94" ? item.Cost : -1 * item.Cost);
                    }
                    else
                    {
                        icItems.Add(
                            new IcItem()
                            {
                                ItemCode = item.ItemCode,
                                Qty = (item.Kode == "94" ? item.Qty : -1 * item.Qty),
                                Cost = (item.Kode == "94" ? item.Jumlah : -1 * item.Jumlah),
                                BefNetto = (item.Kode == "94" ? item.Cost : -1 * item.Cost)

                            });
                    }

                }



            }

            foreach (var item in icItems)
            {
                var produk = GetIcItemProduk(item.ItemCode);

                item.NamaItem = produk.NamaItem;
                item.Satuan = produk.Satuan;
                item.HrgUsd = produk.HrgUsd;
                item.Harga = produk.HrgNetto;
                // item.Harga = item.BefNetto / item.Qty;
                item.HrgJual = produk.HrgJual;
                // item.HrgJual = item.Cost / item.Qty;
                item.QtyPo = produk.Qty;
            }

            return icItems;
        }

        public List<ArCust> CetakCustomerItem(DateTime Tanggal1, DateTime Tanggal2)
        {
            List<OeTransH> irTransHs = new List<OeTransH>();
            List<OeTransD> irTransDs = new List<OeTransD>();
            List<ArCust> icItems = new List<ArCust>();

            irTransHs = _contextOE.OeTransHs.Where(x => x.Tanggal.Date >= Tanggal1.Date && x.Tanggal.Date <= Tanggal2.Date).ToList();
            foreach (var items in irTransHs)
            {


                if (icItems.Any(x => x.Customer == items.Customer))
                {
                    icItems.Find(x => x.Customer == items.Customer).Piutang += (items.Kode == "94" ? items.TtlJumlah : -1 * items.TtlJumlah);
                    icItems.Find(x => x.Customer == items.Customer).Disc1 += (items.Kode == "94" ? items.Jumlah : -1 * items.Jumlah);
                    icItems.Find(x => x.Customer == items.Customer).Disc2 += (items.Kode == "94" ? items.Ongkos : -1 * items.Ongkos);
                    icItems.Find(x => x.Customer == items.Customer).SldAwal += (items.Kode == "94" ? items.Ppn : -1 * items.Ppn);
                }
                else
                {
                    icItems.Add(
                        new ArCust()
                        {
                            Customer = items.Customer,
                            Piutang = (items.Kode == "94" ? items.TtlJumlah : -1 * items.TtlJumlah),
                            Disc1 = (items.Kode == "94" ? items.Jumlah : -1 * items.Jumlah),
                            Disc2 = (items.Kode == "94" ? items.Ongkos : -1 * items.Ongkos),
                            SldAwal = (items.Kode == "94" ? items.Ppn : -1 * items.Ppn)



                        });
                }


            }

            foreach (var item in icItems)
            {
                var customer = GetCustomerCode(item.Customer);

                item.NamaCust = customer.NamaCust;
                item.Alamat = customer.Alamat;
                item.Kota = customer.Kota;
                item.Telpon = customer.Telpon;

            }

            return icItems;
        }

        public List<OeKurir> CetakKurirCustomer(DateTime Tanggal1, DateTime Tanggal2)
        {
            List<OeTransH> irTransHs = new List<OeTransH>();
            List<OeTransD> irTransDs = new List<OeTransD>();
            List<OeKurir> icItems = new();

            irTransHs = _contextOE.OeTransHs.Include(p => p.OeTransDs).Where(x => x.Tanggal.Date >= Tanggal1.Date && x.Tanggal.Date <= Tanggal2.Date && !string.IsNullOrEmpty(x.Kurir)).ToList();
            foreach (var items in irTransHs)
            {


                if (icItems.Any(x => x.Kurir == items.Kurir))
                {
                    icItems.Find(x => x.Kurir == items.Kurir).Piutang += (items.Kode == "94" ? items.TtlJumlah : -1 * items.TtlJumlah);
                    icItems.Find(x => x.Kurir == items.Kurir).Disc1 += (items.Kode == "94" ? items.Jumlah : -1 * items.Jumlah);
                    icItems.Find(x => x.Kurir == items.Kurir).Disc2 += (items.Kode == "94" ? items.Ongkos : -1 * items.Ongkos);
                    icItems.Find(x => x.Kurir == items.Kurir).SldAwal += (items.Kode == "94" ? items.Ppn : -1 * items.Ppn);
                }
                else
                {
                    icItems.Add(
                        new OeKurir()
                        {
                            Kurir = items.Kurir,
                            Piutang = (items.Kode == "94" ? items.TtlJumlah : -1 * items.TtlJumlah),
                            Disc1 = (items.Kode == "94" ? items.Jumlah : -1 * items.Jumlah),
                            Disc2 = (items.Kode == "94" ? items.Ongkos : -1 * items.Ongkos),
                            SldAwal = (items.Kode == "94" ? items.Ppn : -1 * items.Ppn)



                        });
                }





            }

            foreach (var item in icItems)
            {
                if (!string.IsNullOrEmpty(item.Kurir))
                {
                    var produk = GetOeKurirProduk(item.Kurir);

                    item.NamaKurir = produk.NamaKurir;
                    item.NamaLengkap = produk.NamaLengkap;
                    item.Alamat = produk.Alamat;
                    item.Kota = produk.Kota;
                }
            }

            return icItems;
        }

        public List<OeSalesman> CetakSalesCustomer(DateTime Tanggal1, DateTime Tanggal2)
        {
            List<OeTransH> irTransHs = new List<OeTransH>();
            List<OeTransD> irTransDs = new List<OeTransD>();
            List<OeSalesman> icItems = new();

            irTransHs = _contextOE.OeTransHs.Include(p => p.OeTransDs).Where(x => x.Tanggal.Date >= Tanggal1.Date && x.Tanggal.Date <= Tanggal2.Date && !string.IsNullOrEmpty(x.Salesman)).ToList();
            foreach (var items in irTransHs)
            {

                if (icItems.Any(x => x.Salesman == items.Salesman))
                {
                    icItems.Find(x => x.Salesman == items.Salesman).Piutang += (items.Kode == "94" ? items.TtlJumlah : -1 * items.TtlJumlah);
                    icItems.Find(x => x.Salesman == items.Salesman).Disc1 += (items.Kode == "94" ? items.Jumlah : -1 * items.Jumlah);
                    icItems.Find(x => x.Salesman == items.Salesman).Disc2 += (items.Kode == "94" ? items.Ongkos : -1 * items.Ongkos);
                    icItems.Find(x => x.Salesman == items.Salesman).SldAwal += (items.Kode == "94" ? items.Ppn : -1 * items.Ppn);
                }
                else
                {
                    icItems.Add(
                        new OeSalesman()
                        {
                            Salesman = items.Salesman,
                            Piutang = (items.Kode == "94" ? items.TtlJumlah : -1 * items.TtlJumlah),
                            Disc1 = (items.Kode == "94" ? items.Jumlah : -1 * items.Jumlah),
                            Disc2 = (items.Kode == "94" ? items.Ongkos : -1 * items.Ongkos),
                            SldAwal = (items.Kode == "94" ? items.Ppn : -1 * items.Ppn)



                        });
                }


            }

            foreach (var item in icItems)
            {
                if (!string.IsNullOrEmpty(item.Salesman))
                {
                    var produk = GetOeSalesProduk(item.Salesman);

                    item.NamaSales = produk.NamaSales;
                    item.NamaLengkap = produk.NamaLengkap;
                    item.Alamat = produk.Alamat;
                    item.Kota = produk.Kota;
                }
            }

            return icItems;
        }
        public ArCust GetCustomerCode(string xKode)
        {
            return _contextAR.ArCusts.Where(x => x.Customer == xKode).FirstOrDefault();
        }

        #endregion

        public string GetBulan(int bulan)
        {
            string namabulan = "";

            switch (bulan)
            {
                case 1:
                    namabulan = "Januari";
                    break;
                case 2:
                    namabulan = "Pebruari";
                    break;
                case 3:
                    namabulan = "Maret";
                    break;
                case 4:
                    namabulan = "April";
                    break;
                case 5:
                    namabulan = "Mei";
                    break;
                case 6:
                    namabulan = "Juni";
                    break;
                case 7:
                    namabulan = "Juli";
                    break;
                case 8:
                    namabulan = "Agustus";
                    break;
                case 9:
                    namabulan = "September";
                    break;
                case 10:
                    namabulan = "Oktober";
                    break;
                case 11:
                    namabulan = "Nopember";
                    break;
                case 12:
                    namabulan = "Desember";
                    break;
            }
            return namabulan;
        }

        #region PenjualanperTahun

        public List<OePerTahun> PenjualanperTahun(int tahun)
        {
            int bulantanggal = 0;
            var transHOE = _contextOE.OeTransHs.Include(p => p.OeTransDs).Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).ToList();
            var transHIR = _contextIR.IrTransHs.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).ToList();
            //     var transHOEDt = _contextOE.OeTransHs.Include(p => p.OeTransDs).Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).ToList();

            List<OePerTahun> transaksi = new List<OePerTahun>();



            foreach (var item in transHOE)
            {
                if (item.Tanggal.Month != bulantanggal)
                {
                    bulantanggal = item.Tanggal.Month;

                    OePerTahun perTahun = new OePerTahun();
                    perTahun.Bulan = bulantanggal;
                    perTahun.NamaItem = GetBulan(bulantanggal);
                    perTahun.Bulan01 = (item.Ppn > 0 ? item.Jumlah : (item.Jumlah / (decimal)1.11));
                    perTahun.Bulan02 = (item.Ppn > 0 ? item.Ppn : (item.Jumlah / (decimal)1.11) * (decimal)0.11);
                    perTahun.Bulan03 = item.Ongkos;

                    transaksi.Add(perTahun);

                }
                else
                {
                    if (transaksi.Any(x => x.Bulan == item.Tanggal.Month))
                    {
                        if (item.Kode == "94")
                        {
                            transaksi.Find(x => x.Bulan == item.Tanggal.Month).Bulan01 += (item.Ppn > 0 ? item.Jumlah : (item.Jumlah / (decimal)1.11));
                            transaksi.Find(x => x.Bulan == item.Tanggal.Month).Bulan02 += (item.Ppn > 0 ? item.Ppn : (item.Jumlah / (decimal)1.11) * (decimal)0.11);
                            transaksi.Find(x => x.Bulan == item.Tanggal.Month).Bulan03 += item.Ongkos;
                            transaksi.Find(x => x.Bulan == item.Tanggal.Month).Bulan05 += item.OeTransDs.Sum(x => x.Cost);
                        }
                        else
                        {
                            transaksi.Find(x => x.Bulan == item.Tanggal.Month).Bulan01 -= (item.Ppn > 0 ? item.Jumlah : (item.Jumlah / (decimal)1.11));
                            transaksi.Find(x => x.Bulan == item.Tanggal.Month).Bulan02 -= (item.Ppn > 0 ? item.Ppn : (item.Jumlah / (decimal)1.11) * (decimal)0.11);
                            transaksi.Find(x => x.Bulan == item.Tanggal.Month).Bulan03 -= item.Ongkos;
                            transaksi.Find(x => x.Bulan == item.Tanggal.Month).Bulan05 -= item.OeTransDs.Sum(x => x.Cost);
                        }


                    }
                }

            }

            // Pembelian

            foreach (var item in transHIR)
            {

                if (transaksi.Any(x => x.Bulan == item.Tanggal.Month))
                {
                    if (item.Kode == "82")
                    {
                        transaksi.Find(x => x.Bulan == item.Tanggal.Month).BeliBulan01 += (item.Kurs > 0 ? item.Nilai : item.Jumlah);
                        transaksi.Find(x => x.Bulan == item.Tanggal.Month).BeliBulan02 += (item.Kurs > 0 ? item.Ppn * item.Kurs : item.Ppn);
                        transaksi.Find(x => x.Bulan == item.Tanggal.Month).BeliBulan03 += (item.Kurs > 0 ? item.Ongkos * item.Kurs : item.Ongkos);
                    }
                    else
                    {
                        transaksi.Find(x => x.Bulan == item.Tanggal.Month).BeliBulan01 -= (item.Kurs > 0 ? item.Nilai : item.Jumlah);
                        transaksi.Find(x => x.Bulan == item.Tanggal.Month).BeliBulan02 -= (item.Kurs > 0 ? item.Ppn * item.Kurs : item.Ppn);
                        transaksi.Find(x => x.Bulan == item.Tanggal.Month).BeliBulan03 -= (item.Kurs > 0 ? item.Ongkos * item.Kurs : item.Ongkos);
                    }


                }
                //  }

            }


            transaksi.ForEach(x =>
            {
                x.Bulan04 = x.Bulan01 + x.Bulan02 + x.Bulan03;
                x.BeliBulan04 = x.BeliBulan01 + x.BeliBulan02 + x.BeliBulan03;
            });
            OePerTahun totalJual = new();
            totalJual.Bulan = 14;
            totalJual.NamaItem = "Saldo";
            totalJual.Bulan01 = transaksi.Sum(x => x.Bulan01);
            totalJual.Bulan02 = transaksi.Sum(x => x.Bulan02);
            totalJual.Bulan03 = transaksi.Sum(x => x.Bulan03);
            totalJual.Bulan04 = transaksi.Sum(x => x.Bulan04);
            totalJual.Bulan05 = transaksi.Sum(x => x.Bulan05);
            totalJual.BeliBulan01 = transaksi.Sum(x => x.BeliBulan01);
            totalJual.BeliBulan02 = transaksi.Sum(x => x.BeliBulan02);
            totalJual.BeliBulan03 = transaksi.Sum(x => x.BeliBulan03);
            totalJual.BeliBulan04 = transaksi.Sum(x => x.BeliBulan04);
            transaksi.Add(totalJual);
            return transaksi;
        }
        public List<OePerTahun> ItemPertahun(int tahun, List<string> kodeDiv)
        {

            var transPOH = _contextOR.PoTransHs.Where(x => x.Cek == "1" && x.Kode == "71").ToList();
            var TransPOD = _contextOR.PoTransDs.Where(x => x.Kode == "71").ToList();

            var transDOE = _contextOE.OeTransDs.Where(x => x.Tanggal.Year == tahun).ToList();
            var transDIR = _contextIR.IrTransDs.Where(x => x.Tanggal.Year == tahun).ToList();

            List<IcItem> persediaan = (from e in _context.IcItems where kodeDiv.Contains(e.Divisi) select e).ToList();

            List<OePerTahun> rekening = new List<OePerTahun>();

            List<PoTransD> transPO = (from transH in transPOH join transD in TransPOD on transH.NoLpb equals transD.NoLpb select transD).ToList();

            var transaksiOrder = (from transD in transPO
                                  join items in persediaan on transD.ItemCode equals items.ItemCode

                                  select new OePerTahun()
                                  {
                                      Kode = transD.Kode,
                                      ItemCode = transD.ItemCode,
                                      NamaItem = transD.NamaItem,
                                      Divisi = items.Divisi,
                                      Qty = transD.Qty,
                                      Bulan = transD.Tanggal.Month,
                                      BulanTotal = items.Qty

                                  }
           ).ToList();

            var transaksi = (from transD in transDOE
                             join items in persediaan on transD.ItemCode equals items.ItemCode

                             select new OePerTahun()
                             {
                                 Kode = transD.Kode,
                                 ItemCode = transD.ItemCode,
                                 NamaItem = transD.NamaItem,
                                 Divisi = items.Divisi,
                                 Qty = transD.Qty,
                                 Bulan = transD.Tanggal.Month,
                                 BulanTotal = items.Qty

                             }
            ).ToList();



            var pembelian = (from transD in transDIR
                             join items in persediaan on transD.ItemCode equals items.ItemCode

                             select new OePerTahun()
                             {
                                 Kode = transD.Kode,
                                 ItemCode = transD.ItemCode,
                                 NamaItem = transD.NamaItem,
                                 Divisi = items.Divisi,
                                 Qty = transD.Qty,
                                 Bulan = transD.Tanggal.Month,
                                 BulanTotal = items.Qty
                             }
                            ).ToList();

            foreach (var item in transaksi)
            {
                if (rekening.Any(x => x.ItemCode == item.ItemCode))
                {
                    switch (item.Bulan)
                    {
                        case 1:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan01 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 2:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan02 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 3:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan03 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 4:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan04 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 5:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan05 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 6:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan06 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 7:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan07 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 8:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan08 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 9:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan09 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 10:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan10 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 11:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan11 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 12:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan12 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;

                    }

                }
                else
                {
                    rekening.Add(
                            new OePerTahun()
                            {
                                ItemCode = item.ItemCode,
                                NamaItem = item.NamaItem,
                                Divisi = item.Divisi,
                                BulanTotal = item.BulanTotal

                            });


                    switch (item.Bulan)
                    {
                        case 1:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan01 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 2:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan02 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 3:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan03 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 4:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan04 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 5:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan05 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 6:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan06 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 7:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan07 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 8:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan08 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 9:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan09 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 10:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan10 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 11:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan11 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 12:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan12 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;

                    }

                }
                ;
            }

            foreach (var item in pembelian)
            {
                if (rekening.Any(x => x.ItemCode == item.ItemCode))
                {
                    switch (item.Bulan)
                    {
                        case 1:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan01 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 2:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan02 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 3:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan03 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 4:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan04 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 5:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan05 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 6:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan06 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 7:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan07 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 8:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan08 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 9:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan09 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 10:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan10 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 11:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan11 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 12:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan12 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;

                    }

                }
                else
                {
                    rekening.Add(
                            new OePerTahun()
                            {
                                ItemCode = item.ItemCode,
                                NamaItem = item.NamaItem,
                                Divisi = item.Divisi,
                                BulanTotal = item.BulanTotal
                            });


                    switch (item.Bulan)
                    {
                        case 1:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan01 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 2:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan02 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 3:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan03 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 4:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan04 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 5:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan05 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 6:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan06 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 7:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan07 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 8:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan08 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 9:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan09 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 10:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan10 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 11:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan11 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 12:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan12 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;

                    }

                }
                ;
            }

            foreach (var item in transaksiOrder)
            {


                if (rekening.Any(x => x.ItemCode == item.ItemCode))
                {
                    rekening.Find(x => x.ItemCode == item.ItemCode).TotalOrder += item.Qty;


                }
                else
                {
                    rekening.Add(
                            new OePerTahun()
                            {
                                ItemCode = item.ItemCode,
                                NamaItem = item.NamaItem,
                                Divisi = item.Divisi,
                                BulanTotal = item.BulanTotal
                            });

                    rekening.Find(x => x.ItemCode == item.ItemCode).TotalOrder += item.Qty;


                };
            }
            rekening.ForEach(x =>
            {
                x.TotalBeli = x.BeliBulan01 + x.BeliBulan02 + x.BeliBulan03 + x.BeliBulan04 + x.BeliBulan05 + x.BeliBulan06
                            + x.BeliBulan07 + x.BeliBulan08 + x.BeliBulan09 + x.BeliBulan10 + x.BeliBulan11 + x.BeliBulan12;
                x.TotalJual = x.Bulan01 + x.Bulan02 + x.Bulan03 + x.Bulan04 + x.Bulan05 + x.Bulan06
                            + x.Bulan07 + x.Bulan08 + x.Bulan09 + x.Bulan10 + x.Bulan11 + x.Bulan12;
            });

            return rekening.OrderBy(x => x.ItemCode).Where(x => x.TotalJual != 0 || x.TotalOrder != 0).ToList();
        }

        public List<OePerTahun> DivisiPertahun(int tahun, List<string> kodeDiv)
        {

            var transPOH = _contextOR.PoTransHs.Where(x => x.Cek == "1" && x.Kode == "71").ToList();
            var TransPOD = _contextOR.PoTransDs.Where(x => x.Kode == "71").ToList();

            var transDOE = _contextOE.OeTransDs.Where(x => x.Tanggal.Year == tahun).ToList();
            var transDIR = _contextIR.IrTransDs.Where(x => x.Tanggal.Year == tahun).ToList();

            List<IcItem> persediaan = (from e in _context.IcItems where kodeDiv.Contains(e.Divisi) select e).ToList();

            List<OePerTahun> rekening = new List<OePerTahun>();

            List<PoTransD> transPO = (from transH in transPOH join transD in TransPOD on transH.NoLpb equals transD.NoLpb select transD).ToList();

            // var transaksiOrder = (from transD in transPO
            //                       join items in persediaan on transD.ItemCode equals items.ItemCode

            //                       select new OePerTahun()
            //                       {
            //                           Kode = transD.Kode,
            //                           ItemCode = transD.ItemCode,
            //                           NamaItem = GetIcDivKd(items.Divisi).NamaDiv,
            //                           Divisi = items.Divisi,
            //                           Qty = transD.Jumlah,
            //                           Bulan = transD.Tanggal.Month,
            //                           BulanTotal = items.Cost

            //                       }
            //).ToList();

            var transaksi = (from transD in transDOE
                             join items in persediaan on transD.ItemCode equals items.ItemCode

                             select new OePerTahun()
                             {
                                 Kode = transD.Kode,
                                 ItemCode = transD.ItemCode,
                                 NamaItem = transD.NamaItem,
                                 Divisi = items.Divisi,
                                 Qty = transD.Qty,
                                 Bulan = transD.Tanggal.Month,
                                 BulanTotal = items.Qty

                             }
            ).ToList();



            var pembelian = (from transD in transDIR
                             join items in persediaan on transD.ItemCode equals items.ItemCode

                             select new OePerTahun()
                             {
                                 Kode = transD.Kode,
                                 ItemCode = transD.ItemCode,
                                 NamaItem = transD.NamaItem,
                                 Divisi = items.Divisi,
                                 Qty = transD.Qty,
                                 Bulan = transD.Tanggal.Month,
                                 BulanTotal = items.Qty
                             }
                            ).ToList();

            foreach (var item in transaksi)
            {
                if (rekening.Any(x => x.ItemCode == item.ItemCode))
                {
                    switch (item.Bulan)
                    {
                        case 1:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan01 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 2:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan02 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 3:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan03 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 4:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan04 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 5:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan05 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 6:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan06 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 7:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan07 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 8:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan08 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 9:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan09 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 10:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan10 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 11:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan11 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 12:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan12 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;

                    }

                }
                else
                {
                    rekening.Add(
                            new OePerTahun()
                            {
                                ItemCode = item.ItemCode,
                                NamaItem = item.NamaItem,
                                Divisi = item.Divisi,
                                BulanTotal = item.BulanTotal

                            });


                    switch (item.Bulan)
                    {
                        case 1:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan01 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 2:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan02 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 3:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan03 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 4:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan04 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 5:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan05 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 6:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan06 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 7:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan07 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 8:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan08 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 9:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan09 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 10:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan10 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 11:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan11 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;
                        case 12:
                            rekening.Find(x => x.ItemCode == item.ItemCode).Bulan12 += (item.Kode == "94" ? item.Qty : -1 * item.Qty);
                            break;

                    }

                }
                ;
            }

            foreach (var item in pembelian)
            {
                if (rekening.Any(x => x.ItemCode == item.ItemCode))
                {
                    switch (item.Bulan)
                    {
                        case 1:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan01 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 2:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan02 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 3:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan03 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 4:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan04 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 5:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan05 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 6:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan06 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 7:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan07 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 8:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan08 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 9:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan09 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 10:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan10 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 11:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan11 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 12:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan12 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;

                    }

                }
                else
                {
                    rekening.Add(
                            new OePerTahun()
                            {
                                ItemCode = item.ItemCode,
                                NamaItem = item.NamaItem,
                                Divisi = item.Divisi,
                                BulanTotal = item.BulanTotal
                            });


                    switch (item.Bulan)
                    {
                        case 1:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan01 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 2:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan02 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 3:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan03 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 4:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan04 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 5:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan05 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 6:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan06 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 7:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan07 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 8:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan08 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 9:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan09 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 10:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan10 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 11:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan11 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 12:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan12 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;

                    }

                }
                ;
            }

            //foreach (var item in transaksiOrder)
            //{


            //    if (rekening.Any(x => x.Divisi == item.Divisi))
            //    {
            //        rekening.Find(x => x.Divisi == item.Divisi).TotalOrder += item.Qty;


            //    }
            //    else
            //    {
            //        rekening.Add(
            //                new OePerTahun()
            //                {
            //                    ItemCode = item.ItemCode,
            //                    NamaItem = GetIcDivKd(item.Divisi).NamaDiv,
            //                    Divisi = item.Divisi,
            //                    BulanTotal = item.BulanTotal
            //                });

            //        rekening.Find(x => x.Divisi == item.Divisi).TotalOrder += item.Qty;


            //    };
            //}
            rekening.ForEach(x =>
            {
                x.NamaItem = GetIcDivKd(x.Divisi).NamaDiv;

                x.TotalBeli = x.BeliBulan01 + x.BeliBulan02 + x.BeliBulan03 + x.BeliBulan04 + x.BeliBulan05 + x.BeliBulan06
                            + x.BeliBulan07 + x.BeliBulan08 + x.BeliBulan09 + x.BeliBulan10 + x.BeliBulan11 + x.BeliBulan12;
                x.TotalJual = x.Bulan01 + x.Bulan02 + x.Bulan03 + x.Bulan04 + x.Bulan05 + x.Bulan06
                            + x.Bulan07 + x.Bulan08 + x.Bulan09 + x.Bulan10 + x.Bulan11 + x.Bulan12;
            });

            //     return rekening.OrderBy(x => x.Divisi).Where(x => x.TotalJual != 0 || x.TotalOrder != 0).ToList();
            return rekening.OrderBy(x => x.Divisi).ToList();
        }

        #endregion

        #region TidakLakupertahun
        public List<OePerTahun> ItemTidakLakuPertahun(int tahun, List<string> kodeDiv)
        {
            // transHOE = _contextOE.OeTransHs.Include(p => p.OeTransDs).Where(x => x.Tanggal.Date >= Tanggal1.Date && x.Tanggal.Date <= Tanggal2.Date).ToList();
            var transDOE = _contextOE.OeTransDs.Where(x => x.Tanggal.Year == tahun).ToList();
            var transDIR = _contextIR.IrTransDs.Where(x => x.Tanggal.Year == tahun).ToList();

            List<IcItem> persediaan = (from e in _context.IcItems where kodeDiv.Contains(e.Divisi) && e.Qty != 0 select e).ToList();

            List<OePerTahun> rekening = new List<OePerTahun>();




            var transaksi = (from items in persediaan
                             let transD = transDOE.FirstOrDefault(x => x.ItemCode == items.ItemCode)
                             where transD == null
                             select new OePerTahun()
                             {
                                 Kode = "94",
                                 ItemCode = items.ItemCode,
                                 NamaItem = items.NamaItem,
                                 Divisi = items.Divisi,
                                 BulanTotal = items.Qty

                             }
            ).ToList();

            rekening.AddRange(transaksi);


            var pembelian = (from transD in transDIR
                             join items in persediaan on transD.ItemCode equals items.ItemCode

                             select new OePerTahun()
                             {
                                 Kode = transD.Kode,
                                 ItemCode = transD.ItemCode,
                                 NamaItem = transD.NamaItem,
                                 Divisi = items.Divisi,
                                 Qty = transD.Qty,
                                 Bulan = transD.Tanggal.Month,
                                 BulanTotal = items.Qty
                             }
                            ).ToList();

            //foreach (var item in transaksi)
            //{

            //        rekening.Add(
            //                new OePerTahun()
            //                {
            //                    ItemCode = item.ItemCode,
            //                    NamaItem = item.NamaItem,
            //                    Divisi = item.Divisi,
            //                    BulanTotal = item.BulanTotal

            //                });

            //}

            foreach (var item in pembelian)
            {
                if (rekening.Any(x => x.ItemCode == item.ItemCode))
                {
                    switch (item.Bulan)
                    {
                        case 1:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan01 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 2:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan02 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 3:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan03 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 4:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan04 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 5:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan05 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 6:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan06 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 7:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan07 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 8:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan08 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 9:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan09 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 10:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan10 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 11:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan11 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 12:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan12 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;

                    }

                }
                else
                {
                    rekening.Add(
                            new OePerTahun()
                            {
                                ItemCode = item.ItemCode,
                                NamaItem = item.NamaItem,
                                Divisi = item.Divisi,
                                BulanTotal = item.BulanTotal
                            });


                    switch (item.Bulan)
                    {
                        case 1:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan01 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 2:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan02 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 3:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan03 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 4:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan04 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 5:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan05 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 6:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan06 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 7:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan07 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 8:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan08 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 9:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan09 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 10:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan10 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 11:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan11 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;
                        case 12:
                            rekening.Find(x => x.ItemCode == item.ItemCode).BeliBulan12 += (item.Kode == "82" ? item.Qty : -1 * item.Qty);
                            break;

                    }

                }
                ;
            }

            //foreach (var item in transaksiOrder)
            //{


            //    if (rekening.Any(x => x.Divisi == item.Divisi))
            //    {
            //        rekening.Find(x => x.Divisi == item.Divisi).TotalOrder += item.Qty;


            //    }
            //    else
            //    {
            //        rekening.Add(
            //                new OePerTahun()
            //                {
            //                    ItemCode = item.ItemCode,
            //                    NamaItem = GetIcDivKd(item.Divisi).NamaDiv,
            //                    Divisi = item.Divisi,
            //                    BulanTotal = item.BulanTotal
            //                });

            //        rekening.Find(x => x.Divisi == item.Divisi).TotalOrder += item.Qty;


            //    };
            //}
            rekening.ForEach(x =>
            {
                x.NamaItem = GetIcDivKd(x.Divisi).NamaDiv;

                x.TotalBeli = x.BeliBulan01 + x.BeliBulan02 + x.BeliBulan03 + x.BeliBulan04 + x.BeliBulan05 + x.BeliBulan06
                            + x.BeliBulan07 + x.BeliBulan08 + x.BeliBulan09 + x.BeliBulan10 + x.BeliBulan11 + x.BeliBulan12;
                x.TotalJual = x.Bulan01 + x.Bulan02 + x.Bulan03 + x.Bulan04 + x.Bulan05 + x.Bulan06
                            + x.Bulan07 + x.Bulan08 + x.Bulan09 + x.Bulan10 + x.Bulan11 + x.Bulan12;
            });

            //     return rekening.OrderBy(x => x.Divisi).Where(x => x.TotalJual != 0 || x.TotalOrder != 0).ToList();
            return rekening.OrderBy(x => x.Divisi).ToList();
        }

        #endregion

        #region CustomerperDivision
        public class CustomerDivisionReport
        {
            public string Customer { get; set; }
            public string NamaCust { get; set; }
            public string ItemCode { get; set; }
            public string NamaItem { get; set; }
            public DateTime Tanggal { get; set; }
            public decimal HrgUsd { get; set; }
            public string CurrencyCode { get; set; }
            public decimal ItemHarga { get; set; }
            public string Divisi { get; set; }
            public string NamaDiv { get; set; }
            public decimal TotalQty { get; set; }
            public decimal TotalHarga { get; set; }
        }

        public byte[] CustomerPerDivision(List<string> kodeDiv)
        {
            var package = new XLWorkbook();

            // Initialize the result list
            List<CustomerDivisionReport> resultList = new List<CustomerDivisionReport>();

            // Query transaction header and detail data
            var transData = (from transH in _contextOE.OeTransHs
                             join transD in _contextOE.OeTransDs on transH.OeTransHId equals transD.OeTransHId
                             select new
                             {
                                 transH.OeTransHId,
                                 transH.Kode,
                                 transH.NoLpb,
                                 transH.Tanggal,
                                 transH.Customer,
                                 transH.NamaCust,
                                 transD.ItemCode,
                                 transD.NamaItem,
                                 transD.Harga,
                                 transD.Qty
                             }).ToList();

            // Query item and division data filtered by the provided division codes
            var itemData = (from item in _context.IcItems
                            join division in _context.IcDivs on item.Divisi equals division.Divisi
                            where kodeDiv.Contains(division.Divisi)
                            select new
                            {
                                item.ItemCode,
                                item.HrgUsd,
                                item.CurrencyCode,
                                item.Harga,
                                item.Divisi,
                                division.NamaDiv
                            }).ToList();

            // Join transaction data with item data on ItemCode
            var rincian = (from trans in transData
                           join item in itemData on trans.ItemCode equals item.ItemCode
                           select new
                           {
                               trans.OeTransHId,
                               trans.Kode,
                               trans.NoLpb,
                               trans.Customer,
                               trans.NamaCust,
                               trans.Tanggal,
                               trans.ItemCode,
                               trans.NamaItem,
                               trans.Qty,
                               trans.Harga,
                               item.Divisi,
                               item.NamaDiv,
                               item.HrgUsd,
                               item.CurrencyCode,
                               itemHrg = item.Harga
                           }).ToList();

            // Group the joined data by customer and item code, and calculate total quantity and price
            var groupedData = from r in rincian
                              group r by new { r.Customer, r.ItemCode } into g
                              orderby g.Key.Customer
                              select new
                              {
                                  g.Key.Customer,
                                  NamaCust = g.First().NamaCust,
                                  g.Key.ItemCode,
                                  NamaItem = g.First().NamaItem,
                                  Tanggal = g.Max(x => x.Tanggal.Date),  // Get the latest date
                                  HrgUsd = g.First().HrgUsd,
                                  CurrencyCode = g.First().CurrencyCode,
                                  ItemHarga = g.First().itemHrg,
                                  Divisi = g.First().Divisi,
                                  NamaDiv = g.First().NamaDiv,
                                  TotalQty = g.Sum(x => x.Qty),
                                  TotalHarga = g.Sum(x => x.Harga * x.Qty) // Calculate total price
                              };

            // Populate the result list with grouped data
            foreach (var item in groupedData)
            {
                resultList.Add(new CustomerDivisionReport
                {
                    Customer = item.Customer,
                    NamaCust = item.NamaCust,
                    ItemCode = item.ItemCode,
                    NamaItem = item.NamaItem,
                    Tanggal = item.Tanggal,
                    HrgUsd = item.HrgUsd,
                    CurrencyCode = item.CurrencyCode,
                    ItemHarga = item.ItemHarga,
                    Divisi = item.Divisi,
                    NamaDiv = item.NamaDiv,
                    TotalQty = item.TotalQty,
                    TotalHarga = item.TotalHarga
                });
            }

            // Export to Excel
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Customer Per Division");

                // Adding headers
                worksheet.Cell(1, 1).Value = "Customer";
                worksheet.Cell(1, 2).Value = "Nama Cust";
                worksheet.Cell(1, 3).Value = "Item Code";
                worksheet.Cell(1, 4).Value = "Nama Item";
                worksheet.Cell(1, 5).Value = "Tanggal";
                worksheet.Cell(1, 6).Value = "Hrg Usd";
                worksheet.Cell(1, 7).Value = "Currency Code";
                worksheet.Cell(1, 8).Value = "Item Harga";
                worksheet.Cell(1, 9).Value = "Divisi";
                worksheet.Cell(1, 10).Value = "Nama Div";
                worksheet.Cell(1, 11).Value = "Total Qty";
                worksheet.Cell(1, 12).Value = "Total Harga";

                // Adding data
                for (int i = 0; i < resultList.Count; i++)
                {
                    var item = resultList[i];
                    worksheet.Cell(i + 2, 1).Value = item.Customer;
                    worksheet.Cell(i + 2, 2).Value = item.NamaCust;
                    worksheet.Cell(i + 2, 3).Value = item.ItemCode;
                    worksheet.Cell(i + 2, 4).Value = item.NamaItem;
                    worksheet.Cell(i + 2, 5).Value = item.Tanggal.ToString("dd-MM-yyyy");
                    worksheet.Cell(i + 2, 6).Value = item.HrgUsd;
                    worksheet.Cell(i + 2, 7).Value = item.CurrencyCode;
                    worksheet.Cell(i + 2, 8).Value = item.ItemHarga;
                    worksheet.Cell(i + 2, 9).Value = item.Divisi;
                    worksheet.Cell(i + 2, 10).Value = item.NamaDiv;
                    worksheet.Cell(i + 2, 11).Value = item.TotalQty;
                    worksheet.Cell(i + 2, 12).Value = item.TotalHarga;
                }



                // Saving the file
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    var content = stream.ToArray();
                    return content;
                    // File.WriteAllBytes("CustomerPerDivision.xlsx", stream.ToArray());
                }
            }

            // Return the populated result list
            //  return resultList;
        }
        #endregion

        #region laporanrekapstock

        public List<IcRekapStock> RekapStock(DateTime Tanggal1, DateTime Tanggal2)
        {
            var awal = Tanggal1.Date;
            var akhirExclusive = Tanggal2.Date.AddDays(1);

            // Ambil transaksi sebelum awal (Saldo Awal) LANGSUNG dari detail
            var transOeAwal = _contextOE.OeTransDs
                .Where(x => x.Tanggal >= DateTime.MinValue && x.Tanggal < awal)
                .Select(x => new { x.ItemCode, x.Kode, x.Qty, x.Cost })
                .ToList();

            var transIrAwal = _contextIR.IrTransDs
                .Where(x => x.Tanggal >= DateTime.MinValue && x.Tanggal < awal)
                .Select(x => new { x.ItemCode, x.Kode, x.Qty, x.JumDpp })
                .ToList();

            var transIcAwal = _context.IcTransDs
                .Where(x => x.Tanggal >= DateTime.MinValue && x.Tanggal < awal)
                .Select(x => new { x.ItemCode, x.Kode, x.QtyShp, x.Jumlah })
                .ToList();

            // Ambil transaksi dalam periode LANGSUNG dari detail
            var transOe = _contextOE.OeTransDs
                .Where(x => x.Tanggal >= awal && x.Tanggal < akhirExclusive)
                .Select(x => new { x.ItemCode, x.Kode, x.Qty, x.Cost })
                .ToList();

            var transIr = _contextIR.IrTransDs
                .Where(x => x.Tanggal >= awal && x.Tanggal < akhirExclusive)
                .Select(x => new { x.ItemCode, x.Kode, x.Qty, x.JumDpp })
                .ToList();

            var transIc = _context.IcTransDs
                .Where(x => x.Tanggal >= awal && x.Tanggal < akhirExclusive)
                .Select(x => new { x.ItemCode, x.Kode, x.QtyShp, x.Jumlah })
                .ToList();

            var items = _context.IcItems
                .AsNoTracking()
                .Select(x => new { x.ItemCode, x.NamaItem, x.Satuan, x.Divisi, x.SaldoAwal, x.CostAwal })
                .ToList();

            Dictionary<string, IcRekapStock> rekapStock = new();

            foreach (var item in items)
            {
                rekapStock[item.ItemCode] = new IcRekapStock()
                {
                    ItemCode = item.ItemCode,
                    NamaItem = item.NamaItem,
                    Satuan = item.Satuan,
                    Divisi = item.Divisi,
                    QtyAwal = item.SaldoAwal,
                    SaldoAwal = item.CostAwal,
                    QtyMasuk = 0,
                    SaldoMasuk = 0,
                    QtyKeluar = 0,
                    SaldoKeluar = 0,
                    QtyAdjust = 0,
                    SaldoAdjust = 0
                };
            }

            // OPTIMIZATION: Use GroupBy instead of looping multiple times
            // Saldo awal dari pembelian (IR)
            var beliAwalGrouped = transIrAwal.GroupBy(x => x.ItemCode).ToDictionary(g => g.Key, g => g.ToList());
            foreach (var itemCode in beliAwalGrouped.Keys)
            {
                if (!rekapStock.TryGetValue(itemCode, out var stok))
                    continue;

                foreach (var beli in beliAwalGrouped[itemCode])
                {
                    if (beli.Kode == "82")
                    {
                        stok.QtyAwal += beli.Qty;
                        stok.SaldoAwal += beli.JumDpp;
                    }
                    else if (beli.Kode == "83")
                    {
                        stok.QtyAwal -= beli.Qty;
                        stok.SaldoAwal -= beli.JumDpp;
                    }
                }
            }

            // Saldo awal dari penjualan (OE)
            var jualAwalGrouped = transOeAwal.GroupBy(x => x.ItemCode).ToDictionary(g => g.Key, g => g.ToList());
            foreach (var itemCode in jualAwalGrouped.Keys)
            {
                if (!rekapStock.TryGetValue(itemCode, out var stok))
                    continue;

                foreach (var jual in jualAwalGrouped[itemCode])
                {
                    if (jual.Kode == "95")
                    {
                        stok.QtyAwal += jual.Qty;
                        stok.SaldoAwal += jual.Cost;
                    }
                    else if (jual.Kode == "94")
                    {
                        stok.QtyAwal -= jual.Qty;
                        stok.SaldoAwal -= jual.Cost;
                    }
                }
            }

            // Saldo awal dari adjustment (IC)
            var adjustAwalGrouped = transIcAwal.GroupBy(x => x.ItemCode).ToDictionary(g => g.Key, g => g.ToList());
            foreach (var itemCode in adjustAwalGrouped.Keys)
            {
                if (!rekapStock.TryGetValue(itemCode, out var stok))
                    continue;

                foreach (var adjust in adjustAwalGrouped[itemCode])
                {
                    if (adjust.Kode == "81")
                    {
                        stok.QtyAwal += adjust.QtyShp;
                        stok.SaldoAwal += adjust.Jumlah;
                    }
                }
            }

            // Masuk periode (IR)
            var beliPeriodeGrouped = transIr.GroupBy(x => x.ItemCode).ToDictionary(g => g.Key, g => g.ToList());
            foreach (var itemCode in beliPeriodeGrouped.Keys)
            {
                if (!rekapStock.TryGetValue(itemCode, out var stok))
                    continue;

                foreach (var beli in beliPeriodeGrouped[itemCode])
                {
                    if (beli.Kode == "82")
                    {
                        stok.QtyMasuk += beli.Qty;
                        stok.SaldoMasuk += beli.JumDpp;
                    }
                    else if (beli.Kode == "83")
                    {
                        stok.QtyMasuk -= beli.Qty;
                        stok.SaldoMasuk -= beli.JumDpp;
                    }
                }
            }

            // Keluar periode (OE)
            var jualPeriodeGrouped = transOe.GroupBy(x => x.ItemCode).ToDictionary(g => g.Key, g => g.ToList());
            foreach (var itemCode in jualPeriodeGrouped.Keys)
            {
                if (!rekapStock.TryGetValue(itemCode, out var stok))
                    continue;

                foreach (var jual in jualPeriodeGrouped[itemCode])
                {
                    if (jual.Kode == "94")
                    {
                        stok.QtyKeluar += jual.Qty;
                        stok.SaldoKeluar += jual.Cost;
                    }
                    else if (jual.Kode == "95")
                    {
                        stok.QtyKeluar -= jual.Qty;
                        stok.SaldoKeluar -= jual.Cost;
                    }
                }
            }

            // Adjust periode (IC)
            var adjustPeriodeGrouped = transIc.GroupBy(x => x.ItemCode).ToDictionary(g => g.Key, g => g.ToList());
            foreach (var itemCode in adjustPeriodeGrouped.Keys)
            {
                if (!rekapStock.TryGetValue(itemCode, out var stok))
                    continue;

                foreach (var adjust in adjustPeriodeGrouped[itemCode])
                {
                    if (adjust.Kode == "81")
                    {
                        stok.QtyAdjust += adjust.QtyShp;
                        stok.SaldoAdjust += adjust.Jumlah;
                    }
                }
            }

            return rekapStock.Values.ToList();
        }

        #endregion

        #region penjualanlunas

        public async Task<List<ResultLunasView>> GetPenjualanLunasAsync(DateTime tanggalLunas1, DateTime tanggalLunas2)
        {
            // Step 1: Ambil data dari masing-masing context
            var penjualans = await _contextOE.OeTransHs
                //  .Where(x => x.Tanggal >= new DateTime(2025, 4, 11))
                .Select(x => new
                {
                    x.NoLpb,
                    x.Tanggal,
                    x.Customer,
                    x.NamaCust,
                    x.OeTransHId
                })
                .ToListAsync();

            var piutangs = await _contextAR.ArPiutngs
                .Where(x => x.Sisa == 0)
                .Select(x => new
                {
                    x.Dokumen,
                    x.Jumlah
                })
                .ToListAsync();

            var transDs = await _contextAR.ArTransDs
                .Select(x => new
                {
                    x.Lpb,
                    x.Tanggal,
                    x.Bukti
                })
                .ToListAsync();

            var transHs = await _contextAR.ArTransHs
                .Select(x => new
                {
                    x.Bukti,
                    x.KdBank

                })
                .ToListAsync();

            // Step 2: Join di memory
            var result = (from jual in penjualans
                          join piutang in piutangs on jual.NoLpb equals piutang.Dokumen
                          join bayar in transDs on jual.NoLpb equals bayar.Lpb
                          join header in transHs on bayar.Bukti equals header.Bukti

                          select new
                          {
                              jual.NoLpb,
                              jual.Tanggal,
                              jual.Customer,
                              jual.NamaCust,
                              jual.OeTransHId,
                              piutang.Jumlah,
                              TglBayar = bayar.Tanggal,
                              header.KdBank
                          }).ToList();

            // Step 3: Grouping & Filtering by TanggalLunas
            return result
                .GroupBy(x => x.NoLpb)
                .Select(grp => new ResultLunasView
                {
                    NoLpb = grp.Key,
                    Tanggal = grp.First().Tanggal,
                    Customer = grp.First().Customer,
                    NamaCust = grp.First().NamaCust,
                    OeTransHId = grp.First().OeTransHId,
                    Jumlah = grp.First().Jumlah,
                    Status = "Lunas",
                    KdBank = grp.First().KdBank,
                    SelisihTanggal = (grp.Max(x => x.TglBayar) - grp.First().Tanggal).Days,
                    TanggalLunas = grp.Max(x => x.TglBayar)
                    
                })
                .Where(x => x.TanggalLunas >= tanggalLunas1 && x.TanggalLunas <= tanggalLunas2)
                .ToList();
        }

        #endregion
    }
}

