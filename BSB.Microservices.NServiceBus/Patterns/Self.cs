namespace BSB.Microservices.NServiceBus
{
    using System.Reflection;

    /// <summary>
    /// Matches the entry assembly
    /// </summary>

    public class Self : StartsWith
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Self" /> class.
        /// </summary>
        internal Self() : base(null)
        {
            this.PatternToMatch = Assembly.GetEntryAssembly().GetName().Name;
        }
    }


   
}