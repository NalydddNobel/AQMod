using Aequus.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials
{
    public class SoulGem : ModItem
    {
        public virtual int TransformID => ModContent.ItemType<SoulGemFilled>();

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Amber;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(silver: 75);
            Item.maxStack = 9999;
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
        public override string Texture => AequusHelpers.GetPath<SoulGem>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Amber;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(silver: 75);
            Item.maxStack = 9999;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, Color.White.UseA(0) * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0.1f, 0.33f),
                0f, origin, scale, SpriteEffects.None, 0f);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.GetItemDrawData(out var frame);
            spriteBatch.Draw(TextureAssets.Item[Type].Value, ItemDefaults.WorldDrawPos(Item, TextureAssets.Item[Type].Value), frame, Color.White.UseA(0) * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0.33f, 0.66f),
                0f, TextureAssets.Item[Type].Value.Size() / 2f, scale, SpriteEffects.None, 0f);
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
}