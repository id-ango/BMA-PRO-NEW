using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Hutang.Model;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Hutang.Data
{
    public class DbContextHutang : DbContext
    {
        public DbContextHutang(DbContextOptions<DbContextHutang> options) : base(options)
        {

        }

        public DbSet<ApSuppl> ApSuppls { get; set; }
        public DbSet<ApAcct> ApAccts { get; set; }
        public DbSet<ApDist> ApDists { get; set; }
        public DbSet<ApTransH> ApTransHs { get; set; }
        public DbSet<ApTransD> ApTransDs { get; set; }
        public DbSet<ApHutang> ApHutangs { get; set; }
    }
   
}
