using Aequus.Items.Accessories.Summon.Sentry;
using Aequus.Projectiles.Misc;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class HyperCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);

            SantankInteractions.OnAI.Add(Type, SantankInteractions.ApplyEquipFunctional_AI);
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 1);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.Aequus().ProjectilesOwned_ConsiderProjectileIdentity(ModContent.ProjectileType<HyperCrystalAuraProj>()) <= 0)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<HyperCrystalAuraProj>(),
                    0, 0f, player.whoAmI, player.Aequus().projectileIdentity + 1);
            }
            player.GetModPlayer<HyperCrystalPlayer>().Add(480f, 0.25f, hideVisual);
        }
    }

    /// <summary>
    /// Used by <see cref="HyperCrystal"/>
    /// </summary>
    public sealed class HyperCrystalPlayer : ModPlayer
    {
        public const int MinVFXCircumference = 8;

        public float diameter;
        public float damageMultiplier;
        public bool hideVisual;
        public float _accFocusCrystalDiameter;
        public float _accFocusCrystalOpacity;

        public override void ResetEffects()
        {
            _accFocusCrystalDiameter = MathHelper.Lerp(_accFocusCrystalDiameter, diameter, 0.2f);
            diameter = 0f;
            damageMultiplier = 0f;
            hideVisual = false;
        }

        public override void UpdateDead()
        {
            _accFocusCrystalDiameter = MathHelper.Lerp(_accFocusCrystalDiameter, 0f, 0.2f);
            diameter = 0f;
            damageMultiplier = 0f;
            hideVisual = true;
        }

        public override void clientClone(ModPlayer clientClone)
        {
            var clone = (HyperCrystalPlayer)clientClone;
            clone.diameter = diameter;
            clone.damageMultiplier = damageMultiplier;
            clone.hideVisual = hideVisual;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            CalcDamage(target.getRect(), ref damage);
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            CalcDamage(target.getRect(), ref damage);
        }
        public void CalcDamage(Rectangle targetRect, ref int damage)
        {
            if (diameter > 0f && Player.Distance(targetRect.ClosestDistance(Player.Center)) < diameter / 2f)
            {
                damage = (int)(damage * (1f + damageMultiplier));
            }
        }

        public void Add(float effectCircumference, float damageMultiplier, bool hideVisual)
        {
            this.diameter += effectCircumference;
            this.damageMultiplier += damageMultiplier;
            this.hideVisual |= hideVisual;
        }
    }
}