using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Pembelian.Model;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Pembelian.Data
{
    public class DbContextBeli : DbContext
    {
        public DbContextBeli(DbContextOptions<DbContextBeli> options) : base(options)
        {

        }

        public DbSet<IrTransH> IrTransHs { get; set; }
        public DbSet<IrTransD> IrTransDs { get; set; }

    }
}
