using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Summon
{
    public class Breadsoul : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.accessory = true;
            item.rare = AQItem.RarityHardmodeDungeon;
            item.value = Item.sellPrice(gold: 10);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255 - item.alpha);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.npcTypeNoAggro[NPCID.DungeonSpirit] = true;
            player.npcTypeNoAggro[ModContent.NPCType<NPCs.Monsters.Heckto>()] = true;
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.grabReach += 0.25f;
            aQPlayer.breadSoul = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Ectoplasm, 15);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>(), 5);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}