using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocNetwork.Models.Db;
using SocNetwork.Models.Service;
using SocNetwork.Models.ViewModel;

namespace SocNetwork.Controllers
{
    [Authorize]
    [Route("Message")]
    public class MessageController:Controller
    {
        private readonly IMessageService _messageService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        public MessageController(IMessageService messageService, UserManager<User> userManager, IMapper mapper)
        {
            _messageService = messageService;
            _userManager = userManager;
            _mapper = mapper;
        }


        [HttpGet("{friendId}")]
        public async Task<IActionResult> Chat(string friendId)
        {
            var currentUserId = _userManager.GetUserId(User);

            var messages = await _messageService.GetConversationAsync(currentUserId, friendId);
            var friend = await _userManager.FindByIdAsync(friendId);
            if (friend == null)
            {
                return NotFound();
            }
            var friendVm = _mapper.Map<UserViewModel>(friend);
            var vm = new ChatViewModel
            {
                Friend = new UserViewModel
                {
                    Id = friend.Id,
                    FullName = friend.FirstName,
                    Image = friend.Image,
                    Email = friend.Email,
                    Status = friend.Status
                },
                Messages = messages,
                CurrentUserId = currentUserId
            };
            return View(vm);

        }


        [HttpPost("Send")] 
        public async Task<IActionResult> Send([FromBody] SendMessageRequest request)
        {
            var senderId = _userManager.GetUserId(User);
            await _messageService.SendMessageAsync(senderId, request.ReceiverId,request.Text);
            return Ok(new { message = "Сообщение отправлено!" });
        }
    }
  
}
