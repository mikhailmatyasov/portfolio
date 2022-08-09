using MediatR;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.Commands.Subscribers
{
    /// <summary>
    /// Represents a subscriber creation command
    /// </summary>
    public class CreateSubscriberCommand : IRequest<int>
    {
        /// <summary>
        /// Gets or sets subscriber model.
        /// </summary>
        public SubscriberModel Subscriber { get; set; }
    }
}