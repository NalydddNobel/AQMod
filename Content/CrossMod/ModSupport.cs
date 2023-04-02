using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal class ModSupport<TMod> : ModSystem where TMod : ModSupport<TMod>
    {
        public static Mod Instance { get; private set; }
        public static string ModName => typeof(TMod).Name;

        public static bool IsLoadingEnabled()
        {
            return ModLoader.HasMod(ModName);
        }

        public static bool AddItemToSet(string name, HashSet<int> hashSet) {
            if (!TryFind<ModItem>(name, out var modItem)) {
                return false;
            }

            hashSet.Add(modItem.Type);
            return true;
        }

        public static int GetItem(string name, int defaultItem = 0) {
            return TryFind<ModItem>(name, out var value) ? value.Type : defaultItem;
        }

        public static bool TryFind<T>(string name, out T value) where T : IModType
        {
            if (Instance == null)
            {
                value = default(T);
                return false;
            }
            return Instance.TryFind(name, out value);
        }

        public static object Call(params object[] args)
        {
            return Instance?.Call(args);
        }
        public static bool TryCall<T>(out T value, params object[] args)
        {
            var callValue = Call(args);
            if (callValue is T output) {
                value = output;
                return true;
            }
            value = default(T);
            return false;
        }

        public override bool IsLoadingEnabled(Mod mod)
        {
            //mod.Logger.Debug($"{ModName} is {(ModLoader.HasMod(ModName) ? "Enabled" : "Disabled")}");
            return IsLoadingEnabled();
        }

        public sealed override void Load()
        {
            Instance = null;
            if (ModLoader.TryGetMod(ModName, out var mod))
            {
                Instance = mod;
                SafeLoad(Instance);
            }
        }

        public virtual void SafeLoad(Mod mod)
        {

        }

        public sealed override void Unload()
        {
            SafeUnload();
            Instance = null;
        }

        public virtual void SafeUnload()
        {

        }
    }
}
