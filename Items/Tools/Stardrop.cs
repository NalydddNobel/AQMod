using AQMod.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Items.Tools
{
    public class Stardrop : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 28;
            item.rare = AQItem.Rarities.OmegaStariteRare + 1;
            item.value = Item.buyPrice(platinum: 1);
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item8;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (lightColor.R < 60)
            {
                lightColor.R = 60;
            }
            if (lightColor.G < 60)
            {
                lightColor.G = 60;
            }
            if (lightColor.B < 60)
            {
                lightColor.B = 60;
            }
            return lightColor;
        }

        public override bool UseItem(Player player)
        {
            WorldDefeats.TownNPCMoveAtNight = !WorldDefeats.TownNPCMoveAtNight;
            if (Main.myPlayer == player.whoAmI)
            {
                if (WorldDefeats.TownNPCMoveAtNight)
                {
                    MessageBroadcast.NewMessage(Language.GetTextValue("Mods.AQMod.Stardrop.True"), new Color(240, 100, 230, 255));
                }
                else
                {
                    MessageBroadcast.NewMessage(Language.GetTextValue("Mods.AQMod.Stardrop.False"), new Color(240, 100, 230, 255));
                }
            }
            return true;
        }
    }
}