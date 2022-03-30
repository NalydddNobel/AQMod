﻿using Aequus.Common;
using Aequus.Sounds;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public class FlawlessNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        public bool[] damagedPlayers;
        public bool preventNoHitCheck;

        public FlawlessNPC()
        {
            damagedPlayers = new bool[Main.maxPlayers];
        }

        public override GlobalNPC NewInstance(NPC npc)
        {
            return new FlawlessNPC();
        }

        private void ResetNoHit(int player)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active)
                    Main.npc[i].GetGlobalNPC<FlawlessNPC>().damagedPlayers[player] = false;
            }
        }

        public override void ResetEffects(NPC npc)
        {
            if (!preventNoHitCheck)
            {
                var manager = ModContent.GetInstance<FlawlessManager>();
                for (int i = 0; i < manager.DamagedPlayers.Count; i++)
                {
                    damagedPlayers[manager.DamagedPlayers[i]] = true;
                }
            }
        }

        public static bool HasBeenNoHit(NPC npc, int player)
        {
            return HasBeenNoHit(npc, npc.GetGlobalNPC<FlawlessNPC>(), player);
        }

        public static bool HasBeenNoHit(NPC npc, FlawlessNPC noHit, int player)
        {
            return npc.playerInteraction[player] && !noHit.damagedPlayers[player];
        }

        public static void PlayJingle(Vector2 position)
        {
            if (Vector2.Distance(position, Main.player[Main.myPlayer].Center) < 3000f)
            {
                SoundHelper.Play(SoundType.Sound, "nohit", position);
            }
        }
    }
}