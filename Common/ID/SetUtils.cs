using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace AQMod.Common.ID
{
    internal static class SetUtils
    {
        public static int Length;
        public static Func<Mod, string, int> GetIDFromType;
        public static Mod CurrentMod;

        public struct SetID
        {
            private int _value;
            private Type _moddedType;
            private string _keyValue;

            public SetID(int value)
            {
                _value = value;
                _moddedType = null;
                _keyValue = null;
            }

            public SetID(Type value)
            {
                _value = -1;
                _moddedType = value;
                _keyValue = null;
            }

            public SetID(string value)
            {
                _value = -1;
                _moddedType = null;
                _keyValue = value;
            }

            public int GetID()
            {
                if (_value == -1)
                {
                    if (_moddedType != null)
                    {
                        _value = GetIDFromType(AQMod.GetInstance(), _moddedType.Name);
                        if (_value == 0)
                        {
                            _value = -1;
                        }
                    }
                    else
                    {
                        try
                        {
                            int position = 0;
                            string modName = AQStringCodes.DecodeModName(AQStringCodes.ExtractParameterText(_keyValue, ref position));
                            var mod = ModLoader.GetMod(modName);
                            if (mod == null)
                            {
                                return -1;
                            }
                            position++;
                            string name = AQStringCodes.ExtractParameterText(_keyValue, ref position);
                            _value = GetIDFromType(mod, name);
                            if (_value == 0)
                            {
                                AQMod.GetInstance().Logger.Warn(modName + "-" + name + " does not have an ID.");
                                return -1;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                return _value;
            }

            public static implicit operator SetID(int value)
            {
                return new SetID(value);
            }

            public static implicit operator SetID(Type value)
            {
                return new SetID(value);
            }

            public static implicit operator SetID(string value)
            {
                return new SetID(value);
            }
        }

        public static bool[] CreateFlagSet(params SetID[] ids)
        {
            var arr = new bool[Length];
            foreach (var id in ids)
            {
                int value = id.GetID();
                if (value != -1)
                    arr[id.GetID()] = true;
            }
            return arr;
        }

        public static List<int> CreateBlacklist(params SetID[] ids)
        {
            var arr = new List<int>();
            foreach (var id in ids)
            {
                int value = id.GetID();
                if (value != -1)
                    arr.Add(value);
            }
            return arr;
        }
    }
}