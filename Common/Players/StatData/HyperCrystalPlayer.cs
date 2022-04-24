using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Players.StatData
{
    /// <summary>
    /// Used by <see cref="Items.Accessories.HyperCrystal"/>
    /// </summary>
    public sealed class HyperCrystalPlayer : ModPlayer
    {
        public const int MinVFXCircumference = 8;

        public float effectCircumference;
        public float damageMultiplier;
        public bool hideVisual;
        public float _accFocusCrystalCircumference;
        public float _accFocusCrystalOpacity;

        public override void ResetEffects()
        {
            _accFocusCrystalCircumference = MathHelper.Lerp(_accFocusCrystalCircumference, effectCircumference, 0.2f);
            effectCircumference = 0f;
            damageMultiplier = 0f;
            hideVisual = false;
        }

        public override void UpdateDead()
        {
            _accFocusCrystalCircumference = MathHelper.Lerp(_accFocusCrystalCircumference, 0f, 0.2f);
            effectCircumference = 0f;
            damageMultiplier = 0f;
            hideVisual = true;
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            CalcDamage(target.getRect(), ref damage);
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            CalcDamage(target.getRect(), ref damage);
        }
        private void CalcDamage(Rectangle targetRect, ref int damage)
        {
            if (effectCircumference > 0f && Player.Distance(targetRect.ClosestDistance(Player.Center)) < (effectCircumference / 2f))
            {
                damage = (int)(damage * damageMultiplier);
            }
        }

        public void Add(float effectCircumference, float damageMultiplier, bool hideVisual)
        {
            this.effectCircumference += effectCircumference;
            this.damageMultiplier += damageMultiplier;
            this.hideVisual |= hideVisual;
        }
    }
}