using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public class AequusItem : GlobalItem
    {
        public static HashSet<int> CritOnlyModifier { get; private set; }

        public override void Load()
        {
            CritOnlyModifier = new HashSet<int>() 
            {
                PrefixID.Keen,
                PrefixID.Zealous,
            };
        }

        public override void Unload()
        {
            CritOnlyModifier?.Clear();
            CritOnlyModifier = null;
        }

        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            if (player.GetModPlayer<AequusPlayer>().permMoro && ItemsCatalogue.SummonStaff.Contains(item.type))
            {
                mult = 0f;
            }
        }

        public override float UseSpeedMultiplier(Item item, Player player)
        {
            if (player.GetModPlayer<AequusPlayer>().permMoro && ItemsCatalogue.SummonStaff.Contains(item.type))
            {
                return 2f;
            }
            return 1f;
        }

        public static int NewItemCloned(IEntitySource source, Vector2 pos, Item item)
        {
            int i = Item.NewItem(source, pos, item.type, item.stack);
            Main.item[i] = item.Clone();
            Main.item[i].active = true;
            Main.item[i].whoAmI = i;
            Main.item[i].Center = pos;
            Main.item[i].stack = item.stack;
            return i;
        }
    }
}