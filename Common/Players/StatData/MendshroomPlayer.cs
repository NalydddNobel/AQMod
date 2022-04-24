using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Players.StatData
{
    /// <summary>
    /// Used by <see cref="Items.Accessories.Mendshroom"/>
    /// </summary>
    public sealed class MendshroomPlayer : ModPlayer
    {
        public float circumference;
        public int regenerationToGive;
        public int idleTime;
        public float _circumferenceForVFX;

        public int increasedRegen;

        public bool EffectActive => idleTime >= 30;

        public override void ResetEffects()
        {
            float lerpTo = 0f;
            if (circumference > 0f)
            {
                if (Player.velocity.Length() < 1f)
                {
                    idleTime++;
                }
                else
                {
                    idleTime = 0;
                }
                if (EffectActive)
                {
                    lerpTo = circumference;
                    HealPlayers(Player);
                }
            }
            _circumferenceForVFX = MathHelper.Lerp(_circumferenceForVFX, lerpTo, 0.2f);
            circumference = 0f;
            regenerationToGive = 0;
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
            var bungus = Main.player[i].GetModPlayer<MendshroomPlayer>();
            if (bungus.increasedRegen < regenerationToGive)
                bungus.increasedRegen = regenerationToGive;
        }

        public override void UpdateDead()
        {
            _circumferenceForVFX = MathHelper.Lerp(_circumferenceForVFX, 0f, 0.2f);
            circumference = 0f;
            regenerationToGive = 0;
        }

        public override void UpdateLifeRegen()
        {
            Player.AddLifeRegen(increasedRegen);
        }

        public void Add(float circumference, int regen)
        {
            this.circumference = Math.Max(this.circumference, circumference);
            regenerationToGive += regen;
        }
    }
}
