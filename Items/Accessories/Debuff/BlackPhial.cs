using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Debuff
{
    [AutoloadEquip(EquipType.Waist)]
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
            Item.value = Item.buyPrice(gold: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.accBlackPhial++;
            aequus.DebuffsInfliction.OverallTimeMultiplier += 0.5f;
        }
    }
}