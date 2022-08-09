using System;
using System.Linq.Expressions;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services
{
    public class ClientSubscriberMapper
    {
        private readonly Func<ClientSubscriber, ClientSubscriberModel> _compiledProjection;

        public ClientSubscriberMapper()
        {
            _compiledProjection = Projection.Compile();
        }

        public Expression<Func<ClientSubscriber, ClientSubscriberModel>> Projection => c => new ClientSubscriberModel
        {
            Id = c.Id,
            Phone = c.Phone,
            Password = c.Password,
            ClientId = c.ClientId,
            Permissions = c.Permissions,
            Name = c.Name,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        };

        public ClientSubscriberModel ToClientSubscriberModel(ClientSubscriber device)
        {
            if ( device == null ) throw new ArgumentNullException(nameof(device));

            return _compiledProjection(device);
        }

        public ClientSubscriber ToClientSubscriber(ClientSubscriberModel model)
        {
            if ( model == null ) throw new ArgumentNullException(nameof(model));

            return new ClientSubscriber
            {
                Id = model.Id,
                Phone = model.Phone,
                Password = model.Password,
                ClientId = model.ClientId,
                Permissions = model.Permissions,
                Name = model.Name,
                IsActive = model.IsActive,
                CreatedAt = model.CreatedAt
            };
        }

        public ClientSubscriber ToClientSubscriber(ClientSubscriber user, ClientSubscriberModel model)
        {
            if ( model == null ) throw new ArgumentNullException(nameof(model));
            if ( user == null ) throw new ArgumentNullException(nameof(user));

            user.Phone = model.Phone;
            user.Password = model.Password;
            user.Permissions = model.Permissions;
            user.Name = model.Name;
            user.IsActive = model.IsActive;

            return user;
        }
    }
}