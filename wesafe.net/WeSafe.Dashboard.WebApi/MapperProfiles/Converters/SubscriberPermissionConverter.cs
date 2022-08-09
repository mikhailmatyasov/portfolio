using System;
using AutoMapper;
using WeSafe.Dashboard.WebApi.Enumerations;

namespace WeSafe.Dashboard.WebApi.MapperProfiles.Converters
{
    public class SubscriberPermissionConverter : IValueConverter<string, SubscriberPermission>
    {
        public SubscriberPermission Convert(string sourceMember, ResolutionContext context)
        {
            if ( Enum.TryParse(sourceMember, true, out SubscriberPermission value) )
            {
                return value;
            }

            return SubscriberPermission.None;
        }
    }
}