using AQMod.Items.Accessories;
using AQMod.Items.Accessories.FishingSeals;
using AQMod.Items.Dedicated.Contributors;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common
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
                        switch (Main.moonPhase)
                        {
                            case 0:
                            {
                                shop.item[nextSlot].SetDefaults(ItemID.MiningPotion);
                                nextSlot++;
                            }
                            break;

                            case 1:
                            case 2:
                            {
                                shop.item[nextSlot].SetDefaults(ItemID.MiningShirt);
                                nextSlot++;
                            }
                            break;

                            case 3:
                            case 4:
                            {
                                shop.item[nextSlot].SetDefaults(ItemID.MiningPants);
                                nextSlot++;
                            }
                            break;

                            case 5:
                            case 6:
                            {
                                shop.item[nextSlot].SetDefaults(ModContent.ItemType<MinersFlashlight>());
                                nextSlot++;
                            }
                            break;
                        }
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
    }
}