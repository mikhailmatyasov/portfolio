using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using WeSafe.Dashboard.WebApi.Commands.Clients;
using WeSafe.Dashboard.WebApi.Models;
using WeSafe.Dashboard.WebApi.Validators.Client;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.CreateClient
{
    public class CreateClientCommandValidatorTests
    {
        [Fact]
        public async Task Validate_CommandIsNull_ThrowsBadRequestException()
        {
            var validator = new CreateClientCommandValidator();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await validator.TestValidateAsync(null, CancellationToken.None));
        }

        [Fact]
        public async Task Validate_InvalidClientValue()
        {
            //arrange
            var validator = new CreateClientCommandValidator();
            var command = new CreateClientCommand
            {
                Client = null
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.Client);

            //assert
            Assert.True(errors.Any());
        }

        [Fact]
        public async Task Validate_ValidData()
        {
            //arrange
            var validator = new CreateClientCommandValidator();
            var command = new CreateClientCommand
            {
                Client = new ClientModel
                {
                    Name = "name",
                    Phone = "+71234567890"
                }
            };

            //act
            var result = await validator.TestValidateAsync(command, CancellationToken.None);

            //assert
            Assert.True(result.IsValid);
        }
    }
}