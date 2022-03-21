using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs
{
    public class NoonBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            Lighting.AddLight(player.Center, new Vector3(1.2f, 0.1f, 0.5f));
            player.nightVision = true;
            player.GetModPlayer<AQPlayer>().fakeDaytime = 1;
        }
    }
}