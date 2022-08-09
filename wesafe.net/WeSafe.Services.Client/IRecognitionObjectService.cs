using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services.Client
{
    /// <summary>
    /// Provides an abstraction for service managing recognition objects.
    /// </summary>
    public interface IRecognitionObjectService
    {
        /// <summary>
        /// Gets all recognition objects sorted by name.
        /// </summary>
        /// <param name="activeOnly"></param>
        /// <returns></returns>
        Task<IEnumerable<RecognitionObjectModel>> GetRecognitionObjectsAsync(bool activeOnly);

        /// <summary>
        /// Gets a recognition object by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<RecognitionObjectModel> GetRecognitionObjectAsync(int id);

        /// <summary>
        /// Creates a recognition object. Recognition object name should be unique.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task CreateRecognitionObjectAsync(RecognitionObjectModel model);

        /// <summary>
        /// Updates a recognition object. Recognition object name wil not be updated.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task UpdateRecognitionObjectAsync(RecognitionObjectModel model);
    }
}