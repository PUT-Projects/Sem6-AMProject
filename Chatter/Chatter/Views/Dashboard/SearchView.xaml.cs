using Chatter.ViewModels.Dashboard;
using System.Windows.Input;

namespace Chatter.Views.Dashboard;

public partial class SearchView : ContentPage
{
    private SearchViewModel _viewModel { get => (SearchViewModel)BindingContext; }
    public SearchView(SearchViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        _viewModel.OnAppearing(searchBar);
    }

    protected override bool OnBackButtonPressed()
    {
        _viewModel.BackCommand.Execute(null);
        return true;
    }
}