﻿using Aequus.Common;
using Aequus.Common.Items;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Materials.Hexoplasm;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Insurgent {
    [AutoloadGlowMask]
    [WorkInProgress]
    public class Insurgency : SceptreBase {
        public override Color GlowColor => Color.Teal;
        public override int DustSpawn => DustID.Vortex;

        public override void SetDefaults() {
            Item.DefaultToNecromancy(50);
            Item.SetWeaponValues(125, 0.8f, 0);
            Item.shootSpeed = 30f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 5);
            Item.mana = 20;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
        }

        public override void AddRecipes() {
#if DEBUG
            CreateRecipe()
                .AddIngredient<Revenant.Revenant>()
                .AddIngredient<Hexoplasm>(5)
                .AddIngredient<DemonicEnergy>()
                .AddTile(TileID.MythrilAnvil)
                .TryRegisterAfter(ItemID.RainbowRod);
#endif
        }
    }
}