using AQMod.Content.CursorDyes;
using AQMod.Items.Weapons.Melee.Flails;
using AQMod.NPCs.SiegeEvent;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Assets
{
    public static class TextureCache
    {
        public const string None = "Assets/Textures/Empty";

        public static TEA<CursorType> SwordCursors { get; private set; }
        public static TEA<CursorType> DemonCursors { get; private set; }
        public static Ref<Texture2D> Pixel { get; private set; }
        public static Ref<Texture2D> DemonSiegeEventIcon { get; private set; }

        public static TextureAsset PowPunchChain { get; private set; }
        public static TextureAsset MagmabubbleLegs { get; private set; }
        public static TextureAsset TrapperImpTail { get; private set; }
        public static TextureAsset TrapperImpWings { get; private set; }
        public static TextureAsset TrapperImpGlow { get; private set; }
        public static TextureAsset TrapperChain { get; private set; }

        internal static void Load()
        {
            SwordCursors = new TEA<CursorType>(CursorType.Count, "AQMod/Assets/Textures", "SwordCursor");
            DemonCursors = new TEA<CursorType>(CursorType.Count, "AQMod/Assets/Textures", "DemonCursor");
            Pixel = new Ref<Texture2D>(ModContent.GetTexture("AQMod/Assets/Textures/Pixel"));
            DemonSiegeEventIcon = new Ref<Texture2D>(ModContent.GetTexture("AQMod/Assets/Textures/EventIcon_DemonSiege"));
            PowPunchChain = TextureAsset.FromT<PowPunchProjectile>("_Chain");
            MagmabubbleLegs = TextureAsset.FromT<Magmalbubble>("_Legs");
            TrapperImpTail = TextureAsset.FromT<TrapImp>("_Tail");
            TrapperImpWings = TextureAsset.FromT<TrapImp>("_Wings");
            TrapperImpGlow = TextureAsset.FromT<TrapImp>("_Glow");
            TrapperChain = TextureAsset.FromT<Trapper>("_Chain");
        }

        internal static void Unload()
        {
            TrapperImpGlow =
            TrapperImpWings = 
            TrapperImpTail =
            MagmabubbleLegs =
            PowPunchChain = null;
            DemonSiegeEventIcon = 
            Pixel = null;
            DemonCursors =
            SwordCursors = null;
        }

        public static Texture2D GetTexture(this ModItem ModItem)
        {
            return GetProjectile(ModItem.item.type);
        }

        public static Texture2D GetTexture(this Item Item)
        {
            return GetItem(Item.type);
        }

        public static Texture2D GetItem(int type)
        {
            //Main.instance.LoadItem(type);
            return Main.itemTexture[type];
        }

        public static Texture2D GetTexture(this ModProjectile ModProjectile)
        {
            return GetProjectile(ModProjectile.projectile.type);
        }

        public static Texture2D GetTexture(this Projectile Projectile)
        {
            return GetProjectile(Projectile.type);
        }

        public static Texture2D GetProjectile(int type)
        {
            Main.instance.LoadProjectile(type);
            return Main.projectileTexture[type];
        }

        public static Texture2D GetNPC(int type)
        {
            Main.instance.LoadNPC(type);
            return Main.npcTexture[type];
        }

        public static Texture2D GetBuff(int type)
        {
            //Main.instance.LoadBuff(type);
            return Main.buffTexture[type];
        }

        public static Texture2D GetWall(int type)
        {
            Main.instance.LoadWall(type);
            return Main.wallTexture[type];
        }
    }
}