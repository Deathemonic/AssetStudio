using System;
using System.Collections.Generic;
using System.Linq;
using AssetStudio.GUI.Models.Documents;
using AssetStudio.GUI.Utils;

namespace AssetStudio.GUI.Logic;

public static class AssetSearch
{
    public static IEnumerable<AssetItem> PerformCompleteSearch(
        IEnumerable<AssetItem> assets,
        string searchText,
        SearchMethod searchMethod,
        IncludeExcludeMode includeMode,
        IEnumerable<string> selectedAssetTypes)
    {
        var performCompleteSearch = assets as AssetItem[] ?? assets.ToArray();
        
        try
        {
            var assetItems = assets as AssetItem[] ?? performCompleteSearch.ToArray();
            var selectedTypes = selectedAssetTypes.ToList();

            var typeFilteredAssets = assetItems.AsEnumerable();
            if (selectedTypes.Any() && !selectedTypes.Contains("All"))
                typeFilteredAssets = typeFilteredAssets.Where(asset => selectedTypes.Contains(asset.Type));

            if (string.IsNullOrWhiteSpace(searchText)) return typeFilteredAssets;

            return SearchUtility.PerformTextSearch(
                typeFilteredAssets,
                searchText,
                searchMethod,
                includeMode,
                DoesAssetMatchText);
        }
        catch (Exception)
        {
            return performCompleteSearch;
        }
    }

    private static bool DoesAssetMatchText(AssetItem asset, string searchText, SearchMethod searchMethod)
    {
        return SearchUtility.PerformStringMatch(asset.Name, searchText, searchMethod) ||
               SearchUtility.PerformStringMatch(asset.Container, searchText, searchMethod) ||
               SearchUtility.PerformStringMatch(asset.PathId.ToString(), searchText, searchMethod);
    }
}