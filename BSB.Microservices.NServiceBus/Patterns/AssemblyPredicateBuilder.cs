using System;
using System.Collections.Generic;

namespace BSB.Microservices.NServiceBus
{
    /// <summary>
    /// A Fluent API for building search predicates that will be used to filter the list of assemblies to scan.
    /// </summary>
    public sealed class AssemblyPredicateBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyPredicateBuilder"/> class.
        /// </summary>
        internal AssemblyPredicateBuilder()
        {
            this.Patterns = new List<IAssemblySearchPattern>();
        }

        /// <summary>
        /// Gets the list of patterns.
        /// </summary>
        /// <value>
        /// The patterns.
        /// </value>
        public List<IAssemblySearchPattern> Patterns
        {
            get;
            private set;
        }

        /// <summary>
        /// The assembly name contains the pattern
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns>SearchPatternFactory</returns>
        public AssemblyPredicateBuilder Contains(string pattern)
        {
            this.Patterns.Add(new Contains(pattern));
            return this;
        }

        /// <summary>
        /// The assembly name ends with the pattern
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns>SearchPatternFactory</returns>
        public AssemblyPredicateBuilder EndsWith(string pattern)
        {
            this.Patterns.Add(new EndsWith(pattern));
            return this;
        }

        public AssemblyPredicateBuilder Is(string pattern)
        {
            this.Patterns.Add(new Is(pattern));
            return this;
        }

        /// <summary>
        /// The assembly name is the same as the entry assembly
        /// </summary>
        /// <returns>SearchPatternFactory</returns>
        public AssemblyPredicateBuilder Self()
        {
            this.Patterns.Add(new Self());
            return this;
        }

        /// <summary>
        /// A regex.
        /// </summary>
        /// <returns></returns>
        public AssemblyPredicateBuilder RegEx(string pattern)
        {
            this.Patterns.Add(new RegularExpression(pattern));
            return this;
        }

        /// <summary>
        /// Matches all assemblies.
        /// </summary>
        /// <returns>SearchPatternFactory</returns>
        public AssemblyPredicateBuilder All()
        {
            this.Patterns.Add(new All());
            return this;
        }

        /// <summary>
        /// Matches all assemblies.
        /// </summary>
        /// <returns>SearchPatternFactory</returns>
        public AssemblyPredicateBuilder None()
        {
            this.Patterns.Add(new None());
            return this;
        }

        /// <summary>
        /// The assembly name starts with the pattern
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns>SearchPatternFactory</returns>
        public AssemblyPredicateBuilder StartsWith(string pattern)
        {
            this.Patterns.Add(new StartsWith(pattern));
            return this;
        }

        /// <summary>
        /// Add a custom search pattern.
        /// </summary>
        /// <param name="pattern">The pattern delegate.</param>
        /// <returns></returns>
        public AssemblyPredicateBuilder Custom(Func<string, bool> pattern)
        {
            this.Patterns.Add(new Custom(pattern));
            return this;
        }
    }

    public static class AssemblyPredicateBuilderExtensions
    {
        /// <summary>
        /// Excludes Microsoft assemblies.  This includes assemblies with the Microsoft or System prefixes.
        /// </summary>
        /// <param name="assemblyPredicateBuilder">The assembly predicate builder.</param>
        /// <returns></returns>
        public static AssemblyPredicateBuilder Microsoft(this AssemblyPredicateBuilder assemblyPredicateBuilder)
        {
            return assemblyPredicateBuilder.StartsWith("Microsoft").StartsWith("System");
        }
    }
}