using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Security2.Rsa;
using TresemiusSecurity.Server.Domains;
using Web.Models;

namespace TresemiusSecurity.Server.Controllers
{
    [ApiController, Route("[controller]")]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        //Тут хранится вся важная инфа(В памяти)
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<UserController> _logger;
        private readonly RsaService _rsaService;
        private readonly RsaServerKeys _rsaServerKeys;



        public UserController(UserService userService, IMemoryCache memoryCache, ILogger<UserController> logger, RsaService rsaService, RsaServerKeys rsaServerKeys)
        {
            _userService = userService;
            _memoryCache = memoryCache;
            _logger = logger;
            _rsaService = rsaService;
            _rsaServerKeys = rsaServerKeys;
        }

        [HttpPost("Register")]
        public async Task<Guid> Register(UserModel model)
        {
            model.Email = _rsaService.Decrypt<string>(model.Email, _rsaServerKeys.PrivateKey);
            model.Password = _rsaService.Decrypt<string>(model.Password, _rsaServerKeys.PrivateKey);
            return await Task.FromResult(_userService.Register(model));
        }

        [HttpPost("Login")]
        public async Task Login(UserModel model)
        {
            model.Email = _rsaService.Decrypt<string>(model.Email, _rsaServerKeys.PrivateKey);
            model.Password = _rsaService.Decrypt<string>(model.Password, _rsaServerKeys.PrivateKey);
            model.Key = _rsaService.Decrypt<string>(model.Key, _rsaServerKeys.PrivateKey);

            var user = _userService.Login(model);
            if (user == null)
                throw new Exception("Такого пользователя не существует.");
            _logger.LogInformation($"Пользователь c email: {model.Email} зашёл на сайт и установил ключ {model.Key}.");

            _memoryCache.Set(model.Email + "key", model.Key);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }

        [HttpDelete("Logout"), Authorize]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync();
        }

        [HttpPost("SetKey")]
        public void SetKey(string key)
        {
            var user = HttpContext.GetLogin();
            _memoryCache.Set(user, key);
            _logger.LogInformation($"Пользователь c ником:{user} установил себе ключ:{key}.");

        }
    }
}
