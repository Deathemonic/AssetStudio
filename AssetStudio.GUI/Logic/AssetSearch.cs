using System;
using System.Collections.Generic;
using System.Linq;
using AssetStudio.GUI.Models.Documents;

namespace AssetStudio.GUI.Logic;

public static class AssetSearch
{
    public static IEnumerable<AssetItem> FilterAssets(
        IEnumerable<AssetItem> assets,
        string searchText,
        SearchMethod searchMethod,
        IncludeExcludeMode includeMode)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return assets;

        var assetItems = assets as AssetItem[] ?? assets.ToArray();
        var results = assetItems.AsEnumerable();

        switch (searchMethod)
        {
            case SearchMethod.Exact:
                results = results.Where(asset =>
                    string.Equals(asset.Name, searchText, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(asset.Type, searchText, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(asset.Container, searchText, StringComparison.OrdinalIgnoreCase));
                break;

            case SearchMethod.Contains:
                results = results.Where(asset =>
                    asset.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    asset.Type.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    asset.Container.Contains(searchText, StringComparison.OrdinalIgnoreCase));
                break;

            case SearchMethod.Fuzzy:
            case SearchMethod.Regex:
                // Not implemented yet, fall back to contains
                results = results.Where(asset =>
                    asset.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    asset.Type.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    asset.Container.Contains(searchText, StringComparison.OrdinalIgnoreCase));
                break;
        }

        if (includeMode == IncludeExcludeMode.Exclude) results = assetItems.Except(results);

        return results;
    }

    public static bool DoesAssetMatch(AssetItem asset, string searchText, SearchMethod searchMethod)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return true;

        return searchMethod switch
        {
            SearchMethod.Exact =>
                string.Equals(asset.Name, searchText, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(asset.Type, searchText, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(asset.Container, searchText, StringComparison.OrdinalIgnoreCase),

            SearchMethod.Contains =>
                asset.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                asset.Type.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                asset.Container.Contains(searchText, StringComparison.OrdinalIgnoreCase),

            SearchMethod.Fuzzy or SearchMethod.Regex =>
                // Not implemented yet, fall back to contains
                asset.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                asset.Type.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                asset.Container.Contains(searchText, StringComparison.OrdinalIgnoreCase),

            _ => false
        };
    }
}