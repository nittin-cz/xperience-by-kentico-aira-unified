using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Aira.Models;

namespace Kentico.Xperience.Aira.Widgets;

public class RegistrationWidgetProperties : IWidgetProperties
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public RegistrationViewModel Model { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}
