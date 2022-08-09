using MediatR;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.Commands.Clients
{
    /// <summary>
    /// Represents a client creation command.
    /// </summary>
    public class CreateClientCommand : IRequest<int>
    {
        /// <summary>
        /// <see cref="ClientModel"/> to create.
        /// </summary>
        public ClientModel Client { get; set; }
    }
}