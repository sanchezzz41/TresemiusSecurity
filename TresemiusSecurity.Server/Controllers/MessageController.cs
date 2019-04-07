using System.Threading.Tasks;
using Cipher;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Web.Models;

namespace TresemiusSecurity.Server.Controllers
{
    [ApiController, Route("[controller]")]
    public class MessageController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<MessageController> _logger;

        public MessageController(IMemoryCache memoryCache, ILogger<MessageController> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        [HttpPost]
        public async Task<string> CreateMessage(MessageInfo model)
        {
            _logger.LogInformation($"Пришедшие данные:{model.Text}/{model.Date}");
            var key = _memoryCache.GetKey2Laba(HttpContext.GetLogin());
            _logger.LogInformation($"Ключ пользователя {HttpContext.GetLogin()}: {key}");

            var text = TresCipher.Decrypt(model.Text, key);
            var date = TresCipher.Decrypt(model.Date, key);
            _logger.LogInformation($"Дешифрованный текст: {text}");
            _logger.LogInformation($"Дешифрованный дата: {date}");

            return "OK";
        }

        [HttpGet]
        public async Task<MessageInfo> GetTestMessage()
        {
            var key = _memoryCache.GetKey2Laba(HttpContext.GetLogin());
            _logger.LogInformation($"Ключ пользователя {HttpContext.GetLogin()}: {key}");
            var model = new MessageInfo("Test Test text");
            model.Text = TresCipher.Encrypt(model.Text, key);
            model.Date = TresCipher.Encrypt(model.Date, key);
            _logger.LogInformation($"Зашифрованный текст: {model.Text}");
            _logger.LogInformation($"Зашифрованная дата: {model.Date}");

            return model;
        }
    }
}
