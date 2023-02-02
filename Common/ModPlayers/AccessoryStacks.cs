using Terraria.ModLoader;

namespace Aequus.Common.ModPlayers
{
    public partial class AequusPlayer : ModPlayer
    {
        public void PostUpdateEquips_AccessoryStackInteractions()
        {
            if (Player.boneGloveItem != null && Player.boneGloveTimer > 1)
            {
                Player.boneGloveTimer -= Player.boneGloveItem.Aequus().accStacks - 1;
                if (Player.boneGloveTimer < 1)
                {
                    Player.boneGloveTimer = 1;
                }
            }
        }
    }
}