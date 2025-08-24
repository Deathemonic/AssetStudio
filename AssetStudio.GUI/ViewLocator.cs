using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Core;

namespace AssetStudio.GUI;

public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        if (data == null)
        {
            return new TextBlock { Text = "Null view model" };
        }

        var dataType = data.GetType();
        var name = dataType.FullName!.Replace("ViewModel", "View");
        var type = dataType.Assembly.GetType(name);

        if (type == null) return new TextBlock { Text = "Not Found: " + name };
        
        var instance = (Control)Activator.CreateInstance(type)!;
        return instance;

    }

    public bool Match(object? data)
    {
        return data is ObservableObject or IDockable;
    }
}