﻿using Aequus.Common.Items;
using Aequus.Core.Assets;
using System;
using Terraria.Graphics.Shaders;

namespace Aequus.Content.Dyes;

[Autoload(false)]
internal sealed class InstancedDyeItem : InstancedModItem {
    public readonly Int32 _rarity;
    public readonly Int32 _value;
    private Func<ArmorShaderData> _shaderDataFactory;

    private Action<ModItem> customRecipes;

    public InstancedDyeItem SafeAddImage(RequestCache<Texture2D> texture) {
        var oldMethod = _shaderDataFactory;
        _shaderDataFactory = () => {
            var oldValue = oldMethod();
            return oldValue.UseImage(texture.Asset);
        };
        return this;
    }

    public InstancedDyeItem WithCustomRecipe(Action<ModItem> recipeFactory) {
        customRecipes += recipeFactory;
        return this;
    }

    private InstancedDyeItem AddAncientVariant() {
        return this;
    }

    public InstancedDyeItem(String name, Func<ArmorShaderData> shaderDataFactory, Int32 itemRarity = ItemRarityID.Blue, Int32 value = Item.silver * 50) : base(name, $"{Helper.NamespaceFilePath(typeof(InstancedDyeItem))}/Items/{name}") {
        _shaderDataFactory = shaderDataFactory;
        _rarity = itemRarity;
        _value = value;
    }

    public override String LocalizationCategory => "Misc.Dyes";

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 3;
        if (!Main.dedServ) {
            GameShaders.Armor.BindShader(Type, _shaderDataFactory());
        }
    }

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 20;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = _value;
        Item.rare = _rarity;
    }

    public override void AddRecipes() {
        customRecipes?.Invoke(this);
    }
}