using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Foods
{
    public class SuspiciousLookingSteak : ModItem
    {
        public override string Texture
        {
            get
            {
                string path = this.GetPath();
                if (AQMod.AprilFools)
                    return path + "_AprilFools";
                return path;
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 20;
            item.useTime = 20;
            item.useTurn = true;
            item.UseSound = SoundID.Item2;
            item.maxStack = 999;
            item.consumable = true;
            item.rare = ItemRarityID.LightRed;
            item.value = Item.buyPrice(gold: 2);
            item.buffType = BuffID.WellFed;
            item.buffTime = 162000;
        }

        public override bool UseItem(Player player)
        {
            player.AddBuff(BuffID.Regeneration, item.buffTime);
            player.AddBuff(BuffID.NightOwl, item.buffTime);
            player.AddBuff(BuffID.Ironskin, item.buffTime);
            player.AddBuff(BuffID.Swiftness, item.buffTime);
            player.AddBuff(BuffID.Gills, item.buffTime);
            return true;
        }
    }
}