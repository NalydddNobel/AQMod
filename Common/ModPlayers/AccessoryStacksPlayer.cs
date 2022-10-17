using Terraria.ModLoader;

namespace Aequus.Common.Players
{
    public class AccessoryStacksPlayer : ModPlayer
    {
        public override void PostUpdateEquips()
        {
            if (Player.boneGloveItem != null)
            {
                Player.boneGloveTimer -= Player.boneGloveItem.Aequus().accStacks - 1;
            }
        }
    }
}