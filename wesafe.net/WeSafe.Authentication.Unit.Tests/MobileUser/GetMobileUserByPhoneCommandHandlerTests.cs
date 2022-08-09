using System;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using Moq;
using WeSafe.Authentication.WebApi.Commands.MobileUser;
using WeSafe.Authentication.WebApi.Models;
using WeSafe.DAL.Abstractions;
using Xunit;

namespace WeSafe.Authentication.Unit.Tests.MobileUser
{
    public class GetMobileUserByPhoneCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMobileUserRepository> _repository;
        private readonly Mock<IMapper> _mapper;

        public GetMobileUserByPhoneCommandHandlerTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _repository = new Mock<IMobileUserRepository>();
            _mapper = new Mock<IMapper>();

            _unitOfWork.Setup(c => c.GetRepository<DAL.Entities.MobileUser>(true)).Returns(_repository.Object);
        }

        [Fact]
        public void Process_ArgumentsIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GetMobileUserByPhoneCommandHandler(null, _mapper.Object));
            Assert.Throws<ArgumentNullException>(() => new GetMobileUserByPhoneCommandHandler(_unitOfWork.Object, null));
        }

        [Fact]
        public async Task Process_GetByPhoneExistingUser_Success()
        {
            _repository.Setup(c => c.FindByPhoneNumberAsync(It.IsAny<string>(), It.IsAny<bool>()))
                       .ReturnsAsync(() => new DAL.Entities.MobileUser
                       {
                           Id = 1,
                           Phone = "+71234567890"
                       })
                       .Verifiable();
            _mapper.Setup(c => c.Map<MobileUserModel>(It.IsAny<DAL.Entities.MobileUser>()))
                   .Returns<DAL.Entities.MobileUser>(mobileUser => new MobileUserModel
                   {
                       Id = mobileUser.Id,
                       Phone = mobileUser.Phone
                   })
                   .Verifiable();

            var handler = new GetMobileUserByPhoneCommandHandler(_unitOfWork.Object, _mapper.Object);
            var user = await handler.Handle(new GetMobileUserByPhoneCommand
            {
                PhoneNumber = "+71234567890"
            }, default);

            Assert.NotNull(user);
            Assert.Equal("+71234567890", user.Phone);
        }

        [Fact]
        public async Task Process_GetByPhoneNotFoundUser_Success()
        {
            _repository.Setup(c => c.FindByPhoneNumberAsync(It.IsAny<string>(), It.IsAny<bool>()))
                       .ReturnsAsync(() => null)
                       .Verifiable();
            _mapper.Setup(c => c.Map<MobileUserModel>(It.IsAny<DAL.Entities.MobileUser>()))
                   .Returns<DAL.Entities.MobileUser>(mobileUser => null)
                   .Verifiable();

            var handler = new GetMobileUserByPhoneCommandHandler(_unitOfWork.Object, _mapper.Object);
            var user = await handler.Handle(new GetMobileUserByPhoneCommand
            {
                PhoneNumber = "+71234567890"
            }, default);

            Assert.Null(user);
        }
    }
}