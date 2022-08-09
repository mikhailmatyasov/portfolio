using System;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using MediatR;
using Moq;
using WeSafe.Authentication.WebApi.Commands.MobileLogin;
using WeSafe.Authentication.WebApi.Commands.MobileUser;
using WeSafe.Authentication.WebApi.Enumerations;
using WeSafe.Authentication.WebApi.Models;
using WeSafe.Authentication.WebApi.Services.Abstract;
using WeSafe.Shared.Roles;
using WeSafe.Web.Common.Authentication;
using Xunit;

namespace WeSafe.Authentication.Unit.Tests.MobileLogin
{
    public class MobileLoginCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IAuthTokenGenerator> _authTokenGenerator;
        private readonly Mock<IMediator> _mediator;

        public MobileLoginCommandHandlerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _authTokenGenerator = new Mock<IAuthTokenGenerator>();
            _mediator = new Mock<IMediator>();

            _authTokenGenerator.Setup(c => c.CreateToken(It.IsAny<TokenRequestModel>()))
                               .Returns<TokenRequestModel>((request) => new TokenResponseModel
                               {
                                   AccessToken = "some_value",
                                   DisplayName = request.Name,
                                   UserName = request.Name,
                                   Role = request.Role,
                                   Demo = request.Demo,
                                   ExpiresAt = request.Expires
                               }).Verifiable();
        }

        [Fact]
        public void Process_ArgumentsIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MobileLoginCommandHandler(null, null, null));
            Assert.Throws<ArgumentNullException>(() => new MobileLoginCommandHandler(_unitOfWork.Object, null, null));
            Assert.Throws<ArgumentNullException>(() => new MobileLoginCommandHandler(null, _authTokenGenerator.Object, null));
        }

        [Fact]
        public async Task Process_SuccessWithNewUser()
        {
            MobileUserModel mobileUser = null;
            _mediator.Setup(c => c.Send(It.IsAny<GetMobileUserByPhoneCommand>(), CancellationToken.None))
                     .ReturnsAsync(() => mobileUser)
                     .Verifiable();
            _mediator.Setup(c => c.Send(It.IsAny<CreateMobileUserCommand>(), CancellationToken.None))
                     .ReturnsAsync(() =>
                     {
                         mobileUser = new MobileUserModel
                         {
                             Phone = "+71234567890",
                             CreatedAt = DateTimeOffset.UtcNow,
                             Status = MobileUserStatus.Active
                         };

                         return 1;
                     })
                     .Verifiable();

            var handler = new MobileLoginCommandHandler(_unitOfWork.Object, _authTokenGenerator.Object, _mediator.Object);

            var token = await handler.Handle(new MobileLoginCommand
            {
                PhoneNumber = "+71234567890"
            }, default);

            Assert.NotNull(token);
            Assert.Equal("+71234567890", token.UserName);
            Assert.Equal("+71234567890", token.DisplayName);
            Assert.Equal(UserRoles.Users, token.Role);
            Assert.NotNull(token.AccessToken);

            _authTokenGenerator.Verify(c => c.CreateToken(It.IsAny<TokenRequestModel>()), Times.Once);
            _mediator.Verify(c => c.Send(It.IsAny<GetMobileUserByPhoneCommand>(), CancellationToken.None), Times.Exactly(2));
            _mediator.Verify(c => c.Send(It.IsAny<CreateMobileUserCommand>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Process_SuccessWithExistingUser()
        {
            _mediator.Setup(c => c.Send(It.IsAny<GetMobileUserByPhoneCommand>(), CancellationToken.None))
                     .ReturnsAsync(() => new MobileUserModel
                     {
                         Phone = "+71234567890",
                         CreatedAt = DateTimeOffset.UtcNow,
                         Status = MobileUserStatus.Active
                     })
                     .Verifiable();
            _mediator.Setup(c => c.Send(It.IsAny<CreateMobileUserCommand>(), CancellationToken.None)).Verifiable();
            var handler = new MobileLoginCommandHandler(_unitOfWork.Object, _authTokenGenerator.Object, _mediator.Object);

            var token = await handler.Handle(new MobileLoginCommand
            {
                PhoneNumber = "+71234567890"
            }, default);

            Assert.NotNull(token);
            Assert.Equal("+71234567890", token.UserName);
            Assert.Equal("+71234567890", token.DisplayName);
            Assert.Equal(UserRoles.Users, token.Role);
            Assert.NotNull(token.AccessToken);

            _mediator.Verify(c => c.Send(It.IsAny<GetMobileUserByPhoneCommand>(), CancellationToken.None), Times.Once);
            _mediator.Verify(c => c.Send(It.IsAny<CreateMobileUserCommand>(), CancellationToken.None), Times.Never);
            _authTokenGenerator.Verify(c => c.CreateToken(It.IsAny<TokenRequestModel>()), Times.Once);
        }
    }
}