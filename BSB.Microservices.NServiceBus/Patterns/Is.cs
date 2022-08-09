using System;
using System.Reflection;

namespace BSB.Microservices.NServiceBus
{
    internal class Is : BaseSearchPattern
    {
        public Is(string patternToMatch) : base(patternToMatch)
        {
        }

        public override bool Match(string assemblyName)
        {
            return PatternToMatch.Equals(assemblyName, StringComparison.OrdinalIgnoreCase);
        }

        protected override bool Match(AssemblyName assemblyName)
        {
            return Match(assemblyName.Name);
        }
    }
}