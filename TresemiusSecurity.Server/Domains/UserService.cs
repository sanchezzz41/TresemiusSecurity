
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
            if (model.Password.Length < 4)
                throw new Exception("Длина пароля должна быть больше 5");
            _users.Add(resultUser);
            return resultUser.Id;
        }

        public User Login(UserModel model)
        {
            var user = _users.First(x => x.Email == model.Email);
            if (ValidatePassword(model.Password, user.Password))
                return user;
            return null;
        }

        /// <summary>
        /// Валидация пароля(по рандому)
        /// </summary>
        /// <param name="inputPassword"></param>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        private bool ValidatePassword(string inputPassword, string userPassword)
        {
            var random = new Random();
            var checkDict = new Dictionary<int, char>();
            for (int i = 0; i < userPassword.Length / 2; i++)
            {
                var index = random.Next(0, userPassword.Length);
                var letter = userPassword[index];
                checkDict.Add(index, letter);
            }

            foreach (var item in checkDict)
            {
                if (inputPassword[item.Key] != item.Value)
                    return false;
            }

            return true;
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
