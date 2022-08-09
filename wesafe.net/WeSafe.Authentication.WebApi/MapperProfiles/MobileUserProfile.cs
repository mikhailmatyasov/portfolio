using System;
using AutoMapper;
using WeSafe.Authentication.WebApi.Models;
using WeSafe.DAL.Entities;

namespace WeSafe.Authentication.WebApi.MapperProfiles
{
    public class MobileUserProfile : Profile
    {
        public MobileUserProfile()
        {
            CreateMap<MobileUser, MobileUserModel>()
                .ForMember(dest => dest.Status, src => src.ConvertUsing(new MobileUserStatusConverter()));
        }
    }
}