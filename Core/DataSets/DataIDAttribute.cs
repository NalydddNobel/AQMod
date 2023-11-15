using ReLogic.Reflection;
using System;
using System.Reflection;

namespace Aequus.Core.DataSets;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
public sealed class DataIDAttribute : Attribute {
    private readonly Type idSet;

    public DataIDAttribute(Type idSet) {
        this.idSet = idSet;
    }

    public IdDictionary GetIdDictionary() {
        return (IdDictionary)idSet.GetField("Search", BindingFlags.Public | BindingFlags.Static).GetValue(null);
    }
}