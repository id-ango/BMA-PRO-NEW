using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Persediaan.Model;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Persediaan.Data
{
    public class DbContextPersediaan : DbContext
    {
        public DbContextPersediaan(DbContextOptions<DbContextPersediaan> options) : base(options)
        {

        }

        public DbSet<IcItem> IcItems { get; set; }
        public DbSet<IcAltItem> IcAltItems { get; set; }
        public DbSet<IcLokasi> Iclokasis { get; set; }
        public DbSet<IcDiv> IcDivs { get; set; }
        public DbSet<IcCat> IcCats { get; set; }
        public DbSet<IcAcct> IcAccts { get; set; }
        public DbSet<IcTransH> IcTransHs { get; set; }
        public DbSet<IcTransD> IcTransDs { get; set; }
        public DbSet<IcStockCard> IcStockCards { get; set; }
    }
}
