using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;
using WeSafe.Shared.Results;

namespace WeSafe.Services.Client
{
    public interface ILicensePlateRestrictionService
    {
        Task<SuccessExecutionResult> AddLicensePlateRestrictionAsync(int deviceId, LicensePlateRestrictionModel model);

        List<LicensePlateRestrictionModel> GetLicensePlateRestrictions(int deviceId);

        Task<IExecutionResult> DeleteLicensePlateRestrictionAsync(int id);
    }
}
