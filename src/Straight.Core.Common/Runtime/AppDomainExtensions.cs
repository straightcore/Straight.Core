using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Straight.Core.Common.Runtime
{
    public static class AppDomainExtensions
    {
        public static IEnumerable<Assembly> GetAssemblies()
        {
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            return dependencies.Select(library => Assembly.Load(new AssemblyName(library.Name)))
                               .ToList();
        }

        public static Assembly GetAssembly(this string assemblyName, StringComparison ctrComparaison = StringComparison.CurrentCultureIgnoreCase)
        {
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            return dependencies.Where(d => IsCandidateLibrary(d, assemblyName, ctrComparaison))
                               .Select(library => Assembly.Load(new AssemblyName(library.Name)))
                               .FirstOrDefault();
        }

        public static Assembly GetAssembly<T>(this T instance) where T : class
        {
            return instance.GetType().AssemblyQualifiedName.GetAssembly(StringComparison.CurrentCultureIgnoreCase);
        }

        private static bool IsCandidateLibrary(RuntimeLibrary library, string assemblyName, StringComparison ctrComparaison = StringComparison.CurrentCultureIgnoreCase)
        {
            return library.Name.Equals(assemblyName, ctrComparaison)
                   || library.Dependencies.Any(d => d.Name.StartsWith(assemblyName));
        }
    }
}
