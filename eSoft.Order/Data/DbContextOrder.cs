using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Order.Model;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Order.Data
{
    public class DbContextOrder : DbContext
    {
        public DbContextOrder(DbContextOptions<DbContextOrder> options) : base(options)
        {

        }

        public DbSet<PoTransH> PoTransHs { get; set; }
        public DbSet<PoTransD> PoTransDs { get; set; }

    }
}
