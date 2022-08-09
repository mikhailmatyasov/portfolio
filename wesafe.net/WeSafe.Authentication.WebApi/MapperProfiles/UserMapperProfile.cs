using AutoMapper;
using WeSafe.Authentication.WebApi.Commands.Users;
using WeSafe.Bus.Contracts.Register;
using WeSafe.Bus.Contracts.User;

namespace WeSafe.Authentication.WebApi.MapperProfiles
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            CreateMap<ICreateUserContract, CreateUserCommand>();

            CreateMap<ICreateUserValidationContract, CreateUserCommand>();
        }
    }
}
