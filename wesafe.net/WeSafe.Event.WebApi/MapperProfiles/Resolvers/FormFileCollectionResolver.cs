using AutoMapper;
using System.Collections.Generic;
using WeSafe.Bus.Contracts.Event;
using WeSafe.Event.WebApi.Commands.AddEvent;
using WeSafe.Web.Common.Services;
using Blob = WeSafe.Bus.Contracts.Blob;

namespace WeSafe.Event.WebApi.MapperProfiles.Resolvers
{
    public class FormFileCollectionResolver : IValueResolver<AddEventCommand, ICreateEventContract, IEnumerable<Blob>>
    {
        public IEnumerable<Blob> Resolve(AddEventCommand source, ICreateEventContract destination, IEnumerable<Blob> destMember,
            ResolutionContext context)
        {
            return BlobExtensions.EncodeFile(source.Frames);
        }
    }
}
