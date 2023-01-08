using Aequus.Common;
using Aequus.Content.CrossMod.ModCalls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
        public static UserInterface UserInterface { get; private set; }

        /// <summary>
        /// Determines whether or not the "Game World" is active. This means that the game is most likely running regular tick updates.
        /// </summary>
        public static bool GameWorldActive => Main.instance.IsActive && !Main.gamePaused && !Main.gameInactive;
        /// <summary>
        /// Easier to write version of <see cref="ClientConfig.Instance"/>.<see cref="ClientConfig.HighQuality">HighQuality</see>.
        /// </summary>
        public static bool HQ => ClientConfig.Instance.HighQuality;
        /// <summary>
        /// Easier to write version of <see cref="ClientConfig.Instance"/>.<see cref="ClientConfig.InfoDebugLogs">HighQuality</see>.
        /// </summary>
        public static bool LogMore => ClientConfig.Instance.InfoDebugLogs;

        /// <summary>
        /// Better way to write the following check <code><see cref="Main.hardMode"/> || <see cref="AequusWorld.downedOmegaStarite"/></code>
        /// </summary>
        public static bool HardmodeTier => Main.hardMode || AequusWorld.downedOmegaStarite;

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

        /// <summary>
        /// Easier to write version of 
        /// <code><see cref="ModPacket"/> packet = <see cref="Instance"/>.<see cref="Mod.GetPacket(int)">GetPacket(int)</see>;</code>
        /// <code>packet.Write((<see cref="byte"/>)<see cref="PacketType"/>.X);</code>
        /// </summary>
        /// <param name="type">The ID of the Packet</param>
        /// <returns></returns>
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
            if (Main.netMode != NetmodeID.Server)
            {
                UserInterface = new UserInterface();
            }

            foreach (var t in GetContent<IOnModLoad>())
            {
                t.OnModLoad(this);
            }

            On.Terraria.WorldGen.TileFrame += WorldGen_TileFrame;
        }

        private void WorldGen_TileFrame(On.Terraria.WorldGen.orig_TileFrame orig, int i, int j, bool resetFrame, bool noBreak)
        {
            //if (!Main.gameMenu)
            //Logger.Info($"X:{i} Y:{j} | TileType:{Main.tile[i, j].TileType} ({AequusText.GetInternalNameOrUnknown(Main.tile[i, j].TileType, TileID.Search)}) | {resetFrame}, {noBreak}");
            orig(i, j, resetFrame, noBreak);
        }

        public override void Unload()
        {
            requestedHookAccess = false;
            Instance = null;
            UserInterface = null;
        }

        public override object Call(params object[] args)
        {
            return ModCallManager.HandleModCall(args);
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PacketSystem.HandlePacket(reader);
        }

        public static Dictionary<string, List<string>> GetContentArrayFile(string name)
        {
            using (var stream = Instance.GetFileStream($"Content/{name}.json", newFileStream: true))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(streamReader.ReadToEnd());
                }
            }
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
            return Main.netMode == NetmodeID.Server ? false : Main.player[Main.myPlayer].Distance(where) < 1500f;
        }

        internal static SoundStyle GetSounds(string name, int num, float volume = 1f, float pitch = 0f, float variance = 0f)
        {
            return new SoundStyle(SoundsPath + name, 0, num) { Volume = volume, Pitch = pitch, PitchVariance = variance, };
        }
        internal static SoundStyle GetSound(string name, float volume = 1f, float pitch = 0f, float variance = 0f)
        {
            return new SoundStyle(SoundsPath + name) { Volume = volume, Pitch = pitch, PitchVariance = variance, };
        }

        public static bool IsAnglerQuest(int itemType)
        {
            return Main.anglerQuestItemNetIDs.IndexInRange(Main.anglerQuest) && Main.anglerQuestItemNetIDs[Main.anglerQuest] == itemType;
        }

        public static bool SetQuestFish(int itemType)
        {
            for (int i = 0; i < Main.anglerQuestItemNetIDs.Length; i++)
            {
                if (Main.anglerQuestItemNetIDs[i]== itemType)
                {
                    Main.anglerQuest = i;
                    return true;
                }
            }
            return false;
        }
    }
}