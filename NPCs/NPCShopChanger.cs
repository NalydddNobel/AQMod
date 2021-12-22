using AQMod.Common;
using AQMod.Content.WorldEvents.GlimmerEvent;
using AQMod.Items.Accessories;
using AQMod.Items.Accessories.FishingSeals;
using AQMod.Items.Dedicated.Contributors;
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
                    if (Main.hardMode)
                    {
                        shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Tools.GoldPowder>());
                        nextSlot++;
                    }
                }
                break;

                case NPCID.Dryad:
                {
                    var plr = Main.LocalPlayer;
                    if (Main.hardMode && NPC.downedPlantBoss)
                    {
                        shop.item[nextSlot].SetDefaults(ModContent.ItemType<Baguette>());
                        nextSlot++;
                    }
                }
                break;

                case NPCID.Clothier:
                if (Main.eclipse)
                {
                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Vanities.MonoxideHat>());
                    nextSlot++;
                }
                break;

                case NPCID.Painter:
                {
                    if (GlimmerEvent.IsActive && WorldDefeats.DownedStarite)
                    {
                        if (Main.moonPhase != Constants.MoonPhases.FullMoon)
                        {
                            for (int i = 19; i < Chest.maxItems; i++) // skips most of the starting stuff, since that's all paint and blah
                            {
                                if (shop.item[i].type == ItemID.None || shop.item[i].createTile == -1 && shop.item[i].paint == 0) // at the very end of the paintings, and will intercept the slot for any walls or blank slots
                                {
                                    InterceptShop(shop, ModContent.ItemType<Items.Placeable.Furniture.OmegaStaritePainting>(), i, nextSlot);
                                    break;
                                }
                            }
                            nextSlot++;
                        }
                    }
                }
                break;

                case NPCID.DyeTrader:
                {
                    if (GlimmerEvent.IsActive && WorldDefeats.DownedStarite)
                    {
                        if (Main.moonPhase != Constants.MoonPhases.FullMoon)
                        {
                            if (Main.moonPhase < 3)
                            {
                                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Vanities.Dyes.DiscoDye>());
                                nextSlot++;
                            }
                            else if (Main.moonPhase > 4)
                            {
                                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Vanities.Dyes.EnchantedDye>());
                                nextSlot++;
                            }
                            else
                            {
                                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Vanities.Dyes.RainbowOutlineDye>());
                                nextSlot++;
                            }
                        }
                    }
                }
                break;

                case NPCID.PartyGirl:
                {
                    if (!Main.dayTime && Main.moonPhase == 0)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.WhoopieCushion);
                        nextSlot++;
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
                        if (Main.bloodMoon && !Main.hardMode && AQMod.SudoHardmode)
                        {
                            shop.item[nextSlot].SetDefaults(ItemID.SlapHand);
                            nextSlot++;
                        }
                    }
                }
                break;
            }
        }

        public override void SetupTravelShop(int[] shop, ref int nextSlot)
        {
            if (AddFoodToTravelShop(shop, nextSlot))
                nextSlot++;
        }

        private bool AddFoodToTravelShop(int[] shop, int slot)
        {
            var foodChoices = new List<int>();
            if (WorldDefeats.DownedCrabSeason)
                foodChoices.Add(ModContent.ItemType<Items.Foods.CrabSeason.CheesePuff>());
            if (WorldDefeats.DownedGlimmer)
                foodChoices.Add(ModContent.ItemType<Items.Foods.GlimmerEvent.NeutronJuice>());
            if (WorldDefeats.DownedGaleStreams)
            {
                foodChoices.Add(ModContent.ItemType<Items.Foods.GaleStreams.PeeledCarrot>());
                foodChoices.Add(ModContent.ItemType<Items.Foods.GaleStreams.CinnamonRoll>());
            }
            if (NPC.downedQueenBee)
                foodChoices.Add(ModContent.ItemType<Items.Foods.LarvaEel>());
            if (NPC.downedPlantBoss)
            {
                foodChoices.Add(ModContent.ItemType<Items.Foods.RedLicorice>());
                foodChoices.Add(ModContent.ItemType<Items.Foods.GrapePhanta>());
            }
            if (foodChoices.Count == 1)
            {
                shop[slot] = foodChoices[0];
                return true;
            }
            else if (foodChoices.Count > 0)
            {
                shop[slot] = foodChoices[Main.rand.Next(foodChoices.Count)];
                return true;
            }
            return false;
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