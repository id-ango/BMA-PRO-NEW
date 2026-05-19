using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.CashBank.Model;
using Microsoft.EntityFrameworkCore;


namespace eSoft.CashBank.Data
{
    public class DbContextBank : DbContext
    {
        public DbContextBank(DbContextOptions<DbContextBank> options) : base(options)
        {
        }
        public DbSet<CbBank> CbBanks { get; set; }
        public DbSet<CbTransH> CbTransHs { get; set; }
        public DbSet<CbTransD> CbTransDs { get; set; }
        public DbSet<CbTransfer> CbTransfers { get; set; }
        public DbSet<CbSrcCode> CbSrcCodes { get; set; }
        public DbSet<CbGrp> CbGrps { get; set; }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //    builder.Entity<CbBank>()
        //        .HasIndex(p => p.KodeBank)
        //        .IsUnique();
        //    builder.Entity<CbBank>().Property(p => p.KSaldo).HasColumnType("decimal(18,4)");
        //    builder.Entity<CbBank>().Property(p => p.KSldAwal).HasColumnType("decimal(18,4)");
        //    builder.Entity<CbBank>().Property(p => p.Saldo).HasColumnType("decimal(18,4)");
        //    builder.Entity<CbBank>().Property(p => p.SldAwal).HasColumnType("decimal(18,4)");
        //}
    }

}
