﻿using Aequus.Common.Effects.ShaderData;
using Aequus.Content.Items.Materials.OmniGem;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Aequus.Items.Misc.Dyes {
    public class RainbowOutlineDye : DyeItemBase
    {
        public override int Rarity => ItemRarityID.Green;

        public override string Pass => "OutlineColorPass";

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorShaderDataDynamicColor(Effect, Pass, (e, d) => Main.DiscoColor);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<OutlineDye>()
                .AddIngredient<OmniGem>()
                .AddTile(TileID.DyeVat)
                .TryRegisterAfter(ItemID.IntenseRainbowDye);
        }
    }
}