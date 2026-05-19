using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Company.Model;
using Microsoft.EntityFrameworkCore;


namespace eSoft.Company.Data
{
    public class DbContextCompany : DbContext
    {
        public DbContextCompany(DbContextOptions<DbContextCompany> options) : base(options)
        {
        }
        public DbSet<CoSetup> CoSetups { get; set; }
       
    }

}
