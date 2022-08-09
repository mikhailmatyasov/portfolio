using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Moq;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.Dashboard.WebApi.Behaviors;
using WeSafe.Dashboard.WebApi.Commands.Clients;
using WeSafe.Dashboard.WebApi.Models;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.CreateClient
{
    public class CreateClientValidatorTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IClientRepository> _repository;

        public CreateClientValidatorTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _repository = new Mock<IClientRepository>();

            _unitOfWork.Setup(c => c.GetRepository<Client>(true)).Returns(_repository.Object);
        }

        [Fact]
        public void Process_UnitOfWorkIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CreateClientValidator(null));
        }

        [Fact]
        public async Task Process_ClientFound_ThrowsInvalidOperationException()
        {
            _repository.Setup(x => x.FindByPhoneAsync(It.IsAny<string>(), true)).ReturnsAsync(() => new Client()).Verifiable();

            var validator = new CreateClientValidator(_unitOfWork.Object);

            // assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await validator.Process(new CreateClientCommand
                {
                    Client = new ClientModel()
                }, CancellationToken.None));

            _repository.Verify(x => x.FindByPhoneAsync(It.IsAny<string>(), true), Times.Once);
        }

        [Fact]
        public async Task Process_ClientNotFound_Success()
        {
            _repository.Setup(x => x.FindByPhoneAsync(It.IsAny<string>(), true)).ReturnsAsync(() => null).Verifiable();

            var validator = new CreateClientValidator(_unitOfWork.Object);

            await validator.Process(new CreateClientCommand
            {
                Client = new ClientModel()
            }, CancellationToken.None);

            _repository.Verify(x => x.FindByPhoneAsync(It.IsAny<string>(), true), Times.Once);
        }
    }
}