namespace Kentico.Xperience.Aira.NavBar;

public class NavBarViewModel
{
    public string LogoImgRelativePath { get; set; } = string.Empty;
    public string TitleText { get; set; } = string.Empty;
    public string TitleImagePath { get; set; } = string.Empty;
    public MenuItemModel ChatItem { get; set; } = new MenuItemModel();
    public MenuItemModel SmartUploadItem { get; set; } = new MenuItemModel();
}

public class MenuItemModel
{
    public string Title { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
