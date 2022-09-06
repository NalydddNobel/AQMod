using Aequus.Biomes;
using Aequus.Buffs;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Placeable.Banners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Underworld
{
    public class TrapperImp : ModNPC
    {
        public const int TAIL_FRAME_COUNT = 15;
        public const int WING_FRAME_COUNT = 1;

        public Asset<Texture2D> GlowTexture => ModContent.Request<Texture2D>(Texture + "_Glow");
        public Asset<Texture2D> TailTexture => ModContent.Request<Texture2D>(Texture + "Tail");
        public Asset<Texture2D> WingsTexture => ModContent.Request<Texture2D>(Texture + "Wings");

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(1f, 12f)
            });
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData()
            {
                SpecificallyImmuneTo = AequusBuff.DemonSiegeEnemyImmunity.ToArray(),
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 50;
            NPC.aiStyle = -1;
            NPC.damage = 50;
            NPC.defense = 12;
            NPC.lifeMax = 200;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.lavaImmune = true;
            NPC.trapImmune = true;
            NPC.value = 200f;
            NPC.noGravity = true;
            NPC.knockBackResist = 0.1f;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.CursedInferno] = true;
            NPC.buffImmune[BuffID.Confused] = false;
            NPC.buffImmune[BuffID.Ichor] = false;
            NPC.SetLiquidSpeeds(lava: 1f);

            Banner = NPC.type;
            BannerItem = ModContent.ItemType<TrapperImpBanner>();

            this.SetBiome<DemonSiegeBiome>();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry);
        }

        public override void AI()
        {
            if ((int)NPC.ai[0] == -1)
            {
                NPC.velocity.X *= 0.98f;
                NPC.velocity.Y -= 0.025f;
                return;
            }
            if ((int)NPC.ai[0] == 0)
            {
                NPC.ai[0]++;
                int count = Main.rand.Next(4) + 1;
                if (Main.getGoodWorld)
                {
                    count *= 3;
                }
                int spawnX = (int)NPC.position.X + NPC.width / 2;
                int spawnY = (int)NPC.position.Y + NPC.height / 2;
                int type = ModContent.NPCType<Trapper>();
                for (int i = 0; i < count; i++)
                {
                    NPC.NewNPC(NPC.GetSource_FromThis("Aequus:TrapperImpInit"), spawnX, spawnY, type, NPC.whoAmI, 0f, NPC.whoAmI + 1f);
                }
            }
            NPC.TargetClosest(faceTarget: false);
            if (NPC.HasValidTarget)
            {
                float speed = 7f;
                if (NPC.ai[1] > 240f)
                    speed /= 2f;
                NPC.ai[1]++;
                if (NPC.ai[1] > 320f)
                    NPC.ai[1] = 0f;
                var gotoPosition = Main.player[NPC.target].Center + new Vector2(0f, NPC.height * -2.5f);
                var difference = gotoPosition - NPC.Center;
                var gotoVelocity = Vector2.Normalize(difference);
                if (!NPC.noTileCollide && NPC.ai[1] > 180f && NPC.ai[1] > 210f)
                {
                    gotoVelocity = -gotoVelocity;
                    NPC.noTileCollide = true;
                }
                else if (NPC.noTileCollide && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    NPC.noTileCollide = false;
                }
                NPC.velocity = Vector2.Lerp(NPC.velocity, gotoVelocity * speed, 0.015f);
            }
            else
            {
                NPC.noTileCollide = true;
                NPC.ai[0] = -1f;
            }
            NPC.rotation = NPC.velocity.X * 0.0314f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            int count = 1;
            if (NPC.life <= 0)
            {
                count = 28;

                for (int i = -1; i <= 1; i++)
                {
                    if (i != 0)
                    {
                        NPC.DeathGore("TrapperImp_0", new Vector2(12f * i, -20f));
                        NPC.DeathGore("TrapperImp_2", new Vector2(12f * i, 0f));
                        NPC.DeathGore("TrapperImp_2", new Vector2(12f * i, 20f));
                        NPC.DeathGore("TrapperImp_3", new Vector2(12f * i, 0f));
                    }
                }

                NPC.DeathGore("TrapperImp_1");
            }
            for (int i = 0; i < count; i++)
            {
                var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch);
                d.velocity = (d.position - NPC.Center) / 8f;
                if (Main.rand.NextBool(3))
                {
                    d.velocity *= 2f;
                    d.scale *= 1.75f;
                    d.fadeIn = d.scale + Main.rand.NextFloat(0.5f, 0.75f);
                    d.noGravity = true;
                }
            }
            for (int i = 0; i < count * 2; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.FoodPiece, newColor: Color.DarkRed);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add<DemonicEnergy>(chance: 10, stack: 1)
                .Add(ItemID.ObsidianRose, chance: 25, stack: 1);
        }

        //public override void NPCLoot()
        //{
        //    if (Main.rand.NextBool(24))
        //        Item.NewItem(NPC.getRect(), ModContent.ItemType<DemonicCursorDye>());
        //    if (Main.rand.NextBool(3))
        //        Item.NewItem(NPC.getRect(), DemonSiege.HellBanners[Main.rand.Next(DemonSiege.HellBanners.Count)]);
        //}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var drawPosition = new Vector2(NPC.position.X + NPC.width / 2f, NPC.position.Y + NPC.height / 2f);
            drawPosition.Y -= 10.5f;

            var texture = TextureAssets.Npc[Type].Value;
            var glowTexture = GlowTexture.Value;
            var orig = new Vector2(NPC.frame.Width / 2f, NPC.frame.Height / 2f);

            DrawWings(spriteBatch, drawPosition, screenPos, drawColor);
            DrawTail(spriteBatch, drawPosition, screenPos, drawColor);

            spriteBatch.Draw(texture, drawPosition - screenPos, NPC.frame, drawColor, NPC.rotation, orig, NPC.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glowTexture, drawPosition - screenPos, NPC.frame, new Color(200, 200, 200, 0), NPC.rotation, orig, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
        public void DrawTail(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, Color drawColor)
        {
            var tailTexture = TailTexture.Value;
            int frameTime = (int)(Main.GlobalTimeWrappedHourly * 15f);
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
            var offset = new Vector2(0f, 8f).RotatedBy(NPC.rotation);
            spriteBatch.Draw(tailTexture, drawPosition - screenPos + offset, tailFrame, drawColor, NPC.rotation, tailOrig, NPC.scale, effects, 0f);
        }
        public void DrawWings(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, Color drawColor)
        {
            drawPosition.Y -= 10f;
            var wingsTexture = WingsTexture.Value;
            var wingFrame = new Rectangle(0, 0, wingsTexture.Width / 2 - 2, wingsTexture.Height);
            var wingOrig = new Vector2(wingFrame.Width, 4f);
            float wingRotation = NPC.rotation + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 25f) * 0.314f;
            var wingOffset = new Vector2(-8f, 0f).RotatedBy(NPC.rotation);
            spriteBatch.Draw(wingsTexture, drawPosition - screenPos + wingOffset + new Vector2(0f, 6f), wingFrame, drawColor, wingRotation, wingOrig, NPC.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(wingsTexture, drawPosition - screenPos - wingOffset + new Vector2(0f, 6f), new Rectangle(wingFrame.Width + 2, wingFrame.Y, wingFrame.Width, wingFrame.Height), drawColor, -wingRotation, new Vector2(wingFrame.Width - wingOrig.X, wingOrig.Y), NPC.scale, SpriteEffects.None, 0f);
        }

        public override bool? CanFallThroughPlatforms()
        {
            if (Main.player[NPC.target].dead)
            {
                return true;
            }
            else
            {
                return Main.player[NPC.target].position.Y
                    > NPC.position.Y + NPC.height;
            }
        }
    }
}