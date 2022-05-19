using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader.IO;

namespace Aequus.Common
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SaveDataAttribute : Attribute
    {
        public string Name;

        public SaveDataAttribute(string name)
        {
            Name = name;
        }

        public bool SaveData(TagCompound tag, object me, MemberInfo info)
        {
            if (info is FieldInfo field)
            {
                var f = field.GetValue(me);
                if (f != null)
                {
                    tag[Name] = f;
                    return true;
                }
            }
            else if (info is PropertyInfo property)
            {
                var p = property.GetValue(me);
                if (p != null)
                {
                    tag[Name] = p;
                    return true;
                }
            }
            return false;
        }

        public bool LoadData(TagCompound tag, object me, MemberInfo info)
        {
            if (!tag.ContainsKey(Name))
            {
                return false;
            }
            object obj = tag.Get<object>(Name);
            if (obj == null) // wtf tagcompound lies
            {
                return false;
            }
            //Aequus.Instance.Logger.Debug(Name);
            //Aequus.Instance.Logger.Debug(obj.GetType().FullName + ": " + obj.ToString());
            if (info is FieldInfo field)
            {
                obj = TagIO.Deserialize(field.FieldType, obj);
                field.SetValue(me, obj);
                //Aequus.Instance.Logger.Debug(field.FieldType + ": " + field.GetValue(me));
            }
            else if (info is PropertyInfo property)
            {
                obj = TagIO.Deserialize(property.PropertyType, obj);
                property.SetValue(me, obj);
                //Aequus.Instance.Logger.Debug(property.PropertyType + ": " + property.GetValue(me));
            }

            return false;
        }

        public static void SaveData(TagCompound tag, object me)
        {
            foreach (var m in AequusHelpers.GetFieldsPropertiesOfAttribute<SaveDataAttribute>(me.GetType()))
            {
                m.attr.SaveData(tag, me, m.info);
            }
        }

        public static void LoadData(TagCompound tag, object me)
        {
            foreach (var m in AequusHelpers.GetFieldsPropertiesOfAttribute<SaveDataAttribute>(me.GetType()))
            {
                m.attr.LoadData(tag, me, m.info);
            }
        }
    }
}