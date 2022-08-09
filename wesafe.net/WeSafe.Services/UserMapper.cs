using System;
using System.Linq.Expressions;
using WeSafe.DAL.Entities;
using WeSafe.Services.Client.Models;

namespace WeSafe.Services
{
    public class UserMapper
    {
        private readonly Func<User, UserModel> _compiledProjection;

        public UserMapper()
        {
            _compiledProjection = Projection.Compile();
        }

        public Expression<Func<User, UserModel>> Projection => user => new UserModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Phone = user.PhoneNumber,
            Email = user.Email,
            IsActive = user.IsActive,
            DisplayName = user.DisplayName
        };

        public UserModel ToUserModel(User user)
        {
            if ( user == null ) throw new ArgumentNullException(nameof(user));

            return _compiledProjection(user);
        }

        public User ToUser(UserModel model)
        {
            if ( model == null ) throw new ArgumentNullException(nameof(model));

            return new User
            {
                Id = model.Id,
                UserName = model.UserName,
                PhoneNumber = model.Phone,
                Email = model.Email,
                IsActive = model.IsActive,
                DisplayName = model.DisplayName
            };
        }

        public User ToUser(User user, UserModel model)
        {
            if ( model == null ) throw new ArgumentNullException(nameof(model));
            if ( user == null ) throw new ArgumentNullException(nameof(user));

            user.UserName = model.UserName;
            user.PhoneNumber = model.Phone;
            user.Email = model.Email;
            user.IsActive = model.IsActive;
            user.DisplayName = model.DisplayName;

            return user;
        }
    }
}