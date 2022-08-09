using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using WeSafe.Dashboard.WebApi.Models;
using WeSafe.Dashboard.WebApi.Validators.Client;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.CreateClient
{
    public class ClientModelValidatorTests
    {
        [Fact]
        public async Task Validate_CommandIsNull_ThrowsBadRequestException()
        {
            var validator = new ClientModelValidator();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await validator.TestValidateAsync(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("some value")]
        [InlineData("+764555434434554")]
        public async Task Validate_InvalidPhoneValue(string phone)
        {
            //arrange
            var validator = new ClientModelValidator();
            var model = new ClientModel
            {
                Name = "name",
                Phone = phone
            };

            //act
            var result = await validator.TestValidateAsync(model, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.Phone);

            //assert
            Assert.True(errors.Any());
        }

        [Fact]
        public async Task Validate_InvalidNameValue()
        {
            //arrange
            var validator = new ClientModelValidator();
            var model = new ClientModel
            {
                Name = new string('a', 260),
                Phone = "+71234567890"
            };

            //act
            var result = await validator.TestValidateAsync(model, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.Name);

            //assert
            Assert.True(errors.Any());
        }

        [Fact]
        public async Task Validate_ValidData()
        {
            //arrange
            var validator = new ClientModelValidator();
            var model = new ClientModel
            {
                Name = "name",
                Phone = "+71234567890"
            };

            //act
            var result = await validator.TestValidateAsync(model, CancellationToken.None);

            //assert
            Assert.True(result.IsValid);
        }
    }
}