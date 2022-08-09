namespace BSB.Microservices.NServiceBus
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Matches an assembly name if it contatins PatternToMatch
    /// </summary>
    public class Contains : BaseSearchPattern
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Contains"/> class.
        /// </summary>
        /// <param name="patternToMatch">The pattern to match.</param>
        internal Contains(string patternToMatch) : base(patternToMatch)
        {
        }

        /// <summary>
        /// Matches the specified assembly name.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>bool</returns>
        protected override bool Match(AssemblyName assemblyName)
        {
            return assemblyName.Name.IndexOf(this.PatternToMatch, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Matches the specified assembly name.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns></returns>
        public override bool Match(string assemblyName)
        {
            return assemblyName.IndexOf(this.PatternToMatch, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}