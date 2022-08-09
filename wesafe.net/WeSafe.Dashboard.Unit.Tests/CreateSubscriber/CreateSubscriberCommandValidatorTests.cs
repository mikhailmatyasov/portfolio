using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using WeSafe.Dashboard.WebApi.Commands.Subscribers;
using WeSafe.Dashboard.WebApi.Models;
using WeSafe.Dashboard.WebApi.Validators.Subscribers;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.CreateSubscriber
{
    public class CreateSubscriberCommandValidatorTests
    {
        [Fact]
        public async Task Validate_CommandIsNull_ThrowsBadRequestException()
        {
            var validator = new CreateSubscriberCommandValidator();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await validator.TestValidateAsync(null, CancellationToken.None));
        }

        [Fact]
        public async Task Validate_InvalidSubscriberValue()
        {
            //arrange
            var validator = new CreateSubscriberCommandValidator();
            var command = new CreateSubscriberCommand
            {
                Subscriber = null
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.Subscriber);

            //assert
            Assert.True(errors.Any());
        }

        [Fact]
        public async Task Validate_ValidData()
        {
            //arrange
            var validator = new CreateSubscriberCommandValidator();
            var command = new CreateSubscriberCommand
            {
                Subscriber = new SubscriberModel
                {
                    Phone = "+71234567890",
                    Name = "name",
                    ClientId = 1
                }
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);

            //assert
            Assert.True(result.IsValid);
        }
    }
}