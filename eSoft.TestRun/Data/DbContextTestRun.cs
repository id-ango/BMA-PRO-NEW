using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.TestRun.Model;
using Microsoft.EntityFrameworkCore;

namespace eSoft.TestRun.Data
{
    public class DbContextTestRun : DbContext
    {
        public DbContextTestRun(DbContextOptions<DbContextTestRun> options) : base(options)
        {
        }

        public DbSet<TsSchedule> TsSchedules { get; set; }
    }
}
