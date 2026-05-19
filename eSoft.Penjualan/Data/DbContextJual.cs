using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Penjualan.Model;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Penjualan.Data
{
    public class DbContextJual : DbContext
    {
        public DbContextJual(DbContextOptions<DbContextJual> options) : base(options)
        {

        }

        public DbSet<OeTransH> OeTransHs { get; set; }
        public DbSet<OeTransD> OeTransDs { get; set; }
        public DbSet<OeSalesman> OeSalesmans { get; set; }
        public DbSet<OeKurir> OeKurirs { get; set; }
        public DbSet<OeAltItem> OeAltItems { get; set; }

    }
}
