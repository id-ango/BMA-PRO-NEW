using eSoft.TestRun.Data;
using eSoft.TestRun.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace eSoft.TestRun.Services
{
    public class TestRunServices : ITestRunServices
    {
        private readonly IDbContextFactory<DbContextTestRun> _context;
        public TestRunServices(IDbContextFactory<DbContextTestRun> context)
        {
            _context = context;
        }

        public List<TsSchedule> GetTsSchedules()
        {
            using var db = _context.CreateDbContext();
            return db.TsSchedules.OrderBy(x => x.Dokumen).ToList();
        }
    }
}
