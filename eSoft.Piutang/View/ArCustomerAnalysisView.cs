using System;
using System.Collections.Generic;

namespace eSoft.Piutang.View
{
    /// <summary>
    /// Hasil analisa perilaku pembayaran per customer.
    /// Label risiko ditentukan dari kondisi OUTSTANDING SAAT INI,
    /// bukan rata-rata histori keseluruhan.
    /// </summary>
    public class ArCustomerAnalysisView
    {
        public string Customer { get; set; }
        public string NamaCust { get; set; }
        /// <summary>Salesman unik yang tercatat pada seluruh faktur customer.</summary>
        public string Salesman { get; set; }
        /// <summary>Nama salesman unik yang tercatat pada seluruh faktur customer.</summary>
        public string NamaSales { get; set; }

        // ── Outstanding saat ini ────────────────────────────────
        public decimal TotalOutstanding { get; set; }
        /// <summary>Total saldo uang muka customer yang belum dialokasikan ke nota.</summary>
        public decimal UangMukaBelumDialokasikan { get; set; }
        /// <summary>Outstanding nota setelah dikurangi uang muka customer.</summary>
        public decimal TotalOutstandingBersih { get; set; }
        public int JumlahFakturOpen { get; set; }

        /// <summary>
        /// Faktur dicicil (ada bayar masuk, sisa > 0).
        /// </summary>
        public int JumlahFakturCicilan { get; set; }

        /// <summary>
        /// Jumlah faktur outstanding yang sudah lewat due date >60 hari (termasuk yang berhenti cicil).
        /// </summary>
        public int CountTelat60 { get; set; }

        /// <summary>
        /// Total sisa dari faktur outstanding >60 hari.
        /// </summary>
        public decimal OutstandingTelat60 { get; set; }

        /// <summary>
        /// Hari terlama "diam" pada faktur outstanding:
        /// = today - tanggal_bayar_terakhir (jika ada cicilan), atau today - due_date (jika belum pernah bayar dan sudah jatuh tempo).
        /// Ini adalah indikator UTAMA penentuan label risiko.
        /// </summary>
        public int MaxHariMacetOutstanding { get; set; }

        /// <summary>Tanggal bayar terakhir untuk faktur outstanding yang paling "diam".</summary>
        public DateTime? TglTerakhirBayarOutstanding { get; set; }

        // ── Histori keseluruhan (untuk catatan, bukan penentu label utama) ──
        public int TotalFaktur { get; set; }
        public int FakturLunas { get; set; }
        /// <summary>Total nilai invoice non-CA dari seluruh histori sampai hari ini.</summary>
        public decimal TotalNilaiTransaksi { get; set; }

        /// <summary>Rata-rata hari dari invoice ke pembayaran (faktur lunas).</summary>
        public double AvgHariBayar { get; set; }

        /// <summary>Rata-rata keterlambatan bayar (hari) dari faktur yang sudah lunas.</summary>
        public double AvgDaysLate { get; set; }

        /// <summary>Keterlambatan terpanjang dari faktur yang sudah lunas.</summary>
        public int MaxDaysLate { get; set; }

        /// <summary>Persen faktur lunas yang dibayar tepat waktu.</summary>
        public double OnTimeRate { get; set; }

        /// <summary>DSO — Days Sales Outstanding.</summary>
        public double DSO { get; set; }

        /// <summary>Jeda terpanjang antar pembayaran customer dalam hari.</summary>
        public int MaxJedaPembayaran { get; set; }

        /// <summary>Jumlah pembayaran customer dalam 60 hari terakhir.</summary>
        public int Pembayaran60HariTerakhir { get; set; }

        // ── Label & Rekomendasi ─────────────────────────────────
        public int RiskScore { get; set; }

        /// <summary>Baik / Hati-hati / Cukup / Macet / Blacklist</summary>
        public string RiskLabel { get; set; }

        public string Rekomendasi { get; set; }
    }
}
