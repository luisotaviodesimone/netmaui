using AppAcademia.Models;

namespace AppAcademia;

public partial class CreateExercise : ContentPage
{
	public CreateExercise()
	{
		InitializeComponent();
	}

	private async void OnSaveClicked(object sender, EventArgs e)
	{
		// Validar campos obrigatórios
		if (string.IsNullOrWhiteSpace(ExerciseTypeEntry.Text))
		{
			await ShowFeedback("Por favor, informe o tipo de exercício", isError: true);
			return;
		}

		if (!int.TryParse(RepetitionsEntry.Text, out int repetitions) || repetitions <= 0)
		{
			await ShowFeedback("Por favor, informe um número válido de repetições", isError: true);
			return;
		}

		if (!double.TryParse(LoadEntry.Text, out double load) || load < 0)
		{
			await ShowFeedback("Por favor, informe uma carga válida", isError: true);
			return;
		}

		// Criar novo exercício
		var exercise = new Exercise
		{
			ExerciseType = ExerciseTypeEntry.Text.Trim(),
			Repetitions = repetitions,
			Load = load,
			EquipmentPhotoPath = EquipmentPhotoEntry.Text?.Trim() ?? string.Empty,
			CreatedAt = DateTime.Now,
			UpdatedAt = DateTime.Now
		};

		// Salvar no banco de dados
		await MainPage.Database.SaveExerciseAsync(exercise);

		// Mostrar mensagem de sucesso
		await ShowFeedback("✅ Exercício salvo com sucesso!", isError: false);

		// Limpar campos após 1 segundo
		await Task.Delay(1500);
		ClearFields();
	}

	private void OnCancelClicked(object sender, EventArgs e)
	{
		ClearFields();
		HideFeedback();
	}

	private void ClearFields()
	{
		ExerciseTypeEntry.Text = string.Empty;
		RepetitionsEntry.Text = string.Empty;
		LoadEntry.Text = string.Empty;
		EquipmentPhotoEntry.Text = string.Empty;
		HideFeedback();
	}

	private async Task ShowFeedback(string message, bool isError)
	{
		FeedbackLabel.Text = message;
		var theme = Application.Current?.RequestedTheme ?? AppTheme.Unspecified;
		if (isError)
		{
			FeedbackLabel.TextColor = theme == AppTheme.Dark ? Color.FromArgb("#EF9A9A") : Color.FromArgb("#D32F2F");
			FeedbackLabel.BackgroundColor = theme == AppTheme.Dark ? Color.FromArgb("#4A0F13") : Color.FromArgb("#FFEBEE");
		}
		else
		{
			FeedbackLabel.TextColor = theme == AppTheme.Dark ? Color.FromArgb("#81C784") : Color.FromArgb("#388E3C");
			FeedbackLabel.BackgroundColor = theme == AppTheme.Dark ? Color.FromArgb("#1B5E20") : Color.FromArgb("#E8F5E9");
		}
		FeedbackLabel.IsVisible = true;

		// Auto-ocultar mensagem de erro após 3 segundos
		if (isError)
		{
			await Task.Delay(3000);
			HideFeedback();
		}
	}

	private void HideFeedback()
	{
		FeedbackLabel.IsVisible = false;
	}
}