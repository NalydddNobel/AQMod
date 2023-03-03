using Aequus.Common;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials.Gems
{
    public class SoulGem : ModItem
    {
        public virtual int TransformID => ModContent.ItemType<SoulGemFilled>();

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Amber;
            SacrificeTotal = 25;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EmptySoulGemTile>());
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(silver: 75);
        }

        public static void TryFillSoulGems(Player player, AequusPlayer aequus, EnemyKillInfo npc)
        {
            var soulGem = player.FindItemInInvOrVoidBag((item) => item.ModItem is SoulGem soulGemBase, out bool inVoidBag);
            if (soulGem != null)
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    soulGem.stack--;
                    Item newSoulGem = null;
                    if (!inVoidBag)
                    {
                        var canStack = player.FindItem((item) => item.type == soulGem.ModItem<SoulGem>().TransformID && item.stack < item.maxStack);
                        if (canStack != null)
                        {
                            newSoulGem = canStack;
                            newSoulGem.stack++;
                        }
                        else if (soulGem.stack <= 0)
                        {
                            soulGem.stack = 1;
                            newSoulGem = soulGem;
                        }
                    }
                    if (newSoulGem == null && inVoidBag)
                    {
                        var canStack = player.bank4.FindItem((item) => item.type == soulGem.ModItem<SoulGem>().TransformID && item.stack < item.maxStack);
                        if (canStack != null)
                        {
                            newSoulGem = canStack;
                            newSoulGem.stack++;
                        }
                        else
                        {
                            canStack = player.bank4.FindEmptySlot();
                            if (canStack != null)
                            {
                                newSoulGem = canStack;
                            }
                            else if (soulGem.stack <= 0)
                            {
                                soulGem.stack = 1;
                                newSoulGem = soulGem;
                            }
                        }
                    }

                    if (newSoulGem == null)
                    {
                        newSoulGem = player.QuickSpawnItemDirect(player.GetSource_OpenItem(soulGem.type), soulGem.ModItem<SoulGem>().TransformID);
                        newSoulGem.newAndShiny = !ItemID.Sets.NeverAppearsAsNewInInventory[soulGem.type];
                    }
                    else
                    {
                        if (newSoulGem.type != soulGem.ModItem<SoulGem>().TransformID)
                            newSoulGem.SetDefaults(soulGem.ModItem<SoulGem>().TransformID);
                        newSoulGem.Center = player.Top;
                        SoundEngine.PlaySound(SoundID.Grab, player.Top);
                        int p = PopupText.NewText(inVoidBag ? PopupTextContext.ItemPickupToVoidContainer : PopupTextContext.RegularItemPickup, newSoulGem, 1);
                        Main.popupText[p].lifeTime /= 4;
                        Main.popupText[p].lifeTime *= 3;
                    }
                    SoundEngine.PlaySound(SoundID.Item4.WithVolume(0.5f).WithPitchOffset(0.3f), player.Center);
                }
            }
        }
    }

    public class SoulGemFilled : ModItem
    {
        public override string Texture => Helper.GetPath<SoulGem>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Amber;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SoulGemTile>());
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(silver: 75);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, Color.White.UseA(0) * Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0.1f, 0.33f),
                0f, origin, scale, SpriteEffects.None, 0f);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.GetItemDrawData(out var frame);
            spriteBatch.Draw(TextureAssets.Item[Type].Value, ItemDefaults.WorldDrawPos(Item, TextureAssets.Item[Type].Value) + new Vector2(0f, -2f), frame, Color.White.UseA(0) * Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0.33f, 0.66f),
                rotation, TextureAssets.Item[Type].Value.Size() / 2f, scale, SpriteEffects.None, 0f);
        }

        public override void AddRecipes()
        {
            Recipe.Create(ItemID.LifeCrystal)
                .AddIngredient(Type)
                .AddIngredient<BloodyTearstone>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }

    public class EmptySoulGemTile : ModTile
    {
        public override string Texture => $"{this.NamespacePath()}/SoulGemTile";

        protected virtual Color MapColor => new Color(20, 105, 140);
        protected virtual int Item => ModContent.ItemType<SoulGem>();

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileNoFail[Type] = true;

            TileID.Sets.DisableSmartCursor[Type] = true;

            AddMapEntry(MapColor, Lang.GetItemName(Item));
            DustType = DustID.BlueCrystalShard;
            ItemDrop = Item;
        }

        public override bool CanPlace(int i, int j)
        {
            var top = Framing.GetTileSafely(i, j - 1);
            if (top.HasTile && !top.BottomSlope && top.TileType >= 0 && Main.tileSolid[top.TileType] && !Main.tileSolidTop[top.TileType])
            {
                return true;
            }
            var bottom = Framing.GetTileSafely(i, j + 1);
            if (bottom.HasTile && !bottom.IsHalfBlock && !bottom.TopSlope && bottom.TileType >= 0 && (Main.tileSolid[bottom.TileType] || Main.tileSolidTop[bottom.TileType]))
            {
                return true;
            }
            var left = Framing.GetTileSafely(i - 1, j);
            if (left.HasTile && left.TileType >= 0 && Main.tileSolid[left.TileType] && !Main.tileSolidTop[left.TileType])
            {
                return true;
            }
            var right = Framing.GetTileSafely(i + 1, j);
            if (right.HasTile && right.TileType >= 0 && Main.tileSolid[right.TileType] && !Main.tileSolidTop[right.TileType])
            {
                return true;
            }
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.01f;
            g = 0.1f;
            b = 0.2f;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            AequusTile.GemFrame(i, j);
            return false;
        }
    }

    public class SoulGemTile : EmptySoulGemTile
    {
        protected override int Item => ModContent.ItemType<SoulGemFilled>();
        protected override Color MapColor => new Color(40, 140, 180);

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.075f;
            g = 0.22f;
            b = 0.5f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                TextureAssets.Tile[Type].Value, 
                new Vector2(i * 16f, j * 16f) - Main.screenPosition + Helper.TileDrawOffset, 
                new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 16, 16), 
                Color.White.UseA(0) * Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0.1f, 0.33f),
                0f, 
                Vector2.Zero, 
                1f, SpriteEffects.None, 0f);
        }
    }
}