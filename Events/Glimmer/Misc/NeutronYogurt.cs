using Aequus.Buffs;
using Aequus.Content;
using Aequus.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Events.Glimmer.Misc
{
    public class NeutronYogurt : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            this.StaticDefaultsToDrink(Color.HotPink);
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 10;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);
            Item.buffType = ModContent.BuffType<NeutronYogurtBuff>();
            Item.buffTime = 36000;
        }
    }
}

namespace Aequus.Buffs
{
    public class NeutronYogurtBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            PotionColorsDatabase.BuffToColor.Add(Type, new Color(61, 219, 255));
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.gravity *= 1.8f;
        }
    }
}