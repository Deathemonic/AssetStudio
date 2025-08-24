using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using AssetStudio.GUI.Logic;
using AssetStudio.GUI.Models.Documents;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;

namespace AssetStudio.GUI.ViewModels.Documents;

public partial class ClassListDocumentViewModel : Document
{
    [ObservableProperty] private DataGridCollectionView? _collectionView;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private int _selectedIncludeExcludeMode;
    [ObservableProperty] private int _selectedSearchFilterMode;

    public ClassListDocumentViewModel()
    {
        Id = "ClassList";
        Title = "Classes";
        CanClose = false;
        RefreshView();

        PropertyChanged += OnPropertyChanged;
    }

    private ObservableCollection<ClassItem> Classes { get; } = [];

    public void UpdateData(List<ClassItem> newClasses)
    {
        Classes.Clear();
        foreach (var classItem in newClasses.OrderBy(c => c.Id))
        {
            Classes.Add(classItem);
        }

        RefreshView();
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(SearchText):
            case nameof(SelectedSearchFilterMode):
            case nameof(SelectedIncludeExcludeMode):
                RefreshView();
                break;
        }
    }


    private void RefreshView()
    {
        var filteredClasses = ClassSearch.PerformSearch(
            Classes,
            SearchText,
            (SearchMethod)SelectedSearchFilterMode,
            (IncludeExcludeMode)SelectedIncludeExcludeMode);

        CollectionView = new DataGridCollectionView(new ObservableCollection<ClassItem>(filteredClasses));
    }
}