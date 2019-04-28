using System.Collections.Generic;
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

        public static List<MessageInfo> _list = new List<MessageInfo>();

        public MessageController(IMemoryCache memoryCache, ILogger<MessageController> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        [HttpPost]
        public async Task<string> CreateMessage(MessageInfo model)
        {
            _logger.LogInformation($"Пришедшие данные:{model.Text}/{model.Date}");
            var key = _memoryCache.Get<string>(HttpContext.GetLogin() + "key");
            _logger.LogInformation($"Ключ пользователя {HttpContext.GetLogin()}: {key}");

            var text = TresCipher.Decrypt(model.Text, key);
            var date = TresCipher.Decrypt(model.Date, key);
            model.Text = text;
            model.Date = date;
            _logger.LogInformation($"Дешифрованный текст: {text}");
            _logger.LogInformation($"Дешифрованный дата: {date}");
            _list.Add(model);
            return "OK";
        }

        [HttpGet]
        public async Task<List<MessageInfo>> GetTestMessage()
        {
            var key = _memoryCache.Get<string>(HttpContext.GetLogin() + "key");
            _logger.LogInformation($"Ключ пользователя {HttpContext.GetLogin()}: {key}");
            var returnedList = new List<MessageInfo>();
            foreach (var item in _list)
            {
                returnedList.Add(new MessageInfo
                {
                    Date = TresCipher.Encrypt(item.Date, key),
                    Text = TresCipher.Encrypt(item.Text, key)

                });
            }
            return returnedList;
        }
    }
}
