using FluentValidation;
using WeSafe.Logger.WebApi.Commands.AddLogs;

namespace WeSafe.Logger.WebApi.Validators.LogValidators
{
    public class AddLogsCommandValidator : AbstractValidator<AddLogsCommand>
    {
        public AddLogsCommandValidator()
        {
            RuleForEach(x => x.Logs).SetValidator((command, logModel) => new DeviceLogModelValidator());
        }
    }
}
