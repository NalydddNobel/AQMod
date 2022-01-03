using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ModLoader;

namespace AQMod.NPCs.Boss
{
    public sealed class OmegaStariteBossBar : ModBossBar
    {
        private int _headIndex;

        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            if (_headIndex != -1)
            {
                return TextureAssets.NpcHeadBoss[_headIndex];
            }
            return null;
        }

        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float lifePercent, ref float shieldPercent)
        {
            NPC NPC = Main.npc[info.npcIndexToAimAt];
            if (!NPC.active || (int)NPC.ai[0] == OmegaStarite.PHASE_DEAD)
                return false;

            _headIndex = NPC.GetBossHeadTextureIndex();
            lifePercent = Utils.Clamp(NPC.life / (float)NPC.lifeMax, 0f, 1f);

            return true;
        }
    }
}