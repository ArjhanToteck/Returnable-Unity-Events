using System;
using System.Collections.Generic;
using System.Linq;
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

            // get method name
            string functionLabel = function.Name + " ( ";

            // add parameters to function label
            List<ParameterInfo> parameters = function.GetParameters().ToList();
            List<string> parameterNames = parameters.Select(parameter => parameter.ParameterType.Name).ToList();
            functionLabel += string.Join(", ", parameterNames);
            functionLabel += " )";

            searchTree.Add(new SearchTreeEntry(new GUIContent(functionLabel))
            {
                userData = function,
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