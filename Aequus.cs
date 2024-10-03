using Aequus.Common.Net;
using Aequus.Common.Preferences;
using Aequus.Content.DamageClasses;
using MonoMod.RuntimeDetour;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria.Audio;
using Terraria.UI;

namespace Aequus;
public class Aequus : Mod {
    public const char MOD_NAME_SEPERATOR = '/';

    public static readonly string ModSources = Path.Join(Main.SavePath, "ModSources", nameof(Aequus));

    internal delegate void LegacyDrawMethod(Texture2D texture, Vector2 position, Rectangle? frame, Color color, float scale, Vector2 origin, float rotation, SpriteEffects effects, float layerDepth);

#if DEBUG
    public static bool DevelopmentFeatures = true;
#else
    public static bool DevelopmentFeatures = false;
#endif

    public const string VanillaTexture = "Terraria/Images/";
    public const string BlankTexture = "Aequus/Assets/None";
    public const string AssetsPath = "Aequus/Assets/";
    public const string SoundsPath = AssetsPath + "Sounds/";

    public const string PlaceholderDebuff = "Aequus/Buffs/Debuffs/Debuff";
    public const string PlaceholderBuff = "Terraria/Images/Buff";
    public const string PlaceholderItem = "ModLoader/UnloadedItem";
    public const string PlaceholderFurniture = "ModLoader/UnloadedSupremeFurniture";

    public static NecromancyClass NecromancyClass => ModContent.GetInstance<NecromancyClass>();
    public static NecromancyMagicClass NecromancyMagicClass => ModContent.GetInstance<NecromancyMagicClass>();
    public static NecromancySummonerClass NecromancySummonerClass => ModContent.GetInstance<NecromancySummonerClass>();

    public static ArtistClass ArtistClass => ArtistClass.Instance;

    public static bool AnyBossDefeated => NPC.downedSlimeKing || NPC.downedDeerclops || AequusWorld.downedCrabson || AequusWorld.downedOmegaStarite || AequusWorld.downedDustDevil || Condition.DownedEarlygameBoss.IsMet();
    public static bool ZenithSeed => Main.getGoodWorld && Main.remixWorld;
    /// <summary>
    /// Shorthand for a bunch of checks determining whether the game is unpaused.
    /// </summary>
    public static bool GameWorldActive => (Main.instance.IsActive && !Main.gamePaused && !Main.gameInactive) || Main.netMode != NetmodeID.SinglePlayer;
    /// <summary>
    /// Shorthand for <see cref="ClientConfig.Instance"/>.<see cref="ClientConfig.HighQuality">HighQuality</see>.
    /// </summary>
    public static bool HQ => ClientConfig.Instance.HighQuality;
    /// <summary>
    /// Shorthand for <see cref="ClientConfig.Instance"/>.<see cref="ClientConfig.InfoDebugLogs">HighQuality</see>.
    /// </summary>
    public static bool InfoLogs => ClientConfig.Instance.InfoDebugLogs;

    /// <summary>
    /// Shorthand for checking <code><see cref="Main.hardMode"/> || <see cref="AequusWorld.downedOmegaStarite"/></code>
    /// </summary>
    public static bool MediumMode => Main.hardMode || AequusWorld.downedOmegaStarite;

    public static int GetMusicOrDefault(string musicName, int defaultValue) {
        int slot = MusicLoader.GetMusicSlot($"AequusMusic/Assets/Music/{musicName}");

        return slot != 0 ? slot : defaultValue;
    }

    public static Hook Detour(MethodInfo source, MethodInfo target) {
        //MonoModHooks.RequestNativeAccess();
        var hook = new Hook(source, target);
        hook.Apply();
        return hook;
    }

    public static Aequus Instance => ModContent.GetInstance<Aequus>();
    public static UserInterface UserInterface { get; private set; }

    /// <summary>
    /// Shorthand for
    /// <code><see cref="ModPacket"/> packet = <see cref="Instance"/>.<see cref="Mod.GetPacket(int)">GetPacket(int)</see>;</code>
    /// <code>packet.Write((<see cref="byte"/>)<see cref="PacketType"/>.X);</code>
    /// </summary>
    /// <param name="type">The ID of the Packet</param>
    /// <returns></returns>
    public static ModPacket GetPacket(PacketType type) {
        var p = Instance.GetPacket();
        p.Write((byte)type);
        return p;
    }

    public static T GetPacket<T>() where T : PacketHandler {
        return ModContent.GetInstance<T>();
    }

    public override void Load() {
        if (Main.netMode != NetmodeID.Server) {
            UserInterface = new UserInterface();
        }

        //On.Terraria.WorldGen.TileFrame += WorldGen_TileFrame;
    }

    //private static void WorldGen_TileFrame(On.Terraria.WorldGen.orig_TileFrame orig, int i, int j, bool resetFrame, bool noBreak) {

    //    if (Helper.iterations < 100 && Main.fpsTimer.ElapsedMilliseconds > 1000) {
    //        Instance.Logger.Info(Environment.StackTrace);
    //        Helper.iterations++;
    //    }
    //    orig(i, j, resetFrame, noBreak);
    //}

    public override void Unload() {
        UserInterface = null;
    }

    public override object Call(params object[] args) {
        return ModCallHandler.Instance.Call(args)!; // Lying to the compiler.
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI) {
        PacketSystem.HandlePacket(reader, whoAmI);
    }

    [Obsolete("Use Common.ContentArrayFile instead.")]
    public static Dictionary<string, List<string>> GetContentArrayFile(string name) {
        using (var stream = Instance.GetFileStream($"Content/{name}.json", newFileStream: true)) {
            using (var streamReader = new StreamReader(stream)) {
                return JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(streamReader.ReadToEnd());
            }
        }
    }

    public static Dictionary<string, Dictionary<string, string>> GetContentFile(string name) {
        using (var stream = Instance.GetFileStream($"Content/{name}.json", newFileStream: true)) {
            using (var streamReader = new StreamReader(stream)) {
                return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(streamReader.ReadToEnd());
            }
        }
    }

    public static bool CloseToEffect(Vector2 where) {
        return Main.netMode == NetmodeID.Server ? false : Main.player[Main.myPlayer].Distance(where) < 1500f;
    }

    [Obsolete("Replaced with AequusSounds")]
    internal static SoundStyle GetSounds(string name, int num, float volume = 1f, float pitch = 0f, float variance = 0f) {
        return new SoundStyle(SoundsPath + name, 0, num) { Volume = volume, Pitch = pitch, PitchVariance = variance, };
    }

    public static string TileTexture(int id) {
        return $"{VanillaTexture}Tiles_{id}";
    }
    public static string ItemTexture(int id) {
        return $"{VanillaTexture}Item_{id}";
    }
    public static string ProjectileTexture(int id) {
        return $"{VanillaTexture}Projectile_{id}";
    }
    public static string NPCTexture(int id) {
        return $"{VanillaTexture}NPC_{id}";
    }
}