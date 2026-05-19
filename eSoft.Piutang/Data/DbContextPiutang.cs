using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Piutang.Model;
using Microsoft.EntityFrameworkCore;


namespace eSoft.Piutang.Data
{
    public class DbContextPiutang : DbContext
    {
        public DbContextPiutang(DbContextOptions<DbContextPiutang> options) : base(options)
        {
        }
        public DbSet<ArCust> ArCusts { get; set; }
        public DbSet<ArAcct> ArAccts { get; set; }
        public DbSet<ArDist> ArDists { get; set; }
        public DbSet<ArTransH> ArTransHs { get; set; }
        public DbSet<ArTransD> ArTransDs { get; set; }
        public DbSet<ArPiutng> ArPiutngs { get; set; }
     

       
    }
}
