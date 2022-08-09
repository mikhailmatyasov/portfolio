using AutoMapper;
using WeSafe.DAL.Entities;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.MapperProfiles
{
    public class ClientMapperProfile : Profile
    {
        public ClientMapperProfile()
        {
            CreateMap<Client, ClientModel>().ReverseMap();
        }
    }
}