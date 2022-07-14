using ReLogic.Reflection;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Common.Utilities
{
    internal sealed class DatabaseBuilder<TValue>
    {
        private readonly Dictionary<int, TValue> Dict;
        private readonly Mod Mod;
        private readonly IdDictionary IDs;

        public DatabaseBuilder(Dictionary<int, TValue> dict, Mod mod, IdDictionary id)
        {
            Dict = dict;
            Mod = mod;
            IDs = id;
        }

        public DatabaseBuilder<TValue> TryAddModEntry(string name, TValue value)
        {
            if (IDs.TryGetId(Mod.Name + "/" + name, out int modID))
            {
                try
                {
                    Dict.Add(modID, value);
                }
                catch
                {
                    string valueText = value == null ? "Null" : value.ToString();
                    Aequus.Instance.Logger.Error(Mod.Name + "/" + name + " already exists with " + valueText);
                }
                return this;
            }
            Aequus.Instance.Logger.Error(Mod.Name + " does not contain " + name);
            return this;
        }
    }
}