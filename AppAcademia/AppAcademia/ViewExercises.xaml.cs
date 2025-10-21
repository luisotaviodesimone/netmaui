
namespace AppAcademia;

public partial class ViewExercises : ContentPage
{


	private DateTime _selectedDate = DateTime.Today;

	public ViewExercises()
	{
		InitializeComponent();
		this.Appearing += async (s, e) => await LoadExercisesAsync();
	}

	private async Task LoadExercisesAsync()
	{
		// Set DatePicker date if not set
		if (FilterDatePicker.Date == DateTime.MinValue)
		{
			FilterDatePicker.Date = DateTime.Today;
		}

		_selectedDate = FilterDatePicker.Date.Date;
		var list = await MainPage.Database.GetExercisesByDateAsync(_selectedDate);
		ExercisesListView.ItemsSource = list;

		// Atualiza o contador para a data selecionada
		TodayCountLabel.Text = list.Count.ToString();
	}

	private async void OnDateSelected(object sender, DateChangedEventArgs e)
	{
		_selectedDate = e.NewDate.Date;
		await LoadExercisesAsync();
	}

	private async void OnTodayClicked(object sender, EventArgs e)
	{
		FilterDatePicker.Date = DateTime.Today;
		_selectedDate = DateTime.Today;
		await LoadExercisesAsync();
	}

}