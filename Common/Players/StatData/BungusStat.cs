using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Aequus.Common.Players.StatData
{
    /// <summary>
    /// Used by <see cref="Items.Accessories.Mendshroom"/>
    /// </summary>
    public sealed class BungusStat : PlayerStat
    {
        public static BungusStat Zero => new BungusStat(0f, 0);

        public float circumference;
        public int regenerationIncrease;
        public float idleTime;
        public float _circumferenceForVFX;

        public bool EffectActive => idleTime >= 30;

        public BungusStat()
        {
        }

        public BungusStat(float circumference, int regenIncrease)
        {
            this.circumference = circumference;
            regenerationIncrease = regenIncrease;
        }

        public override void ResetEffects(Player player, AequusPlayer aequus)
        {
            if (circumference > 0f)
            {
                if (player.velocity.Length() < 1f)
                {
                    idleTime++;
                }
                else
                {
                    idleTime = 0;
                }
                float lerpTo = 0f;
                if (EffectActive)
                {
                    lerpTo = circumference;
                    HealPlayers(player);
                }
                _circumferenceForVFX = MathHelper.Lerp(_circumferenceForVFX, lerpTo, 0.2f);
            }
            base.ResetEffects(player, aequus);
        }
        private void HealPlayers(Player healer)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (i == healer.whoAmI || (Main.player[i].active && !Main.player[i].dead && Main.player[i].Distance(healer.Center) < circumference / 2f))
                {
                    HealPlayer(healer, i);
                }
            }
        }
        private void HealPlayer(Player healer, int i)
        {
            var aequus = Main.player[i].GetModPlayer<AequusPlayer>();
            if (aequus.bungusRegen < regenerationIncrease)
                aequus.bungusRegen = regenerationIncrease;
        }

        public override void UpdateDead(Player player, AequusPlayer aequus)
        {
            _circumferenceForVFX = MathHelper.Lerp(_circumferenceForVFX, 0f, 0.2f);
            Clear();
        }

        public override void Clear()
        {
            circumference = 0f;
            regenerationIncrease = 0;
        }

        public override PlayerStat GetNewInstance()
        {
            return Zero;
        }

        public override void Add(PlayerStat playerStat)
        {
            if (playerStat is BungusStat bungusStat)
            {
                circumference = Math.Max(circumference, bungusStat.circumference);
                regenerationIncrease += bungusStat.regenerationIncrease;
            }
        }

        public static BungusStat operator +(BungusStat value1, BungusStat value2)
        {
            value1.Add(value2);
            return value1;
        }
    }
}
