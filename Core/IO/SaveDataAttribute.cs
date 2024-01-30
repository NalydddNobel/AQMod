using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader.IO;

namespace Aequus.Core.IO;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class SaveDataAttribute : Attribute {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IsListedBooleanAttribute : Attribute {
    }

    public string Name;

    public SaveDataAttribute(string name) {
        Name = name;
    }

    public bool SaveData(TagCompound tag, object me, MemberInfo info) {
        if (info is FieldInfo field) {
            var f = field.GetValue(me);
            if (f != null) {
                tag[Name] = f;
                return true;
            }
        }
        else if (info is PropertyInfo property) {
            var p = property.GetValue(me);
            if (p != null) {
                tag[Name] = p;
                return true;
            }
        }
        return false;
    }

    public bool LoadData(TagCompound tag, object me, MemberInfo info) {
        if (!tag.ContainsKey(Name)) {
            return false;
        }
        object obj = tag.Get<object>(Name);
        if (obj == null) {
            return false; // wtf tagcompound lies
        }
        //Aequus.Instance.Logger.Debug(Name);
        //Aequus.Instance.Logger.Debug(obj.GetType().FullName + ": " + obj.ToString());
        if (info is FieldInfo field) {
            obj = TagIO.Deserialize(field.FieldType, obj);
            field.SetValue(me, obj);
            //Aequus.Instance.Logger.Debug(field.FieldType + ": " + field.GetValue(me));
        }
        else if (info is PropertyInfo property) {
            obj = TagIO.Deserialize(property.PropertyType, obj);
            property.SetValue(me, obj);
            //Aequus.Instance.Logger.Debug(property.PropertyType + ": " + property.GetValue(me));
        }

        return false;
    }

    public static void SaveData(TagCompound tag, object me) {
        var list = new List<string>();
        foreach (var m in ReflectionHelper.GetMembersWithAttribute<SaveDataAttribute>(me.GetType())) {
            m.attributeInstance.SaveData(tag, me, m.memberInfo);
        }
        if (list.Count > 0) {
            tag["List"] = list;
        }
    }

    public static void LoadData(TagCompound tag, object me) {
        foreach (var m in ReflectionHelper.GetMembersWithAttribute<SaveDataAttribute>(me.GetType())) {
            m.attributeInstance.LoadData(tag, me, m.memberInfo);
        }
    }
}