using Aequus.Content.Necromancy;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Aequus.UI
{
    public class NecromancyInterface : BaseUserInterface
    {
        public override string Layer => AequusUI.InterfaceLayers.EntityHealthBars_16;

        public override bool Draw(SpriteBatch spriteBatch)
        {
            for (int i = Main.maxNPCs - 1; i >= 0; i--)
            {
                if (Main.npc[i].active && (Main.npc[i].realLife == -1 || Main.npc[i].realLife == i) && Main.npc[i].TryGetGlobalNPC<NecromancyNPC>(out var n) && n.isZombie)
                {
                    n.DrawHealthbar(Main.npc[i], spriteBatch, Main.screenPosition);
                }
            }
            return true;
        }
    }
}