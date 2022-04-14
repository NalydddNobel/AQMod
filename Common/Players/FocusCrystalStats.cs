using Terraria;

namespace Aequus.Common.Players
{
    public struct FocusCrystalStats : IPlayerStat
    {
        public const int MinVFXCircumference = 8;

        public float effectCircumference;
        public float damageMultiplier;
        public bool hideVisual;

        public FocusCrystalStats(float radius, float damageMult, bool hide = false)
        {
            effectCircumference = radius;
            damageMultiplier = damageMult;
            hideVisual = hide;
        }

        public void ResetEffects(Player player, AequusPlayer aequus)
        {
            Clear();
        }

        public void Clear()
        {
            effectCircumference = 0f;
            damageMultiplier = 0f;
            hideVisual = false;
        }

        public override string ToString()
        {
            return "{Circumference: " + effectCircumference + ", DamageMultiplier: " + damageMultiplier + "}";
        }

        public static FocusCrystalStats operator +(FocusCrystalStats value1, FocusCrystalStats value2)
        {
            return new FocusCrystalStats(value1.effectCircumference + value2.effectCircumference, value1.damageMultiplier + value2.damageMultiplier, value1.hideVisual || value2.hideVisual);
        }
    }
}