using Aequus.Common;
using Aequus.Common.PlayerLayers;
using Aequus.Items.Equipment.Armor.SetNecromancer;
using Aequus.Items.Materials.Hexoplasm;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Armor.SetSeraphim {
    [AutoloadEquip(EquipType.Body)]
    [WorkInProgress]
    public class SeraphimRobes : NecromancerRobe {
        public override void SetStaticDefaults() {
            ForceDrawShirt.BodyShowShirt.Add(Item.bodySlot);
        }

        public override void SetDefaults() {
            Item.defense = 10;
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateEquip(Player player) {
            player.GetDamage<SummonDamageClass>() += 0.2f;
            var aequus = player.Aequus();
            aequus.ghostLifespan += 3600;
            aequus.ghostSlotsMax += 2;
        }

        public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) {
        }

        public override void ArmorArmGlowMask(Player drawPlayer, float shadow, ref int glowMask, ref Color color) {
        }

        public override void AddRecipes() {
#if DEBUG
            CreateRecipe()
                .AddIngredient<NecromancerRobe>()
                .AddIngredient<Hexoplasm>(10)
                .AddTile(TileID.Loom)
                .TryRegisterBefore(ItemID.GravediggerShovel);
#endif
        }
    }
}