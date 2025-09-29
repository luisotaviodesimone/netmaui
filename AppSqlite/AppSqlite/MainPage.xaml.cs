using System;
using AppSqlite.Services;
using AppSqlite.Models;

namespace AppSqlite;

public partial class MainPage : ContentPage
{
	public static DatabaseService Database { get; private set; }
	public MainPage()
	{
		InitializeComponent();
		string dbpath = Path.Combine(FileSystem.AppDataDirectory, "tarefas.db3");

		IniciaDB(dbpath);
		AtualizarLista();
	}

	private async void OnAdicionarClicked(object sender, EventArgs e)
	{
		var tarefa = new Tarefa
		{
			Nome = EntryTarefa.Text,
			Concluida = ConcluidaTarefa.IsChecked
		};

		await Database.SaveTarefaAsync(tarefa);
		EntryTarefa.Text = string.Empty;
		AtualizarLista();

	}

	private async void AtualizarLista()
	{
		var tarefas = await Database.GetTarefasAsync();
		ListViewTarefas.ItemsSource = tarefas;
	}

	private void IniciaDB(string dbpath)
	{
		Database = new DatabaseService(dbpath);
	}

}

