using System;
using System.Reflection;

namespace BSB.Microservices.NServiceBus
{
    public class Custom : IAssemblySearchPattern
    {
        /// <summary>
        /// Gets or sets the pattern to match.
        /// </summary>
        /// <value>
        /// The pattern to match.
        /// </value>
        public string PatternToMatch { get; set; }

        /// <summary>
        /// The name predicate
        /// </summary>
        private Func<string, bool> namePredicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom" /> class.
        /// </summary>
        /// <param name="namePredicate">The name predicate.</param>
        public Custom(Func<string, bool> namePredicate = null)
        {
            this.namePredicate = namePredicate;
        }

        /// <summary>
        /// Matches the specified assembly name.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>
        /// bool
        /// </returns>

        public bool Match(string assemblyName)
        {
            var result = this.namePredicate?.Invoke(assemblyName);
            return result.HasValue ? result.Value : false;
        }

        /// <summary>
        /// Matches the specified assembly name.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>
        /// bool
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool Match(Assembly assemblyName)
        {
            var name = assemblyName.GetName();
            return Match(name.Name);
        }
    }
}