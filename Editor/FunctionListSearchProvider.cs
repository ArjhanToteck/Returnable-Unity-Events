using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FunctionListSearchProvider : ScriptableObject, ISearchWindowProvider
{
    public List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        return searchList;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        return true;
    }
}