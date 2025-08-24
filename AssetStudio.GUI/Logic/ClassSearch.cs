using System;
using System.Collections.Generic;
using System.Linq;
using AssetStudio.GUI.Models.Documents;
using AssetStudio.GUI.Utils;

namespace AssetStudio.GUI.Logic;

public static class ClassSearch
{
    public static IEnumerable<ClassItem> PerformSearch(
        IEnumerable<ClassItem> classes,
        string searchText,
        SearchMethod searchMethod,
        IncludeExcludeMode includeMode)
    {
        var performSearch = classes as ClassItem[] ?? classes.ToArray();
        
        try
        {
            return SearchUtility.PerformTextSearch(
                performSearch,
                searchText,
                searchMethod,
                includeMode,
                DoesClassMatchText);
        }
        catch (Exception)
        {
            return performSearch;
        }
    }

    private static bool DoesClassMatchText(ClassItem classItem, string searchText, SearchMethod searchMethod)
    {
        return SearchUtility.PerformStringMatch(classItem.ClassName, searchText, searchMethod) ||
               SearchUtility.PerformStringMatch(classItem.Id.ToString(), searchText, searchMethod);
    }
}