using Aequus.Content.Necromancy.Rendering;
using Aequus.Core.Graphics;
using Aequus.Items.Materials.SoulGem;
using Aequus.Content.Necromancy;

namespace Aequus.Content.Necromancy.Equipment.Accessories.Chains;

public class SouljointCuffs : ModItem {
    public override void SetStaticDefaults() {
        if (Main.netMode != NetmodeID.Server) {
            DrawLayers.Instance.WorldBehindTiles += DrawChainedNPCs;
        }
    }

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.accessory = true;
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.sellPrice(gold: 3);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().ghostChains++;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.Shackle)
            .AddIngredient<SoulGemFilled>(5)
            .AddTile(TileID.DemonAltar)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.MagicCuffs);
    }

    public static void DrawChainedNPCs(SpriteBatch spriteBatch) {
        if (GhostRenderer.ChainedUpNPCs.Count > 0) {
            var t = AequusTextures.SoulChains_Chains;
            foreach (var v in GhostRenderer.ChainedUpNPCs) {
                int i = 0;
                Helper.DrawChain(t, v.Item2.Center, v.Item1.Center, Main.screenPosition, (loc) => {
                    var zombie = v.Item1.GetGlobalNPC<NecromancyNPC>();
                    float m = 0.5f;
                    if (zombie.ghostChainsTime < 30) {
                        m *= zombie.ghostChainsTime / 30f;
                    }
                    return GhostRenderer.GetColorTarget(Main.player[zombie.zombieOwner], zombie.renderLayer).getDrawColor() with { A = 128 } * m;
                });
            }
            GhostRenderer.ChainedUpNPCs.Clear();
        }
    }
}