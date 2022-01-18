using AQMod.Assets;
using AQMod.Common;
using AQMod.Content.Quest.Lobster;
using AQMod.Content.World.Events.GlimmerEvent;
using AQMod.Items.Placeable.CraftingStations;
using AQMod.Items.Placeable.Furniture;
using AQMod.Items.Potions;
using AQMod.Items.Tools;
using AQMod.Items.Tools.MagicPowders;
using AQMod.Items.Weapons.Melee;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.NPCs.Friendly
{
    [AutoloadHead()]
    public class Robster : ModNPC
    {
        private static byte _resetQuest;

        public static Color JeweledTileMapColor => new Color(255, 185, 25, 255);
        public static Color RobsterBroadcastMessageColor => new Color(255, 215, 105, 255);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 25;
            NPCID.Sets.ExtraFramesCount[npc.type] = 9;
            NPCID.Sets.AttackFrameCount[npc.type] = 4;
            NPCID.Sets.DangerDetectRange[npc.type] = 400;
            NPCID.Sets.AttackType[npc.type] = 3; // -1 is none? 0 is shoot, 1 is magic shoot?, 2 is dryad aura, 3 is melee
            NPCID.Sets.AttackTime[npc.type] = 10;
            NPCID.Sets.AttackAverageChance[npc.type] = 10;
            NPCID.Sets.HatOffsetY[npc.type] = 8;

            Initialize();
        }

        internal static void Initialize()
        {
            _resetQuest = 0;
        }

        public override void SetDefaults()
        {
            npc.townNPC = true;
            npc.friendly = true;
            npc.width = 18;
            npc.height = 40;
            npc.aiStyle = 7;
            npc.damage = 10;
            npc.defense = 15;
            npc.lifeMax = 250;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.5f;
            animationType = NPCID.Guide;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int dustAmount = npc.life > 0 ? 1 : 5;
            for (int k = 0; k < dustAmount; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood);
            }
        }

        public override void AI()
        {
            npc.breath = 200;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return WorldDefeats.DownedCrabson;
        }

        public override string TownNPCName()
        {
            switch (WorldGen.genRand.Next(12))
            {
                default:
                    return "Larry";
                case 0:
                    return "Ronald";
                case 1:
                    return "Captain";
                case 2:
                    return "Crabort";
                case 3:
                    return "Robson";
                case 4:
                    return "Geezer";
                case 5:
                    return "Albrecht";
                case 6:
                    return "Eugene";
                case 7:
                    return "Utagawa";
                case 8:
                    return "Ebirah";
                case 9:
                    return "Tamatoa";
                case 10:
                    return "Crablante";
                case 11:
                    return "Robster";
            }
        }

        public override string GetChat()
        {
            try
            {
                if (ModLoader.GetMod("StarlightRiver") != null)
                {
                    npc.life = -1;
                    npc.HitEffect();
                    npc.active = false; // 😎
                }
            }
            catch
            {
            }

            _resetQuest = 0;
            var potentialText = new List<string>();
            int angler = NPC.FindFirstNPC(NPCID.Angler);

            if (BirthdayParty.GenuineParty || BirthdayParty.ManualParty)
                potentialText.Add(AQText.RobsterChat(12).Value);

            if (Main.bloodMoon)
            {
                potentialText.Add(AQText.RobsterChat(7).Value);
                potentialText.Add(AQText.RobsterChat(8).Value);
                if (Main.hardMode)
                {
                    if (Main.moonPhase % 2 == 0)
                        potentialText.Add(AQText.RobsterChat(9).Value);
                    else
                    {
                        potentialText.Add(AQText.RobsterChat(10).Value);
                    }
                    if (angler != -1)
                    {
                        string text = AQText.RobsterChat(11).Value;
                        potentialText.Add(string.Format(text, Main.npc[angler].GivenName));
                    }
                }
            }

            if (Main.eclipse)
            {
                potentialText.Add(AQText.RobsterChat(5).Value);
                if (NPC.downedGolemBoss)
                    potentialText.Add(AQText.RobsterChat(6).Value);
            }

            if (GlimmerEvent.IsGlimmerEventCurrentlyActive())
            {
                potentialText.Add(AQText.RobsterChat(2).Value);
                potentialText.Add(AQText.RobsterChat(3).Value);
                potentialText.Add(AQText.RobsterChat(4).Value);
            }

            potentialText.Add(AQText.RobsterChat(0).Value);
            potentialText.Add(AQText.RobsterChat(1).Value);
            return Language.GetTextValue(potentialText[Main.rand.Next(potentialText.Count)]);
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            if (_resetQuest != 0)
                button2 = AQText.ModText("Common.RobsterQuitHunt").Value;
            else
            {
                if (HuntSystem.Hunt != null && HuntSystem.Hunt.IsHuntComplete(Main.LocalPlayer))
                    button2 = AQText.ModText("Common.CompleteRobsterHunt").Value;
                else
                {
                    button2 = AQText.ModText("Common.RobsterHunt").Value;
                }
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
                shop = true;
            else
            {
                if (_resetQuest == 2)
                {
                    Main.PlaySound(SoundID.Grab);
                    if (HuntSystem.Hunt != null)
                        HuntSystem.Hunt.OnQuit(Main.player[Main.myPlayer]);
                    HuntSystem.QuitHunt(Main.LocalPlayer);
                    _resetQuest = 0;
                }
                if (_resetQuest == 1)
                    Main.npcChatText = AQText.ModText("Common.RobsterQuitHuntQuestion").Value;
                else
                {
                    if (HuntSystem.Hunt == null)
                    {
                        if (!HuntSystem.RandomizeHunt(Main.LocalPlayer))
                        {
                            Main.npcChatText = AQText.ModText("Common.RobsterCantDoRandomHunt").Value;
                            Main.npcChatCornerItem = 0;
                            return;
                        }
                    }
                    if (HuntSystem.Hunt.IsHuntComplete(Main.LocalPlayer))
                    {
                        Main.PlaySound(SoundID.Grab);
                        Main.LocalPlayer.ConsumeItem(HuntSystem.Hunt.GetQuestItem());

                        HuntSystem.Hunt.OnComplete(Main.LocalPlayer);
                        HuntSystem.Hunt.RemoveHunt();
                        HuntSystem.RandomizeHunt(Main.LocalPlayer);
                    }
                    else
                    {
                        string text = HuntSystem.Hunt.QuestChat();
                        Main.npcChatText = Language.GetTextValue(text, Lang.GetItemName(HuntSystem.Hunt.GetQuestItem()));
                        Main.npcChatCornerItem = HuntSystem.Hunt.GetQuestItem();
                        if (HuntSystem.TargetNPC != -1)
                        {
                            text = " " + AQText.ModText("Common.RobsterSawInSomeonesHouse");
                            Main.npcChatText += string.Format(text, Main.npc[HuntSystem.TargetNPC].GivenName);
                        }
                    }
                }
                _resetQuest++;
            }
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            if (Main.hardMode)
            {
                if (NPC.downedPirates)
                {
                    shop.item[nextSlot].SetDefaults(ItemID.DiscountCard);
                    nextSlot++;
                }
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<BlurryDiscountCard>());
                nextSlot++;
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<SpoilsPotion>());
                nextSlot++;
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<GoldPowder>());
                nextSlot++;
            }
            else
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<BlurryDiscountCard>());
                nextSlot++;
            }
            if (NPC.downedBoss3 && Main.moonPhase % 2 == 1)
            {
                shop.item[nextSlot].SetDefaults(ItemID.GoldenKey);
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 15);
                nextSlot++;
            }
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<FishingCraftingStation>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<CrabClock>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Materials.Energies.AquaticEnergy>());
            shop.item[nextSlot].shopCustomPrice = AQItem.Prices.EnergyBuyValue;
            nextSlot++;
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

        public override void DrawTownAttackSwing(ref Texture2D item, ref int itemSize, ref float scale, ref Vector2 offset)
        {
            item = TextureGrabber.GetItem(ModContent.ItemType<Crabsol>());
            itemSize = 40;
            scale = 0.5f;
            offset = new Vector2(0f, 0f);
        }

        public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
        {
            itemWidth = 30;
            itemHeight = 30;
        }
    }
}