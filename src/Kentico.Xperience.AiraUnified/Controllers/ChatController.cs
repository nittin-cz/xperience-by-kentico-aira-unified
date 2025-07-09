using Kentico.Xperience.AiraUnified.Models;

using Microsoft.AspNetCore.Mvc;

namespace Kentico.Xperience.AiraUnified.Controllers;

public class ChatController : Controller
{
    public IActionResult Index()
    {
        // if (!_chatConfig.IsChatEnabled())
        // {
        //     return NotFound();
        // }

         var model = new ChatViewModel
         {
             //SessionId = HttpContext.Session.Id,
             SessionId = Guid.NewGuid().ToString()
             //Configuration = _chatConfig
         };

        return View(model);
    }
}
