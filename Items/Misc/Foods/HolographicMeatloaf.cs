﻿using Aequus.Common.Items;
using Aequus.NPCs.Town.PhysicistNPC.Analysis;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Foods {
    public class HolographicMeatloaf : ModItem {
        public override void SetStaticDefaults() {
            this.StaticDefaultsToFood(Color.Brown.UseA(0) * 0.75f, Color.RosyBrown.UseA(0) * 0.75f, Color.Red.UseA(0) * 0.75f);
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults() {
            Item.DefaultToFood(20, 20, BuffID.WellFed, 3600);
            Item.maxStack = 1;
            Item.consumable = false;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 5);
        }

        public override Color? GetAlpha(Color lightColor) {
            return lightColor.MaxRGBA(128).UseA(150) * Helper.Wave(Main.GlobalTimeWrappedHourly * 10f, 0.66f, 1f);
        }
    }
}