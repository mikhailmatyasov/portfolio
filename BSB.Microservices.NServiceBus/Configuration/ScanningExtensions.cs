using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BSB.Microservices.NServiceBus
{
    public static class ScanningExtensions
    {
        /// <summary>
        /// Produces an enumeration of assemblies with our scan configuration predicates applied.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="assemblyScanConfiguration">The assembly scan configuration.</param>
        /// <returns></returns>
        public static IEnumerable<Assembly> IncludeAssemblies(this IEnumerable<Assembly> assemblies, IBusStartup assemblyScanConfiguration)
        {
            var include = assemblyScanConfiguration.InvokeIncludeDelegate();
            foreach (var assembly in assemblies.Where(x => !x.IsDynamic))
            {
                if (include?.Patterns?.Any(p => p.Match(assembly)) == true)
                {
                    yield return assembly;
                }
                else
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Produces an enumeration of assembly names with our scan configuration predicates applied.
        /// </summary>
        /// <param name="assemblyScanConfiguration">The assembly scan configuration.</param>
        /// <param name="baseDirectory">The base directory.</param>
        /// <returns></returns>
        public static IEnumerable<string> BuildAssemblyScanExclusion(this IBusStartup assemblyScanConfiguration, string baseDirectory)
        {
            var include = assemblyScanConfiguration.InvokeIncludeDelegate();
            var exclude = assemblyScanConfiguration.InvokeExcludeDelegate();

            foreach (var fileName in Directory.EnumerateFiles(baseDirectory, "*", assemblyScanConfiguration.ScanAssembliesInNestedDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                    .Select(Path.GetFileName))
            {
                var ext = Path.GetExtension(fileName);
                if (ext.Equals("dll", System.StringComparison.OrdinalIgnoreCase) ||
                        ext.Equals("exe", System.StringComparison.OrdinalIgnoreCase)){
                    if (include?.Patterns?.Any(x => x.Match(fileName)) == true)
                    {
                        continue;
                    }
                    else if (exclude?.Patterns?.Any(x => x.Match(fileName)) == true)
                    {
                        yield return fileName;
                    }
                    else
                    {
                        yield return fileName;
                    }
                }
            }
        }
    }
}