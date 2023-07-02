using Aequus.Items.Armor.SetGravetender;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Materials.SoulGem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.SetNecromancer {
    [AutoloadEquip(EquipType.Body)]
    public class NecromancerRobe : ModItem {

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.defense = 6;
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 1);
            Item.legSlot = -1;
        }

        public override void UpdateEquip(Player player) {
            player.GetDamage<MagicDamageClass>() += 0.1f;
            player.GetDamage<SummonDamageClass>() += 0.1f;
            player.Aequus().ghostLifespan += 1800;
            player.Aequus().ghostSlotsMax++;
        }

        public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) {
            glowMaskColor = Color.White with { A = 0 } * (1f - shadow);
        }

        public override void ArmorArmGlowMask(Player drawPlayer, float shadow, ref int glowMask, ref Color color) {
            color = Color.White with { A = 0 } * (1f - shadow);
        }

        public override void EquipFrameEffects(Player player, EquipType type) {
            if (Main.netMode == NetmodeID.Server) {
                return;
            }

            player.Aequus().LegsOverlayTexture = AequusTextures.NecromancerRobe_Legs;
            player.Aequus().LegsOverlayGlowTexture = AequusTextures.NecromancerRobe_Legs_Glow;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<GravetenderRobes>()
                .AddIngredient<DemonicEnergy>(1)
                .AddIngredient<SoulGemFilled>(5)
                .AddTile(TileID.Loom)
                .TryRegisterBefore(ItemID.GravediggerShovel);
        }
    }
}