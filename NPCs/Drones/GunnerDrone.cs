using Aequus;
using Aequus.NPCs.Drones.Misc;
using Aequus.Projectiles.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Drones
{
    public class GunnerDrone : TownDroneBase
    {
        public override int ItemDrop => ModContent.ItemType<InactivePylonGunner>();

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 13;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 20;
            NPC.friendly = true;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.lifeMax = 250;
            NPC.dontTakeDamage = true;
            NPC.damage = 15;
        }

        public override void AI()
        {
            Main.npcFrameCount[Type] = 13;
            base.AI();
            int tileHeight = 30;
            int tileX = ((int)NPC.position.X + NPC.width / 2) / 16;
            int tileY = ((int)NPC.position.Y + NPC.height / 2) / 16;
            for (int i = 0; i < 30; i++)
            {
                if (WorldGen.InWorld(tileX, tileY + i, 10) && Main.tile[tileX, tileY + i].IsSolid())
                {
                    tileHeight = i + 1;
                    break;
                }
            }

            int target = AequusHelpers.FindTarget(NPC.position, NPC.width, NPC.height, 900f, NPC);

            if (NPC.frame.Y >= NPC.frame.Height * 7 && NPC.frame.Y < NPC.frame.Height * (Main.npcFrameCount[Type] - 1))
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 7.0 && NPC.frame.Y < NPC.frame.Height * (Main.npcFrameCount[Type] - 1))
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y += NPC.frame.Height;
                }
            }
            if (NPC.ai[0] > 0f && NPC.ai[0] < 20f)
            {
                NPC.ai[0]++;
                if (target == -1)
                {
                    if (NPC.ai[0] >= 20f)
                    {
                        NPC.ai[0] = 0f;
                    }
                }
                NPC.velocity *= 0.95f;
                return;
            }

            float targetDistance = 1600f;
            float minDistance = 200f;
            var diff = Vector2.Zero;
            if (target != -1)
            {
                diff = Main.npc[target].Center - NPC.Center;
                if (Collision.CanHitLine(NPC.position, NPC.width, NPC.height, Main.npc[target].position, Main.npc[target].width, Main.npc[target].height) || NPC.ai[0] < 20f)
                {
                    NPC.ai[0]++;
                    if (NPC.ai[0] > (diff.Length() < minDistance ? 60f : 90f))
                    {
                        NPC.frame.Y = NPC.frame.Height * 7;
                        var shootPosition = NPC.Center + new Vector2(0f, 12f);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            var p = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), shootPosition, Vector2.Normalize(Main.npc[target].Center - shootPosition).RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f)) * 10f,
                                ModContent.ProjectileType<GunnerDroneProj>(), NPC.damage, 1f, Main.myPlayer, NPC.whoAmI);
                            p.ArmorPenetration += 5;
                            p.npcProj = true;
                        }
                        SoundEngine.PlaySound(SoundID.Item158.WithVolumeScale(1.5f).WithPitchOffset(0.33f), NPC.Center);
                        NPC.ai[0] = 1f + Main.rand.NextFloat(10f);
                        NPC.velocity -= Vector2.Normalize(diff) * 2f;
                    }
                }
                else
                {
                    minDistance = 20f;
                }
                targetDistance = NPC.Distance(Main.npc[target].Center);
            }

            if (target != -1 && targetDistance >= minDistance)
            {
                movementPoint = NPC.Center;
                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(Main.npc[target].Center - NPC.Center + new Vector2(0f, tileHeight < 10 ? -30f : 0f)) * 9f, 0.1f);
            }
            else
            {
                DefaultMovement();
            }
            if (target != -1)
            {
                NPC.direction = Math.Sign(diff.X);
                float r = diff.ToRotation();
                if (NPC.direction == -1)
                {
                    r -= MathHelper.Pi;
                }
                NPC.rotation = r;
                NPC.ai[1] = target + 1;
            }
            else
            {
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter > 100)
                {
                    NPC.frame.Y += NPC.frame.Height;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frameCounter > 2)
                {
                    if (NPC.frame.Y > 0 && (NPC.frame.Y < NPC.frame.Height * 3 || NPC.frame.Y > NPC.frame.Height * 3 && NPC.frame.Y < NPC.frame.Height * 6))
                    {
                        NPC.frame.Y += NPC.frame.Height;
                        NPC.frameCounter = 0.0;
                    }
                    else if (NPC.frame.Y == 0)
                    {
                        NPC.frameCounter += Main.rand.Next(-7, 0) * 0.2;
                        if (NPC.frameCounter < 0.0)
                            NPC.frameCounter = 0.0;
                    }
                }
                if (NPC.frame.Y >= NPC.frame.Height * 6)
                {
                    NPC.frame.Y = 0;
                }

                NPC.rotation = MathHelper.Lerp(NPC.rotation % MathHelper.Pi, NPC.velocity.X * 0.1f, 0.1f);
                NPC.ai[1] = 0f;
            }

            NPC.spriteDirection = NPC.direction;
            NPC.CollideWithOthers(0.1f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC.GetDrawInfo(out var texture, out var off, out var frame, out var origin, out int _);

            var color = GetPylonColor();
            float turretRotation = AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, -1f, 1f);
            int npcTarget = (int)NPC.ai[1] - 1;
            if (npcTarget > -1)
            {
                turretRotation = (Main.npc[npcTarget].Center - NPC.Center).ToRotation() + MathHelper.PiOver2;
            }
            var extra = ModContent.Request<Texture2D>(Texture + "_Extras", AssetRequestMode.ImmediateLoad).Value;
            var clr = NPC.GetNPCColorTintedByBuffs(drawColor);
            spriteBatch.Draw(extra, NPC.position + off - screenPos, null, clr,
                0f, extra.Size() / 2f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Extras_Glow", AssetRequestMode.ImmediateLoad).Value, NPC.position + off - screenPos, null, color * SpawnInOpacity,
                0f, extra.Size() / 2f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            spriteBatch.Draw(texture, NPC.position + off - screenPos, frame, clr,
                NPC.rotation, origin, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow", AssetRequestMode.ImmediateLoad).Value, NPC.position + off - screenPos, frame, color * SpawnInOpacity,
                NPC.rotation, origin, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }
}