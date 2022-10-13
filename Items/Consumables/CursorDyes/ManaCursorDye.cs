using Aequus.Content.CursorDyes;
using Microsoft.Xna.Framework;
using ShopQuotesMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.CursorDyes
{
    public class ManaCursorDye : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            CursorDyeSystem.Register(Type, new ColorChangeCursor(() => Color.Lerp(Color.White, Color.Blue, MathHelper.Clamp(Main.LocalPlayer.statMana / (float)Main.LocalPlayer.statManaMax2, 0f, 1f))));
            ModContent.GetInstance<QuoteDatabase>().AddNPC(NPCID.DyeTrader, Mod).SetQuote(Type, "Mods.Aequus.Chat.DyeTrader.ShopQuote.ManaCursorDye");
        }

        public override void SetDefaults()
        {
            Item.DefaultToCursorDye();
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 1);
        }

        public override bool CanUseItem(Player player)
        {
            return player.Aequus().CursorDye != CursorDyeSystem.ItemIDToCursor(Type).Type;
        }

        public override bool? UseItem(Player player)
        {
            player.Aequus().CursorDye = CursorDyeSystem.ItemIDToCursor(Type).Type;
            return true;
        }
    }
}