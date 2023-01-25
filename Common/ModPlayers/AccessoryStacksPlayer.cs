using Terraria.ModLoader;

namespace Aequus.Common.ModPlayers
{
    public class AccessoryStacksPlayer : ModPlayer
    {
        public override void PostUpdateEquips()
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