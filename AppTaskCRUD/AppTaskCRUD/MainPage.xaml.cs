using AppTaskCRUD.Services;

namespace AppTaskCRUD;

public partial class MainPage : ContentPage
{

	private static TaskDatabase database = new TaskDatabase();
	
	public MainPage()
	{
		InitializeComponent();
		BindingContext = new ViewModels.TaskViewModel();
	}

	private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		if (sender is CheckBox checkBox && checkBox.BindingContext is Models.TaskItem task)
		{
			// print task id
			System.Diagnostics.Debug.WriteLine($"Task ID: {task.Id}, IsDone: {e.Value}");
			task.IsDone = e.Value;
			await database.SaveTaskAsync(task);
		}
	}
}

