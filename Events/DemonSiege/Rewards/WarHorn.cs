using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Events.DemonSiege.Rewards
{
    public class WarHorn : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(20, 14);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accWarHorn++;
        }

        public static void EmitSound(Vector2 loc)
        {
            SoundEngine.PlaySound(Aequus.GetSound("Item/warhorn").WithVolume(0.3f), loc);
        }
    }
}