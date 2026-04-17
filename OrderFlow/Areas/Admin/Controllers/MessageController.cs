using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OrderFlow.Data.Models;
using OrderFlow.Hubs;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Message;

namespace OrderFlow.Areas.Admin.Controllers
{
    public class MessageController : BaseAdminController
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly ILogger<MessageController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessageController(IMessageService messageService,
                                 IHubContext<MessageHub> hubContext,
                                 ILogger<MessageController> logger,
                                 UserManager<ApplicationUser> userManager)
        {
            _messageService = messageService;
            _hubContext = hubContext;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateNotificationMessageViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                if (!Guid.TryParse(this.GetUserId(), out Guid senderID))
                    return Unauthorized();

                model.SenderID = senderID;

                Message newMessage = await _messageService.CreateMessageAsync(model);
                if (newMessage == null)
                    return Forbid();

                string senderName = (await _userManager.FindByIdAsync(model.SenderID.ToString() ?? string.Empty))?.UserName ?? string.Empty;

                await _hubContext.Clients
                    .Group(model.NotificationID.ToString() ?? string.Empty)
                    .SendAsync("MessageAdded", new DetailsNotificationMessageViewModel
                    {
                        MessageID = newMessage.MessageID,
                        SenderID = senderID,
                        SenderName = senderName,
                        Content = newMessage.Content,
                        SentAt = newMessage.SentAt,
                        IsRead = newMessage.IsRead,
                    });

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating message.");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateNotificationMessageViewModel model, string messageId)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (string.IsNullOrEmpty(messageId) || !Guid.TryParse(messageId, out Guid messageID))
                return BadRequest("Valid Message ID is required.");

            if (!Guid.TryParse(this.GetUserId(), out Guid senderID))
                return Unauthorized();

            model.SenderID = senderID;

            try
            {

                Message updatedMessage = await _messageService.UpdateMessageAsync(model, messageID, senderID);

                if (updatedMessage == null)
                    return Unauthorized("You can only edit your own messages.");

                string senderName = (await _userManager.FindByIdAsync(model.SenderID.ToString() ?? string.Empty))?.UserName ?? string.Empty;

                await _hubContext.Clients
                    .Group(model.NotificationID.ToString() ?? string.Empty)
                    .SendAsync("MessageEdited", new DetailsNotificationMessageViewModel
                    {
                        MessageID = updatedMessage.MessageID,
                        SenderID = senderID,
                        SenderName = senderName,
                        Content = updatedMessage.Content,
                        SentAt = updatedMessage.SentAt,
                        IsRead = updatedMessage.IsRead,
                    });

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing message.");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, Guid notificationID)
        {
            if (!Guid.TryParse(this.GetUserId(), out Guid senderID))
                return Unauthorized();
            try
            {

                bool success = await _messageService.DeleteMessageAsync(id, senderID);
                if (!success)
                    return Forbid();

                await _hubContext.Clients
                    .Group(notificationID.ToString())
                    .SendAsync("MessageDeleted", id);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting message.");
                return StatusCode(500);
            }
        }
    }
}
