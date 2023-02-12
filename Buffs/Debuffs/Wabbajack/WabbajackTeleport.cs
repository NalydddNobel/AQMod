using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs.Wabbajack
{
    public class WabbajackTeleport : ModBuff
    {
        public override string Texture => Aequus.PlaceholderDebuff;

        private static Vector2 GetTeleportPosition(NPC me, NPC to)
        {
            return to.Bottom + new Vector2(-me.width / 2f, -me.height);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                var l = new List<NPC>();
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i == npc.whoAmI)
                        continue;

                    if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].immortal && (Main.npc[i].realLife == -1 || Main.npc[i].realLife == Main.npc[i].whoAmI)
                        && !NPCID.Sets.TeleportationImmune[Main.npc[i].type] && npc.Distance(Main.npc[i].position) < 700f)
                    {
                        l.Add(Main.npc[i]);
                    }
                }
                if (l.Count > 0)
                {
                    var chosenNPC = Main.rand.Next(l);
                    Projectile.NewProjectile(npc.GetSource_Buff(buffIndex), chosenNPC.Bottom, Vector2.Zero,
                        ModContent.ProjectileType<WabbajackEffect>(), 0, 0f, Main.myPlayer);
                    Projectile.NewProjectile(chosenNPC.GetSource_Buff(buffIndex), npc.Bottom, Vector2.Zero,
                        ModContent.ProjectileType<WabbajackEffect>(), 0, 0f, Main.myPlayer);

                    var chosenNPCTeleportLocation = GetTeleportPosition(chosenNPC, npc);
                    var myVelocity = npc.velocity;
                    npc.Teleport(GetTeleportPosition(npc, chosenNPC), -1);
                    npc.velocity = chosenNPC.velocity;
                    chosenNPC.Teleport(chosenNPCTeleportLocation, -1);
                    chosenNPC.velocity = myVelocity;

                    chosenNPC.netUpdate = true;
                    npc.AddBuff(ModContent.BuffType<WabbajackEffectParticles>(), 400);
                    chosenNPC.AddBuff(ModContent.BuffType<WabbajackEffectParticles>(), 400);
                }
            }
            npc.DelBuff(buffIndex);
            buffIndex--;
            npc.netUpdate = true;
        }
    }
}