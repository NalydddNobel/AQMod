using AQMod.Common.Graphics.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod
{
    public class AQPlayer : ModPlayer
    {
        public bool blueFire;

        public override void ResetEffects()
        {
            blueFire = false;
        }

        public override void UpdateDead()
        {
            blueFire = false;
        }

        public override void UpdateBadLifeRegen()
        {
            if (blueFire)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 10;
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (blueFire)
            {
                if (Main.netMode != NetmodeID.Server && AQMod.GameWorldActive)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var pos = drawInfo.Position - new Vector2(2f, 2f);
                        var rect = new Rectangle((int)pos.X, (int)pos.Y, drawInfo.drawPlayer.width + 4, drawInfo.drawPlayer.height + 4);
                        var dustPos = new Vector2(Main.rand.Next(rect.X, rect.X + rect.Width), Main.rand.Next(rect.Y, rect.Y + rect.Height));
                        var particle = new BurningParticle(dustPos, new Vector2((drawInfo.drawPlayer.velocity.X + Main.rand.NextFloat(-3f, 3f)) * 0.3f, ((drawInfo.drawPlayer.velocity.Y + Main.rand.NextFloat(-3f, 3f)) * 0.4f).Abs() - 2f),
                            new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f), Main.rand.NextFloat(0.2f, 1.2f));
                        Main.ParticleSystem_World_OverPlayers.Add(particle);
                    }
                }
                Lighting.AddLight(drawInfo.drawPlayer.Center, 0.4f, 0.4f, 1f);
                fullBright = true;
            }
        }
    }
}