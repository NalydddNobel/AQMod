using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.HookBarbs
{
    public class HookBarbPlayer : ModPlayer
    {
        public byte BarbCount { get; set; }
        public Func<Projectile, HookBarbsProjectile, Player, HookBarbPlayer, bool> BarbPreAI { get; set; }
        public Action<Projectile, HookBarbsProjectile, Player, HookBarbPlayer> BarbAI { get; set; }
        public Action<Projectile, HookBarbsProjectile, Player, HookBarbPlayer> BarbPostAI { get; set; }

        private void ResetBarbAttachments()
        {
            BarbPreAI = null;
            BarbAI = null;
            BarbPostAI = null;
        }

        public override void Initialize()
        {
            BarbCount = byte.MaxValue;
            ResetBarbAttachments();
        }

        public override void ResetEffects()
        {
            BarbCount = 0;
            ResetBarbAttachments();
        }
    }
}