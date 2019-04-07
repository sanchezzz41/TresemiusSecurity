
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Models;

namespace TresemiusSecurity.Server.Domains
{
    /// <summary>
    /// Сервис для работы с юзерами
    /// </summary>
    public class UserService
    {
        private static List<User> _users = new List<User>();

        public UserService()
        {
        }

        public Guid Register(UserModel model)
        {
            var resultUser = new User(model.Email, model.Password);
            _users.Add(resultUser);
            return resultUser.Id;
        }

        public User Login(UserModel model)
        {
            return _users.First(x => x.Email == model.Email && model.Password == x.Password);
        }
    }

    public class User
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        /// <inheritdoc />
        public User(string email, string password)
        {
            Id = Guid.NewGuid();
            Email = email;
            Password = password;
        }
    }
}
