using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Straight.Core.DataAccess.Tests
{
    internal static class RuntimeExtensions
    {
        public static Assembly GetAssembly(this string name, StringComparison ctrComparaison = StringComparison.CurrentCultureIgnoreCase)
        {
            return Assembly.GetEntryAssembly()
                           .GetReferencedAssemblies()
                           .Where(aName => IsCandidateLibrary(aName, name, ctrComparaison))
                           .Select(library => Assembly.Load(new AssemblyName(library.Name)))
                           .FirstOrDefault();
        }
        private static bool IsCandidateLibrary(AssemblyName assemblyName, string name, StringComparison ctrComparaison = StringComparison.CurrentCultureIgnoreCase)
        {
            return string.Equals(assemblyName.Name, name, ctrComparaison);
        }
    }
}
