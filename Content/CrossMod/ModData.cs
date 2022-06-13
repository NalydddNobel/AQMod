using System;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal struct ModData
    {
        public readonly string Name;
        public readonly string CodeName;
        public Mod Mod { get; private set; }
        public bool Enabled => Mod != null;

        public static ModData Unloaded => new ModData();

        public ModData(string name)
        {
            Name = name;
            CodeName = name;
            Mod = null;
            try
            {
                if (ModLoader.TryGetMod(name, out var modValue))
                {
                    Mod = modValue;
                }
            }
            catch (Exception ex)
            {
                Aequus.Instance.Logger.Error("Mod failed to be checked active: " + name, ex);
            }
        }

        public object Call(params object[] args)
        {
            return Mod.Call(args);
        }

        public static implicit operator Mod(ModData data)
        {
            return data.Mod;
        }

        public void Clear()
        {
            Mod = null;
        }
    }
}