using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Umbrella
{
    public class Umystick : ModItem, IUmbrellaDamage
    {
        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.damage = 45;
            item.useTime = 24;
            item.useAnimation = 24;
            item.rare = AQItem.Rarities.GaleStreamsRare + 1;
            item.value = AQItem.Prices.GaleStreamsValue;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.magic = true;
            item.knockBack = 4f;
            item.autoReuse = true;
            item.mana = 20;
        }
    }
}