using AQMod.Common.Configuration;
using AQMod.Common.Graphics;
using AQMod.Dusts;
using AQMod.Items.Materials;
using AQMod.Items.Recipes;
using AQMod.Projectiles.Melee;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class ThunderClap : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.Glowmask();
        }

        public override void SetDefaults()
        {
            item.damage = 88;
            item.width = 30;
            item.height = 30;
            item.useTime = 100;
            item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.rare = AQItem.RarityGaleStreams;
            item.value = AQItem.Prices.GaleStreamsWeaponValue;
            item.melee = true;
            item.shoot = ModContent.ProjectileType<ThunderClapProj>();
            item.knockBack = 32f;
            item.scale = 1.35f;
            item.autoReuse = true;
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<RedSpriteDust>());
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (!AQNPC.Sets.Instance.WindUpdates.Contains(target.type))
            {
                if (target.knockBackResist > 0)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        AQSound.LegacyPlay(SoundType.NPCHit, AQSound.Paths.ThunderClapSlap, target.Center, 0.3f);
                    }
                    target.velocity = Vector2.Normalize(target.Center - player.Center) * player.GetWeaponKnockback(item, item.knockBack) * 1.75f * target.knockBackResist;
                }
            }
            else
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    AQSound.LegacyPlay(SoundType.NPCHit, AQSound.Paths.ThunderClapSlap, target.Center, 0.3f);
                }
                target.velocity = Vector2.Normalize(target.Center - player.Center) * player.GetWeaponKnockback(item, item.knockBack) * 2f * Math.Max(target.knockBackResist, 0.2f);
            }
            target.AddBuff(BuffID.OnFire, 800);
            target.GetGlobalNPC<AQNPC>().ChangeTemperature(target, 60);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int p = Projectile.NewProjectile(new Vector2(Main.MouseWorld.X + Main.rand.NextFloat(-20f, 20f), position.Y - 666f), new Vector2(0f, 0f), type, damage * 2, 0f, player.whoAmI);
            if (Main.netMode != NetmodeID.Server)
            {
                AQSound.LegacyPlay(SoundType.Item, AQSound.Paths.ThunderClap, position, 0.55f);
            }
            if (AQConfigClient.Instance.Screenshakes)
            {
                AQMod.Effects.SetShake(AQGraphics.MultIntensity(10), 10f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            AQRecipes.GaleStreamsMinibossWeapon(mod, ItemID.SlapHand, ModContent.ItemType<Fluorescence>(), this);
        }
    }
}