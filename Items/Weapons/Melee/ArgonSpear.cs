using AQMod.Content.Dusts;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Placeable;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class ArgonSpear : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.melee = true;
            item.damage = 15;
            item.knockBack = 1f;
            item.useAnimation = 32;
            item.useTime = 48;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item1;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(silver: 40);
            item.shootSpeed = 7.5f;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<ArgonSpearProjectile>();
            item.autoReuse = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<VineSword>());
            r.AddIngredient(ModContent.ItemType<ArgonMushroom>(), 2);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>(), 3);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(this);
            r.AddRecipe();
        }
    }

    public class ArgonSpearProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.aiStyle = 19;
            projectile.penetrate = -1;
            projectile.alpha = 0;
            projectile.hide = true;
            projectile.ownerHitCheck = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 6;
            projectile.manualDirectionChange = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            var player = Main.player[projectile.owner];
            float speedMultiplier = 1f + (1f - player.meleeSpeed);
            if (projectile.localAI[0] == 0f)
                projectile.localAI[0] = 200f * speedMultiplier;
            var playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            projectile.direction = player.direction;
            player.heldProj = projectile.whoAmI;
            player.itemTime = player.itemAnimation;
            projectile.position.X = playerCenter.X - projectile.width / 2;
            projectile.position.Y = playerCenter.Y - projectile.height / 2;
            float lerpAmount = MathHelper.Clamp(1f - speedMultiplier, 0.1f, 1f);
            if (!player.frozen)
            {
                if (projectile.ai[0] == 0f)
                {
                    projectile.ai[0] = 3f;
                    projectile.netUpdate = true;
                }
                if (player.itemAnimation < player.itemAnimationMax / 3f)
                {
                    projectile.ai[0] = MathHelper.Lerp(projectile.ai[0], 0f, MathHelper.Clamp(lerpAmount + 0.45f, 0.55f, 1f));
                }
                else
                {
                    projectile.ai[0] = MathHelper.Lerp(projectile.ai[0], projectile.localAI[0], MathHelper.Clamp(lerpAmount + 0.25f, 0.35f, 1f));
                }
            }
            if (player.itemAnimation == 0)
                projectile.Kill();
            if (Main.myPlayer == player.whoAmI && lerpAmount > 0f)
                projectile.velocity = Vector2.Lerp(projectile.velocity, new Vector2(projectile.ai[0], 0f).RotatedBy((Main.MouseWorld - playerCenter).ToRotation()), lerpAmount * 0.45f);
            Main.dust[Dust.NewDust(projectile.Center + new Vector2(-6f, -6f) + projectile.velocity, 4, 4, ModContent.DustType<ArgonDust>())].velocity *= 0.1f;
            projectile.direction = projectile.velocity.X <= 0f ? -1 : 1;
            projectile.spriteDirection = projectile.direction;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4 * 3f;
            if (projectile.spriteDirection == -1)
                projectile.rotation += -MathHelper.PiOver2;
            player.ChangeDir(projectile.direction);
        }
    }
}