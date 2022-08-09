using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services
{
    /// <inheritdoc cref="IRecognitionObjectService"/>
    public class RecognitionObjectService : BaseService, IRecognitionObjectService
    {
        #region Constructors

        public RecognitionObjectService(WeSafeDbContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory)
        {
        }

        #endregion

        #region IRecognitionObjectService implementation

        public async Task<IEnumerable<RecognitionObjectModel>> GetRecognitionObjectsAsync(bool activeOnly)
        {
            return await DbContext.RecognitionObjects
                                  .Where(c => !activeOnly || c.IsActive)
                                  .OrderBy(c => c.Name)
                                  .Select(c => new RecognitionObjectModel
                                  {
                                      Id = c.Id,
                                      Name = c.Name,
                                      Description = c.Description,
                                      IsActive = c.IsActive
                                  })
                                  .ToListAsync();
        }

        public async Task<RecognitionObjectModel> GetRecognitionObjectAsync(int id)
        {
            return await DbContext.RecognitionObjects
                                  .Select(c => new RecognitionObjectModel
                                  {
                                      Id = c.Id,
                                      Name = c.Name,
                                      Description = c.Description,
                                      IsActive = c.IsActive
                                  })
                                  .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task CreateRecognitionObjectAsync(RecognitionObjectModel model)
        {
            if ( model == null )
            {
                throw new ArgumentNullException(nameof(model));
            }

            await ValidateModel(model);

            DbContext.RecognitionObjects.Add(new RecognitionObject
            {
                Name = model.Name,
                Description = model.Description,
                IsActive = model.IsActive
            });

            await SaveChangesAsync();
        }

        public async Task UpdateRecognitionObjectAsync(RecognitionObjectModel model)
        {
            if ( model == null )
            {
                throw new ArgumentNullException(nameof(model));
            }

            var recognitionObject = await DbContext.RecognitionObjects.FindAsync(model.Id);

            if ( recognitionObject == null )
            {
                throw new InvalidOperationException("Recognition object is not found.");
            }

            recognitionObject.Description = model.Description;
            recognitionObject.IsActive = model.IsActive;

            await SaveChangesAsync();
        }

        #endregion

        #region Private members

        private async Task ValidateModel(RecognitionObjectModel model)
        {
            if ( String.IsNullOrEmpty(model.Name) )
            {
                throw new ValidationException("Recognition object name is required.");
            }

            if ( model.Name.Length > 50 )
            {
                throw new ValidationException("Recognition object name too long.");
            }

            if ( await DbContext.RecognitionObjects.AnyAsync(c => c.Name == model.Name) )
            {
                throw new ValidationException($"Recognition object with name '{model.Name}' already exists.");
            }
        }

        #endregion
    }
}