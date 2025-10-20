using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocNetwork.Models.Db;
using SocNetwork.Models.Service;

namespace SocNetwork.Controllers
{
    [Authorize]
    [Route("FriendShip")]
    public class FriendShipController: Controller
    {
        private readonly IFriendShipService _friendShipService;
        private readonly UserManager<User> _userManager;

        public FriendShipController(IFriendShipService friendShipService, UserManager<User> userManager)
        {
            _friendShipService = friendShipService;
            _userManager = userManager;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendRequest(string friendId)
        {
            Console.WriteLine($"📨 Friend request sent to: {friendId}");
            var userId =  _userManager.GetUserId(User);
            await _friendShipService.SendFriendrequestAsync(userId, friendId);
            return Ok();
        }


        [HttpPost("accept")]
        public async Task<IActionResult> AcceptFriend(string requestId )
        {
            if (string.IsNullOrWhiteSpace(requestId)) return BadRequest("Некорректный идентификатор заявки.");

            try
            {
                if (int.TryParse(requestId, out var id)) // для перевода из стринга URL в int
                {
                    await _friendShipService.AcceptFriendRequestAsync(id);
                    return Ok();
                }
                else
                {
                    return BadRequest("Неккоректный ID");
                }
               
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(KeyNotFoundException)
            {
                return NotFound("Заявка не найдена.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Произошла ошибка при обработке запроса.");
            }


            
            
            
            
            
            
         
        }
    }
}
