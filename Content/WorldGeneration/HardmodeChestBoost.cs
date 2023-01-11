using Aequus.Content.ItemPrefixes.Potions;
using Aequus.Tiles;
using Aequus.Tiles.Furniture.HardmodeChests;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.WorldGeneration
{
    public class HardmodeChestBoost : ModSystem
    {
        public struct OreTierData
        {
            public int Tile;
            public int Ore;
            public int Bar;

            public OreTierData(int oreTile, int oreItem, int barItem)
            {
                Tile = oreTile;
                Ore = oreItem;
                Bar = barItem;
            }
        }

        public static Dictionary<int, OreTierData> TileIDToOreTier { get; private set; }

        public override void Load()
        {
            TileIDToOreTier = new Dictionary<int, OreTierData>()
            {
                [TileID.Cobalt] = new OreTierData(TileID.Cobalt, ItemID.CobaltOre, ItemID.CobaltBar),
                [TileID.Mythril] = new OreTierData(TileID.Mythril, ItemID.MythrilOre, ItemID.MythrilBar),
                [TileID.Adamantite] = new OreTierData(TileID.Adamantite, ItemID.AdamantiteOre, ItemID.AdamantiteBar),
                [TileID.Palladium] = new OreTierData(TileID.Palladium, ItemID.PalladiumOre, ItemID.PalladiumBar),
                [TileID.Orichalcum] = new OreTierData(TileID.Orichalcum, ItemID.OrichalcumOre, ItemID.OrichalcumBar),
                [TileID.Titanium] = new OreTierData(TileID.Titanium, ItemID.TitaniumOre, ItemID.TitaniumBar),
            };
        }

        private static OreTierData GetFromTileOrDefault(int tileID, int defaultTileID)
        {
            if (TileIDToOreTier.TryGetValue(tileID, out var val))
                return val;
            return TileIDToOreTier[defaultTileID];
        }

        private static void ReplaceChestBarsAndOres(int barItem, OreTierData oreData)
        {
            for (int i = 0; i < ChestOpenedTracker.UnopenedChests.Count; i++)
            {
                int chestID = Chest.FindChest(ChestOpenedTracker.UnopenedChests[i].X, ChestOpenedTracker.UnopenedChests[i].Y);
                if (chestID > -1 && ChestOpenedTracker.IsRealChest(chestID) && ChestType.IsGenericUndergroundChest(Main.chest[chestID]))
                {
                    for (int k = 0; k < Chest.maxItems; k++)
                    {
                        if (Main.chest[chestID].item[k].type == barItem)
                        {
                            int stack = Main.chest[chestID].item[k].stack;
                            Main.chest[chestID].item[k].SetDefaults(oreData.Bar);
                            Main.chest[chestID].item[k].stack = stack;
                        }
                    }
                }
            }
        }

        private static bool Crimson()
        {
            if (Main.drunkWorld)
                return WorldGen.genRand.NextBool();
            return WorldGen.crimson;
        }

        public static void Hardmodify(Chest chest)
        {
            int chestType = ChestType.GetStyle(chest);
            int chestTile = Main.tile[chest.x, chest.y].TileType;
            for (int i = 0; i < Chest.maxItems; i++)
            {
                var item = chest.item[i];
                if (item.type == ItemID.FlamingArrow || item.type == ItemID.WoodenArrow)
                {
                    if (Crimson())
                    {
                        item.SetDefaults(ItemID.IchorArrow);
                        item.stack = WorldGen.genRand.Next(50, 100);
                    }
                    else
                    {
                        item.SetDefaults(ItemID.CursedArrow);
                        item.stack = WorldGen.genRand.Next(50, 100);
                    }
                }
                else if (item.type == ItemID.ThrowingKnife || item.type == ItemID.Shuriken)
                {
                    if (Crimson())
                    {
                        item.SetDefaults(ItemID.IchorBullet);
                        item.stack = WorldGen.genRand.Next(50, 100);
                    }
                    else
                    {
                        item.SetDefaults(ItemID.CursedBullet);
                        item.stack = WorldGen.genRand.Next(50, 100);
                    }
                }
                else if (item.type == ItemID.RecallPotion)
                {
                    item.SetDefaults(ItemID.TeleportationPotion);
                }
                else if (item.type == ItemID.Glowstick)
                {
                    int stack = item.stack;
                    item.SetDefaults(ItemID.SpelunkerGlowstick);
                    item.stack = stack;
                }
                else if (item.type == ItemID.LesserHealingPotion)
                {
                    int stack = item.stack;
                    item.SetDefaults(ItemID.HealingPotion);
                    item.stack = stack;
                }
                else if (item.type == ItemID.HealingPotion)
                {
                    int stack = item.stack;
                    item.SetDefaults(ItemID.GreaterHealingPotion);
                    item.stack = stack;
                }
                switch (WorldGen.genRand.Next(3))
                {
                    case 0:
                        if (ModContent.GetInstance<BoundedPrefix>().CanRoll(item))
                        {
                            int stack = item.stack;
                            item.SetDefaults(item.type);
                            item.stack = stack;
                            item.Prefix(ModContent.PrefixType<BoundedPrefix>());
                        }
                        break;

                    case 1:
                        if (ModContent.GetInstance<DoubledTimePrefix>().CanRoll(item))
                        {
                            int stack = item.stack;
                            item.SetDefaults(item.type);
                            item.stack = stack;
                            item.Prefix(ModContent.PrefixType<DoubledTimePrefix>());
                        }
                        break;

                    case 2:
                        if (ModContent.GetInstance<EmpoweredPrefix>().CanRoll(item))
                        {
                            int stack = item.stack;
                            item.SetDefaults(item.type);
                            item.stack = stack;
                            item.Prefix(ModContent.PrefixType<EmpoweredPrefix>());
                        }
                        break;
                }
            }

            switch (WorldGen.genRand.Next(5))
            {
                case 0:
                    chest.AddItem(ItemID.SoulofLight, WorldGen.genRand.Next(1, 5));
                    break;
                case 1:
                    chest.AddItem(ItemID.SoulofNight, WorldGen.genRand.Next(1, 5));
                    break;
                case 2:
                    chest.AddItem(Crimson() ? ItemID.Ichor : ItemID.CursedFlame, WorldGen.genRand.Next(1, 10));
                    break;
                case 3:
                    chest.AddItem(ItemID.PixieDust, WorldGen.genRand.Next(1, 16));
                    break;
            }
            if (chestTile == TileID.Containers)
            {
                switch (chestType)
                {
                    case ChestType.Frozen:
                        if (WorldGen.genRand.NextBool(3))
                        {
                            chest.AddItem(ItemID.FrostCore);
                        }
                        break;
                }
            }
            else if (chestTile == TileID.Containers2 && chestType == ChestType.Sandstone)
            {
                if (WorldGen.genRand.NextBool(3))
                {
                    chest.AddItem(ItemID.AncientBattleArmorMaterial);
                }
            }

            ChangeChestToHardmodeVariant(chest, chestType, chestTile);
        }
        public static void ChangeChestToHardmodeVariant(Chest chest, int chestType, int chestTile)
        {
            if (chestTile == TileID.Containers)
            {
                switch (chestType)
                {
                    case ChestType.Gold:
                        {
                            InnerChangeChestToHardmodeVariant<AdamantiteChestTile>(chest.x, chest.y);
                        }
                        break;
                }
            }
            else if (chestTile == TileID.Containers2)
            {

            }
        }
        public static void InnerChangeChestToHardmodeVariant<T>(int x, int y) where T : ModTile
        {
            x -= Main.tile[x, y].TileFrameX % 36 / 18;
            y -= Main.tile[x, y].TileFrameY % 36 / 18;
            var tileType = (ushort)ModContent.TileType<T>();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Main.tile[x + i, y + j].Active(value: true);
                    Main.tile[x + i, y + j].TileType = tileType;
                    Main.tile[x + i, y + j].TileFrameX = (short)(i * 18);
                    Main.tile[x + i, y + j].TileFrameY = (short)(j * 18);
                }
            }
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, x, y, 2, 2);
            }
        }

        public override void PostUpdateWorld()
        {
            if (!Main.hardMode || WorldGen.IsGeneratingHardMode || Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            if (!AequusWorld.hardmodeChests && WorldGen.SavedOreTiers.Cobalt > 0)
            {
                for (int i = 0; i < ChestOpenedTracker.UnopenedChests.Count; i++)
                {
                    int chestID = Chest.FindChest(ChestOpenedTracker.UnopenedChests[i].X, ChestOpenedTracker.UnopenedChests[i].Y);
                    if (chestID > -1 && ChestOpenedTracker.IsRealChest(chestID) && ChestType.IsGenericUndergroundChest(Main.chest[chestID]))
                    {
                        Hardmodify(Main.chest[chestID]);
                    }
                }
                AequusWorld.hardmodeChests = true;
                AequusText.Broadcast("Announcement.HardmodeChests", AequusText.EventMessage.HueAdd(0.075f));
            }
            else if (!AequusWorld.chestCobaltTier && WorldGen.SavedOreTiers.Cobalt > 0)
            {
                ReplaceChestBarsAndOres(ItemID.IronBar, GetFromTileOrDefault(WorldGen.SavedOreTiers.Cobalt, TileID.Cobalt));
                ReplaceChestBarsAndOres(ItemID.LeadBar, GetFromTileOrDefault(WorldGen.SavedOreTiers.Cobalt, TileID.Cobalt));
                AequusWorld.chestCobaltTier = true;
            }
            else if (!AequusWorld.chestMythrilTier && WorldGen.SavedOreTiers.Mythril > 0)
            {
                ReplaceChestBarsAndOres(ItemID.SilverBar, GetFromTileOrDefault(WorldGen.SavedOreTiers.Mythril, TileID.Mythril));
                ReplaceChestBarsAndOres(ItemID.TungstenBar, GetFromTileOrDefault(WorldGen.SavedOreTiers.Mythril, TileID.Mythril));
                AequusWorld.chestMythrilTier = true;
            }
            else if (!AequusWorld.chestAdamantiteTier && WorldGen.SavedOreTiers.Adamantite > 0)
            {
                ReplaceChestBarsAndOres(ItemID.GoldBar, GetFromTileOrDefault(WorldGen.SavedOreTiers.Adamantite, TileID.Adamantite));
                ReplaceChestBarsAndOres(ItemID.PlatinumBar, GetFromTileOrDefault(WorldGen.SavedOreTiers.Adamantite, TileID.Adamantite));
                AequusWorld.chestAdamantiteTier = true;
            }
        }
    }
}