using Aequus.Common;
using Aequus.Content.CrossMod.ModCalls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
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

        private static bool requestedHookAccess;

        public static object Hook(MethodInfo info, MethodInfo info2)
        {
            if (!requestedHookAccess)
            {
                MonoModHooks.RequestNativeAccess();
                requestedHookAccess = true;
            }
            new Hook(info, info2).Apply();
            return null;
        }

        public static ModPacket GetPacket(PacketType type)
        {
            var p = Instance.GetPacket();
            p.Write((byte)type);
            return p;
        }

        public override void Load()
        {
            requestedHookAccess = false;
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

        public override void Unload()
        {
            requestedHookAccess = false;
            Instance = null;
            InventoryInterface = null;
            NPCTalkInterface = null;
        }

        public override object Call(params object[] args)
        {
            return ModCallManager.HandleModCall(args);
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PacketSystem.HandlePacket(reader);
        }

        public static Dictionary<string, Dictionary<string, string>> GetContentFile(string name)
        {
            using (var stream = Instance.GetFileStream($"Content/{name}.json", newFileStream: true))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(streamReader.ReadToEnd());
                }
            }
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

        public static string LiquidName(byte liquidType)
        {
            switch (liquidType)
            {
                default:
                    return "Unknown";

                case LiquidID.Water:
                    return Language.GetTextValue("Mods.Aequus.Water");
                case LiquidID.Lava:
                    return Language.GetTextValue("Mods.Aequus.Lava");
                case LiquidID.Honey:
                    return Language.GetTextValue("Mods.Aequus.Honey");
                case 3:
                    return Language.GetTextValue("Mods.Aequus.Shimmer");
            }
        }

        internal static SoundStyle GetSounds(string name, int num, float volume = 1f, float pitch = 0f, float variance = 0f)
        {
            return new SoundStyle(SoundsPath + name, 0, num) { Volume = volume, Pitch = pitch, PitchVariance = variance, };
        }
        internal static SoundStyle GetSound(string name, float volume = 1f, float pitch = 0f, float variance = 0f)
        {
            return new SoundStyle(SoundsPath + name) { Volume = volume, Pitch = pitch, PitchVariance = variance, };
        }
    }
}