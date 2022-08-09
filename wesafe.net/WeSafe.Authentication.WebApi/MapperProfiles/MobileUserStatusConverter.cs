using System;
using AutoMapper;
using WeSafe.Authentication.WebApi.Enumerations;

namespace WeSafe.Authentication.WebApi.MapperProfiles
{
    public class MobileUserStatusConverter : IValueConverter<string, MobileUserStatus>
    {
        public MobileUserStatus Convert(string sourceMember, ResolutionContext context)
        {
            return Enum.TryParse(sourceMember, true, out MobileUserStatus value) ? value : MobileUserStatus.None;
        }
    }
}