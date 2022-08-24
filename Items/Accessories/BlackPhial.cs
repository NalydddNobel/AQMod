using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class BlackPhial : ModItem
    {
        public static List<int> DebuffsAfflicted { get; private set; }

        public override void Load()
        {
            DebuffsAfflicted = new List<int>()
            {
                BuffID.Poisoned,
                BuffID.OnFire3,
                BuffID.Frostburn2,
                BuffID.CursedInferno,
                BuffID.Ichor,
                BuffID.ShadowFlame,
            };
        }

        public override void Unload()
        {
            DebuffsAfflicted?.Clear();
            DebuffsAfflicted = null;
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(14, 20);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.accVial = 4;
            aequus.enemyDebuffDuration += 1f;
        }
    }
}