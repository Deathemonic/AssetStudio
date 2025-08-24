using System;
using System.Collections.Generic;
using AssetStudio.GUI.Models.Panels;

namespace AssetStudio.GUI.Logic;

public static class TreeNodeSearch
{
    public static bool ExpandToMatches(IEnumerable<TreeNodeItem> rootItems, string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return false;

        var foundAny = false;

        foreach (var rootItem in rootItems)
            if (ExpandNodeToMatches(rootItem, searchText))
                foundAny = true;

        return foundAny;
    }

    private static bool ExpandNodeToMatches(TreeNodeItem node, string searchText)
    {
        var nodeMatches = DoesNodeMatch(node, searchText);
        var hasMatchingDescendants = false;

        foreach (var child in node.Children)
            if (ExpandNodeToMatches(child, searchText))
                hasMatchingDescendants = true;

        if (!nodeMatches && !hasMatchingDescendants) return false;
        node.IsExpanded = true;
        return true;

    }

    public static bool DoesNodeMatch(TreeNodeItem node, string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return true;

        return node.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }
}