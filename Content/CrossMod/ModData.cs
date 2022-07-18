using System;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    [Obsolete("Use Terraria.ModLoader.Mod and the ModSupport class for various functions.")]
    internal struct ModData
    {
        public readonly string Name;
        public readonly string CodeName;
        public Mod Mod { get; private set; }
        public bool Enabled => Mod != null;

        public ModData(string name)
        {
            Name = name;
            CodeName = name;
            Mod = null;
            try
            {
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