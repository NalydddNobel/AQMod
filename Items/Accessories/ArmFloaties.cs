using Aequus.Common;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class ArmFloaties : ModItem
    {
        public static List<int> EquippedCache { get; private set; }

        public override void Load()
        {
            EquippedCache = new List<int>();
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void Unload()
        {
            EquippedCache?.Clear();
            EquippedCache = null;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(20, 14);
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1, silver: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            EquippedCache.Add(player.whoAmI);
            player.Aequus().accArmFloaties++;
        }

        public static void Proc(Player player, AequusPlayer aequus, EnemyKillInfo npc)
        {
            if (aequus.accArmFloaties > 0 && player.breath < player.breathMax)
            {
                player.breath = Math.Min(player.breath + player.breathMax / 4 * aequus.accArmFloaties, player.breathMax - 1);
            }
        }
    }
}