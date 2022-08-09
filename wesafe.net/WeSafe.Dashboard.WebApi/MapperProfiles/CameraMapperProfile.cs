using AutoMapper;
using WeSafe.DAL.Entities;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.MapperProfiles
{
    public class CameraMapperProfile : Profile
    {
        public CameraMapperProfile()
        {
            CreateMap<Camera, CameraModelResponse>();
            CreateMap<CameraBaseModel, Camera>();
        }
    }
}
