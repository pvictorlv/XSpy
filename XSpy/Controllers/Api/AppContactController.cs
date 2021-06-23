using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XSpy.Controllers.Base;
using XSpy.Database.Services;
using XSpy.Utils;

namespace XSpy.Controllers.Api
{
    public class AppContactController : BaseApiController
    {
        private MessageService _messageService;

        public AppContactController(MessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("{contactId}"), PreExecution]
        public async Task<IActionResult> SendSms([FromRoute] Guid contactId)
        {
            return Ok(await _messageService.GetMessagesForContact(contactId));
        }
    }
}