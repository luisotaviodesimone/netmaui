using AppAgenda.MVVM.Views;
using AppAgenda.Services;

namespace AppAgenda;

public partial class App : Application
{
	public App(IAgendaService agendaService)
	{
		InitializeComponent();

		MainPage = new NavigationPage(new AgendaView(agendaService));
	}
}
