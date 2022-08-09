using System;
using System.Linq.Expressions;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services
{
    public class ClientMapper
    {
        private readonly Func<DAL.Entities.Client, ClientModel> _compiledProjection;

        public ClientMapper()
        {
            _compiledProjection = Projection.Compile();
        }

        public Expression<Func<DAL.Entities.Client, ClientModel>> Projection => client => new ClientModel
        {
            Id = client.Id,
            Token = client.Token,
            Name = client.Name,
            Phone = client.Phone,
            Info = client.Info,
            ContractNumber = client.ContractNumber,
            CreatedAt = client.CreatedAt,
            IsActive = client.IsActive,
            SendToDevChat = client.SendToDevChat
        };

        public ClientModel ToClientModel(DAL.Entities.Client device)
        {
            if ( device == null ) throw new ArgumentNullException(nameof(device));

            return _compiledProjection(device);
        }

        public DAL.Entities.Client ToClient(ClientModel model)
        {
            if ( model == null ) throw new ArgumentNullException(nameof(model));

            return new DAL.Entities.Client
            {
                Id = model.Id,
                Token = model.Token,
                Name = model.Name,
                Phone = model.Phone,
                Info = model.Info,
                ContractNumber = model.ContractNumber,
                CreatedAt = model.CreatedAt,
                IsActive = model.IsActive,
                SendToDevChat = model.SendToDevChat
            };
        }

        public DAL.Entities.Client ToClient(DAL.Entities.Client client, ClientModel model)
        {
            if ( model == null ) throw new ArgumentNullException(nameof(model));
            if ( client == null ) throw new ArgumentNullException(nameof(client));

            client.ContractNumber = model.ContractNumber;
            client.Info = model.Info;
            client.IsActive = model.IsActive;
            client.Name = model.Name;
            client.Phone = model.Phone;
            client.SendToDevChat = model.SendToDevChat;

            return client;
        }
    }
}