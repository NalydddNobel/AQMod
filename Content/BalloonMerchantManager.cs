using AQMod.Common;
using AQMod.Content.World.Events.GaleStreams;
using AQMod.NPCs.Friendly;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content
{
    public sealed class BalloonMerchantManager : ModWorld
    {
        public static bool MerchantSetup;
        public static bool SellPlantSeeds;
        public static int SellBanner;
        public static bool SellGoldCrate;
        public static bool SellCrates;
        public static byte MaterialSold;
        public static int MerchantStealSeed;
        public static bool StealShopUseless;

        public static bool SettingUpShopStealing { get; internal set; }

        public override void PreUpdate()
        {
            if (!GaleStreams.IsActive)
            {
                if (MerchantSetup)
                    ResetMerchant();
                return;
            }
            int merchant = BalloonMerchant.Find();
            if (merchant == -1)
            {
                if ((Main.windSpeed > 100f || Main.rand.NextBool(1250)))
                {
                    SetupMerchant();
                    SpawnMerchant();
                }
                else
                {
                    if (MerchantSetup)
                        ResetMerchant();
                }
            }
        }

        public override void Initialize()
        {
            ResetMerchant();
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["MerchantSetup"] = MerchantSetup,
                ["SellBanner2"] = SpecialTagCompounds.Item.SaveItemID(SellBanner),
                ["SellPlantSeeds"] = SellPlantSeeds,
                ["SellGoldCrate"] = SellGoldCrate,
                ["SellCrates"] = SellCrates,
                ["MaterialSold"] = MaterialSold,
                ["MerchantStealSeed"] = MerchantStealSeed,
                ["StealShopUseless"] = StealShopUseless,
            };
        }

        public override void Load(TagCompound tag)
        {
            try
            {
                MerchantSetup = tag.GetBool("MerchantSetup");

                SellPlantSeeds = tag.GetBool("SellPlantSeeds");

                SellBanner = SpecialTagCompounds.Item.GetItemID(tag.GetCompound("SellBanner2"));

                SellGoldCrate = tag.GetBool("SellGoldCrate");

                SellCrates = tag.GetBool("SellCrates");

                MaterialSold = tag.GetByte("MaterialSold");

                MerchantStealSeed = tag.GetInt("MerchantStealSeed");

                StealShopUseless = tag.GetBool("StealShopUseless");
            }
            catch (Exception e)
            {
                AQMod.GetInstance().Logger.Debug(e);
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(MerchantSetup);
            if (MerchantSetup)
            {
                writer.Write(SellPlantSeeds);
                writer.Write(SellBanner);
                writer.Write(SellGoldCrate);
                writer.Write(SellCrates);
                writer.Write(MaterialSold);
                writer.Write(MerchantStealSeed);
                writer.Write(StealShopUseless);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            if (reader.ReadBoolean())
            {
                SellPlantSeeds = reader.ReadBoolean();
                SellBanner = reader.ReadInt32();
                SellGoldCrate = reader.ReadBoolean();
                SellCrates = reader.ReadBoolean();
                MaterialSold = reader.ReadByte();
                MerchantStealSeed = reader.ReadInt32();
                StealShopUseless = reader.ReadBoolean();
            }
        }

        public static void ResetMerchant()
        {
            MerchantSetup = false;
            SellPlantSeeds = false;
            SellBanner = 0;
            SellGoldCrate = false;
            SellCrates = false;
            MaterialSold = 0;
            MerchantStealSeed = -1;
            StealShopUseless = false;
        }

        public static void SetupMerchant()
        {
            ResetMerchant();
            SellPlantSeeds = Main.rand.NextBool();
            SellCrates = Main.rand.NextBool(4);
            if (SellCrates)
                SellGoldCrate = Main.rand.NextBool();
            MaterialSold = (byte)Main.rand.Next(5);
            MerchantStealSeed = Main.rand.Next(100, 2000000000); // did you know, the seed "0" gives you 3 waterleaf planter boxes... for me atleast...
            StealShopUseless = Main.rand.NextBool();
            MerchantSetup = true;
            List<int> potentialBanners = new List<int>();
            for (int bannerID = 0; bannerID < NPCLoader.NPCCount; bannerID++)
            {
                int npcID = Item.BannerToNPC(bannerID);
                if (npcID > 0 && !NPCID.Sets.ExcludedFromDeathTally[npcID] && !NPCID.Sets.BelongsToInvasionOldOnesArmy[npcID] && 
                    NPC.killCount[bannerID] > ItemID.Sets.KillsToBanner[Item.BannerToItem(bannerID)])
                {
                    potentialBanners.Add(Item.BannerToItem(bannerID));
                }
            }
            if (potentialBanners.Count > 1)
            {
                SellBanner = potentialBanners[Main.rand.Next(potentialBanners.Count)];
            }
            else if (potentialBanners.Count == 1)
            {
                SellBanner = potentialBanners[0];
            }
        }

        public static int SpawnMerchant()
        {
            int merchant = NPC.NewNPC(Main.maxTilesX * 16, 2400, ModContent.NPCType<BalloonMerchant>());
            if (merchant != -1)
                SpawnMerchant(merchant);
            return merchant;
        }

        public static void SpawnMerchant(int merchant)
        {
            int halfSizeTile = 25;
            int sizeTile = halfSizeTile * 2;
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int x = 60 + Main.rand.Next(0, Main.maxTilesX - 120);
                int y = 60 + Main.rand.Next(0, 100);
                if (checkTiles(new Rectangle(x - halfSizeTile, y - halfSizeTile, sizeTile, sizeTile)) &&
                    checkPlayers(new Rectangle((x - halfSizeTile) * 16, (y - halfSizeTile) * 16, sizeTile * 16, sizeTile * 16)))
                {
                    Main.npc[merchant].position.X = x * 16f - Main.npc[merchant].width / 2f;
                    Main.npc[merchant].position.Y = y * 16f - Main.npc[merchant].height / 2f;
                    Main.npc[merchant].aiStyle = -1;
                    break;
                }
            }
        }

        private static bool checkTiles(Rectangle rect)
        {
            for (int k = rect.X; k < rect.X + rect.Width; k++)
            {
                for (int l = rect.Y; l < rect.Y + rect.Height; l++)
                {
                    if (Main.tile[k, l] == null)
                    {
                        Main.tile[k, l] = new Tile();
                        continue;
                    }
                    if (Main.tile[k, l].active())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool checkPlayers(Rectangle rect)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead && Main.player[i].getRect().Contains(rect))
                {
                    return false;
                }
            }
            return true;
        }
    }
}