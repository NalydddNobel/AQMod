using AQMod.Common;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Tools;
using AQMod.Items.Weapons.Melee;
using AQMod.Items.Weapons.Summon;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.NPCs.Friendly
{
    [AutoloadHead()]
    public class Memorialist : ModNPC
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
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<GhostlyGrave>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<PowPunch>());
            shop.item[nextSlot].shopCustomPrice = AQItem.Prices.MemorialistWeaponBuyValue;
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<CursedKey>());
            shop.item[nextSlot].shopCustomPrice = AQItem.Prices.MemorialistWeaponBuyValue;
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<DemonicEnergy>());
            shop.item[nextSlot].shopCustomPrice = AQItem.Prices.EnergyBuyValue;
            nextSlot++;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int dustAmount = npc.life > 0 ? 1 : 5;
            for (int k = 0; k < dustAmount; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 65);
            }
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return WorldDefeats.DownedDemonSiege;
        }

        public override string TownNPCName()
        {
            switch (WorldGen.genRand.Next(12))
            {
                default:
                    return "Abaddon";
                case 0:
                    return "Cally";
                case 1:
                    return "Brimmy";
                case 2:
                    return "Beelzebub";
                case 3:
                    return "Lucy";
                case 4:
                    return "Sin";
                case 5:
                    return "Revenance";
                case 6:
                    return "Archvince";
                case 7:
                    return "Vincera";
                case 8:
                    return "Baron";
                case 9:
                    return "Spectre";
                case 10:
                    return "Heretic";
                case 11:
                    return "Maykr";
            }
        }

        public override string GetChat()
        {
            var potentialText = new List<string>();
            var player = Main.LocalPlayer;

            potentialText.Add("Memorialist.Chat.0");
            potentialText.Add("Memorialist.Chat.1");
            potentialText.Add("Memorialist.Chat.2");
            potentialText.Add("Memorialist.Chat.3");
            potentialText.Add("Memorialist.Chat.4");
            potentialText.Add("Memorialist.Chat.5");

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
            projType = ModContent.ProjectileType<Projectiles.Memorialist>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 6f;
            randomOffset = 1.5f;
        }
    }
}