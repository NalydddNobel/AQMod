using AQMod.NPCs.Friendly;
using AQMod.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace AQMod.Items.Potions.Concoctions
{
    public class PotionofContainers : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 26;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.rare = ItemRarityID.Green;
            item.value = Item.buyPrice(silver: 50);
            item.maxStack = 30;
        }

        private static int GetChestIndex()
        {
            for (int i = 0; i < Main.maxChests * 10; i++)
            {
                int index = Main.rand.Next(Main.maxChests);
                if (Main.chest[index] != null)
                {
                    var c = Main.chest[index];
                    var t = Framing.GetTileSafely(c.x, c.y);
                    if (Main.tileTable[t.type] || Chest.isLocked(c.x, c.y) || t.lava() ||
                        Main.wallDungeon[t.wall] && !NPC.downedBoss3 ||
                        t.wall == WallID.LihzahrdBrickUnsafe && !NPC.downedPlantBoss)
                    {
                        continue;
                    }
                    return index;
                }
            }
            return -1;
        }

        public override void UseStyle(Player player) // mostly reused logic from the Magic Mirror 
        {
            if (player.itemTime == 0)
            {
                player.itemTime = (int)(item.useTime / PlayerHooks.TotalUseTimeMultiplier(player, item));
            }
            else if (player.itemTime == (int)(item.useTime / PlayerHooks.TotalUseTimeMultiplier(player, item)) / 2)
            {
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, DustID.GoldCoin);
                    Main.dust[d].velocity = player.velocity * 0.2f;
                }
                int chestIndex = GetChestIndex();
                if (chestIndex == -1)
                    return;
                player.PrepareForTeleport();
                Chest chest1 = Main.chest[chestIndex];
                item.stack--;
                if (item.stack <= 0)
                    item.TurnToAir();
                player.Teleport(new Vector2((chest1.x + 1) * 16f, (chest1.y - 1) * 16f), -1);
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, DustID.GoldCoin);
                    Main.dust[d].velocity = player.velocity * 0.2f;
                }
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (ConcoctionUI.Active && InvUI.Hooks.CurrentSlotContext == ItemSlot.Context.InventoryItem
                && Main.LocalPlayer.IsTalkingTo<Memorialist>())
            {
                AQUtils.DrawUIBack(spriteBatch, mod.GetTexture("Items/ConcoctionBack"), position, frame, scale, ConcoctionUI.ChestBGColor);
            }
            return true;
        }

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.alchemy = true;
            recipe.AddIngredient(ItemID.TeleportationPotion);
            recipe.AddIngredient(ItemID.Moonglow);
            recipe.AddIngredient(ItemID.Fireblossom);
            recipe.AddIngredient(ItemID.Deathweed);
            recipe.AddIngredient(ItemID.GoldenKey);
            recipe.AddTile(TileID.Bottles);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}