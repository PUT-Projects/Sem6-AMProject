using System.ComponentModel;

namespace Chatter.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged
{
#pragma warning disable CS8612 // Obsługa wartości null dla typów referencyjnych w typie jest niezgodna z niejawnie implementowaną składową.
    public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS8612 // Obsługa wartości null dla typów referencyjnych w typie jest niezgodna z niejawnie implementowaną składową.
}
