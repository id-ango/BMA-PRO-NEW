using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace eSoft.CashBank.Services
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> GetTypesSafe(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch
            {
                return Enumerable.Empty<Type>();
            }
        }
    }
}
