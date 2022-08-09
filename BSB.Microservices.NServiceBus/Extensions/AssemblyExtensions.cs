using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace System.Reflection
{
    /// <summary>
    ///
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// As you might expect, being able to get a list of types, even if you don’t plan on instantiating instances of them,
        /// is a common and important task.
        /// Fortunately, the ReflectionTypeLoadException thrown when a type can’t be loaded contains all the information you need
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">assembly</exception>
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly, ILogger logger = null)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
            try
            {
                return assembly.GetTypes().Where(x => x.IsPublic || x.IsNestedPublic);
            }
            catch (ReflectionTypeLoadException e)
            {
                StringBuilder b = new StringBuilder();
                foreach (var ex in e.LoaderExceptions)
                {
                    b.AppendLine(ex.Message);
                }
                logger?.LogDebug(b.ToString());
                return e.Types.Where(t => t != null && (t.IsPublic || t.IsNestedPublic));
            }
        }

        public static bool AssemblyNameContains(this Assembly assembly, IEnumerable<string> patterns)
        {
            var fullName = assembly.FullName;
            foreach (var e in patterns)
            {
                if (fullName.StartsWith(e, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}