using eSoft.TestRun.Data;
using eSoft.TestRun.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.TestRun.Services
{
    public class TestRunServices : ITestRunServices
    {
        private readonly DbContextTestRun _context;
        public TestRunServices(DbContextTestRun context)
        {
            _context = context;
        }

        public List<TsSchedule> GetTsSchedules()
        {
            return _context.TsSchedules.OrderBy(x => x.Dokumen).ToList();
        }
    }
}
