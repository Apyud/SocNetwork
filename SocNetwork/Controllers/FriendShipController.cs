using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocNetwork.Models.Db;
using SocNetwork.Models.Repository;
using SocNetwork.Models.Service;
using SocNetwork.Models.ViewModel;

namespace SocNetwork.Controllers
{
    [Authorize]
    [Route("FriendShip")]
    public class FriendShipController : Controller
    {
        private readonly IFriendShipService _friendShipService;
        private readonly UserManager<User> _userManager;

        public FriendShipController(IFriendShipService friendShipService, UserManager<User> userManager)
        {
            _friendShipService = friendShipService;
            _userManager = userManager;
        }

        //  Отправка заявки
        [HttpPost("send")]
        public async Task<IActionResult> SendRequest(string friendId)
        {
            if (string.IsNullOrWhiteSpace(friendId))
                return BadRequest("Некорректный идентификатор пользователя.");

            var userId = _userManager.GetUserId(User);

            try
            {
                await _friendShipService.SendFriendRequestAsync(userId, friendId);
                return Ok("Заявка успешно отправлена.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        // Главная страница друзей
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);

            // получаем друзей
            var friends = await _friendShipService.GetFriendsAsync(currentUserId);
            // получаем заявки
            var pending = await _friendShipService.GetPendingRequestsAsync(currentUserId);

            // разделяем входящие и исходящие заявки
            var incoming = pending.Where(u => u.IsPendingRequestReceived).ToList();
            var outgoing = pending.Where(u => u.IsPendingRequestSent).ToList();

            // формируем модель, которую ждёт Razor
            var vm = new FriendListViewModel
            {
                Friends = friends.ToList(),
                IncomingRequests = incoming,
                SentRequests = outgoing
            };

            return View(vm);
        }

        //  Принять заявку
        [HttpPost("AcceptFriendRequest")] // вернуть на accept
        public async Task<IActionResult> AcceptFriend(string requestId)
        {
            if (string.IsNullOrWhiteSpace(requestId))
                return BadRequest("Некорректный идентификатор пользователя.");

            var currentUserId = _userManager.GetUserId(User);

            try
            {
                await _friendShipService.AcceptFriendRequestAsync(requestId, currentUserId);
                return Ok("Заявка успешно принята.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        //  Отклонить заявку
        [HttpPost("decline")]
        public async Task<IActionResult> DeclineFriend(string requestId)
        {
            if (string.IsNullOrWhiteSpace(requestId))
                return BadRequest("Некорректный идентификатор пользователя.");

            var currentUserId = _userManager.GetUserId(User);

            try
            {
                await _friendShipService.DeclineFriendRequestAsync(requestId, currentUserId);
                return Ok("Заявка отклонена.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        //  Получить список друзей
        [HttpGet("friends")]
        public async Task<IActionResult> GetFriends()
        {
            var userId = _userManager.GetUserId(User);
            var friends = await _friendShipService.GetFriendsAsync(userId);
            return Ok(friends);
        }

        //  Получить ожидающие заявки
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var userId = _userManager.GetUserId(User);
            var pending = await _friendShipService.GetPendingRequestsAsync(userId);
            return Ok(pending);
        }

        //  Удалить друга
        [HttpPost("removeFriend")]
        public async Task<IActionResult> UnFriend(string friendId)
        {
            if (string.IsNullOrWhiteSpace(friendId))
                return BadRequest("Некорректный идентификатор друга.");

            var userId = _userManager.GetUserId(User);
            if (userId == friendId)
                return BadRequest("Нельзя удалить самого себя.");

            try
            {
                await _friendShipService.UnfriendAsync(userId, friendId);
                return Ok("Пользователь успешно удалён из друзей.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Дружба не найдена.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }


        
    }
}
