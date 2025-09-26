namespace AppAgenda.MVVM.Views;

using AppAgenda.MVVM.ViewModels;
using AppAgenda.Services;

public partial class AgendaView : ContentPage
{
	private readonly IAgendaService _agendaService;

	public AgendaView(IAgendaService agendaService)
	{
		_agendaService = agendaService;
		InitializeComponent();
		BindingContext = new AgendaViewModel(_agendaService);
	}
}