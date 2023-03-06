using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.CursorDyes.Items
{
    public abstract class CursorDyeBase : ModItem
    {
        public int cursorDyeID;

        public abstract ICursorDye InitalizeDye();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            CursorDyeSystem.Register(Type, InitalizeDye());
            CursorDyeSystem.IsCursorAcc.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.vanity = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(gold: 1);
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            cursorDyeID = CursorDyeSystem.ItemIDToCursorID(Type);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().CursorDye = cursorDyeID;
        }

        public override void UpdateVanity(Player player)
        {
            player.Aequus().CursorDye = cursorDyeID;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            if (CursorDyeSystem.IsCursorAcc.Contains(incomingItem.type) && !equippedItem.IsAir)
            {
                return !CursorDyeSystem.IsCursorAcc.Contains(equippedItem.type);
            }
            return true;
        }
    }
}
