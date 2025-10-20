
namespace AppAcademia;

public partial class ViewExercises : ContentPage
{



	public ViewExercises()
	{
		InitializeComponent();
		this.Appearing += async (s, e) =>
		{
			ExercisesListView.ItemsSource = await MainPage.Database.GetExercisesAsync();
		};
	}

}