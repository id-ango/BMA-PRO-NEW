using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Asset.Model;
using Microsoft.EntityFrameworkCore;


namespace eSoft.Asset.Data
{
    public class DbContextAssets : DbContext
    {
        public DbContextAssets(DbContextOptions<DbContextAssets> options) : base(options)
        {

        }

        public DbSet<AsItem> AsItems { get; set; }
        public DbSet<AsDivisi> AsDivisis { get; set; }
        public DbSet<AsDistSet> AsDistSets { get; set; }
        public DbSet<AsAssets> AsAssetss { get; set; }
        public DbSet<AsAcctset> AsAcctsets { get; set; }
        public DbSet<AsTransaksi> AsTransaksis { get; set; }
    }
}
