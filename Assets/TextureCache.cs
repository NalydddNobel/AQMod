using AQMod.Assets.Textures;
using AQMod.Common.Graphics.PlayerEquips;
using AQMod.Common.Utilities;
using AQMod.Content.CursorDyes;
using AQMod.Effects;
using AQMod.Items.Armor.Arachnotron;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Assets
{
    public static class TextureCache
    {
        public const string None = "Assets/Textures/Empty";
        public const string Error = "Assets/Textures/error";

        public static TEA<CursorType> SwordCursors { get; private set; }
        public static TEA<CursorType> DemonCursors { get; private set; }
        public static TEA<ArachnotronLegsTextureType> ArachnotronArms { get; private set; }
        public static TEA<PlayerMaskID> PlayerMasks { get; private set; }
        public static TEA<PlayerHeadOverlayID> PlayerHeadOverlays { get; private set; }
        public static Dictionary<ParticleTex, Texture2D> Particles { get; private set; }
        public static Dictionary<LightTex, Texture2D> Lights { get; private set; }
        public static Dictionary<TrailTex, Texture2D> Trails { get; private set; }

        public static Ref<Texture2D> Pixel { get; private set; }
        public static Ref<Texture2D> DemonSiegeEventIcon { get; private set; }
        public static Ref<Texture2D> GlimmerEventEventIcon { get; private set; }
        public static Ref<Texture2D> UnityTeleportable { get; private set; }
        public static Ref<Texture2D> MapIconStarite { get; private set; }
        public static Ref<Texture2D> MapIconGlimmerEvent { get; private set; }
        public static Ref<Texture2D> MapIconEnemyBlip { get; private set; }
        public static Ref<Texture2D> MapIconDungeons { get; private set; }
        public static Ref<Texture2D> MapBGGlimmer { get; private set; }
        public static Ref<Texture2D> MapIconGlobe { get; private set; }
        public static Ref<Texture2D> BuffOutline { get; private set; }
        public static Ref<Texture2D> BGStarite { get; private set; }
        public static Ref<Texture2D> UITexture { get; private set; }

        internal static void Load()
        {
            ThreadPool.QueueUserWorkItem(LoadDictionaries);

            Pixel = new Ref<Texture2D>(ModContent.GetTexture("AQMod/Assets/Textures/Pixel"));
            DemonSiegeEventIcon = new Ref<Texture2D>(ModContent.GetTexture("AQMod/Assets/Textures/EventIcon_DemonSiege"));
            GlimmerEventEventIcon = new Ref<Texture2D>(ModContent.GetTexture("AQMod/Assets/Textures/EventIcon_GlimmerEvent"));
            UnityTeleportable = new Ref<Texture2D>(ModContent.GetTexture("AQMod/Assets/Textures/UnityTeleportable"));
            MapIconStarite = new Ref<Texture2D>(ModContent.GetTexture("AQMod/Assets/Textures/MapIconStarite"));
            MapIconGlimmerEvent = new Ref<Texture2D>(ModContent.GetTexture("AQMod/Assets/Textures/MapIconGlimmerEvent"));
            MapIconEnemyBlip = new Ref<Texture2D>(ModContent.GetTexture("AQMod/Assets/Textures/MapIconEnemyBlip"));
            MapIconDungeons = new Ref<Texture2D>(ModContent.GetTexture("AQMod/Assets/Textures/MapIconDungeons"));
            MapBGGlimmer = new Ref<Texture2D>(ModContent.GetTexture("AQMod/Assets/Textures/MapBGGlimmer"));
            MapIconGlobe = new Ref<Texture2D>(ModContent.GetTexture("AQMod/Assets/Textures/MapIconGlobe"));
            BuffOutline = new Ref<Texture2D>(ModContent.GetTexture("AQMod/Assets/Textures/BuffOutline"));
            BGStarite = new Ref<Texture2D>(ModContent.GetTexture("AQMod/Assets/Textures/BGStarite"));
            UITexture = new Ref<Texture2D>(ModContent.GetTexture("AQMod/Assets/Textures/UI"));
        }

        private static void LoadDictionaries(object callContext)
        {
            SwordCursors = new TEA<CursorType>(CursorType.Count, "AQMod/Assets/Textures", "SwordCursor");
            DemonCursors = new TEA<CursorType>(CursorType.Count, "AQMod/Assets/Textures", "DemonCursor");
            ArachnotronArms = new TEA<ArachnotronLegsTextureType>(ArachnotronLegsTextureType.Count, "AQMod/Items/Armor/Arachnotron", "ArachnotronLeg");
            PlayerMasks = new TEA<PlayerMaskID>(PlayerMaskID.Count, "AQMod/Assets/Textures", "PlayerMask");
            PlayerHeadOverlays = new TEA<PlayerHeadOverlayID>(PlayerHeadOverlayID.Count, "AQMod/Assets/Textures", "HeadOverlay");
            Particles = fillDictionary_ThreadSafe(ParticleTex.Count, "AQMod/Assets/Textures/Particles/Particle_");
            Lights = fillDictionary_ThreadSafe(LightTex.Count, "AQMod/Assets/Textures/Lights/Light_");
            Trails = fillDictionary_ThreadSafe(TrailTex.Count, "AQMod/Assets/Textures/Trail_");
        }

        /// <summary>
        /// Loads a dictionary of textures from a specified path
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="count">The max amount of textures. This is an enum since dictionary cringe, plus an excuse to not write the <T> stuff</param>
        /// <param name="pathWithoutNumbers">The path without the added number at the end. Example: "AQMod/Assets/Textures/Fart_". the paths would add 0, 1, 2, ect at the end of that path</param>
        /// <returns></returns>
        private static Dictionary<TEnum, Texture2D> fillDictionary_ThreadSafe<TEnum>(TEnum count, string pathWithoutNumbers) where TEnum : Enum
        { // why does this summary not work at all :ech:
            // also this might be a bit overkill since this all loads in a single frame right now
            try
            {
                int max = count.GetHashCode();
                var d = new Dictionary<TEnum, Texture2D>(max);
                for (ushort i = 0; i < max; i++)
                {
                    if (AQMod.Unloading)
                    {
                        return null;
                    }
                    d.Add(i.ToEnum<TEnum>(), ModContent.GetTexture(pathWithoutNumbers + i));
                }
                return d;
            }
            catch
            {
                return null;
            }
        }

        internal static void Unload()
        {
            BGStarite = null;
            MapBGGlimmer = null;
            MapIconDungeons = null;
            MapIconEnemyBlip = null;
            MapIconGlimmerEvent = null;
            MapIconStarite = null;
            UnityTeleportable = null;
            GlimmerEventEventIcon = null;
            DemonSiegeEventIcon = null;
            Pixel = null;

            PlayerHeadOverlays = null;
            PlayerMasks = null;
            Trails = null;
            Lights = null;
            ArachnotronArms = null;
            DemonCursors = null;
            SwordCursors = null;
        }

        public static Texture2D GetTexture(this ModItem ModItem)
        {
            return GetItem(ModItem.item.type);
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