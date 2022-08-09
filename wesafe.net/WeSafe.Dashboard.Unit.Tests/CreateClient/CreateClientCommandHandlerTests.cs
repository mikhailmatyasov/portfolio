using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using MediatR;
using Moq;
using WeSafe.DAL.Abstractions;
using WeSafe.DAL.Entities;
using WeSafe.Dashboard.WebApi.Commands.Clients;
using WeSafe.Dashboard.WebApi.Commands.Subscribers;
using WeSafe.Dashboard.WebApi.Models;
using Xunit;

namespace WeSafe.Dashboard.Unit.Tests.CreateClient
{
    public class CreateClientCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IClientRepository> _repository;
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IMapper> _mapper;

        public CreateClientCommandHandlerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _repository = new Mock<IClientRepository>();
            _mediator = new Mock<IMediator>();
            _mapper = new Mock<IMapper>();

            _unitOfWork.Setup(c => c.GetRepository<Client>(true)).Returns(_repository.Object);
            _unitOfWork.Setup(c => c.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(0).Verifiable();
        }

        [Fact]
        public void Process_ArgumentsIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CreateClientCommandHandler(null, _mapper.Object, _mediator.Object));
            Assert.Throws<ArgumentNullException>(() => new CreateClientCommandHandler(_unitOfWork.Object, null, _mediator.Object));
            Assert.Throws<ArgumentNullException>(() => new CreateClientCommandHandler(_unitOfWork.Object, _mapper.Object, null));
        }

        [Fact]
        public async Task Process_Success()
        {
            _mapper.Setup(c => c.Map<Client>(It.IsAny<ClientModel>())).Returns<ClientModel>(model => new Client
            {
                Name = model.Name,
                Phone = model.Phone
            }).Verifiable();

            _repository.Setup(c => c.Insert(It.IsAny<Client>())).Verifiable();
            _mediator.Setup(c => c.Send(It.IsAny<CreateSubscriberCommand>(), CancellationToken.None)).ReturnsAsync(1).Verifiable();

            var handler = new CreateClientCommandHandler(_unitOfWork.Object, _mapper.Object, _mediator.Object);

            var id = await handler.Handle(new CreateClientCommand
            {
                Client = new ClientModel
                {
                    Name = "name",
                    Phone = "+71234567890"
                }
            }, CancellationToken.None);

            _mapper.Verify(c => c.Map<Client>(It.IsAny<ClientModel>()), Times.Once);
            _unitOfWork.Verify(c => c.SaveChangesAsync(It.IsAny<bool>()), Times.Once);
            _repository.Verify(c => c.Insert(It.IsAny<Client>()), Times.Once);
            _mediator.Verify(c => c.Send(It.IsAny<CreateSubscriberCommand>(), CancellationToken.None), Times.Once);
        }
    }
}