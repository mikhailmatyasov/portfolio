using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WeSafe.DAL;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client;
using WeSafe.Services.Client.Models;
using WeSafe.Shared.Results;

namespace WeSafe.Services
{
    public class LicensePlateRestrictionService : BaseService, ILicensePlateRestrictionService
    {
        public LicensePlateRestrictionService(WeSafeDbContext context, ILoggerFactory loggerFactory) : base(context, loggerFactory)
        {
        }

        public async Task<SuccessExecutionResult> AddLicensePlateRestrictionAsync(int deviceId, LicensePlateRestrictionModel model)
        {
            if (model == null)
                throw new InvalidOperationException(nameof(model));

            var licensePlateRestriction = GetLicensePlateRestriction(deviceId, model);

            await DbContext.LicensePlateRestrictions.AddAsync(licensePlateRestriction);
            await DbContext.SaveChangesAsync();

            return new SuccessExecutionResult();
        }

        public List<LicensePlateRestrictionModel> GetLicensePlateRestrictions(int deviceId)
        {
            return DbContext.LicensePlateRestrictions.Where(l => l.DeviceId == deviceId).Select(GetLicensePlateRestrictionModel).ToList();
        }

        public async Task<IExecutionResult> DeleteLicensePlateRestrictionAsync(int id)
        {
            var licensePlateRestriction = await DbContext.LicensePlateRestrictions.FirstOrDefaultAsync(l => l.Id == id);
            if (licensePlateRestriction == null)
                return ExecutionResult.Failed("License plate restriction not found.");

            DbContext.LicensePlateRestrictions.Remove(licensePlateRestriction);
            await DbContext.SaveChangesAsync();

            return new SuccessExecutionResult();
        }

        private LicensePlateRestriction GetLicensePlateRestriction(int deviceId, LicensePlateRestrictionModel model)
        {
            return new LicensePlateRestriction()
            {
                LicensePlate = model.LicensePlate,
                LicensePlateType = model.LicensePlateType,
                DeviceId = deviceId
            };
        }

        private LicensePlateRestrictionModel  GetLicensePlateRestrictionModel(LicensePlateRestriction licensePlateRestriction)
        {
            return new LicensePlateRestrictionModel()
            {
                Id = licensePlateRestriction.Id,
                LicensePlate = licensePlateRestriction.LicensePlate,
                LicensePlateType = licensePlateRestriction.LicensePlateType
            };
        }


    }
}
