using System.Reflection;
using System.Text.RegularExpressions;

namespace BSB.Microservices.NServiceBus
{
    internal class RegularExpression : IAssemblySearchPattern
    {
        public string PatternToMatch { get; set; }

        public RegularExpression(string pattern)
        {
            PatternToMatch = pattern;
        }

        public bool Match(Assembly assemblyName)
        {
            return Match(assemblyName.GetName().Name);
        }

        public bool Match(string assemblyName)
        {
            return Regex.IsMatch(assemblyName, PatternToMatch, RegexOptions.IgnoreCase);
        }
    }
}