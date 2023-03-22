using Aequus.Common;
using Aequus.Items.Armor.Gravetender;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Materials.Gems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.Necromancer {
    [AutoloadEquip(EquipType.Body)]
    public class NecromancerRobe : ModItem {

        public override void Load() {
            GlowMasksHandler.AddGlowmask(AequusTextures.NecromancerRobe_Body_Glow.Path);
        }

        public override void SetStaticDefaults() {
            SacrificeTotal = 1;
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
            player.GetDamage<SummonDamageClass>() += 0.2f;
            player.Aequus().ghostLifespan += 1800;
            player.Aequus().ghostSlotsMax++;
        }

        public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) {
            glowMask = GlowMasksHandler.GetID(AequusTextures.NecromancerRobe_Body_Glow.Path);
            glowMaskColor = Color.White with { A = 0 } * (1f - shadow);
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