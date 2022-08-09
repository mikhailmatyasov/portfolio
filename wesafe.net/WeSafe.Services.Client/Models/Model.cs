using WeSafe.Shared.Abstract;

namespace WeSafe.Services.Client.Models
{
    /// <summary>
    /// Provides a base API model.
    /// </summary>
    public class Model : IHasId
    {
        /// <summary>
        /// Model identity
        /// </summary>
        public int Id { get; set; }
    }
}