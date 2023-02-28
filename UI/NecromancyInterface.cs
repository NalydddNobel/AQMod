using Aequus.Content.Necromancy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.UI
{
    public class NecromancyInterface : BaseUserInterface
    {
        public override string Layer => AequusUI.InterfaceLayers.EntityHealthBars_16;

        public override bool Draw(SpriteBatch spriteBatch)
        {
            for (int i = Main.maxNPCs - 1; i >= 0; i--)
            {
                if (Main.npc[i].active && (Main.npc[i].realLife == -1 || Main.npc[i].realLife == i) && !Main.npc[i].Aequus().isChildNPC && Main.npc[i].TryGetGlobalNPC<NecromancyNPC>(out var n) && n.isZombie && !n.statFreezeLifespan)
                {
                    n.DrawHealthbar(Main.npc[i], spriteBatch, Main.screenPosition);
                }
            }
            var aequus = Main.LocalPlayer.Aequus();
            if (aequus.selectGhostNPC > -1 && Main.npc[aequus.selectGhostNPC].IsZombieAndInteractible(Main.myPlayer))
            {
                var texture = ModContent.Request<Texture2D>("Aequus/Assets/UI/NecromancySelectionCursor").Value;
                var frame = texture.Frame(verticalFrames: 8, frameY: (int)(Main.GameUpdateCount / 6 % 8));
                spriteBatch.Draw(texture, Main.npc[aequus.selectGhostNPC].Center - Main.screenPosition, frame, new Color(255, 255, 255, 128) * 0.65f, 0f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}