using Aequus.Buffs.Debuffs;
using Aequus.Common.Networking;
using Aequus.Graphics;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using ModGlobalsNet;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public class NPCDebuffs : GlobalNPC, IGlobalsNetworker
    {
        public bool hasCorruptionHellfire;
        public byte corruptionHellfireStacks;

        public bool hasCrimsonHellfire;
        public byte crimsonHellfireStacks;

        public bool hasLocust;
        public byte locustStacks;

        public override bool InstancePerEntity => true;

        public override void ResetEffects(NPC npc)
        {
            hasCorruptionHellfire = false;
            hasCrimsonHellfire = false;
            hasLocust = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            UpdateStack(npc, ref hasLocust, ref locustStacks, ref damage, 20, 1f);
            UpdateStack(npc, ref hasCrimsonHellfire, ref crimsonHellfireStacks, ref damage, 20, 1.1f);
            UpdateStack(npc, ref hasCorruptionHellfire, ref corruptionHellfireStacks, ref damage, 20, 1f);
        }
        public void UpdateStack(NPC npc, ref bool has, ref byte stacks, ref int damageNumbers, byte cap = 20, float dotMultiplier = 1f)
        {
            if (!has)
            {
                stacks = 0;
            }
            else
            {
                stacks = Math.Min(stacks, cap);
                int dot = (int)(stacks * dotMultiplier);

                if (dot >= 0)
                {
                    npc.AddRegen(-dot);
                    if (damageNumbers < dot)
                        damageNumbers = dot;
                }
            }
        }

        public override void PostAI(NPC npc)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            if (hasCorruptionHellfire)
            {
                int amt = (int)(npc.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                    EffectsSystem.BehindPlayers.Add(new BloomParticle(Main.rand.NextCircularFromRect(npc.getRect()), -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        CorruptionHellfire.FireColor, CorruptionHellfire.BloomColor, 1.25f, 0.3f));
            }
            if (hasCrimsonHellfire)
            {
                int amt = (int)(npc.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                    EffectsSystem.BehindPlayers.Add(new BloomParticle(Main.rand.NextCircularFromRect(npc.getRect()), -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        CrimsonHellfire.FireColor, CrimsonHellfire.BloomColor, 0.9f, 0.35f));
            }
        }

        public void Send(int whoAmI, BinaryWriter writer)
        {
            writer.Write(hasLocust);
            if (hasLocust)
            {
                writer.Write(locustStacks);
            }
            writer.Write(hasCorruptionHellfire);
            if (hasCorruptionHellfire)
            {
                writer.Write(corruptionHellfireStacks);
            }
            writer.Write(hasCrimsonHellfire);
            if (hasCrimsonHellfire)
            {
                writer.Write(crimsonHellfireStacks);
            }
        }

        public void Receive(int whoAmI, BinaryReader reader)
        {
            if (reader.ReadBoolean())
            {
                hasLocust = true;
                locustStacks = reader.ReadByte();
            }
            if (reader.ReadBoolean())
            {
                hasCorruptionHellfire = true;
                corruptionHellfireStacks = reader.ReadByte();
            }
            if (reader.ReadBoolean())
            {
                hasCrimsonHellfire = true;
                crimsonHellfireStacks = reader.ReadByte();
            }
        }

        public static void SyncDebuffs(int npc)
        {
            if (Main.npc[npc].TryGetGlobalNPC<NPCDebuffs>(out var debuffs))
            {
                PacketHandler.Send((p) => { p.Write((byte)npc); debuffs.Send(npc, p); }, PacketType.SyncDebuffs);
            }
        }
    }
}