using System.Reflection;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// Defines a simple abstraction for filtering assembly names during the assembly scan.
    /// </summary>
    public interface IAssemblySearchPattern
    {
        /// <summary>
        /// Gets or sets the pattern to match.
        /// </summary>
        /// <value>
        /// The pattern to match.
        /// </value>
        string PatternToMatch { get; set; }
        /// <summary>
        /// Matches the specified assembly name.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>bool</returns>
        bool Match(Assembly assemblyName);

        /// <summary>
        /// Matches the specified assembly name.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>bool</returns>
        bool Match(string assemblyName);
    }
}