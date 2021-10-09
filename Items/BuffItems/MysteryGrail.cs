using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.ItemOverlays;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.BuffItems
{
    public class MysteryGrail : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new DynamicInventoryGlowmask(GlowID.MysteryGrail, () => Main.DiscoColor), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 20;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.rare = ItemRarityID.Red;
            item.value = AQItem.PotionValue;
            item.maxStack = 999;
        }

        public override bool UseItem(Player player)
        {
            for (int i = 0; i < 100; i++)
            {
                int buff = Main.rand.Next(BuffLoader.BuffCount);
                if (Main.debuff[i] && Main.rand.NextBool())
                    continue;
                if (Main.buffNoTimeDisplay[i] && Main.rand.NextBool())
                    continue;
                if (Main.persistentBuff[i] && Main.rand.NextBool())
                    continue;
                player.AddBuff(buff, 3600);
                return true;
            }
            player.AddBuff(BuffID.ObsidianSkin, 3600);
            return true;
        }
    }
}