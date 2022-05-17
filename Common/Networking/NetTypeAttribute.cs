using Aequus.Common.Utilities;
using System;
using System.IO;
using System.Reflection;

namespace Aequus.Common.Networking
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public abstract class NetTypeAttribute : Attribute
    {
        public abstract void Send(BinaryWriter writer, object value);
        public abstract object Read(BinaryReader reader);

        public static void SendData(BinaryWriter writer, object me)
        {
            foreach (var m in AequusHelpers.GetFieldsPropertiesOfAttribute<NetTypeAttribute>(me.GetType()))
            {
                if (m.info is FieldInfo field)
                {
                    m.attr.Send(writer, field.GetValue(me));
                }
                else if (m.info is PropertyInfo property)
                {
                    m.attr.Send(writer, property.GetValue(me));
                }
            }
        }

        public static void ReadData(BinaryReader reader, object me)
        {
            foreach (var m in AequusHelpers.GetFieldsPropertiesOfAttribute<NetTypeAttribute>(me.GetType()))
            {
                //Aequus.Instance.Logger.Debug("Setting " + m.info.Name);
                if (m.info is FieldInfo field)
                {
                    field.SetValue(me, m.attr.Read(reader));
                }
                else if (m.info is PropertyInfo property)
                {
                    property.SetValue(me, m.attr.Read(reader));
                }
            }
        }
    }
}