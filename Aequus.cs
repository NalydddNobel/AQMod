using Aequus.Biomes;
using Aequus.Common;
using Aequus.Common.Networking;
using Aequus.Content;
using Aequus.Content.CrossMod;
using Aequus.Content.Necromancy;
using Aequus.Items;
using Aequus.Items.Recipes;
using Aequus.NPCs;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus
{
    public class Aequus : Mod
    {
        internal delegate void LegacyDrawMethod(Texture2D texture, Vector2 position, Rectangle? frame, Color color, float scale, Vector2 origin, float rotation, SpriteEffects effects, float layerDepth);

        public const string TextureNone = "Aequus/Assets/None";
        public const string VanillaTexture = "Terraria/Images/";

        public static Aequus Instance { get; private set; }
        public static UserInterface InventoryInterface { get; private set; }
        public static UserInterface NPCTalkInterface { get; private set; }

        public static bool GameWorldActive => Main.instance.IsActive && !Main.gamePaused && !Main.gameInactive;
        public static bool HQ => ClientConfig.Instance.HighQuality;
        public static bool LogMore => ClientConfig.Instance.InfoDebugLogs;

        internal static Color GreenSlimeColor => ContentSamples.NpcsByNetId[NPCID.GreenSlime].color;
        internal static Color BlueSlimeColor => new Color(0, 80, 255, 100);

        public static Action ResetTileRenderPoints;
        public static Action DrawSpecialTilePoints;

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

            foreach (var t in AutoloadHelper.GetTypes(Code))
            {
                IOnModLoad.CheckAutoload(this, t);
            }

            On.Terraria.GameContent.Drawing.TileDrawing.PreDrawTiles += TileDrawing_PreDrawTiles;
            On.Terraria.GameContent.Drawing.TileDrawing.DrawReverseVines += TileDrawing_DrawReverseVines;
            On.Terraria.Main.SetBackColor += Main_SetBackColor;
        }

        private void Main_SetBackColor(On.Terraria.Main.orig_SetBackColor orig, Main.InfoToSetBackColor info, out Color sunColor, out Color moonColor)
        {
            orig(info, out sunColor, out moonColor);

            if (SkiesDarkness != 1f)
            {
                SkiesDarkness = MathHelper.Clamp(SkiesDarkness, 0.1f, 1f);

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

        private void TileDrawing_DrawReverseVines(On.Terraria.GameContent.Drawing.TileDrawing.orig_DrawReverseVines orig, Terraria.GameContent.Drawing.TileDrawing self)
        {
            orig(self);
            DrawSpecialTilePoints?.Invoke();
        }

        private void TileDrawing_PreDrawTiles(On.Terraria.GameContent.Drawing.TileDrawing.orig_PreDrawTiles orig, Terraria.GameContent.Drawing.TileDrawing self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets)
        {
            orig(self, solidLayer, forRenderTargets, intoRenderTargets);
            bool flag = intoRenderTargets || Lighting.UpdateEveryFrame;
            if (!solidLayer && flag)
            {
                ResetTileRenderPoints?.Invoke();
            }
        }

        public override void PostSetupContent()
        {
            foreach (var t in AutoloadHelper.GetTypes(Code))
            {
                IPostSetupContent.CheckAutoload(this, t);
            }
        }

        public override void AddRecipeGroups()
        {
            AequusRecipes.AddRecipeGroups();
        }

        public override void AddRecipes()
        {
            if (PolaritiesSupport.Polarities.Enabled)
            {
                MonsterBanners.BannerTypesHack.Add(TileID.Search.GetId("Polarities/BannerTile"));
            }
            AutoloadHelper.AutoloadOfType<IAddRecipes>(Code, this);
        }

        public override void PostAddRecipes()
        {
            AutoloadHelper.AutoloadOfType<IPostAddRecipes>(Code, this);
        }

        public override void Unload()
        {
            Instance = null;
            ResetTileRenderPoints = null;
            DrawSpecialTilePoints = null;
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
            PacketType type = PacketSender.ReadPacketType(reader);

            var l = Instance.Logger;
            if (type != PacketType.Unused && type != PacketType.SyncAequusPlayer)
            {
                l.Debug("Recieving Packet: " + type);
            }
            if (type == PacketType.Unused)
            {
            }
            else if (type == PacketType.SyncNecromancyOwnerTier)
            {
                int npc = reader.ReadInt32();
                Main.npc[npc].GetGlobalNPC<NecromancyNPC>().zombieOwner = reader.ReadInt32();
                Main.npc[npc].GetGlobalNPC<NecromancyNPC>().zombieDebuffTier = reader.ReadSingle();
            }
            else if (type == PacketType.SyncAequusPlayer)
            {
                if (Main.player[reader.ReadByte()].TryGetModPlayer<AequusPlayer>(out var aequus))
                {
                    aequus.RecieveChanges(reader);
                }
            }
            else if (type == PacketType.SoundQueue)
            {
                SoundHelpers.ReadSoundQueue(reader);
            }
            else if (type == PacketType.DemonSiegeSacrificeStatus)
            {
                DemonSiegeInvasion.EventSacrifice.ReadPacket(reader);
            }
            else if (type == PacketType.RequestDemonSiege)
            {
                DemonSiegeInvasion.HandleStartRequest(reader);
            }
            else if (type == PacketType.RemoveDemonSiege)
            {
                DemonSiegeInvasion.Sacrifices.Remove(new Point(reader.ReadUInt16(), reader.ReadUInt16()));
            }
            else if (type == PacketType.SyncDebuffs)
            {
                byte npc = reader.ReadByte();
                Main.npc[npc].GetGlobalNPC<NPCDebuffs>().Receive(npc, reader);
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
    }
}