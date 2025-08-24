using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AssetStudio.GUI.Logic;

public static class SearchUtility
{
    public static IEnumerable<T> PerformTextSearch<T>(
        IEnumerable<T> items,
        string searchText,
        SearchMethod searchMethod,
        IncludeExcludeMode includeMode,
        Func<T, string, SearchMethod, bool> matchPredicate)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return items;

        var itemsArray = items as T[] ?? items.ToArray();

        try
        {
            var matchingItems = itemsArray.Where(item => matchPredicate(item, searchText, searchMethod));

            return includeMode == IncludeExcludeMode.Include
                ? matchingItems
                : itemsArray.Except(matchingItems);
        }
        catch (Exception)
        {
            return itemsArray;
        }
    }

    public static bool PerformStringMatch(string target, string searchText, SearchMethod searchMethod)
    {
        if (string.IsNullOrEmpty(target) || string.IsNullOrWhiteSpace(searchText))
            return false;

        return searchMethod switch
        {
            SearchMethod.Exact => string.Equals(target, searchText, StringComparison.OrdinalIgnoreCase),
            SearchMethod.Contains => target.Contains(searchText, StringComparison.OrdinalIgnoreCase),
            SearchMethod.Fuzzy => target.Contains(searchText,
                StringComparison.OrdinalIgnoreCase), // Simple fuzzy for now
            SearchMethod.Regex => Regex.IsMatch(target, searchText, RegexOptions.IgnoreCase),
            _ => target.Contains(searchText, StringComparison.OrdinalIgnoreCase)
        };
    }
}