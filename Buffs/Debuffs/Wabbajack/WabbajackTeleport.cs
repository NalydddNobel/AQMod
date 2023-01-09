using Aequus.Projectiles.Magic;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs.Wabbajack
{
    public class WabbajackTeleport : ModBuff
    {
        public override string Texture => Aequus.Debuff;

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                var l = new List<NPC>();
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i == npc.whoAmI)
                        continue;

                    if (Main.npc[i].active && !npc.dontTakeDamage && !npc.immortal && npc.Distance(Main.npc[i].position) < 700f)
                    {
                        l.Add(Main.npc[i]);
                    }
                }
                if (l.Count > 0)
                {
                    var chosenNPC = Main.rand.Next(l);
                    Projectile.NewProjectile(npc.GetSource_Buff(buffIndex), chosenNPC.Bottom, Microsoft.Xna.Framework.Vector2.Zero,
                        ModContent.ProjectileType<WabbajackEffect>(), 0, 0f, Main.myPlayer);
                    Projectile.NewProjectile(chosenNPC.GetSource_Buff(buffIndex), npc.Bottom, Microsoft.Xna.Framework.Vector2.Zero,
                        ModContent.ProjectileType<WabbajackEffect>(), 0, 0f, Main.myPlayer);

                    var myBottom = npc.Bottom;
                    var myVelocity = npc.velocity;
                    npc.Bottom = chosenNPC.Bottom;
                    npc.velocity = chosenNPC.velocity;
                    chosenNPC.Bottom = myBottom;
                    chosenNPC.velocity = myVelocity;

                    chosenNPC.netUpdate = true;
                }
            }
            npc.DelBuff(buffIndex);
            buffIndex--;
            npc.netUpdate = true;
        }
    }
}