using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Cipher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Web.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Client.Controllers
{

    [ApiController, Route("[controller]"), Authorize]
    public class MessageController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<MessageController> _logger;
        private HttpClient _client;


        public MessageController(IMemoryCache memoryCache, ILogger<MessageController> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000")
            };
        }

        [HttpPost]
        public async Task<string> CreateMessage(MessageInfo model)
        {
            var user = HttpContext.GetLogin();
            var cookie = _memoryCache.Get<IEnumerable<string>>(user + "cookie");
            _client.DefaultRequestHeaders.Add("Cookie", cookie);


            _logger.LogInformation($"Пришедшие данные:{model.Text}/{model.Date}");
            var key = _memoryCache.GetKey2Laba(user);
            _logger.LogInformation($"Ключ пользователя {user}: {key}");

            model.Text = TresCipher.Encrypt(model.Text, key);
            model.Date = TresCipher.Encrypt(model.Date, key);
            _logger.LogInformation($"Зашифрованный текст: {model.Text}");
            _logger.LogInformation($"Зашифрованная дата: {model.Date}");
          
            var response = await _client.PostAsJsonAsync("message", model);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        [HttpGet]
        public async Task<MessageInfo> GetTestMessage()
        {
            var user = HttpContext.GetLogin();
            var cookie = _memoryCache.Get<IEnumerable<string>>(user + "cookie");
            _client.DefaultRequestHeaders.Add("Cookie", cookie);


            var response = await _client.GetAsync("message");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadAsAsync<MessageInfo>();
            var key = _memoryCache.GetKey2Laba(HttpContext.GetLogin());
            _logger.LogInformation($"Ключ пользователя {HttpContext.GetLogin()}: {key}");
            model.Text = TresCipher.Decrypt(model.Text, key);
            model.Date = TresCipher.Decrypt(model.Date, key);
            _logger.LogInformation($"Дешифрованный текст: {model.Text}");
            _logger.LogInformation($"Дешифрованная дата: {model.Date}");

            return model;
        }
    }
}
