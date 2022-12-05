using Aequus.Items.Consumables.Drones;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Friendly.Town.Drones
{
    public class GunnerDrone : TownDroneBase
    {
        public override int ItemDrop => ModContent.ItemType<InactivePylonGunner>();

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

            int target = AequusHelpers.FindTargetWithLineOfSight(NPC.position, NPC.width, NPC.height, 700f, NPC);

            float targetDistance = 1600f;
            float minDistance = 600f;
            if (target != -1)
            {
                if (Collision.CanHitLine(NPC.position, NPC.width, NPC.height, Main.npc[target].position, Main.npc[target].width, Main.npc[target].height))
                {
                    NPC.ai[0]++;
                    if (NPC.ai[0] > 15f)
                    {
                        var shootPosition = NPC.Center + new Vector2(0f, 12f);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            var p = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), shootPosition, Vector2.Normalize(Main.npc[target].Center - shootPosition).RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f)) * 10f, ProjectileID.Bullet,
                                NPC.damage, 1f, Main.myPlayer);
                            p.ArmorPenetration += 5;
                            p.npcProj = true;
                        }
                        SoundEngine.PlaySound(SoundID.Item11, NPC.Center);
                        NPC.ai[0] = 0f;
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
                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(Main.npc[target].Center - NPC.Center + new Vector2(0f, tileHeight < 10 ? -30f : 0f)) * 6f, 0.01f);
            }
            else
            {
                DefaultMovement();
            }
            if (target != -1)
            {
                NPC.ai[1] = target + 1;
            }
            else
            {
                NPC.ai[1] = 0f;
            }

            NPC.rotation = NPC.velocity.X * 0.1f;
            NPC.spriteDirection = NPC.direction;
            NPC.CollideWithOthers(0.1f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC.GetDrawInfo(out var texture, out var off, out var frame, out var origin, out int _);
            var gunTexture = ModContent.Request<Texture2D>(Texture + "Gun", AssetRequestMode.ImmediateLoad);

            var color = GetPylonColor();
            float turretRotation = AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, -1f, 1f);
            int npcTarget = (int)NPC.ai[1] - 1;
            if (npcTarget > -1)
            {
                turretRotation = (Main.npc[npcTarget].Center - NPC.Center).ToRotation() + MathHelper.PiOver2;
            }
            spriteBatch.Draw(gunTexture.Value, NPC.position + off +
                (NPC.rotation + MathHelper.PiOver2).ToRotationVector2() * texture.Height / 2f - screenPos + new Vector2(1f, 0f), null, NPC.GetNPCColorTintedByBuffs(drawColor),
                turretRotation - MathHelper.PiOver2, new Vector2(gunTexture.Value.Width / 2f, 4f), NPC.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, NPC.position + off - screenPos, frame, NPC.GetNPCColorTintedByBuffs(drawColor),
                NPC.rotation, origin, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow", AssetRequestMode.ImmediateLoad).Value, NPC.position + off - screenPos, frame, color * SpawnInOpacity,
                NPC.rotation, origin, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }
}