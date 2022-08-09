using System;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services
{
    public class PermittedAdminIpMapper
    {
        public PermittedAdminIpModel ToPermittedAdminIpModel(PermittedAdminIp permittedAdminIp)
        {
            if (permittedAdminIp == null)
                throw new ArgumentNullException(nameof(permittedAdminIp));

            return new PermittedAdminIpModel
            {
                Id = permittedAdminIp.Id,
                Ip = permittedAdminIp.IpAddress
            };
        }

        public PermittedAdminIp ToPermittedAdminIp(PermittedAdminIpModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return new PermittedAdminIp
            {
                Id = model.Id,
                IpAddress = model.Ip
            };
        }
    }
}
