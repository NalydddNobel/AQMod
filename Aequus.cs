using Aequus.Common;
using Aequus.Common.Networking;
using Aequus.Content.Necromancy;
using Aequus.Items.Recipes;
using Aequus.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus
{
    public class Aequus : Mod
    {
        internal delegate void LegacyDrawMethod(Texture2D texture, Vector2 position, Rectangle? frame, Color color, float scale, Vector2 origin, float rotation, SpriteEffects effects, float layerDepth);

        public const string VanillaTexture = "Terraria/Images/";
        public const string BlankTexture = "Aequus/Assets/None";
        public const string AssetsPath = "Aequus/Assets/";
        public const string SoundsPath = AssetsPath + "Sounds/";
        public const string Debuff = "Aequus/Buffs/Debuffs/Debuff";
        public const string Buff = "Terraria/Images/Buff";

        public static Aequus Instance { get; private set; }
        public static UserInterface InventoryInterface { get; private set; }
        public static UserInterface NPCTalkInterface { get; private set; }

        public static bool GameWorldActive => Main.instance.IsActive && !Main.gamePaused && !Main.gameInactive;
        public static bool HQ => ClientConfig.Instance.HighQuality;
        public static bool LogMore => ClientConfig.Instance.InfoDebugLogs;

        public static bool HardmodeTier => Main.hardMode || AequusWorld.downedOmegaStarite;

        public static float SkiesDarkness;
        public static float SkiesDarknessGoTo;
        public static float SkiesDarknessGoToSpeed;

        public override void Load()
        {
            Instance = this;
            SkiesDarkness = 1f;
            if (Main.netMode != NetmodeID.Server)
            {
                InventoryInterface = new UserInterface();
                NPCTalkInterface = new UserInterface();
            }

            foreach (var t in AutoloadHelper.GetAndOrganizeOfType<IOnModLoad>(Code))
            {
                t.OnModLoad(this);
            }

            On.Terraria.Main.SetBackColor += Hook_DarkenBackground;
        }

        private static void Hook_DarkenBackground(On.Terraria.Main.orig_SetBackColor orig, Main.InfoToSetBackColor info, out Color sunColor, out Color moonColor)
        {
            orig(info, out sunColor, out moonColor);

            if (SkiesDarkness != 1f)
            {
                SkiesDarkness = Math.Clamp(SkiesDarkness, 0.1f, 1f);

                byte a = Main.ColorOfTheSkies.A;
                Main.ColorOfTheSkies *= SkiesDarkness;
                Main.ColorOfTheSkies.A = a;

                if (GameWorldActive)
                {
                    if (SkiesDarkness > 0.9999f)
                    {
                        SkiesDarkness = 1f;
                    }
                    SkiesDarkness = MathHelper.Lerp(SkiesDarkness, SkiesDarknessGoTo, SkiesDarknessGoToSpeed);
                    SkiesDarknessGoTo = 1f;
                    SkiesDarknessGoToSpeed = 0.02f;
                }
            }
        }

        public override void PostSetupContent()
        {
            foreach (var t in AutoloadHelper.GetAndOrganizeOfType<IPostSetupContent>(Code))
            {
                t.PostSetupContent(this);
            }
        }

        public override void AddRecipeGroups()
        {
            AequusRecipes.AddRecipeGroups();
        }

        public override void AddRecipes()
        {
            foreach (var t in AutoloadHelper.GetAndOrganizeOfType<IAddRecipes>(Code))
            {
                t.AddRecipes(this);
            }
        }

        public override void PostAddRecipes()
        {
            foreach (var t in AutoloadHelper.GetAndOrganizeOfType<IPostAddRecipes>(Code))
            {
                t.PostAddRecipes(this);
            }
        }

        public override void Unload()
        {
            Instance = null;
            InventoryInterface = null;
            NPCTalkInterface = null;
        }

        public override object Call(params object[] args)
        {
            switch ((string)args[0])
            {
                case "NecroStats":
                    return ModContent.GetInstance<NecromancyDatabase>().HandleModCall(this, args);

                case "Downed":
                    return ModContent.GetInstance<AequusWorld.DownedCalls>().HandleModCall(this, args);

                case "AddShopQuote":
                    return ShopQuotes.Database.HandleModCall(this, args);
            }
            return null;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PacketHandler.HandlePacket(reader);
        }

        public static bool ShouldDoScreenEffect(Vector2 where)
        {
            return Main.netMode == NetmodeID.Server ? false : Main.player[Main.myPlayer].Distance(where) < 3000f;
        }

        public static void DarkenSky(float to, float speed = 0.05f)
        {
            SkiesDarkness -= 0.01f;
            SkiesDarknessGoTo = Math.Min(SkiesDarknessGoTo, to);
            SkiesDarknessGoToSpeed = Math.Max(SkiesDarknessGoToSpeed, speed);
        }

        internal static SoundStyle GetSounds(string name, int num, float volume = 1f, float pitch = 0f)
        {
            return new SoundStyle(SoundsPath + name, 0, num) { Volume = volume, Pitch = pitch, PitchVariance = 0f, };
        }
        internal static SoundStyle GetSound(string name, float volume = 1f, float pitch = 0f)
        {
            return new SoundStyle(SoundsPath + name) { Volume = volume, Pitch = pitch, PitchVariance = 0f, };
        }
    }
}