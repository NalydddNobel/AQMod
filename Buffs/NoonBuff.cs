using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs
{
    public class NoonBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            Lighting.AddLight(player.Center, new Vector3(0.9f, 0.75f, 0.3f));
            player.nightVision = true;
        }
    }
}