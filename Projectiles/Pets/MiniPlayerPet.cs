using AQMod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Pets
{
    public sealed class MiniPlayerPet : ModProjectile
    {
        public override string Texture => AQMod.TextureNone;

        private Player dummyPlayer;

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 20;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 42;
            projectile.friendly = true;
            projectile.aiStyle = AQProjectile.AIStyles.PetAI;
            aiType = ProjectileID.Penguin;
            projectile.scale = 0.5f;
        }

        public override bool PreAI()
        {
            var parent = Main.player[projectile.owner];
            if (dummyPlayer == null)
            {
                dummyPlayer = new Player(startupInventory: false);
                // parent.DeepCopyTo(dummyPlayer); deep cloning arrays is impossible
            }

            AQProjectile.UpdateProjActive(projectile, ref parent.GetModPlayer<AQPlayer>().familiar);

            // copies player attributes
            dummyPlayer.eyeColor = parent.eyeColor;
            dummyPlayer.hairColor = parent.hairColor;
            dummyPlayer.hairDyeColor = parent.hairDyeColor;
            dummyPlayer.pantsColor = parent.pantsColor;
            dummyPlayer.shirtColor = parent.shirtColor;
            dummyPlayer.shoeColor = parent.shoeColor;
            dummyPlayer.skinColor = parent.skinColor;
            dummyPlayer.underShirtColor = parent.underShirtColor;
            dummyPlayer.Male = parent.Male;
            dummyPlayer.skinVariant = parent.skinVariant;
            dummyPlayer.hairDye = parent.hairDye;
            dummyPlayer.hairDyeVar = parent.hairDyeVar;
            dummyPlayer.hair = parent.hair;

            // copies armors
            //parent.armor.FillOther(dummyPlayer.armor);
            //parent.dye.FillOther(dummyPlayer.dye);
            
            //dummyPlayer.UpdateDyes();

            // copies proj attributes
            dummyPlayer.width = projectile.width;
            dummyPlayer.head = projectile.height;
            dummyPlayer.oldVelocity = projectile.oldVelocity;
            dummyPlayer.velocity = projectile.velocity;
            dummyPlayer.oldDirection = projectile.oldDirection;
            dummyPlayer.wet = projectile.wet;
            dummyPlayer.lavaWet = projectile.lavaWet;
            dummyPlayer.honeyWet = projectile.honeyWet;
            dummyPlayer.wetCount = projectile.wetCount;
            if (projectile.velocity != Vector2.Zero || projectile.direction == 0)
            {
                dummyPlayer.direction = projectile.velocity.X < 0f ? -1 : 1;
            }
            dummyPlayer.oldPosition = projectile.oldPosition;
            dummyPlayer.position = projectile.position;
            dummyPlayer.position.Y -= 42 * (1f - projectile.scale);
            dummyPlayer.whoAmI = projectile.owner;
            dummyPlayer.PlayerFrame();
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (dummyPlayer == null)
            {
                return false;
            }
            var batchData = new BatchData(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.Transform);
            DrawHelper.Hooks.PlayerDrawScale = projectile.scale;
            AQUtils.DrawPlayerFull(dummyPlayer);
            DrawHelper.Hooks.PlayerDrawScale = null;
            spriteBatch.End();
            batchData.Begin(spriteBatch);
            return false;
        }
    }
}