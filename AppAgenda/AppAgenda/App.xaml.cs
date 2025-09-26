using AppAgenda.Services;

namespace AppAgenda;

public partial class App : Application
{
	public App(IAgendaService agendaService)
	{
		InitializeComponent();

		MainPage = new AppShell();
	}
}
