using AppAcademia.Services;

namespace AppAcademia;

public partial class MainPage : TabbedPage
{

	public static DatabaseService Database { get; private set; } = null!;
	public MainPage()
	{
		InitializeComponent();

    IniciaDB(dbpath: Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AppAcademia.db3"));
		AtualizarLista();
	}


	private async void AtualizarLista()
	{
		var exercicios = await Database.GetExercisesAsync();
	}

	private static void IniciaDB(string dbpath)
	{
		Database = new DatabaseService(dbpath);
	}
}

