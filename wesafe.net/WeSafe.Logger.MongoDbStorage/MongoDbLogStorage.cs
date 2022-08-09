using AutoMapper;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Logger.Abstraction.Interfaces;
using WeSafe.Logger.Abstraction.Services;
using WeSafe.Logger.MongoDbStorage.Models;

namespace WeSafe.Logger.MongoDbStorage
{
    public class MongoDbLogStorage : IWeSafeLogStorage
    {
        private readonly MongoLogConfiguration _configuration;
        private readonly IMapper _mapper;

        public MongoDbLogStorage(MongoLogConfiguration configuration, IMapper mapper)
        {
            if (string.IsNullOrWhiteSpace(configuration.Address))
            {
                throw new InvalidOperationException($"{configuration.Address} is empty.");
            }

            if (string.IsNullOrWhiteSpace(configuration.Collection))
            {
                throw new InvalidOperationException($"{configuration.Collection} is empty.");
            }

            if (string.IsNullOrWhiteSpace(configuration.Database))
            {
                throw new InvalidOperationException($"{configuration.Database} is empty.");
            }

            _mapper = mapper;

            _configuration = configuration;
        }

        public async Task Add(IEnumerable<IWeSafeLog> logs)
        {
            var bsonLogs = _mapper.Map<IEnumerable<DeviceLogBsonModel>>(logs);

            MongoClient client = new MongoClient(_configuration.Address);

            IMongoDatabase database = client.GetDatabase(_configuration.Database);

            var collection = database.GetCollection<DeviceLogBsonModel>(_configuration.Collection);

            await collection.InsertManyAsync(bsonLogs);
        }
    }
}
