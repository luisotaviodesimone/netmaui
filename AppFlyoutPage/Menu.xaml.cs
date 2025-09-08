namespace AppFlyoutPage;

public partial class Menu : ContentPage
{
	public Menu()
	{
		InitializeComponent();
	}

    private void OnButtonClikedPage1(object sender, EventArgs e)
    {
        ((FlyoutPage)App.Current.MainPage).Detail = new NavigationPage(new Page1());
    }                                               
    private void OnButtonClikedPage2(object sender, EventArgs e)
    {                                               
        ((FlyoutPage)App.Current.MainPage).Detail = new NavigationPage(new Page2());
    }
    private void OnButtonClikedPage3(object sender, EventArgs e)
    {
        ((FlyoutPage)App.Current.MainPage).Detail = new NavigationPage(new Page3());
    }
}