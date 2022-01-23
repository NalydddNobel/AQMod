using System;
using System.Collections.Generic;
using System.Reflection;

namespace AQMod.Common.CrossMod
{
    internal class CallSpecialAttribute : Attribute
    {
        public Type type;
        private readonly string NameOverride;

        public CallSpecialAttribute(string name = null)
        {
            NameOverride = name;
        }

        public virtual string Name => (NameOverride ?? type.Name).ToLower();
        public virtual FieldInfo[] Fields => type.GetFields();
        public virtual bool AddCallsForFields => true;
        public virtual void CustomCreateCalls(object instance, Dictionary<string, Func<object[], object>> callList)
        {
        }
    }
}