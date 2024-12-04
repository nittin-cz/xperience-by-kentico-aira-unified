using HotChocolate.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace Kentico.Xperience.Aira;

[ApiController]
[Route("[controller]/[action]")]
public sealed class AiraCompanionAppController : Controller
{
    [Route("/aira")]
    [AllowAnonymous]
    public async Task<IActionResult> Index() => throw new NotImplementedException();
}
