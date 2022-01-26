using AQMod.Content.Players;
using AQMod.Items.Armor;
using AQMod.Items.Materials;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.NPCs.Friendly
{
    [AutoloadBossHead]
    public class HermitCrab : ModNPC
    {
        public static bool InUIStorageShop { get; internal set; } = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 15;
        }

        public override void SetDefaults()
        {
            npc.width = 24;
            npc.height = 24;
            npc.lifeMax = 100;
            npc.knockBackResist = 0.002f;
            npc.aiStyle = 0;
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath8;
            npc.friendly = true;
            npc.gfxOffY = -2;
            npc.behindTiles = true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, npc.velocity.X, npc.velocity.X, 0, default(Color), 1.25f);
                }
                var center = npc.Center;
                var offset = new Vector2(6f * npc.direction, 0f);
                int type = mod.GetGoreSlot("Gores/HermitCrab_2");
                Gore.NewGore(center + offset, npc.velocity, type);
                Gore.NewGore(center + offset + new Vector2(2f * npc.direction, 0f), npc.velocity, type);
                Gore.NewGore(center + new Vector2(-8f * npc.direction, 0f), npc.velocity, mod.GetGoreSlot("Gores/HermitCrab_3"));
                Gore.NewGore(center, npc.velocity, mod.GetGoreSlot("Gores/HermitCrab_0"));
                Gore.NewGore(center, npc.velocity, mod.GetGoreSlot("Gores/HermitCrab_4"));
                switch ((int)npc.localAI[0])
                {
                    default:
                        Gore.NewGore(center, npc.velocity, mod.GetGoreSlot("Gores/HermitCrab_1"));
                        break;
                }
            }
            else
            {
                for (int i = 0; i < damage / 5; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, npc.velocity.X, npc.velocity.X, 0, default(Color), 0.9f);
                }
            }
        }

        public override bool CanChat() => true;

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = Language.GetTextValue("Mods.AQMod.HermitCrab.Storage");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            shop = true;
            InUIStorageShop = false;
            if (!firstButton)
            {
                InUIStorageShop = true;
            }
        }

        public override string GetChat()
        {
            InUIStorageShop = false;
            var potentialText = new List<string>();

            if (!Main.dayTime && Main.bloodMoon)
            {
                potentialText.Add("BloodMoon");
            }

            potentialText.Add("Chat.0");
            potentialText.Add("Chat.1");
            potentialText.Add("Chat.2");

            string chosenText = potentialText[Main.rand.Next(potentialText.Count)];
            string text = Language.GetTextValue("Mods.AQMod.HermitCrab." + chosenText);
            if (text == "Mods.AQMod.HermitCrab." + chosenText)
                return chosenText;
            return text;
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            if (InUIStorageShop)
            {
                var storage = Main.player[Main.myPlayer].GetModPlayer<PlayerStorage>();
                if (storage.HermitCrab == null)
                {
                    storage.HermitCrab = new PlayerStorage.HermitCrabStorage[PlayerStorage.MaxHermitCrabStorageItems];
                }
                for (int i = 0; i < storage.HermitCrab.Length && nextSlot < Chest.maxItems; i++)
                {
                    if (!storage.HermitCrab[i].IsAir)
                    {
                        shop.item[nextSlot] = storage.HermitCrab[i].Item;
                        shop.item[nextSlot].buyOnce = true;
                        nextSlot++;
                    }
                }
            }
            else
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<CrabShell>());
                nextSlot++;
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<HermitShell>());
                nextSlot++;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.Distance(Main.player[Player.FindClosest(npc.position, npc.width, npc.height)].Center) < 200f)
            {
                npc.frameCounter++;
                if (npc.frameCounter > 3.0 && npc.frame.Y < (frameHeight * (Main.npcFrameCount[npc.type] - 1)))
                {
                    npc.frameCounter = 0.0;
                    npc.frame.Y += frameHeight;
                }
            }
            else
            {
                npc.frameCounter++;
                if (npc.frameCounter > 2.0 && npc.frame.Y > 0)
                {
                    npc.frameCounter = 0.0;
                    npc.frame.Y -= frameHeight;
                    if (npc.frame.Y < 0)
                    {
                        npc.frame.Y = 0;
                    }
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.GetModPlayer<PlayerBiomes>().zoneCrabCrevice && !NPC.AnyNPCs(ModContent.NPCType<HermitCrab>()))
                return 0.08f;
            return 0f;
        }
    }
}