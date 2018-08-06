using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.MarketMakerReports.Client.Api;
using Lykke.Service.MarketMakerReports.Client.Models;
using Lykke.Service.MarketMakerReports.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.MarketMakerReports.Controllers
{
    [Route("/api/[controller]")]
    public class AuditMessagesController : Controller, IAuditMessagesApi
    {
        private readonly IAuditMessageService _auditMessageService;

        public AuditMessagesController(IAuditMessageService auditMessageService)
        {
            _auditMessageService = auditMessageService;
        }
        
        /// <response code="200">Audit messages</response>
        [HttpGet]
        [SwaggerOperation("AuditMessageGet")]
        [ProducesResponseType(typeof(IReadOnlyList<AuditMessageModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyList<AuditMessageModel>> Get(DateTime? from, DateTime? to, string clientId = null)
        {
            var auditMessages = await _auditMessageService.GetAsync(from, to, clientId);
            var model = Mapper.Map<List<AuditMessageModel>>(auditMessages);
            return model;
        }
    }
}
