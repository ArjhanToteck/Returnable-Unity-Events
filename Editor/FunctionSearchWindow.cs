using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FunctionSearchWindow : ScriptableObject, ISearchWindowProvider
{
    public List<SearchTreeEntry> searchTree = new List<SearchTreeEntry>() {
        new SearchTreeGroupEntry(new GUIContent("Functions"))
    };

    public void AddGroup(string groupName, MethodInfo[] functions, int level = 1)
    {
        // add group entry
        searchTree.Add(new SearchTreeGroupEntry(new GUIContent(groupName), level));
        
        // add children
        for (int i = 0; i < functions.Length; i++)
        {
            if (functions[i] == null)
            {
                continue;
            }
            
            searchTree.Add( new SearchTreeEntry(new GUIContent(functions[i].Name))
            {
                userData = functions[i],
                level = level + 1
            });
        }
    }
    
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        return searchTree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        return true;
    }
}