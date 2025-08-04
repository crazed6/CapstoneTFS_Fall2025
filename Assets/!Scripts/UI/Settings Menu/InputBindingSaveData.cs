using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic; // Added to resolve 'List' and 'BindingWrapper'

[System.Serializable]
public class InputBindingSaveData
{
    public List<BindingWrapper> bindings = new List<BindingWrapper>(); // Fixed declaration

}

// Assuming BindingWrapper is a class or struct, define it if missing
[System.Serializable]
public class BindingWrapper
{
    // Add properties or fields as needed
    public string actionName;
    public int bindingIndex;
    public string overridePath;
}
