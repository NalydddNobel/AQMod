using AQMod.Common;
using AQMod.Common.ItemOverlays;
using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class UltimateSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new UltimateSwordOverlayData(), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.height = 50;
            item.rare = ItemRarityID.Pink;
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = AQItem.OmegaStariteWeaponValue * 2;
            item.damage = 65;
            item.melee = true;
            item.knockBack = 4f;
            item.autoReuse = true;
            item.useTurn = true;
            item.scale = 1.3f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (player.immuneTime < 5)
            {
                player.immuneTime = 5;
                player.immuneNoBlink = true;
            }
            target.AddBuff(ModContent.BuffType<Buffs.Debuffs.Sparkling>(), 1200);
            if (crit)
            {
                player.AddBuff(ModContent.BuffType<Buffs.Ultima>(), 300);
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.BlueFire>(), 120);
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
        }
    }
}