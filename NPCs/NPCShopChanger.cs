using AQMod.Common;
using AQMod.Common.ID;
using AQMod.Content.World.Events.GaleStreams;
using AQMod.Content.World.Events.GlimmerEvent;
using AQMod.Items.Accessories.FishingSeals;
using AQMod.Items.Accessories.Vanity;
using AQMod.Items.Dyes;
using AQMod.Items.Foods;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs
{
    public class NPCShopChanger : GlobalNPC
    {
        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            switch (type)
            {
                case NPCID.Merchant:
                    {
                        for (int i = 0; i < Main.maxInventory; i++)
                        {
                            if (Main.player[Main.myPlayer].inventory[i].useAmmo == AmmoID.Dart)
                            {
                                shop.item[nextSlot].SetDefaults(ItemID.Seed);
                                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(copper: 10);
                                nextSlot++;
                                break;
                            }
                        }
                    }
                    break;

                case NPCID.Clothier:
                    if (Main.eclipse)
                    {
                        shop.item[nextSlot].SetDefaults(ModContent.ItemType<MonoxideHat>());
                        nextSlot++;
                    }
                    break;

                case NPCID.Painter:
                    {
                        if (GlimmerEvent.IsGlimmerEventCurrentlyActive() && WorldDefeats.DownedStarite && Main.moonPhase != MoonPhases.FullMoon)
                        {
                            for (int i = 19; i < Chest.maxItems; i++) // skips most of the starting stuff, since that's all paint and blah
                            {
                                if (shop.item[i].type == ItemID.None || (shop.item[i].createTile == -1 && shop.item[i].paint == 0)) // at the very end of the paintings, and will intercept the slot for any walls or blank slots
                                {
                                    InterceptShop(shop, ModContent.ItemType<Items.Placeable.Furniture.OmegaStaritePainting>(), i, nextSlot);
                                    break;
                                }
                            }
                            nextSlot++;
                        }
                    }
                    break;

                case NPCID.DyeTrader:
                    {
                        if (Main.player[Main.myPlayer].ZoneBeach)
                        {
                            if (Main.moonPhase < 4)
                            {
                                shop.item[nextSlot].SetDefaults(ModContent.ItemType<BreakdownDye>());
                                nextSlot++;
                            }
                            else
                            {
                                shop.item[nextSlot].SetDefaults(ModContent.ItemType<SimplifiedDye>());
                                nextSlot++;
                            }
                        }
                        if (!Main.dayTime && WorldDefeats.DownedStarite)
                        {
                            if (Main.moonPhase != MoonPhases.FullMoon)
                            {
                                if (Main.moonPhase < 3)
                                {
                                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<HypnoDye>());
                                    nextSlot++;
                                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<DiscoDye>());
                                    nextSlot++;
                                }
                                else if (Main.moonPhase > 4)
                                {
                                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<ScrollDye>());
                                    nextSlot++;
                                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<EnchantedDye>());
                                    nextSlot++;
                                }
                                else
                                {
                                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<OutlineDye>());
                                    nextSlot++;
                                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<RainbowOutlineDye>());
                                    nextSlot++;
                                }
                            }
                        }
                        if (WorldDefeats.DownedGaleStreams && Main.player[Main.myPlayer].position.Y < GaleStreams.MinimumGaleStreamsSpawnOverride)
                        {
                            if (Main.moonPhase < 3)
                            {
                                shop.item[nextSlot].SetDefaults(ModContent.ItemType<CensorDye>());
                                nextSlot++;
                            }
                            else if (Main.moonPhase > 4)
                            {
                                shop.item[nextSlot].SetDefaults(ModContent.ItemType<RedSpriteDye>());
                                nextSlot++;
                            }
                            else
                            {
                                shop.item[nextSlot].SetDefaults(ModContent.ItemType<FrostbiteDye>());
                                nextSlot++;
                            }
                        }
                    }
                    break;

                case NPCID.Pirate:
                    {
                        if (Main.player[Main.myPlayer].anglerQuestsFinished >= 20)
                        {
                            shop.item[nextSlot].SetDefaults(ItemID.AnglerPants);
                            nextSlot++;
                            shop.item[nextSlot].SetDefaults(ItemID.AnglerVest);
                            nextSlot++;
                            shop.item[nextSlot].SetDefaults(ItemID.AnglerHat);
                            nextSlot++;
                            shop.item[nextSlot].SetDefaults(ModContent.ItemType<CopperSeal>());
                            nextSlot++;
                            shop.item[nextSlot].SetDefaults(ModContent.ItemType<SilverSeal>());
                            nextSlot++;
                            shop.item[nextSlot].SetDefaults(ModContent.ItemType<GoldSeal>());
                            nextSlot++;
                        }
                        else if (Main.player[Main.myPlayer].anglerQuestsFinished >= 15)
                        {
                            shop.item[nextSlot].SetDefaults(ItemID.AnglerVest);
                            nextSlot++;
                            shop.item[nextSlot].SetDefaults(ItemID.AnglerHat);
                            nextSlot++;
                            shop.item[nextSlot].SetDefaults(ModContent.ItemType<CopperSeal>());
                            nextSlot++;
                            shop.item[nextSlot].SetDefaults(ModContent.ItemType<SilverSeal>());
                            nextSlot++;
                        }
                        else if (Main.player[Main.myPlayer].anglerQuestsFinished >= 10)
                        {
                            shop.item[nextSlot].SetDefaults(ItemID.AnglerHat);
                            nextSlot++;
                            shop.item[nextSlot].SetDefaults(ModContent.ItemType<CopperSeal>());
                            nextSlot++;
                            shop.item[nextSlot].SetDefaults(ModContent.ItemType<SilverSeal>());
                            nextSlot++;
                        }
                        else if (Main.player[Main.myPlayer].anglerQuestsFinished >= 2)
                        {
                            shop.item[nextSlot].SetDefaults(ModContent.ItemType<CopperSeal>());
                            nextSlot++;
                        }
                    }
                    break;

                case NPCID.SkeletonMerchant:
                    {
                        var plr = Main.LocalPlayer;
                        if (!Main.dayTime)
                        {
                            if (Main.bloodMoon && !Main.hardMode && WorldDefeats.SudoHardmode)
                            {
                                shop.item[nextSlot].SetDefaults(ItemID.SlapHand);
                                nextSlot++;
                            }
                        }
                    }
                    break;
            }
        }

        private static void InterceptShop(Chest shop, int itemID, int i, int currSlot)
        {
            if (currSlot >= Chest.maxItems)
                currSlot = Chest.maxItems - 1;
            for (int j = currSlot; j > i; j--)
            {
                shop.item[j] = shop.item[j - 1];
            }
            shop.item[i] = new Item(); // removes the object reference in the slot after this
            shop.item[i].SetDefaults(itemID);
        }
    }
}