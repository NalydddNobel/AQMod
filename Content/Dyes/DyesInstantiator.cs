using Aequus.Content.Items.Material;
using Aequus.Content.Items.Material.Energy.Cosmic;
using Aequus.Content.Items.Material.OmniGem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Dyes;

public class DyesInstantiator : ModSystem {
    private InstancedDyeItem RegisterDye(string name, Func<ArmorShaderData> shaderDataFactory, int itemRarity = ItemRarityID.Blue, int value = Item.silver * 50) {
        var item = new InstancedDyeItem(name, shaderDataFactory, itemRarity, value);
        Mod.AddContent(item);
        return item;
    }

    private InstancedDyeItem RegisterDye(string name, string pass, Ref<Effect> effect, int itemRarity = ItemRarityID.Blue, int value = Item.silver * 50, float useOpacity = 1f, Color? useColor = null) {
        return RegisterDye(name, () => new ArmorShaderData(effect, pass).UseOpacity(useOpacity).UseColor(useColor ?? Color.Transparent), itemRarity, value);
    }

    public override void Load() {
        var effect = new Ref<Effect>(ModContent.Request<Effect>($"Aequus/Assets/Shaders/DyeShaders", AssetRequestMode.ImmediateLoad).Value);
        RegisterDye("AncientBreakdownDye", "BreakdownPass", effect, useOpacity: 1f);
        RegisterDye("CensorDye", "CensorPass", effect, useOpacity: 4f);
        RegisterDye("DiscoDye", "DiscoPass", effect, itemRarity: ItemRarityID.Green)
            .WithCustomRecipe((m) => {
                m.CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<OmniGem>()
                .AddTile(TileID.DyeVat)
                .Register();
            });
        RegisterDye("HueshiftDye", "HueShiftPass", effect, itemRarity: ItemRarityID.Green)
            .WithCustomRecipe((m) => {
                m.CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<OmniGem>()
                .AddTile(TileID.DyeVat)
                .Register();
            });
        RegisterDye("EnchantedDye", "EnchantmentPass", effect, itemRarity: ItemRarityID.Orange, useOpacity: 0.8f)
            .SafeAddImage(AequusTextures.EnchantedDyeEffect)
            .WithCustomRecipe((m) => {
                m.CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.DyeVat)
                .Register();
            });
        RegisterDye("FrostbiteDye", "FrostbitePass", effect, itemRarity: ItemRarityID.Orange)
            .SafeAddImage(AequusTextures.FrostbiteDyeEffect)
            .WithCustomRecipe((m) => {
                m.CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<FrozenTear>()
                .AddTile(TileID.DyeVat)
                .Register();
            });
        RegisterDye("ScorchingDye", "ScorchingPass", effect, itemRarity: ItemRarityID.Orange, useColor: new Color(140, 0, 21))
            .SafeAddImage(AequusTextures.EffectNoise)
            .WithCustomRecipe((m) => {
                m.CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<Fluorescence>()
                .AddTile(TileID.DyeVat)
                .Register();
            });
    }
}