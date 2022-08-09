using System.Reflection;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Matches all assemblies.
    /// </summary>
    /// <seealso cref="BSB.Microservices.NServiceBus.IAssemblySearchPattern" />
    internal class All : IAssemblySearchPattern
    {
        public string PatternToMatch { get; set; }

        public bool Match(Assembly assemblyName)
        {
            return true;
        }

        public bool Match(string assemblyName)
        {
            return true;
        }
    }
}