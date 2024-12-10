using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CMS.DataEngine;
using CMS.Membership;
using Kentico.Membership;
using Kentico.Membership.Internal;
using Kentico.Xperience.Aira.Models;

namespace Kentico.Xperience.Aira.Widgets;

public class RegistrationWidgetViewComponent(
        AdminUserManager adminUserManager,
        IUserInfoProvider userInfoProvider,
        UserManager<ApplicationUser> userManager,
        IInfoProvider<RoleInfo> roleInfoProvider,
        IUserManagementService userManagementService
        ) : ViewComponent
{
    private readonly AdminUserManager adminUserManager = adminUserManager;
    private readonly IUserInfoProvider userInfoProvider = userInfoProvider;
    private readonly UserManager<ApplicationUser> userManager = userManager;
    private readonly IInfoProvider<RoleInfo> roleInfoProvider = roleInfoProvider;
    private readonly IUserManagementService userManagementService = userManagementService;

    public IViewComponentResult Invoke(RegistrationWidgetProperties properties)
    {
        properties.Model ??= new RegistrationViewModel();
        return View("~/Features/Registration/_RegistrationWidget.cshtml", properties);
    }
}
