using Aequus.Buffs;
using Aequus.Items.Accessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.GlobalNPCs
{
    public class LittleInfernoGlobalNPC : GlobalNPC
    {
        public bool active;

        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public override void SetDefaults(NPC npc)
        {
            active = false;
        }

        public override void AI(NPC npc)
        {
            if (!npc.SpawnedFromStatue && npc.justHit)
            {
                foreach (var b in AequusBuff.CountsAsFire)
                {
                    if (npc.HasBuff(b))
                    {
                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            if (Main.player[i].active && !Main.player[i].dead && npc.Distance(Main.player[i].Center) < 1000f)
                            {
                                if (Main.player[i].Aequus().accLittleInferno > 0)
                                {
                                    active = true;
                                }
                            }
                        }
                        break;
                    }
                }
            }
            if (active)
            {
                int player = npc.HasValidTarget ? npc.target : Player.FindClosest(npc.position, npc.width, npc.height);
                LittleInferno.InfernoPotionEffect(Main.player[player], npc.Center, npc.whoAmI);

                for (int i = 0; i < NPC.maxBuffs; i++)
                {
                    if (AequusBuff.CountsAsFire.Contains(npc.buffType[i]))
                    {
                        return;
                    }
                }
                active = false;
            }
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!npc.IsABestiaryIconDummy)
            {
                if (active)
                {
                    float opacity = 0.5f;
                    int time = 0;
                    for (int i = 0; i < NPC.maxBuffs; i++)
                    {
                        if (AequusBuff.CountsAsFire.Contains(npc.buffType[i]))
                        {
                            time = Math.Max(npc.buffTime[i], time);
                        }
                    }
                    if (time < 60)
                    {
                        opacity *= time / 60f;
                    }
                    LittleInferno.DrawInfernoRings(npc.Center - screenPos, opacity);
                }
            }
            return true;
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(active);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            active = bitReader.ReadBit();
        }
    }
}