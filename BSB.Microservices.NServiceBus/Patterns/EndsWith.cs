namespace BSB.Microservices.NServiceBus
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Matches an assembly name if it ends with PatternToMatch
    /// </summary>
    /// <seealso cref="BaseSearchPattern" />
    public class EndsWith : BaseSearchPattern
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndsWith"/> class.
        /// </summary>
        /// <param name="patternToMatch">The pattern to match.</param>
        internal EndsWith(string patternToMatch) : base(patternToMatch)
        {
        }

        /// <summary>
        /// Matches the specified assembly name.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>bool</returns>
        protected override bool Match(AssemblyName assemblyName)
        {
            return assemblyName.Name.EndsWith(this.PatternToMatch, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Matches the specified assembly name.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>bool</returns>
        public override bool Match(string assemblyName)
        {
            return assemblyName.EndsWith(this.PatternToMatch, StringComparison.OrdinalIgnoreCase);
        }
    }
}