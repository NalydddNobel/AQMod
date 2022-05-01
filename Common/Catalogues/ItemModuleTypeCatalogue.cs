using Aequus.Common.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Common.Catalogues
{
    public class ItemModuleTypeCatalogue : ILoadable
    {
        public const int BarbHook = 0;
        public const int BarbChain = 1;
        public const int BarbMisc = 2;
        public const int MaxModuleTypes = 3;

        public static Dictionary<int, SpriteFrameData> TypeToTexture { get; private set; }

        private static int reservedID;

        public static int Count => reservedID;

        /// <summary>
        /// Gives you an ID to classify a specific item module type
        /// <para>
        /// It may be confusing to wrap around your head what this ID is used for, but this example should help:
        /// <code>public static int MyCustomModuleType;</code>
        /// <code>...Load()</code>
        /// <code>MyCustomEquipType = ItemModuleType.GetReservedID();</code>
        /// <code>...SetStaticDefaults() in a Grappling Hook Item...</code>
        /// <code>ModularItemManager.Catalogue.AllowEquipType(Type, MyCustomEquipType);</code>
        /// </para>
        /// </summary>
        /// <returns></returns>
        public static int GetReservedID()
        {
            reservedID++;
            return reservedID - 1;
        }

        void ILoadable.Load(Mod mod)
        {
            ResetReservedIDToCount();
            if (!Main.dedServ)
            {
                LoadTextureDict();
            }
        }
        private void LoadTextureDict()
        {
            TypeToTexture = new Dictionary<int, SpriteFrameData>()
            {
                [BarbHook] = new SpriteFrameData(TextureAssets.Extra[54], 3, 6, 2, 4, -2, -2),
                [BarbChain] = new SpriteFrameData(TextureAssets.Extra[54], 3, 6, 2, 2, -2, -2),
                [BarbMisc] = new SpriteFrameData(TextureAssets.Extra[54], 3, 6, 2, 5, -2, -2),
            };
        }

        void ILoadable.Unload()
        {
            ResetReservedIDToCount();
        }

        private static void ResetReservedIDToCount()
        {
            reservedID = MaxModuleTypes;
        }
    }
}