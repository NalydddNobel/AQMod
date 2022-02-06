using AQMod.Content.World.Events.DemonSiege;
using AQMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Effects.WorldEffects
{
    public class DemonSiegeSpawnEffect : WorldVisualEffect
    {
        private readonly DemonSiegeEnemy _enemy;
        private int _timer;

        public DemonSiegeSpawnEffect(int x, int y, DemonSiegeEnemy enemy) : base(x, y)
        {
            _timer = DemonSiegeEvent.SPAWN_ENEMY_DELAY;
            _enemy = enemy;
        }

        public override bool Update()
        {
            float offX = (float)Math.Sin((60 - _timer) * 0.1f) * _enemy.spawnWidth;
            offX *= _timer % 2 == 0 ? 1 : -1;
            var spawnPosition = new Vector2(x + offX, y);
            int d = Dust.NewDust(spawnPosition, 2, 2, ModContent.DustType<DemonSpawnDust>());
            Main.dust[d].velocity.X *= 0.5666f;
            Main.dust[d].velocity.Y -= 3.666f;
            _timer--;
            if (_timer < 0)
            {
                Main.PlaySound(SoundID.DD2_BetsyFlameBreath.WithVolume(0.6f), new Vector2(x, y));
                return false;
            }
            return true;
        }
    }
}