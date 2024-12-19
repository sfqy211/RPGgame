using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField]private int bassValue;
    public List<int> modifiers; // 修改器
    public int GetValue()
    {
        int finalValue = bassValue;

        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }
        
        return finalValue;
    }

    public void SetDefaultValue(int _value)
    {
        bassValue = _value;
    }

    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }

    public void RemoveModifier(int _modifier)
    {
        modifiers.Remove(_modifier);
    }
}
