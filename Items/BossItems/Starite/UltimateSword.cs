using AQMod.Assets.ItemOverlays;
using AQMod.Common;
using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.BossItems.Starite
{
    public class UltimateSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                ItemOverlayLoader.Register(new UltimateSwordOverlay(), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.height = 50;
            item.rare = ItemRarityID.Lime;
            item.useTime = 16;
            item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = AQItem.OmegaStariteWeaponValue * 4;
            item.damage = 75;
            item.melee = true;
            item.knockBack = 6f;
            item.autoReuse = true;
            item.useTurn = true;
            item.scale = 1.35f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (player.immuneTime < 11)
            {
                player.immuneTime = 11;
                player.immuneNoBlink = true;
            }
            player.AddBuff(ModContent.BuffType<Buffs.Ultima>(), 1200);
            target.AddBuff(BuffID.CursedInferno, 60);
            target.AddBuff(BuffID.Ichor, 20);
            if (target.lifeMax <= 400)
            {
                Main.PlaySound(SoundID.Item14, target.position);
                var value = ModContent.DustType<UltimaDust>();
                for (int i = 0; i < 40; i++)
                {
                    Dust.NewDust(target.position, target.width, target.height, value);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    value = ModContent.ProjectileType<Projectiles.Melee.UltimaFlare>();
                    var value1 = (float)Math.Sqrt(target.width * target.width + target.height * target.height) + 8f;
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 normal = new Vector2(1f, 0f).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
                        Projectile.NewProjectile(target.Center + normal * value1, normal * Main.rand.NextFloat(2f, 12f), value, 10, knockBack, player.whoAmI);
                    }
                }
            }
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, ModContent.DustType<UltimaDust>());
        }
    }
}