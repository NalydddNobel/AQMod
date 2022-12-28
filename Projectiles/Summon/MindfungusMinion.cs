using Aequus.Buffs.Debuffs;
using Aequus.Buffs.Minion;
using Aequus.Content.Necromancy;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon
{
    public class MindfungusMinion : MinionBase
    {
        public override string Texture => Aequus.VanillaTexture + "Item_" + ItemID.ViciousMushroom;

        public int npcAttached;

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;

            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            this.SetTrail(6);
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 65;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool MinionContactDamage()
        {
            return (int)Projectile.ai[0] == 1;
        }

        public override void AI()
        {
            if (!AequusHelpers.UpdateProjActive<MindfungusBuff>(Projectile))
            {
                return;
            }
            int size = 10;
            int leader = -1;
            int minionPos = 0;
            int count = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == Projectile.type)
                {
                    if ((int)Main.projectile[i].ai[0] == 0)
                    {
                        if (leader == -1)
                        {
                            leader = i;
                        }
                        if (i == Projectile.whoAmI)
                        {
                            minionPos = count;
                        }
                    }
                    count++;
                }
            }

            switch ((int)Projectile.ai[0])
            {
                case 0:
                    {
                        Projectile.Center = IdlePosition(Main.player[Projectile.owner], leader, minionPos, count);
                        int target = Projectile.FindTargetWithLineOfSight(400f);
                        if (target != -1)
                        {
                            if (Projectile.ai[1] <= 0f)
                            {
                                Projectile.ai[1] = minionPos * 10;
                            }
                            Projectile.ai[1]--;
                            if (Projectile.ai[1] <= 0f)
                            {
                                npcAttached = -1;
                                Projectile.velocity = Vector2.Normalize(Main.npc[target].Center - Projectile.Center).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 15f;
                                Projectile.ai[0] = 1f;
                                Projectile.ai[1] = 0f;
                            }
                        }
                        else
                        {
                            Projectile.ai[1] = 0f;
                        }
                    }
                    break;

                case 1:
                    {
                        if (npcAttached > -1)
                        {
                            Projectile.tileCollide = false;
                            if (!Main.npc[npcAttached].active || Main.npc[npcAttached].teleporting)
                            {
                                npcAttached = -1;
                                Projectile.ai[0] = 2f;
                                break;
                            }

                            if (NecromancyDatabase.TryGet(Main.npc[npcAttached], out var info) && info.EnoughPower(1.1f))
                            {
                                Main.npc[npcAttached].GetGlobalNPC<NecromancyNPC>().zombieOwner = Projectile.owner;
                            }

                            size = 30;

                            var npcCenter = Main.npc[npcAttached].Center;
                            if (Projectile.velocity == Vector2.Zero)
                            {
                                Projectile.velocity = Vector2.Normalize(Projectile.Center - npcCenter);
                            }
                            var n = Projectile.velocity.RotatedBy(Main.npc[npcAttached].rotation + Main.npc[npcAttached].direction == -1 ? MathHelper.Pi : 0f);
                            Projectile.Center = npcCenter + n * Main.npc[npcAttached].Size / 2f;
                            Projectile.rotation = n.ToRotation() + MathHelper.PiOver2;
                            Main.npc[npcAttached].Aequus().mindfungusStacks = (byte)count;
                            break;
                        }
                        Projectile.tileCollide = true;
                        Projectile.velocity.X *= 0.999f;
                        if (Projectile.velocity.Y < 0f)
                        {
                            Projectile.velocity.Y *= 0.99f;
                        }
                        if (Projectile.velocity == Vector2.Zero || Projectile.Distance(Main.player[Projectile.owner].Center) > 600f)
                        {
                            Projectile.ai[0] = 2f;
                            Projectile.tileCollide = false;
                        }
                        Projectile.velocity.Y += 0.3f;
                        Projectile.rotation += Projectile.direction * 0.1f;
                    }
                    break;

                case 2:
                    {
                        Projectile.tileCollide = false;
                        var goalPosition = IdlePosition(Main.player[Projectile.owner], leader, minionPos, count);
                        float speed = Projectile.GetMinionReturnSpeed(10f, 1.25f);
                        if ((Projectile.Center - goalPosition).Length() <= 10f)
                        {
                            Projectile.ai[0] = 0f;
                            Projectile.velocity = Vector2.Zero;
                            Projectile.rotation = 0f;
                            break;
                        }
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(goalPosition - Projectile.Center) * speed, 0.1f);
                        Projectile.rotation += Projectile.direction * Math.Min(Projectile.velocity.Length() * 0.02f, 0.1f);
                    }
                    break;
            }

            Projectile.width = (int)(size * Projectile.scale);
            Projectile.height = (int)(size * Projectile.scale);
        }

        public override Vector2 IdlePosition(Player player, int leader, int minionPos, int count)
        {
            var originPoint = new Vector2(player.position.X + player.width / 2f, player.position.Y - Projectile.height);
            minionPos = count - minionPos - 1;
            int x = minionPos % 3;
            originPoint.X += (x == 0 ? 0 : x == 1 ? 1 : -1) * Projectile.width;
            originPoint.Y -= minionPos / 3 * Projectile.height * 2;
            return originPoint;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            knockback = npcAttached == -1 ? knockback : 0f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<MindfungusDebuff>(), Projectile.localNPCHitCooldown + 10);
            if (target.lifeMax < 500 && target.defense < 50 && NecromancyDatabase.TryGet(target, out var info) && info.EnoughPower(1.1f))
            {
                target.GetGlobalNPC<NecromancyNPC>().zombieOwner = Projectile.owner;
            }
            if (npcAttached == -1)
            {
                npcAttached = target.whoAmI;
                Projectile.velocity = Vector2.Zero;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0] = 2f;
            Projectile.tileCollide = false;
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int trailLength);

            bool drawT = (int)Projectile.ai[0] == 1 && npcAttached == -1;
            int endT = trailLength;

            if (drawT)
            {
                for (int i = 0; i < trailLength; i++)
                {
                    if (i != trailLength - 1 && (Projectile.oldPos[i] - Projectile.oldPos[i + 1]).Length() <= 1f)
                    {
                        endT = i;
                        break;
                    }
                    float p = AequusHelpers.CalcProgress(trailLength, i);
                    Main.EntitySpriteDraw(t, (Projectile.oldPos[i] + off - Main.screenPosition).Floor(), frame, new Color(30, 30, 30, 0) * p * p, Projectile.oldRot[i], origin, Projectile.scale + 0.4f * (1f - p), Projectile.spriteDirection.ToSpriteEffect(), 0);
                }
            }
            var v = Projectile.position + off - Main.screenPosition.Floor();
            
            if ((int)Projectile.ai[0] == 0 && npcAttached <= 0)
            {
                v += Main.OffsetsPlayerHeadgear[Main.player[Projectile.owner].bodyFrame.Y / 56];
                v = v.Floor();
            }
            Main.EntitySpriteDraw(t, v, frame, lightColor, Projectile.rotation, origin, Projectile.scale, Projectile.spriteDirection.ToSpriteEffect(), 0);
            if (drawT)
            {
                for (int i = 0; i < trailLength; i++)
                {
                    if (i == endT)
                    {
                        break;
                    }
                    float p = AequusHelpers.CalcProgress(trailLength, i);
                    Main.EntitySpriteDraw(t, (Projectile.oldPos[i] + off - Main.screenPosition).Floor(), frame, new Color(20, 20, 20, 0) * p, Projectile.oldRot[i], origin, (Projectile.scale + 0.4f * (1f - p)) * 1.25f, Projectile.spriteDirection.ToSpriteEffect(), 0);
                }
            }

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npcAttached);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npcAttached = reader.ReadInt32();
        }
    }
}