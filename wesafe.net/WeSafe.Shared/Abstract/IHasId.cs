namespace WeSafe.Shared.Abstract
{
    /// <summary>
    /// Provides an abstraction for data types that has an integer identity.
    /// </summary>
    public interface IHasId
    {
        /// <summary>
        /// Identity value.
        /// </summary>
        int Id { get; set; }
    }
}