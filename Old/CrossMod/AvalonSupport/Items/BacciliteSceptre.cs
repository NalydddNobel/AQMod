﻿using Aequus.Content.CrossMod;
using Aequus.Core.CrossMod;
using Aequus.Core.Initialization;
using Aequus.Old.Content.Necromancy;
using Aequus.Old.Content.Necromancy.Sceptres.Evil;

namespace Aequus.Old.CrossMod.AvalonSupport.Items;

[AutoloadGlowMask]
public class BacciliteSceptre : CrossModItem {
    public override void SafeSetStaticDefaults() {
        Item.staff[Type] = true;
    }

    public override void SetDefaults() {
        Item.DefaultToNecromancy(30);
        Item.SetWeaponValues(10, 1f, 0);
        Item.shoot = ModContent.ProjectileType<BacciliteSceptreProj>();
        Item.shootSpeed = 9f;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 50);
        Item.mana = 20;
        Item.UseSound = SoundID.Item8;
    }

    public override bool MagicPrefix() {
        return true;
    }

    protected override void SafeAddRecipes() {
        if (!Avalon.TryGetItem("BacciliteBar", out ModItem bacciliteBar)) {
            return;
        }

        CreateRecipe()
            .AddIngredient(bacciliteBar.Type, 6)
            .AddIngredient(ItemID.LifeCrystal)
            .AddTile(TileID.Anvils)
            .Register()
            .SortAfterFirstRecipesOf(ModContent.ItemType<CrimsonSceptre>());
    }
}