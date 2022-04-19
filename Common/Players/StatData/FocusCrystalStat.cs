using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Common.Players.StatData
{
    /// <summary>
    /// Added to by <see cref="Items.Accessories.FocusCrystal"/>
    /// </summary>
    public sealed class FocusCrystalStat : PlayerStat
    {
        public const int MinVFXCircumference = 8;

        public static FocusCrystalStat Zero => new FocusCrystalStat(0f, 0f, false);

        public float effectCircumference;
        public float damageMultiplier;
        public bool hideVisual;
        public float _accFocusCrystalCircumference;
        public float _accFocusCrystalOpacity;

        public FocusCrystalStat()
        {
        }

        public FocusCrystalStat(float radius, float damageMult, bool hide = false)
        {
            effectCircumference = radius;
            damageMultiplier = damageMult;
            hideVisual = hide;
        }

        public override PlayerStat GetNewInstance()
        {
            return Zero;
        }

        public override void ResetEffects(Player player, AequusPlayer aequus)
        {
            _accFocusCrystalCircumference = MathHelper.Lerp(_accFocusCrystalCircumference, effectCircumference, 0.2f);
            base.ResetEffects(player, aequus);
        }

        public override void UpdateDead(Player player, AequusPlayer aequus)
        {
            _accFocusCrystalCircumference = MathHelper.Lerp(_accFocusCrystalCircumference, 0f, 0.2f);
            Clear();
        }

        public override void Clear()
        {
            effectCircumference = 0f;
            damageMultiplier = 0f;
            hideVisual = false;
        }

        public override void Add(PlayerStat playerStat)
        {
            if (playerStat is FocusCrystalStat focusCrystalStat)
            {
                effectCircumference += focusCrystalStat.effectCircumference;
                damageMultiplier += focusCrystalStat.damageMultiplier;
                hideVisual |= focusCrystalStat.hideVisual;
            }
        }

        public override string ToString()
        {
            return "{Circumference: " + effectCircumference + ", DamageMultiplier: " + damageMultiplier + "}";
        }

        public static FocusCrystalStat operator +(FocusCrystalStat value1, FocusCrystalStat value2)
        {
            value1.Add(value2);
            return value1;
        }
    }
}