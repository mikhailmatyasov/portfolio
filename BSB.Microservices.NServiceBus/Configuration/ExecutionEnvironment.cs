using System;

/// <summary>
/// Models an environment local|development|testing|staging|production.  IHostingEnvironment is slightly different when using 
/// WebHost vs GenericHost, therefore, we will just pass the environment string and wrap it in  a class.
/// 
/// </summary>
public class ExecutionEnvironment
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionEnvironment"/> class.
    /// </summary>
    /// <param name="environment">The environment.</param>
    internal ExecutionEnvironment(string environment)
    {
        Name = environment ?? "production";
    }

    /// <summary>
    /// Determines whether this instance is local.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance is local; otherwise, <c>false</c>.
    /// </returns>
    public bool IsLocal()
    {
        if (Name == null)
        {
            return false;
        }

        return Name.Equals("development", StringComparison.OrdinalIgnoreCase) ||
               Name.ToUpper().Contains("DEBUG") ||
               Name.ToUpper().Contains("LOCAL");
    }
}