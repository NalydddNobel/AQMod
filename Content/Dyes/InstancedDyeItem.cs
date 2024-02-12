using Aequus.Core.Assets;
using Aequus.Core.ContentGeneration;
using System;
using Terraria.Graphics.Shaders;

namespace Aequus.Content.Dyes;

[Autoload(false)]
internal sealed class InstancedDyeItem : InstancedModItem {
    public readonly int _rarity;
    public readonly int _value;

    [CloneByReference]
    private Func<ArmorShaderData> _shaderDataFactory;
    [CloneByReference]
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

    public InstancedDyeItem(string name, Func<ArmorShaderData> shaderDataFactory, int itemRarity = ItemRarityID.Blue, int value = Item.silver * 50) : base(name, $"{Helper.NamespaceFilePath(typeof(InstancedDyeItem))}/Items/{name}") {
        _shaderDataFactory = shaderDataFactory;
        _rarity = itemRarity;
        _value = value;
    }

    public override string LocalizationCategory => "Misc.Dyes";

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