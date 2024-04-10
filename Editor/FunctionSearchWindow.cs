using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FunctionSearchWindow : ScriptableObject, ISearchWindowProvider
{
    public Action<SearchTreeEntry> onSelectEntryCallback;

    public List<SearchTreeEntry> searchTree = new List<SearchTreeEntry>();

    public void AddGroup(string groupName, List<MethodInfo> functions, int level = 1)
    {
        // add group entry
        searchTree.Add(new SearchTreeGroupEntry(new GUIContent(groupName), level));

        // add children
        foreach (MethodInfo function in functions)
        {
            if (function == null)
            {
                continue;
            }

            searchTree.Add(new SearchTreeEntry(new GUIContent(function.Name))
            {
                userData = function.Name,
                level = level + 1
            });
        }
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        return searchTree;
    }

    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        onSelectEntryCallback.Invoke(searchTreeEntry);

        return true;
    }
}