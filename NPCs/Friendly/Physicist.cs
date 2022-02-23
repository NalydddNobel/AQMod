using AQMod.Common;
using AQMod.Common.ID;
using AQMod.Content.World.Events;
using AQMod.Items.Accessories;
using AQMod.Items.Boss.Summons;
using AQMod.Items.Tools;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.NPCs.Friendly
{
    [AutoloadHead()]
    public class Physicist : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 25;
            NPCID.Sets.ExtraFramesCount[npc.type] = 9;
            NPCID.Sets.AttackFrameCount[npc.type] = 4;
            NPCID.Sets.DangerDetectRange[npc.type] = 400;
            NPCID.Sets.AttackType[npc.type] = 0;
            NPCID.Sets.AttackTime[npc.type] = 10;
            NPCID.Sets.AttackAverageChance[npc.type] = 10;
            NPCID.Sets.HatOffsetY[npc.type] = 0;
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

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Stardrop>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<EquivalenceMachine>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Cosmicanon>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Accessories.FidgetSpinner.FidgetSpinner>());
            nextSlot++;
            if (Main.dayTime)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<TheFan>());
                nextSlot++;
            }
            else if (WorldDefeats.ObtainedUltimateSword)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Weapons.Melee.UltimateSword>());
                nextSlot++;
            }
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<MythicStarfruit>());
            nextSlot++;
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
            return WorldDefeats.DownedStarite;
        }

        public override string TownNPCName()
        {
            switch (WorldGen.genRand.Next(12))
            {
                default:
                    return "Rose";
                case 0:
                    return "Deahdeah";
                case 1:
                    return "Lina";
                case 2:
                    return "Peach";
                case 3:
                    return "Lumian";
                case 4:
                    return "Astrajanelon";
                case 5:
                    return "Astrablaghn";
                case 6:
                    return "Stoffien";
                case 7:
                    return "Constructa";
                case 8:
                    return "Eridani";
                case 9:
                    return "Asphodene";
                case 10:
                    return "Termina";
                case 11:
                    return "Kristal";
            }
        }

        public override string GetChat()
        {
            if (!WorldDefeats.PhysicistIntroduction)
            {
                WorldDefeats.PhysicistIntroduction = true;
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetHelper.UpdateFlag(NetHelper.PacketType.Flag_PhysicistIntroduction);
                return Language.GetTextValue("Mods.AQMod.Physicist.Chat.Introduction", npc.GivenName);
            }
            var potentialText = new List<string>();
            var player = Main.LocalPlayer;
            if (player.ZoneHoly)
            {
                potentialText.Add("Physicist.Chat.Hallow.0");
                potentialText.Add("Physicist.Chat.Hallow.1");
                potentialText.Add("Physicist.Chat.Hallow.2");
                potentialText.Add("Physicist.Chat.Hallow.3");
                potentialText.Add("Physicist.Chat.Hallow.4");
                potentialText.Add("Physicist.Chat.Hallow.5");
            }
            else if (player.ZoneCorrupt)
            {
                return Language.GetTextValue("Mods.AQMod.Physicist.Chat.Corruption");
            }
            else if (player.ZoneCrimson)
            {
                return Language.GetTextValue("Mods.AQMod.Physicist.Chat.Crimson");
            }
            else if (!player.ZoneJungle && !player.ZoneSnow && player.ZoneOverworldHeight)
            {
                potentialText.Add("Physicist.Chat.Purity.0");
                potentialText.Add("Physicist.Chat.Purity.1");
                potentialText.Add("Physicist.Chat.Purity.2");
            }
            potentialText.Add("Physicist.Chat.1");
            potentialText.Add("Physicist.Chat.2");
            potentialText.Add("Physicist.Chat.3");
            potentialText.Add("Physicist.Chat.4");
            potentialText.Add("Physicist.Chat.5");
            potentialText.Add("Physicist.Chat.7");
            potentialText.Add("Physicist.Chat.8");
            potentialText.Add(Language.GetTextValue("Mods.AQMod.Physicist.Chat.6", player.name));
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].townNPC && !string.IsNullOrEmpty(Main.npc[i].GivenName))
                {
                    potentialText.Add(Language.GetTextValue("Mods.AQMod.Physicist.Chat.9", player.name, Main.npc[i].GivenName));
                    break;
                }
            }
            if (player.armor[0].type > ItemID.None || player.armor[1].type > ItemID.None || player.armor[2].type > ItemID.None)
            {
                for (int i = 0; i < 50; i++)
                {
                    int index = Main.rand.Next(3); // chooses a random armor to use in the text
                    if (player.armor[index].type > ItemID.None)
                    {
                        potentialText.Add(Language.GetTextValue("Mods.AQMod.Physicist.Chat.0", player.name, Lang.GetItemName(player.armor[index].type).Value));
                        break;
                    }
                }
            }
            if (Main.bloodMoon)
            {
                potentialText.Add("Physicist.Chat.BloodMoon.0");
                potentialText.Add("Physicist.Chat.BloodMoon.1");
                potentialText.Add("Physicist.Chat.BloodMoon.2");
                if (WorldGen.crimson || WorldGen.tBlood > 0)
                    potentialText.Add("Physicist.Chat.BloodMoon.CrimsonWarning");
                if (Main.moonPhase == MoonPhases.NewMoon)
                    potentialText.Add("Physicist.Chat.BloodMoon.NewMoon");
                bool bunnyText = false;
                bool killText = false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active)
                    {
                        if (!bunnyText)
                        {
                            if (Main.npc[i].type == NPCID.CorruptBunny || Main.npc[i].type == NPCID.CrimsonBunny)
                            {
                                potentialText.Add("Physicist.Chat.BloodMoon.EvilBunny");
                                bunnyText = true;
                            }
                        }
                        if (!killText)
                        {
                            if (!Main.npc[i].friendly && !Main.npc[i].townNPC && Main.npc[i].lifeMax > 5 && Main.npc[i].damage > 0 && !Main.npc[i].dontTakeDamage) // chooses first hostile NPC
                            {
                                string name = Main.npc[i].FullName;
                                if (string.IsNullOrEmpty(name) || name == "???")
                                    continue;
                                potentialText.Add(Language.GetTextValue("Mods.AQMod.Physicist.Chat.BloodMoon.KillRequest." + Main.rand.Next(4), name, player.name));
                                killText = true;
                            }
                        }
                        if (bunnyText && killText)
                            break;
                    }
                }
            }
            if (Glimmer.SpawnsCheck(player))
            {
                potentialText.Add("Physicist.Chat.GlimmerEvent.0");
                potentialText.Add("Physicist.Chat.GlimmerEvent.1");
                potentialText.Add("Physicist.Chat.GlimmerEvent.2");
                potentialText.Add("Physicist.Chat.GlimmerEvent.3");
                if (Glimmer.Distance(player) > 1000)
                {
                    if (Glimmer.tileX < (int)(player.position.X + player.width / 2f) / 16f)
                        potentialText.Add("Physicist.Chat.GlimmerEvent.Source.West");
                    else
                    {
                        potentialText.Add("Physicist.Chat.GlimmerEvent.Source.East");
                    }
                }
            }
            string chosenText = potentialText[Main.rand.Next(potentialText.Count)];
            string text = Language.GetTextValue("Mods.AQMod." + chosenText);
            if (text == "Mods.AQMod." + chosenText)
                return chosenText;
            return text;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
                shop = true;
        }

        public override bool CanGoToStatue(bool toKingStatue)
        {
            return !toKingStatue;
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
            projType = ModContent.ProjectileType<Projectiles.Physicist>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 12f;
            randomOffset = 2f;
        }
    }
}