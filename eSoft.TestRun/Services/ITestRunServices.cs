using eSoft.TestRun.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.TestRun.Services
{
    public interface ITestRunServices
    {
        List<TsSchedule> GetTsSchedules();
    }
}
