using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Umbrella
{
    public class Umberserk : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.damage = 76;
            item.useTime = 24;
            item.useAnimation = 24;
            item.rare = AQItem.Rarities.GaleStreamsRare + 1;
            item.value = AQItem.Prices.GaleStreamsValue;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.knockBack = 8f;
            item.scale = 1.1f;
            item.autoReuse = true;
            item.crit = 20;
        }
    }
}