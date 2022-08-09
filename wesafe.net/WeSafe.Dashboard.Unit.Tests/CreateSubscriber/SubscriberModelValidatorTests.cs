using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using WeSafe.Dashboard.WebApi.Models;
using WeSafe.Dashboard.WebApi.Validators.Subscribers;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.CreateSubscriber
{
    public class SubscriberModelValidatorTests
    {
        [Fact]
        public async Task Validate_CommandIsNull_ThrowsBadRequestException()
        {
            var validator = new SubscriberModelValidator();

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
            var validator = new SubscriberModelValidator();
            var model = new SubscriberModel
            {
                Name = "name",
                Phone = phone,
                ClientId = 1
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
            var validator = new SubscriberModelValidator();
            var model = new SubscriberModel
            {
                Name = new string('a', 260),
                Phone = "+71234567890",
                ClientId = 1
            };

            //act
            var result = await validator.TestValidateAsync(model, CancellationToken.None);
            var errors = result.ShouldHaveValidationErrorFor(x => x.Name);

            //assert
            Assert.True(errors.Any());
        }

        [Fact]
        public async Task Validate_InvalidClientIdValue()
        {
            //arrange
            var validator = new SubscriberModelValidator();
            var model = new SubscriberModel
            {
                Name = new string('a', 260),
                Phone = "+71234567890",
                ClientId = 0
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
            var validator = new SubscriberModelValidator();
            var model = new SubscriberModel
            {
                Name = "name",
                Phone = "+71234567890",
                ClientId = 1
            };

            //act
            var result = await validator.TestValidateAsync(model, CancellationToken.None);

            //assert
            Assert.True(result.IsValid);
        }
    }
}