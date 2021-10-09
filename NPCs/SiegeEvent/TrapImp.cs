using AQMod.Assets;
using AQMod.Common.NPCIMethods;
using AQMod.Common.Utilities;
using AQMod.Content.WorldEvents.Siege;
using AQMod.Items;
using AQMod.Items.Vanities.CursorDyes;
using AQMod.Items.Weapons.Melee.Flails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.SiegeEvent
{
    public class TrapImp : ModNPC, IDecideFallThroughPlatforms
    {
        public const int TAIL_FRAME_COUNT = 15;
        public const int WING_FRAME_COUNT = 1;

        public override void SetDefaults()
        {
            npc.width = 30;
            npc.height = 50;
            npc.aiStyle = -1;
            npc.damage = 50;
            npc.defense = 22;
            npc.lifeMax = 200;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.lavaImmune = true;
            npc.trapImmune = true;
            npc.value = 200f;
            npc.noGravity = true;
            npc.knockBackResist = 0.1f;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;
            npc.buffImmune[BuffID.Confused] = false;
            npc.buffImmune[BuffID.Ichor] = false;
            npc.SetLiquidSpeed(lava: 1f);
            banner = npc.type;
            bannerItem = ModContent.ItemType<Items.Placeable.Banners.TrapperImpBanner>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.65f);
        }

        public override void AI()
        {
            if ((int)npc.ai[0] == -1)
            {
                npc.velocity.X *= 0.98f;
                npc.velocity.Y -= 0.025f;
                return;
            }
            if ((int)npc.ai[0] == 0)
            {
                npc.ai[0]++;
                int count = Main.rand.Next(4) + 1;
                int spawnX = (int)npc.position.X + npc.width / 2;
                int spawnY = (int)npc.position.Y + npc.height / 2;
                int type = ModContent.NPCType<Trapper>();
                for (int i = 0; i < count; i++)
                {
                    NPC.NewNPC(spawnX, spawnY, type, npc.whoAmI, 0f, npc.whoAmI + 1f);
                }
            }
            npc.TargetClosest(faceTarget: false);
            if (npc.HasValidTarget)
            {
                float speed = 7f;
                if (npc.ai[1] > 240f)
                {
                    speed /= 2f;
                }
                npc.ai[1]++;
                if (npc.ai[1] > 320f)
                {
                    npc.ai[1] = 0f;
                }
                var gotoPosition = Main.player[npc.target].Center + new Vector2(0f, npc.height * -2.5f);
                var difference = gotoPosition - npc.Center;
                var gotoVelocity = Vector2.Normalize(difference);
                if (!npc.noTileCollide && npc.ai[1] > 180f && npc.ai[1] > 210f)
                {
                    gotoVelocity = -gotoVelocity;
                    npc.noTileCollide = true;
                }
                else if (npc.noTileCollide && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                {
                    npc.noTileCollide = false;
                }
                npc.velocity = Vector2.Lerp(npc.velocity, gotoVelocity * speed, 0.015f);
            }
            else
            {
                npc.noTileCollide = true;
                npc.ai[0] = -1f;
            }
            npc.rotation = npc.velocity.X * 0.0314f;
        }

        private void DrawTail(Vector2 drawPosition, Vector2 screenPos, Color drawColor)
        {
            var tailTexture = TextureCache.TrapperImpTail.GetValue();
            int frameTime = (int)(Main.GlobalTime * 15f);
            int animation = frameTime % (TAIL_FRAME_COUNT * 2);
            int frame;
            if (animation > TAIL_FRAME_COUNT)
            {
                frame = TAIL_FRAME_COUNT - animation % TAIL_FRAME_COUNT;
            }
            else
            {
                frame = animation % TAIL_FRAME_COUNT;
            }
            var effects = SpriteEffects.None;
            int frameHeight = tailTexture.Height / TAIL_FRAME_COUNT;
            var tailFrame = new Rectangle(0, frameHeight * frame, tailTexture.Width, frameHeight - 2);
            var tailOrig = new Vector2(tailFrame.Width / 2f, 4f);
            var offset = new Vector2(0f, 8f).RotatedBy(npc.rotation);
            Main.spriteBatch.Draw(tailTexture, drawPosition - screenPos + offset, tailFrame, drawColor, npc.rotation, tailOrig, npc.scale, effects, 0f);
        }

        private void DrawWings(Vector2 drawPosition, Vector2 screenPos, Color drawColor)
        {
            var wingsTexture = TextureCache.TrapperImpWings.GetValue();
            var wingFrame = new Rectangle(0, 0, wingsTexture.Width / 2 - 2, wingsTexture.Height);
            var wingOrig = new Vector2(wingFrame.Width, 4f);
            float wingRotation = npc.rotation + (float)Math.Sin(Main.GlobalTime * 25f) * 0.314f;
            var wingOffset = new Vector2(-8f, 0f).RotatedBy(npc.rotation);
            Main.spriteBatch.Draw(wingsTexture, drawPosition - screenPos + wingOffset + new Vector2(0f, 6f), wingFrame, drawColor, wingRotation, wingOrig, npc.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(wingsTexture, drawPosition - screenPos - wingOffset + new Vector2(0f, 6f), new Rectangle(wingFrame.Width + 2, wingFrame.Y, wingFrame.Width, wingFrame.Height), drawColor, -wingRotation, new Vector2(wingFrame.Width - wingOrig.X, wingOrig.Y), npc.scale, SpriteEffects.None, 0f);
        }

        private void DrawEyes(Vector2 drawPosition, Vector2 screenPos, Vector2 orig)
        {
            var glowTexture = TextureCache.TrapperImpGlow.GetValue();
            Main.spriteBatch.Draw(glowTexture, drawPosition - screenPos, npc.frame, new Color(200, 200, 200, 0), npc.rotation, orig, npc.scale, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            var drawPosition = new Vector2(npc.position.X + npc.width / 2f, npc.position.Y + npc.height / 2f);
            drawPosition.Y -= 10.5f;
            var screenPos = Main.screenPosition;

            DrawWings(drawPosition, screenPos, drawColor);
            DrawTail(drawPosition, screenPos, drawColor);

            var texture = Main.npcTexture[npc.type];
            var orig = new Vector2(npc.frame.Width / 2f, npc.frame.Height / 2f);

            Main.spriteBatch.Draw(texture, drawPosition - screenPos, npc.frame, drawColor, npc.rotation, orig, npc.scale, SpriteEffects.None, 0f);
            DrawEyes(drawPosition, screenPos, orig);
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int count = 1;
            if (npc.life <= 0)
            {
                count = 20;
            }
            for (int i = 0; i < count; i++)
            {
                Dust.NewDust(npc.position + new Vector2(0f, -8f), npc.width, npc.height, DustID.Fire);
            }
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(5))
            {
                if (Main.rand.NextBool())
                {
                    Item.NewItem(npc.getRect(), ItemID.ObsidianRose);
                }
                else
                {
                    Item.NewItem(npc.getRect(), ModContent.ItemType<PowPunch>());
                }
            }
            if (Main.rand.NextBool(24))
            {
                Item.NewItem(npc.getRect(), ModContent.ItemType<DemonicCursorDye>());
            }
            if (Main.rand.NextBool(3))
            {
                Item.NewItem(npc.getRect(), DemonSiege.GetHellBannerDrop(Main.rand));
            }
            if (Main.rand.NextBool())
                Item.NewItem(npc.getRect(), ModContent.ItemType<DemonicEnergy>());
        }

        bool IDecideFallThroughPlatforms.Decide()
        {
            if (Main.player[npc.target].dead)
            {
                return true;
            }
            else
            {
                return Main.player[npc.target].position.Y
                    > npc.position.Y + npc.height;
            }
        }
    }
}