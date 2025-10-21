using AppAcademia.Services;

namespace AppAcademia;

public partial class MainPage : TabbedPage
{

	public static DatabaseService Database { get; private set; } = null!;
	public MainPage()
	{
		InitializeComponent();

		var projectDir = Environment.CurrentDirectory;
		var dbPath = Path.Combine(projectDir, "AppAcademia.db3");

		IniciaDB(dbPath);
		AtualizarLista();
	}


	private async void AtualizarLista()
	{
		var exercicios = await Database.GetExercisesAsync();
	}

	private async static void IniciaDB(string dbpath)
	{
		Database = new DatabaseService(dbpath);
		await Database.InitAsync();
	}
}

