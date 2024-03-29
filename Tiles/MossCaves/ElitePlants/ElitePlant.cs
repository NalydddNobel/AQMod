﻿using Aequus.Common.Recipes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.MossCaves.ElitePlants {
    public abstract class ElitePlantBase : ModItem {
        public abstract int Style { get; }

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<ElitePlantTile>(), Style);
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 2, copper: 50);
        }
    }
    public abstract class ElitePlantHostileBase<T> : ElitePlantBase where T : ElitePlantBase {
        public override string Texture => Helper.GetPath<T>();
        public override int Style => ModContent.GetInstance<T>().Style;
        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            AequusRecipes.AddShimmerCraft(ModContent.ItemType<T>(), Type);
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
            ItemID.Sets.DrawUnsafeIndicator[Type] = true;
        }

        public override void SetDefaults() {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<EliteBuffPlantsHostile>();
        }
    }

    [LegacyName("ArgonMushroom")]
    public class ElitePlantArgon : ElitePlantBase {
        public override int Style => ElitePlantTile.Argon;
    }
    public class ElitePlantArgonHostile : ElitePlantHostileBase<ElitePlantArgon> { }

    [LegacyName("KryptonMushroom")]
    public class ElitePlantKrypton : ElitePlantBase {
        public override int Style => ElitePlantTile.Krypton;
    }
    public class ElitePlantKryptonHostile : ElitePlantHostileBase<ElitePlantKrypton> { }

    public class ElitePlantNeon : ElitePlantBase {
        public override int Style => ElitePlantTile.Neon;
    }
    public class ElitePlantNeonHostile : ElitePlantHostileBase<ElitePlantNeon> { }

    [LegacyName("XenonMushroom")]
    public class ElitePlantXenon : ElitePlantBase {
        public override int Style => ElitePlantTile.Xenon;
    }
    public class ElitePlantXenonHostile : ElitePlantHostileBase<ElitePlantXenon> { }
}