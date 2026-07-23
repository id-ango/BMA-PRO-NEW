using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.CashBank.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;



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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Index on KodeBank for CbBank lookups
            builder.Entity<CbBank>()
                .HasIndex(p => p.KodeBank)
                .IsUnique();

            // Index on DocNo for CbTransH to optimize document number generation queries
            builder.Entity<CbTransH>()
                .HasIndex(p => p.DocNo)
                .IsUnique();
        }
    }

}
