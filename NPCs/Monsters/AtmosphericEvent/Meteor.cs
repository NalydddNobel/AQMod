using AQMod.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.AtmosphericEvent
{
    public class Meteor : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.width = 30;
            npc.height = 30;
            npc.lifeMax = 250;
            npc.damage = 2;
            npc.defense = 45;
            npc.HitSound = SoundID.NPCHit39;
            npc.DeathSound = SoundID.NPCDeath55;
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.knockBackResist = 0.3f;
            npc.value = Item.buyPrice(silver: 2);
            npc.npcSlots = 0.25f;
            for (int i = 0; i < npc.buffImmune.Length; i++)
            {
                npc.buffImmune[i] = true;
            }
        }

        public override void AI()
        {
            if ((int)npc.ai[0] == 0)
            {
                npc.ai[0] = 1f;
                npc.velocity = new Vector2(Main.rand.NextFloat(1f, 2.5f), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                npc.localAI[0] = Main.rand.Next(Main.npcFrameCount[npc.type]) + 1f;
            }
            if (npc.position.Y > 3200f)
            {
                npc.noGravity = false;
                if (npc.collideX || npc.collideY)
                {
                    npc.TargetClosest(faceTarget: false);
                    if (npc.HasValidTarget)
                    {
                        Main.player[npc.target].ApplyDamageToNPC(npc, npc.lifeMax, npc.velocity.Length(), 0, true);
                    }
                    else
                    {
                        npc.lifeMax = -1;
                        npc.HitEffect();
                        npc.active = false;
                    }
                    var p = npc.Center.ToTileCoordinates();
                    AQNPC.CrashMeteor(p.X, p.Y, 9);
                }
            }
            else if (npc.position.Y > 1600f)
            {
                npc.velocity.Y -= 0.01f;
            }
            npc.rotation += npc.velocity.Length() * 0.00157f;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = frameHeight * (int)(npc.localAI[0] - 1f);
        }
    }
}