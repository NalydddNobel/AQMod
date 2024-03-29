﻿using Aequus.Items.Materials.GaleStreams;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Aequus.Items.Misc.Dyes {
    public class ScorchingDye : DyeItemBase
    {
        public override Ref<Effect> Effect => FromAssetFolder("Dyes/ScorchingDyeShader");
        public override string Pass => "ScorchingDyePass";
        public override int Rarity => ItemRarityID.Orange;

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorShaderData(Effect, Pass).UseColor(new Color(140, 0, 21, 255)).UseImage("Images/Misc/noise");
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<Fluorescence>()
                .AddTile(TileID.DyeVat)
                .TryRegisterAfter(ItemID.FlameDye);
        }
    }
}