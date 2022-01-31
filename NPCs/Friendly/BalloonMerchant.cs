using AQMod.Common;
using AQMod.Common.ID;
using AQMod.Content.NameTags;
using AQMod.Content.World;
using AQMod.Items.Materials;
using AQMod.Items.Placeable.Nature;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AQMod.NPCs.Friendly
{
    [AutoloadBossHead()]
    public class BalloonMerchant : ModNPC
    {
        private int _oldSpriteDirection;
        private int _balloonFrameCounter;
        private int _balloonFrame;
        private int _balloonColor;
        private bool _init;

        public int currentAction;

        public override bool Autoload(ref string name)
        {
            mod.AddBossHeadTexture(this.GetPath("_Head_Boss_Invisible"), -1);
            return base.Autoload(ref name);
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 25;
            NPCID.Sets.ExtraFramesCount[npc.type] = 9;
            NPCID.Sets.AttackFrameCount[npc.type] = 4;
            NPCID.Sets.DangerDetectRange[npc.type] = 700;
            NPCID.Sets.AttackType[npc.type] = 0;
            NPCID.Sets.AttackTime[npc.type] = 90;
            NPCID.Sets.AttackAverageChance[npc.type] = 50;
            NPCID.Sets.HatOffsetY[npc.type] = 0;
        }

        public override void SetDefaults()
        {
            npc.friendly = true;
            npc.width = 18;
            npc.height = 40;
            npc.aiStyle = AQNPC.AIStyles.PassiveAI;
            npc.damage = 10;
            npc.defense = 15;
            npc.lifeMax = 250;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.5f;
            animationType = NPCID.Guide;
            currentAction = 7;
        }

        public override void BossHeadSlot(ref int index)
        {
            if (!WorldDefeats.AirMerchantHasBeenFound)
            {
                index = ModContent.GetModBossHeadSlot(this.GetPath("_Head_Boss_Invisible"));
            }
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            spriteEffects = npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            if (EventGaleStreams.IsActive)
            {
                button = Language.GetTextValue("LegacyInterface.28");
                button2 = Language.GetTextValue("Mods.AQMod.BalloonMerchant.RenameItem.ChatButton");
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;
            }
            else
            {
                Main.playerInventory = true;
                Main.npcChatText = "";
                UserInterfaceRenameItem.IsActive = true;
            }
        }

        public override void SetupShop(Chest shop, ref int nextSlot) // a combination of the travelling merchant and skeleton merchant, with items that are sold on specific circumstances and items which are chosen randomly
        {
            var player = Main.LocalPlayer;

            if (!AirHunterWorldData.MerchantSetup)
                AirHunterWorldData.SetupMerchant();

            if (!Main.dayTime)
            {
                switch (Main.moonPhase)
                {
                    case MoonPhases.FullMoon:
                        {
                            if (WorldGen.shadowOrbSmashed)
                            {
                                if (WorldGen.crimson)
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.BloodWater);
                                    nextSlot++;
                                }
                                else
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.UnholyWater);
                                    nextSlot++;
                                }
                                if (WorldGen.crimson)
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.ShadowOrb);
                                    nextSlot++;
                                }
                                else
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.CrimsonHeart);
                                    nextSlot++;
                                }
                            }
                            if (NPC.downedSlimeKing)
                            {
                                shop.item[nextSlot].SetDefaults(ItemID.Daybloom);
                                nextSlot++;
                            }
                        }
                        break;

                    case MoonPhases.WaningGibbious:
                        {
                            if (WorldGen.shadowOrbSmashed)
                            {
                                if (WorldGen.crimson)
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.CrimsonRod);
                                    nextSlot++;
                                }
                                else
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.Vilethorn);
                                    nextSlot++;
                                }
                                if (WorldGen.crimson)
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.BallOHurt);
                                    nextSlot++;
                                }
                                else
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.TheRottedFork);
                                    nextSlot++;
                                }
                            }
                            if (NPC.downedBoss1)
                            {
                                if (AirHunterWorldData.SellPlantSeeds)
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.BlinkrootSeeds);
                                    nextSlot++;
                                }
                                else
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.Blinkroot);
                                    nextSlot++;
                                }
                            }
                        }
                        break;

                    case MoonPhases.ThirdQuarter:
                        {
                            if (WorldGen.shadowOrbSmashed)
                            {
                                if (WorldGen.crimson)
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.PanicNecklace);
                                    nextSlot++;
                                }
                                else
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.BandofStarpower);
                                    nextSlot++;
                                }
                            }
                            if (NPC.downedBoss2)
                            {
                                if (AirHunterWorldData.SellPlantSeeds)
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.DeathweedSeeds);
                                    nextSlot++;
                                }
                                else
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.Deathweed);
                                    nextSlot++;
                                }
                            }
                        }
                        break;

                    case MoonPhases.WaningCrescent:
                        {
                            if (WorldGen.shadowOrbSmashed)
                            {
                                if (WorldGen.crimson)
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.TheUndertaker);
                                    nextSlot++;
                                }
                                else
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.Musket);
                                    nextSlot++;
                                }
                            }
                            if (NPC.downedQueenBee)
                            {
                                if (AirHunterWorldData.SellPlantSeeds)
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.MoonglowSeeds);
                                    nextSlot++;
                                }
                                else
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.Moonglow);
                                    nextSlot++;
                                }
                            }
                        }
                        break;

                    case MoonPhases.NewMoon:
                        {
                            if (WorldGen.shadowOrbSmashed)
                            {
                                if (WorldGen.crimson)
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.UnholyWater);
                                    nextSlot++;
                                }
                                else
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.BloodWater);
                                    nextSlot++;
                                }
                                if (WorldGen.crimson)
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.CrimsonHeart);
                                    nextSlot++;
                                }
                                else
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.ShadowOrb);
                                    nextSlot++;
                                }
                            }
                            if (NPC.downedBoss2)
                            {
                                if (WorldGen.crimson)
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.CorruptPlanterBox);
                                    nextSlot++;
                                }
                                else
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.CrimsonPlanterBox);
                                    nextSlot++;
                                }
                            }
                        }
                        break;

                    case MoonPhases.WaxingCrescent:
                        {
                            if (WorldGen.shadowOrbSmashed)
                            {
                                if (WorldGen.crimson)
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.Musket);
                                    nextSlot++;
                                }
                                else
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.TheUndertaker);
                                    nextSlot++;
                                }
                                if (WorldGen.crimson)
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.TheRottedFork);
                                    nextSlot++;
                                }
                                else
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.BallOHurt);
                                    nextSlot++;
                                }
                            }
                            if (NPC.downedBoss3)
                            {
                                if (WorldGen.crimson)
                                {
                                    if (AirHunterWorldData.SellPlantSeeds)
                                    {
                                        shop.item[nextSlot].SetDefaults(ItemID.WaterleafSeeds);
                                        nextSlot++;
                                    }
                                    else
                                    {
                                        shop.item[nextSlot].SetDefaults(ItemID.WaterleafSeeds);
                                        nextSlot++;
                                    }
                                }
                                else
                                {
                                    if (AirHunterWorldData.SellPlantSeeds)
                                    {
                                        shop.item[nextSlot].SetDefaults(ItemID.ShiverthornSeeds);
                                        nextSlot++;
                                    }
                                    else
                                    {
                                        shop.item[nextSlot].SetDefaults(ItemID.Shiverthorn);
                                        nextSlot++;
                                    }
                                }
                            }
                        }
                        break;

                    case MoonPhases.FirstQuarter:
                        {
                            if (WorldGen.shadowOrbSmashed)
                            {
                                if (WorldGen.crimson)
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.BandofStarpower);
                                    nextSlot++;
                                }
                                else
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.PanicNecklace);
                                    nextSlot++;
                                }
                            }
                            if (NPC.downedBoss3)
                            {
                                if (WorldGen.crimson)
                                {
                                    if (AirHunterWorldData.SellPlantSeeds)
                                    {
                                        shop.item[nextSlot].SetDefaults(ItemID.ShiverthornSeeds);
                                        nextSlot++;
                                    }
                                    else
                                    {
                                        shop.item[nextSlot].SetDefaults(ItemID.Shiverthorn);
                                        nextSlot++;
                                    }
                                }
                                else
                                {
                                    if (AirHunterWorldData.SellPlantSeeds)
                                    {
                                        shop.item[nextSlot].SetDefaults(ItemID.WaterleafSeeds);
                                        nextSlot++;
                                    }
                                    else
                                    {
                                        shop.item[nextSlot].SetDefaults(ItemID.WaterleafSeeds);
                                        nextSlot++;
                                    }
                                }
                            }
                        }
                        break;

                    case MoonPhases.WaxingGibbious:
                        {
                            if (WorldGen.shadowOrbSmashed)
                            {
                                if (WorldGen.crimson)
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.Vilethorn);
                                    nextSlot++;
                                }
                                else
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.CrimsonRod);
                                    nextSlot++;
                                }
                            }
                            if (Main.hardMode)
                            {
                                if (AirHunterWorldData.SellPlantSeeds)
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.FireblossomSeeds);
                                    nextSlot++;
                                }
                                else
                                {
                                    shop.item[nextSlot].SetDefaults(ItemID.Fireblossom);
                                    nextSlot++;
                                }
                            }
                        }
                        break;
                }
                if (Main.time * 3.0 > Main.nightLength * 2.0)
                {
                    shop.item[nextSlot].SetDefaults(ItemID.RestorationPotion);
                    shop.item[nextSlot].value = Item.buyPrice(gold: 1);
                    nextSlot++;
                }
                else if (Main.hardMode && Main.time * 3.0 > Main.nightLength)
                {
                    shop.item[nextSlot].SetDefaults(ItemID.GreaterHealingPotion);
                    shop.item[nextSlot].value = Item.buyPrice(gold: 3);
                    nextSlot++;
                }
                else
                {
                    if (player.ZoneJungle)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.Honeyfin);
                        shop.item[nextSlot].value = Item.buyPrice(gold: 3);
                        nextSlot++;
                    }
                    else
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.HealingPotion);
                        shop.item[nextSlot].value = Item.buyPrice(gold: 1);
                        nextSlot++;
                    }
                }
                if (player.ZoneJungle && Main.time * 3.0 > Main.nightLength)
                {
                    shop.item[nextSlot].SetDefaults(ItemID.GrubSoup);
                    shop.item[nextSlot].value = Item.buyPrice(gold: 3);
                    nextSlot++;
                }
                if (Main.time * 4.0 < Main.nightLength)
                {
                    shop.item[nextSlot].SetDefaults(ItemID.Feather);
                    shop.item[nextSlot].value = Item.buyPrice(silver: 50);
                    nextSlot++;
                }
                if (Main.time * 4.0 > Main.nightLength * 3.0)
                {
                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Accessories.MinersFlashlight>());
                    nextSlot++;
                }
            }
            else
            {
                if (AirHunterWorldData.SellBanner != 0)
                {
                    shop.item[nextSlot].SetDefaults(AirHunterWorldData.SellBanner);
                    shop.item[nextSlot].value = Item.buyPrice(gold: 10);
                    nextSlot++;
                }
                if (AirHunterWorldData.SellCrates && NPC.AnyNPCs(NPCID.Angler))
                {
                    if (player.ZoneCorrupt && WorldGen.shadowOrbSmashed)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.CorruptFishingCrate);
                        shop.item[nextSlot].value = Item.buyPrice(platinum: 1);
                        nextSlot++;
                    }
                    else if (player.ZoneCrimson && WorldGen.shadowOrbSmashed)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.CrimsonFishingCrate);
                        shop.item[nextSlot].value = Item.buyPrice(platinum: 1);
                        nextSlot++;
                    }
                    else if (player.ZoneHoly && Main.hardMode)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.HallowedFishingCrate);
                        shop.item[nextSlot].value = Item.buyPrice(platinum: 1);
                        nextSlot++;
                    }
                    else if (player.ZoneJungle && WorldGen.shadowOrbSmashed)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.JungleFishingCrate);
                        shop.item[nextSlot].value = Item.buyPrice(platinum: 1);
                        nextSlot++;
                    }
                    else if (NPC.downedBoss1)
                    {
                        if (NPC.downedBoss3 && player.HasItem(ItemID.GoldenKey))
                        {
                            shop.item[nextSlot].SetDefaults(ItemID.GoldenKey);
                            shop.item[nextSlot].value = Item.buyPrice(gold: 75);
                            nextSlot++;
                            shop.item[nextSlot].SetDefaults(ItemID.LockBox);
                            shop.item[nextSlot].value = Item.buyPrice(gold: 75);
                            nextSlot++;
                        }
                        else
                        {
                            shop.item[nextSlot].SetDefaults(ItemID.WoodenCrate);
                            shop.item[nextSlot].value = Item.buyPrice(gold: 10);
                            nextSlot++;
                            shop.item[nextSlot].SetDefaults(ItemID.IronCrate);
                            shop.item[nextSlot].value = Item.buyPrice(gold: 25);
                            nextSlot++;
                        }
                        if (NPC.downedBoss2 && AirHunterWorldData.SellGoldCrate)
                        {
                            shop.item[nextSlot].SetDefaults(ItemID.GoldenCrate);
                            shop.item[nextSlot].value = Item.buyPrice(gold: 75);
                            nextSlot++;
                        }
                        else
                        {
                            shop.item[nextSlot].SetDefaults(ItemID.FloatingIslandFishingCrate);
                            shop.item[nextSlot].value = Item.buyPrice(gold: 75);
                            nextSlot++;
                        }
                    }
                }
                switch (AirHunterWorldData.MaterialSold)
                {
                    case 0:
                        {
                            if (Main.time * 2.0 > Main.dayLength)
                            {
                                shop.item[nextSlot].SetDefaults(ItemID.Vertebrae);
                                shop.item[nextSlot].value = Item.buyPrice(silver: 20);
                                nextSlot++;
                            }
                            else
                            {
                                shop.item[nextSlot].SetDefaults(ItemID.RottenChunk);
                                shop.item[nextSlot].value = Item.buyPrice(silver: 20);
                                nextSlot++;
                            }
                        }
                        break;

                    case 1:
                        {
                            if (WorldGen.shadowOrbSmashed)
                            {
                                shop.item[nextSlot].SetDefaults(ItemID.Stinger);
                                shop.item[nextSlot].value = Item.buyPrice(silver: 50);
                                nextSlot++;
                            }
                        }
                        break;

                    case 2:
                        {
                            if (Main.hardMode && Main.time * 2.0 > Main.dayLength)
                            {
                                shop.item[nextSlot].SetDefaults(ItemID.PixieDust);
                                shop.item[nextSlot].value = Item.buyPrice(silver: 50);
                                nextSlot++;
                            }
                            else if (NPC.downedBoss3)
                            {
                                shop.item[nextSlot].SetDefaults(ItemID.Bone);
                                shop.item[nextSlot].value = Item.buyPrice(silver: 50);
                                nextSlot++;
                            }
                        }
                        break;

                    case 3:
                        {
                            if (Main.time * 2.0 > Main.dayLength)
                            {
                                shop.item[nextSlot].SetDefaults(ItemID.SharkFin);
                                shop.item[nextSlot].value = Item.buyPrice(gold: 5);
                                nextSlot++;
                            }
                            else
                            {
                                shop.item[nextSlot].SetDefaults(ModContent.ItemType<CrabShell>());
                                shop.item[nextSlot].value = Item.buyPrice(silver: 20);
                                nextSlot++;
                            }
                        }
                        break;

                    case 4:
                        {
                            if (Main.time * 3.0 > Main.dayLength * 2.0)
                            {
                                shop.item[nextSlot].SetDefaults(ModContent.ItemType<KryptonMushroom>());
                                shop.item[nextSlot].value = Item.buyPrice(gold: 1);
                                nextSlot++;
                            }
                            else if (Main.time * 3.0 > Main.dayLength)
                            {
                                shop.item[nextSlot].SetDefaults(ModContent.ItemType<ArgonMushroom>());
                                shop.item[nextSlot].value = Item.buyPrice(gold: 1);
                                nextSlot++;
                            }
                            else
                            {
                                shop.item[nextSlot].SetDefaults(ModContent.ItemType<XenonMushroom>());
                                shop.item[nextSlot].value = Item.buyPrice(gold: 1);
                                nextSlot++;
                            }
                        }
                        break;
                }
                if (player.ZoneCorrupt && AirHunterWorldData.MaterialSold != 0)
                {
                    if (Main.time * 2.0 > Main.dayLength)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.Vertebrae);
                        shop.item[nextSlot].value = Item.buyPrice(silver: 20);
                        nextSlot++;
                    }
                    else
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.RottenChunk);
                        shop.item[nextSlot].value = Item.buyPrice(silver: 20);
                        nextSlot++;
                    }
                }
                else if (player.ZoneCrimson && AirHunterWorldData.MaterialSold != 0)
                {
                    if (Main.time * 2.0 > Main.dayLength)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.RottenChunk);
                        shop.item[nextSlot].value = Item.buyPrice(silver: 20);
                        nextSlot++;
                    }
                    else
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.Vertebrae);
                        shop.item[nextSlot].value = Item.buyPrice(silver: 20);
                        nextSlot++;
                    }
                }
                else if (player.ZoneJungle && WorldGen.shadowOrbSmashed)
                {
                    if (AirHunterWorldData.MaterialSold == 0)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.Stinger);
                        shop.item[nextSlot].value = Item.buyPrice(silver: 50);
                        nextSlot++;
                    }
                    else if (AirHunterWorldData.MaterialSold == 2)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.Vine);
                        shop.item[nextSlot].value = Item.buyPrice(silver: 50);
                        nextSlot++;
                    }
                    else
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.JungleSpores);
                        shop.item[nextSlot].value = Item.buyPrice(silver: 50);
                        nextSlot++;
                    }
                }
                else if (player.ZoneHoly && Main.hardMode)
                {
                    if (AirHunterWorldData.MaterialSold == 2)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.UnicornHorn);
                        shop.item[nextSlot].value = Item.buyPrice(gold: 5);
                        nextSlot++;
                    }
                    else
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.PixieDust);
                        shop.item[nextSlot].value = Item.buyPrice(silver: 50);
                        nextSlot++;
                    }
                }
                else if (WorldDefeats.DownedStarite && Main.time * 3.0 > Main.dayLength * 2.0)
                {
                    shop.item[nextSlot].SetDefaults(ItemID.FallenStar);
                    shop.item[nextSlot].value = Item.buyPrice(gold: 1);
                    nextSlot++;
                }

                if (Main.time * 4.0 < Main.dayLength)
                {
                    shop.item[nextSlot].SetDefaults(ItemID.Feather);
                    shop.item[nextSlot].value = Item.buyPrice(silver: 50);
                    nextSlot++;
                }
            }

            if (!AirHunterWorldData.SettingUpShopStealing && NPC.CountNPCS(npc.type) == 1 && AirHunterWorldData.MerchantStealSeed != -1)
            {
                var value = StealShop((AirHunterWorldData.MerchantStealSeed * 6969).Abs(), npc.whoAmI);
                if (value != null)
                {
                    if (value.shopCustomPrice != null)
                        value.shopCustomPrice = (int)(value.shopCustomPrice.Value * 0.5f);
                    else
                    {
                        value.value = (int)(value.value * 0.5f);
                    }
                    shop.item[nextSlot] = value;
                    nextSlot++;
                }

                value = StealShop((AirHunterWorldData.MerchantStealSeed * 420420).Abs(), npc.whoAmI);
                if (value != null)
                {
                    if (value.shopCustomPrice != null)
                        value.shopCustomPrice = (int)(value.shopCustomPrice.Value * 0.75f);
                    else
                    {
                        value.value = (int)(value.value * 0.75f);
                        value.shopCustomPrice = value.value;
                    }
                    shop.item[nextSlot] = value;
                    nextSlot++;
                }

                value = StealShop(AirHunterWorldData.MerchantStealSeed, npc.whoAmI);
                if (value != null)
                {
                    shop.item[nextSlot] = value;
                    nextSlot++;
                }
            }

            if (NPC.downedBoss2 || (Main.dayTime && Main.time * 4.0 > Main.dayLength * 3.0))
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Armor.SteelPlatedChestplate>());
                nextSlot++;
            }

            if (WorldDefeats.DownedCrabson)
            {
                shop.item[nextSlot].SetDefaults(ItemID.PeaceCandle);
                shop.item[nextSlot].value = Item.buyPrice(gold: 10);
                nextSlot++;
            }
            if (NPC.downedBoss3)
            {
                shop.item[nextSlot].SetDefaults(ItemID.WaterCandle);
                shop.item[nextSlot].value = Item.buyPrice(gold: 10);
                nextSlot++;
            }
        }

        private static Item StealShop(int seed, int whoAmI, int npc = -1)
        {
            var potentialShops = new List<Chest>();
            if (npc == -1)
                npc = ModContent.NPCType<BalloonMerchant>();
            AirHunterWorldData.SettingUpShopStealing = true;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (i != whoAmI && Main.npc[i].type != npc && Main.npc[i].active && Main.npc[i].townNPC)
                {
                    if (Main.npc[i].type > Main.maxNPCTypes)
                    {
                        try
                        {
                            var tempShop = new Chest();
                            tempShop.SetupShop(Main.npc[i].type);
                            if (tempShop.item[0].type == ItemID.None)
                                continue;
                            potentialShops.Add(tempShop);
                        }
                        catch
                        {
                            break;
                        }
                    }
                    else
                    {
                        int shopID = ShopID.GetShopFromNPCID(Main.npc[i].type);
                        if (shopID != -1)
                        {
                            try
                            {
                                var tempShop = new Chest();
                                tempShop.SetupShop(shopID);
                                if (tempShop.item[0].type == ItemID.None)
                                    continue;
                                potentialShops.Add(tempShop);
                            }
                            catch
                            {
                                break;
                            }
                        }
                    }
                }
            }
            AirHunterWorldData.SettingUpShopStealing = false;
            var shopStealRand = new UnifiedRandom(seed);
            Chest stealShop = null;
            if (potentialShops.Count > 1)
                stealShop = potentialShops[shopStealRand.Next(potentialShops.Count)];
            else if (potentialShops.Count == 1)
            {
                stealShop = potentialShops[0];
            }
            if (stealShop != null)
            {
                var potentialItems = new List<Item>();
                for (int i = 0; i < Chest.maxItems; i++)
                {
                    if (stealShop.item[i].type > ItemID.None && stealShop.item[i].stack > 0 && stealShop.item[i].shopSpecialCurrency == CustomCurrencyID.None)
                        potentialItems.Add(stealShop.item[i]);
                }
                return potentialItems[shopStealRand.Next(potentialItems.Count)].Clone();
            }
            return null;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int dustAmount = npc.life > 0 ? 1 : 5;
            for (int k = 0; k < dustAmount; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood);
            }
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return false;
        }

        public override string TownNPCName()
        {
            switch (WorldGen.genRand.Next(25))
            {
                default:
                    return "Link";
                case 0:
                    return "Buddy";
                case 1:
                    return "Dobby";
                case 2:
                    return "Winky";
                case 3:
                    return "Hermey";
                case 4:
                    return "Altmer";
                case 5:
                    return "Summerset";
                case 6:
                    return "Calcelmo";
                case 7:
                    return "Ancano";
                case 8:
                    return "Nurelion";
                case 9:
                    return "Vingalmo";
                case 10:
                    return "Bosmer";
                case 11:
                    return "Faendal";
                case 12:
                    return "Malborn";
                case 13:
                    return "Niruin";
                case 14:
                    return "Enthir";
                case 15:
                    return "Dunmer";
                case 16:
                    return "Aranea";
                case 17:
                    return "Ienith";
                case 18:
                    return "Brand-Shei";
                case 19:
                    return "Telvanni";
                case 20:
                    return "Jenassa";
                case 21:
                    return "Erandur";
                case 22:
                    return "Neloth";
                case 23:
                    return "Gelebor";
                case 24:
                    return "Vyrthur";
            }
        }

        public override bool CheckActive() => false;

        public override bool CanChat() => true;


        public override string GetChat()
        {
            if (!EventGaleStreams.IsActive)
                return Language.GetTextValue("Mods.AQMod.BalloonMerchant.Chat.Leaving." + Main.rand.Next(3));
            if (!WorldDefeats.HunterIntroduction)
            {
                WorldDefeats.HunterIntroduction = true;
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetHelper.UpdateFlag(NetHelper.PacketType.Flag_AirHunterIntroduction);
                return Language.GetTextValue("Mods.AQMod.BalloonMerchant.Chat.Introduction", npc.GivenName);
            }
            var potentialText = new List<string>();
            var player = Main.LocalPlayer;
            if (player.ZoneHoly)
                potentialText.Add("BalloonMerchant.Chat.Hallow");
            else if (player.ZoneCorrupt)
            {
                return Language.GetTextValue("Mods.AQMod.BalloonMerchant.Chat.Corruption");
            }
            else if (player.ZoneCrimson)
            {
                return Language.GetTextValue("Mods.AQMod.BalloonMerchant.Chat.Crimson");
            }

            potentialText.Add("BalloonMerchant.Chat.0");
            potentialText.Add("BalloonMerchant.Chat.1");
            potentialText.Add("BalloonMerchant.Chat.2");
            potentialText.Add("BalloonMerchant.Chat.3");
            potentialText.Add("BalloonMerchant.Chat.Vraine");
            potentialText.Add("BalloonMerchant.Chat.StreamingBalloon");
            potentialText.Add("BalloonMerchant.Chat.WhiteSlime");

            if (WorldDefeats.SudoHardmode)
            {
                potentialText.Add("BalloonMerchant.Chat.RedSprite");
                potentialText.Add("BalloonMerchant.Chat.SpaceSquid");
            }

            if (EventGaleStreams.MeteorTime())
                potentialText.Add("BalloonMerchant.Chat.MeteorTime");

            string chosenText = potentialText[Main.rand.Next(potentialText.Count)];
            string text = Language.GetTextValue("Mods.AQMod." + chosenText);
            if (text == "Mods.AQMod." + chosenText)
                return chosenText;
            return text;
        }

        private bool IsOffscreen()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && (player.Center - npc.Center).Length() < 1250f)
                    return false;
            }
            return true;
        }

        public override bool PreAI()
        {
            npc.homeless = true;
            if (currentAction != 7)
            {
                return false;
            }
            npc.townNPC = true;
            return true;
        }

        public override void PostAI()
        {
            bool offscreen = IsOffscreen();
            if (npc.life < 80 && !npc.dontTakeDamage)
            {
                if (currentAction == 7)
                    currentAction = -4;
                else
                {
                    currentAction = -3;
                }
                npc.ai[0] = 0f;
                npc.noGravity = true;
                npc.noTileCollide = true;
                npc.dontTakeDamage = true;
                if (npc.velocity.X <= 0)
                {
                    npc.direction = -1;
                    npc.spriteDirection = npc.direction;
                }
                else
                {
                    npc.direction = 1;
                    npc.spriteDirection = npc.direction;
                }
                if (Main.netMode != NetmodeID.Server)
                    AQSound.Play(SoundType.Item, "slidewhistle", npc.Center, 0.5f);
            }
            else if (!offscreen)
            {
                WorldDefeats.AirMerchantHasBeenFound = true;
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetHelper.UpdateFlag(NetHelper.PacketType.Flag_AirMerchantHasBeenFound);
                }
                npc.netUpdate = true;
            }
            if (currentAction == -4)
            {
                if ((int)npc.ai[0] == 0)
                {
                    npc.ai[0]++;
                    npc.velocity.Y = -6f;
                }
                else
                {
                    npc.velocity.Y += 0.3f;
                }
                if (offscreen)
                {
                    npc.active = false;
                    npc.netSkip = -1;
                    npc.life = 0;
                }
                return;
            }
            else if (currentAction == -3)
            {
                npc.velocity.Y -= 0.6f;
                npc.noGravity = true;
                npc.noTileCollide = true;
                if (offscreen)
                {
                    npc.active = false;
                    npc.netSkip = -1;
                    npc.life = 0;
                }
                return;
            }
            if (!_init)
            {
                _init = true;
                if (EventGaleStreams.IsActive)
                {
                    bool notInTown = true;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].townNPC && (npc.Center - Main.npc[i].Center).Length() < 1200f)
                        {
                            SetToTownNPC();
                            notInTown = false;
                            break;
                        }
                    }
                    if (notInTown)
                        SetToBalloon();
                }
            }
            if (!EventGaleStreams.IsActive && offscreen)
            {
                npc.active = false;
                npc.netSkip = -1;
                npc.life = 0;
                return;
            }
            if (npc.position.X <= 240f || npc.position.X + npc.width > Main.maxTilesX * 16f - 240f
                || (currentAction == 7 && offscreen && Main.rand.NextBool(1500)))
            {
                AirHunterWorldData.SpawnMerchant(npc.whoAmI);
                return;
            }

            if (currentAction == -1)
                SetToBalloon();
            if (currentAction == -2)
            {
                npc.noGravity = true;
                if (offscreen)
                    npc.noTileCollide = true;
                else if (npc.noTileCollide && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                {
                    npc.noTileCollide = false;
                }
                bool canSwitchDirection = true;
                if (npc.position.Y > 3600f)
                {
                    currentAction = -3;
                    npc.netUpdate = true;
                }
                else if (npc.position.Y > 3000f)
                {
                    npc.velocity.Y -= 0.0125f;
                }
                else if (npc.position.Y < 1600)
                {
                    npc.velocity.Y += 0.0125f;
                }
                else
                {
                    if (npc.velocity.Y.Abs() > 3f)
                        npc.velocity.Y *= 0.99f;
                    else
                    {
                        npc.velocity.Y += Main.rand.NextFloat(-0.005f, 0.005f) + npc.velocity.Y * 0.0025f;
                    }
                    bool foundStoppingSpot = false;
                    if (EventGaleStreams.IsActive)
                    {
                        if (!npc.noTileCollide)
                        {
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                if (Main.player[i].active && !Main.player[i].dead && (npc.Center - Main.player[i].Center).Length() < 150f)
                                {
                                    npc.velocity.Y *= 0.94f;
                                    npc.velocity.X *= 0.96f;
                                    foundStoppingSpot = true;
                                    break;
                                }
                            }
                        }
                        if (npc.ai[0] <= 0f)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].townNPC && (npc.Center - Main.npc[i].Center).Length() < 800f)
                                {
                                    if (offscreen)
                                    {
                                        npc.position.X = Main.npc[i].position.X + (Main.npc[i].width - npc.width);
                                        npc.position.Y = Main.npc[i].position.Y + (Main.npc[i].height - npc.height);
                                        SetToTownNPC();
                                    }
                                    else if (!npc.noTileCollide)
                                    {
                                        foundStoppingSpot = true;
                                        npc.velocity.Y *= 0.92f;
                                        npc.velocity.X *= 0.92f;
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            npc.ai[0]--;
                        }
                    }
                    if (!foundStoppingSpot)
                    {
                        float windSpeed = Math.Max(Main.windSpeed.Abs() * 3f, 1.5f) * Math.Sign(Main.windSpeed);
                        if (windSpeed < 0f)
                        {
                            if (npc.velocity.X > windSpeed)
                                npc.velocity.X -= 0.025f;
                        }
                        else
                        {
                            if (npc.velocity.X < windSpeed)
                                npc.velocity.X += 0.025f;
                        }
                    }
                    else
                    {
                        canSwitchDirection = false;
                    }
                }

                if (canSwitchDirection)
                {
                    if (npc.spriteDirection == _oldSpriteDirection)
                    {
                        if (npc.velocity.X <= 0)
                        {
                            npc.direction = -1;
                            npc.spriteDirection = npc.direction;
                        }
                        else
                        {
                            npc.direction = 1;
                            npc.spriteDirection = npc.direction;
                        }
                    }
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.townNPC = false;
        }

        private void SetToBalloon()
        {
            currentAction = -2;
            npc.velocity = Vector2.Normalize(Main.MouseWorld - npc.Center);
            if (npc.velocity.X <= 0)
                npc.spriteDirection = -1;
            else
            {
                npc.spriteDirection = 1;
            }
            _oldSpriteDirection = npc.spriteDirection;
            //npc.velocity = new Vector2(Main.rand.NextFloat(1f, 2.5f), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
            npc.netUpdate = true;
            npc.ai[0] = 1000f;
            npc.ai[1] = 0f;
            npc.ai[2] = 0f;
            npc.ai[3] = 0f;
            npc.localAI[0] = 0f;
            npc.localAI[1] = 0f;
            npc.localAI[2] = 0f;
            npc.localAI[3] = 0f;
        }

        private void SetToTownNPC()
        {
            currentAction = 7;
            npc.noTileCollide = false;
            npc.noGravity = false;
            npc.netUpdate = true;
            npc.velocity.X = 0f;
            npc.velocity.Y = 0f;
            npc.ai[0] = 0f;
            npc.ai[1] = 0f;
            npc.ai[2] = 0f;
            npc.ai[3] = 0f;
            npc.localAI[0] = 0f;
            npc.localAI[1] = 0f;
            npc.localAI[2] = 0f;
            npc.localAI[3] = 0f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (currentAction == -4)
            {
                var texture = ModContent.GetTexture(this.GetPath("_Flee"));
                var frame = new Rectangle(0, texture.Height / 2 * ((int)(Main.GlobalTime * 10f) % 2), texture.Width, texture.Height / 2);
                var effects = SpriteEffects.None;
                if (npc.spriteDirection == 1)
                    effects = SpriteEffects.FlipHorizontally;
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, frame, drawColor, 0f, frame.Size() / 2f, 1f, effects, 0f);
                return false;
            }
            if (currentAction != 7)
            {
                var texture = ModContent.GetTexture(this.GetPath("_Basket"));
                int frameX = -1;
                if (npc.spriteDirection != _oldSpriteDirection)
                {
                    _balloonFrameCounter++;
                    if (_balloonFrameCounter > 4)
                    {
                        _balloonFrameCounter = 0;
                        if (_oldSpriteDirection == -1)
                        {
                            if (_balloonFrame < 5 || _balloonFrame > 23)
                                _balloonFrame = 5;
                            else
                            {
                                _balloonFrame++;
                            }
                            if (_balloonFrame > 23)
                            {
                                _oldSpriteDirection = npc.spriteDirection;
                                _balloonFrame = 37;
                            }
                        }
                        else
                        {
                            _balloonFrame++;
                            if (_balloonFrame < 41)
                                _balloonFrame = 41;
                            if (_balloonFrame > 59)
                            {
                                _oldSpriteDirection = npc.spriteDirection;
                                _balloonFrame = 1;
                            }
                        }
                    }
                }
                else
                {
                    if (npc.spriteDirection == 1)
                    {
                        if (_balloonFrame < 37)
                        {
                            _balloonFrame = 37;
                            frameX = _balloonFrame / 18;
                        }
                        _balloonFrameCounter++;
                        if (_balloonFrameCounter > 20)
                        {
                            _balloonFrameCounter = 0;
                            _balloonFrame++;
                            if (_balloonFrame > 40)
                                _balloonFrame = 37;
                        }
                    }
                    else
                    {
                        if (_balloonFrame < 1)
                        {
                            _balloonFrame = 1;
                            frameX = 0;
                        }
                        _balloonFrameCounter++;
                        if (_balloonFrameCounter > 20)
                        {
                            _balloonFrameCounter = 0;
                            _balloonFrame++;
                            if (_balloonFrame > 4)
                                _balloonFrame = 1;
                        }
                    }
                }
                if (frameX == -1)
                    frameX = _balloonFrame / 18;
                if (_balloonColor == 0)
                    _balloonColor = Main.rand.Next(5) + 1;
                var frame = new Rectangle(texture.Width / 4 * frameX, texture.Height / 18 * (_balloonFrame % 18), texture.Width / 4, texture.Height / 18);
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, frame, drawColor, 0f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);

                float yOff = frame.Height / 2f;
                texture = ModContent.GetTexture(this.GetPath("_Balloon"));
                frame = new Rectangle(0, texture.Height / 5 * (_balloonColor - 1), texture.Width, texture.Height / 5);
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, -yOff + 4f), frame, drawColor, 0f, new Vector2(frame.Width / 2f, frame.Height), 1f, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(currentAction);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            currentAction = reader.ReadInt32();
        }

        public override bool CanGoToStatue(bool toKingStatue)
        {
            return toKingStatue;
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 8f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 12;
            randExtraCooldown = 20;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ProjectileID.PoisonedKnife;
            attackDelay = 10;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 12f;
            randomOffset = 2f;
        }

        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            crit = false;
            if (damage > 79)
                damage = 79;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            crit = false;
            if (damage > 79)
                damage = 79;
        }

        public override bool CheckDead()
        {
            if (currentAction == -4 || currentAction == -3)
                return true;
            npc.ai[0] = 0f;
            if (currentAction == 7)
                currentAction = -4;
            else
            {
                currentAction = -3;
            }
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.life = npc.lifeMax;
            if (Main.netMode != NetmodeID.Server)
                AQSound.Play(SoundType.Item, "slidewhistle", npc.Center, 0.5f);
            if (npc.velocity.X <= 0)
            {
                npc.direction = -1;
                npc.spriteDirection = npc.direction;
            }
            else
            {
                npc.direction = 1;
                npc.spriteDirection = npc.direction;
            }
            npc.dontTakeDamage = true;
            return false;
        }

        public static BalloonMerchant FindInstance()
        {
            int index = Find();
            if (index == -1)
                return null;
            return (BalloonMerchant)Main.npc[index].modNPC;
        }

        public static int Find()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<BalloonMerchant>())
                    return i;
            }
            return -1;
        }
    }
}