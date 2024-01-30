using ReLogic.Content;
using System;
using Terraria.Graphics.Shaders;

namespace Aequus.Content.Dyes;

public sealed class DyesInstantiator : ModSystem {
    public static ModItem HueshiftDye { get; private set; }

    private InstancedDyeItem RegisterDye(String name, Func<ArmorShaderData> shaderDataFactory, Int32 itemRarity = ItemRarityID.Blue, Int32 value = Item.silver * 50) {
        var item = new InstancedDyeItem(name, shaderDataFactory, itemRarity, value);
        Mod.AddContent(item);
        return item;
    }

    private InstancedDyeItem RegisterDye(String name, String pass, Ref<Effect> effect, Int32 itemRarity = ItemRarityID.Blue, Int32 value = Item.silver * 50, Single useOpacity = 1f, Color? useColor = null) {
        return RegisterDye(name, () => new ArmorShaderData(effect, pass).UseOpacity(useOpacity).UseColor(useColor ?? Color.Transparent), itemRarity, value);
    }

    public override void Load() {
        Ref<Effect> effect = null;
        if (!Main.dedServ) {
            effect = new Ref<Effect>(ModContent.Request<Effect>($"Aequus/Assets/Shaders/DyeShaders", AssetRequestMode.ImmediateLoad).Value);
        }
        RegisterDye("CensorDye", "CensorPass", effect, useOpacity: 4f);
        RegisterDye("DiscoDye", "DiscoPass", effect, itemRarity: ItemRarityID.Green)
            .WithCustomRecipe((m) => {
                m.CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.RainbowTorch)
                .AddTile(TileID.DyeVat)
                .Register();
            });
        HueshiftDye = RegisterDye("HueshiftDye", "HueShiftPass", effect, itemRarity: ItemRarityID.Green)
            .WithCustomRecipe((m) => {
                m.CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.RainbowBrick)
                .AddTile(TileID.DyeVat)
                .Register();
            });
        RegisterDye("EnchantedDye", "EnchantmentPass", effect, itemRarity: ItemRarityID.Orange, useOpacity: 0.8f)
            .SafeAddImage(AequusTextures.EnchantedDyeEffect)
            .WithCustomRecipe((m) => {
                m.CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddTile(TileID.DyeVat)
                .Register();
            });
        RegisterDye("FrostbiteDye", "FrostbitePass", effect, itemRarity: ItemRarityID.Orange)
            .SafeAddImage(AequusTextures.FrostbiteDyeEffect)
            .WithCustomRecipe((m) => {
                m.CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.FrostCore)
                .AddTile(TileID.DyeVat)
                .Register();
            });
        RegisterDye("ScorchingDye", "ScorchingPass", effect, itemRarity: ItemRarityID.Orange, useColor: new Color(140, 0, 21))
            .SafeAddImage(AequusTextures.EffectNoise)
            .WithCustomRecipe((m) => {
                m.CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.AncientBattleArmorMaterial)
                .AddTile(TileID.DyeVat)
                .Register();
            });
    }
}