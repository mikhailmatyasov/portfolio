using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeSafe.Event.WebApi.Models;

namespace WeSafe.Event.WebApi.Commands.AddEvent
{
    public class AddEventCommand : EventsBaseRequestModel, IRequest
    {
        [BindProperty]
        public IFormFileCollection Frames { get; set; }
    }
}
