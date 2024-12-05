using HotChocolate.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace Kentico.Xperience.Aira;

[ApiController]
[Route("[controller]/[action]")]
public sealed class AiraCompanionAppController : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index() =>
        Ok();
}
