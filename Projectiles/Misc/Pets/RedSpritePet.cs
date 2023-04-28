using Aequus;
using Aequus.Buffs.Pets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.Pets
{
    public class RedSpritePet : ModProjectile
    {
        public static Asset<Texture2D> Glow { get; private set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                Glow = ModContent.Request<Texture2D>(this.GetPath() + "_Glow");
            }
        }

        public override void Unload()
        {
            Glow = null;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Type] = ProjectileID.Sets.SimpleLoop(0, 4, 6).WithOffset(new(0f, -14f));
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.manualDirectionChange = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Helper.UpdateProjActive<RedSpriteBuff>(Projectile);
            var gotoPos = GetIdlePosition();
            Projectile.direction = player.direction;
            var center = Projectile.Center;
            float distance = (center - gotoPos).Length();

            if (Projectile.Distance(Main.player[Projectile.owner].Center) > 2000f)
            {
                Projectile.Center = Main.player[Projectile.owner].Center;
                Projectile.velocity *= 0.1f;
            }

            float snapLength = 0.1f;
            if ((int)Projectile.ai[0] == 1)
            {
                if (distance > snapLength)
                {
                    Projectile.ai[0] = 0f;
                }
                else
                {
                    snapLength = 24f;
                }
            }
            if (distance < snapLength)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.Center = gotoPos;
                Projectile.ai[0] = 1f;
            }
            else
            {
                Projectile.velocity = (center - gotoPos) / -16f;
            }

            Projectile.LoopingFrame(6);

            Projectile.rotation = Projectile.velocity.X * 0.1f;
            Lighting.AddLight(Projectile.Center, new Vector3(1.2f, 1f, 0.5f));
        }
        private Vector2 GetIdlePosition()
        {
            int dir = -Main.player[Projectile.owner].direction;
            float y = -20f;
            var counts = Main.player[Projectile.owner].ownedProjectileCounts;
            if (counts[ProjectileID.IceQueenPet] > 0 || counts[ProjectileID.GlommerPet] > 0)
            {
                dir = -dir;
            }
            return Main.player[Projectile.owner].Center + new Vector2((Main.player[Projectile.owner].width + 16f) * dir, y);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 center = Projectile.Center;
            var drawCoordinates = center - Main.screenPosition;
            var origin = frame.Size() / 2f;
            var effects = Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            var circular = Helper.CircularVector(3, Main.GlobalTimeWrappedHourly);
            for (int i = 0; i < circular.Length; i++)
            {
                Main.EntitySpriteDraw(texture, drawCoordinates + circular[i] * Projectile.scale * Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 2f, 6f), frame,
                    Color.Lerp(Color.Red.UseA(0) * 0.3f, Color.OrangeRed.UseA(0) * 0.5f, Helper.Wave(Main.GlobalTimeWrappedHourly * 6f + i * MathHelper.TwoPi / 3f, 0f, 1f)), Projectile.rotation, origin, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture, drawCoordinates, frame, lightColor.MaxRGBA(24), Projectile.rotation, origin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(Glow.Value, drawCoordinates, frame, Color.White, Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }
    }
}