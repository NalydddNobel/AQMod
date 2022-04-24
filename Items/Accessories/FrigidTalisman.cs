using Aequus.Items.Consumables.Potions;
using Aequus.Items.Misc;
using Aequus.Items.Misc.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class FrigidTalisman : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.accessory = true;
            Item.rare = ItemRarityConstants.GaleStreams;
            Item.value = ItemPriceProperties.GaleStreamsValue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AequusPlayer>().resistHeat = true;
            player.fireWalk = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.Burning] = true;
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.ShadowFlame] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Debuffs.BlueFire>()] = true;
            //player.buffImmune[ModContent.BuffType<Buffs.Debuffs.Sparkling>()] = true;
            //player.buffImmune[ModContent.BuffType<Buffs.Debuffs.CorruptionHellfire>()] = true;
            //player.buffImmune[ModContent.BuffType<Buffs.Debuffs.CrimsonHellfire>()] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FrostPotion>(3)
                .AddIngredient<AtmosphericEnergy>()
                .AddIngredient<FrozenTear>(20)
                .AddIngredient(ItemID.SoulofFlight, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}