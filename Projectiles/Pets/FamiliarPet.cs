using Aequus.Common.Hooklists;
using Aequus.Effects;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Pets
{
    public sealed class FamiliarPet : ModProjectile
    {
        public override string Texture => Aequus.TextureNone;

        private Player dummyPlayer;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 20;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.aiStyle = ProjAIStyleID.Pet;
            AIType = ProjectileID.Penguin;
            Projectile.scale = 0.5f;
        }

        public override bool PreAI()
        {
            var parent = Main.player[Projectile.owner];
            if (dummyPlayer == null)
            {
                dummyPlayer = new Player();
            }

            AequusHelpers.UpdateProjActive(Projectile, ref parent.GetModPlayer<AequusPlayer>().familiarPet);

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
            dummyPlayer.width = Projectile.width;
            dummyPlayer.head = Projectile.height;
            dummyPlayer.oldVelocity = Projectile.oldVelocity;
            dummyPlayer.velocity = Projectile.velocity;
            dummyPlayer.oldDirection = Projectile.oldDirection;
            dummyPlayer.wet = Projectile.wet;
            dummyPlayer.lavaWet = Projectile.lavaWet;
            dummyPlayer.honeyWet = Projectile.honeyWet;
            dummyPlayer.wetCount = Projectile.wetCount;
            if (Projectile.velocity != Vector2.Zero || Projectile.direction == 0)
            {
                dummyPlayer.direction = Projectile.velocity.X < 0f ? -1 : 1;
            }
            dummyPlayer.oldPosition = Projectile.oldPosition;
            dummyPlayer.position = Projectile.position;
            dummyPlayer.position.Y -= 42 * (1f - Projectile.scale);
            dummyPlayer.whoAmI = Projectile.owner;
            dummyPlayer.PlayerFrame();
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (dummyPlayer == null)
            {
                return false;
            }
            var batchData = new SpriteBatchDataCache(Main.spriteBatch);
            PlayerHooklist.PlayerDrawScale = Projectile.scale;
            Main.spriteBatch.End();
            Main.PlayerRenderer.DrawPlayers(Main.Camera, new Player[] { dummyPlayer });
            PlayerHooklist.PlayerDrawScale = null;
            batchData.Begin(Main.spriteBatch);
            return false;
        }
    }
}