using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AQMod.UI
{
    public sealed class InGameHookablesUI : UIState
    {
        public readonly Texture2D Texture;
        private Rectangle frame;
        private Vector2 origin;

        public int grappleTarget;

        public InGameHookablesUI()
        {
            Texture = AQMod.Texture("Assets/UI/MeathookNPC");
            CheckTexture();
        }

        public void CheckTexture()
        {
            frame = Texture.Frame();
            origin = new Vector2(frame.Width / 2f, 0f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var player = Main.LocalPlayer;
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            grappleTarget = -1;
            if (!aQPlayer.meathookUI)
            {
                return;
            }


            float distance = (ProjectileLoader.GetProjectile(player.miscEquips[4].type)?.GrappleRange()).GetValueOrDefault(480f);
            float cursorDistance = 320f;

            int target = -1;
            var plrCenter = player.Center;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && AQNPC.CanBeMeathooked(Main.npc[i]))
                {
                    float c = Main.npc[i].Distance(plrCenter);
                    if (c < distance - Main.npc[i].Size.Length())
                    {
                        float distanceFromCursor = Main.npc[i].Distance(Main.MouseWorld);
                        if (distanceFromCursor < cursorDistance)
                        {
                            target = i;
                            distance = distanceFromCursor;
                        }
                    }
                }
            }

            if (target != -1)
            {
                grappleTarget = target;
                aQPlayer.meathookTarget = grappleTarget;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (grappleTarget == -1)
            {
                return;
            }

            if (!Main.npc[grappleTarget].active)
            {
                grappleTarget = -1;
                return;
            }
            var drawPosition = Main.npc[grappleTarget].Bottom + new Vector2(0f, 12f);
            Main.spriteBatch.Draw(Texture, drawPosition - Main.screenPosition, frame, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}