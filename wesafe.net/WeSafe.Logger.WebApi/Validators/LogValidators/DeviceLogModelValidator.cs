using FluentValidation;
using System;
using WeSafe.Logger.Abstraction.Models;
using WeSafe.Web.Common.Exceptions;

namespace WeSafe.Logger.WebApi.Validators.LogValidators
{
    public class DeviceLogModelValidator : AbstractValidator<DeviceLogModel>
    {
        public DeviceLogModelValidator()
        {
            RuleFor(x => x.DeviceId).GreaterThan(0).WithMessage("Invalid device id");

            RuleFor(x => x.Message).NotEmpty().WithMessage("Message is empty.");

            RuleFor(x => x.DateTime).NotEqual(default(DateTime)).WithMessage("The datetime is default.");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            if(instanceToValidate == null)
                throw new BadRequestException("One of log models is null.");

            base.EnsureInstanceNotNull(instanceToValidate);
        }
    }
}
