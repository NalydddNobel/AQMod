using AQMod.Assets.ItemOverlays;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Consumables.Potion
{
    public class PotionofBossrushium : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new GlowmaskOverlay(this.GetPath("_Glow"), getOutlineColor, drawInventory: true), item.type);
        }

        private Color getOutlineColor()
        {
            float colorOffset = ((float)Math.Sin(Main.GlobalTime) + 1f) * 60f;
            return new Color((int)(20 + colorOffset * 2), 10, (int)(255 - colorOffset / 2), 180);
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 26;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.maxStack = 999;
            item.consumable = true;
            item.rare = ItemRarityID.Red;
            item.value = AQItem.Prices.PotionValue;
            item.buffTime = (int)Main.nightLength;
        }

        public override bool CanUseItem(Player player) => !AQNPC.BossRush;

        public override bool UseItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<Buffs.Bossrush>(), item.buffTime);
            return true;
        }

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ItemID.Lens, 2);
            recipe.AddIngredient(ItemID.RottenChunk, 6);
            recipe.AddIngredient(ItemID.Bone, 15);
            recipe.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            recipe.AddIngredient(ItemID.SoulofLight, 8);
            recipe.AddIngredient(ItemID.SoulofNight, 8);
            recipe.AddIngredient(ItemID.Ectoplasm, 5);
            recipe.AddIngredient(ItemID.LunarTabletFragment, 8);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}