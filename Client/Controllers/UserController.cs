using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Web.Models;

namespace Client.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private HttpClient _client;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000")
            };
        }


        [HttpPost("Register")]
        public async Task<Guid> Register(UserModel model)
        {
           var response = await _client.PostAsJsonAsync("user/Register", model);
           response.EnsureSuccessStatusCode();
           return await response.Content.ReadAsAsync<Guid>();
        }

        [HttpPost("Login")]
        public async Task Login(UserModel model)
        {
            var response = await _client.PostAsJsonAsync("user/Login", model);
            response.EnsureSuccessStatusCode();
            var cookie = response.Headers.GetValues("Set-Cookie");
            _memoryCache.Set(model.Email + "cookie", cookie);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimsIdentity.DefaultNameClaimType, model.Email),
                new Claim(ClaimTypes.Email, model.Email)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }

        [HttpPost("SetKey"), Authorize]
        public async Task SetKey(string key)
        {
            var user = HttpContext.GetLogin();
            var cookie = _memoryCache.Get<IEnumerable<string>>(user + "cookie");
            _client.DefaultRequestHeaders.Add("Cookie", cookie);
            var response = await _client.PostAsync($"user/SetKey?key={key}", null);
            response.EnsureSuccessStatusCode();

            _memoryCache.Set(user, key);
            _logger.LogInformation($"Пользователь c ником:{user} установил себе ключ:{key}.");
        }
    }
}
