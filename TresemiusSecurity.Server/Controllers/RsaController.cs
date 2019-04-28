using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Security2.Rsa;

namespace TresemiusSecurity.Server.Controllers
{
    [ApiController, Route("[controller]")]
    public class RsaController : Controller
    {
        private readonly RsaServerKeys _rsaServerKeys;
        private readonly RsaService _rsaService;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<RsaController> _logger;

        public RsaController(RsaServerKeys rsaServerKeys, 
            IMemoryCache memoryCache, 
            RsaService rsaService,
             ILogger<RsaController> logger)
        {
            _rsaServerKeys = rsaServerKeys;
            _memoryCache = memoryCache;
            _rsaService = rsaService;
            _logger = logger;
        }


        /// <summary>
        /// Возврат пуб. ключа
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("RsaPublicKey")]
        public async Task<RsaPublicKey> GetPublicKey()
        {
            return new RsaPublicKey(_rsaServerKeys.PublicKey);
        }

    }
}
