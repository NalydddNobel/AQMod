using Aequus.Items;
using Aequus.Items.Placeable.Furniture.Gravity;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.Gravity
{
    public class GravityChestTile : BaseChest
    {
        public static HashSet<int> BlackistPickup { get; private set; }

        public override void Load()
        {
            BlackistPickup = new HashSet<int>()
            {
                ItemID.Heart,
                ItemID.Star,
                ItemID.CandyCane,
                ItemID.CandyApple,
                ItemID.SoulCake,
                ItemID.SugarPlum,
                ItemID.ManaCloakStar,
            };
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DustType = DustID.Electric;
            ChestDrop = ModContent.ItemType<GravityChest>();
            AddMapEntry(Helper.ColorFurniture, CreateMapEntryName());
        }

        public override void Unload()
        {
            BlackistPickup?.Clear();
            BlackistPickup = null;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            try
            {
                var tile = Main.tile[i, j];
                int left = i;
                int top = j;
                if (tile.TileFrameX % 36 != 0)
                {
                    left--;
                }
                if (tile.TileFrameY != 0)
                {
                    top--;
                }
                int chest = Chest.FindChest(left, top);
                Main.spriteBatch.Draw(ModContent.Request<Texture2D>($"{Texture}_Glow", AssetRequestMode.ImmediateLoad).Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + Helper.TileDrawOffset,
                    new Rectangle(tile.TileFrameX, 38 * (chest == -1 ? 0 : Main.chest[chest].frame) + tile.TileFrameY, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            catch
            {
            }
        }

        public virtual bool PickupItemLogic(int i, int j, int chestID, Item item, AequusItem aequus)
        {
            return !item.IsACoin && !BlackistPickup.Contains(item.type);
        }

        public static void ItemPickupEffect(Vector2 itemPos, Vector2 chestPos)
        {
            var c = chestPos - itemPos;
            var n = Vector2.Normalize(c);
            var l = c.Length().UnNaN();
            float distance = 0f;
            itemPos -= new Vector2(2f);
            SoundEngine.PlaySound(SoundID.Item114.WithPitchOffset(0.6f), itemPos);
            SoundEngine.PlaySound(SoundID.Grab.WithPitchOffset(0.8f).WithVolumeScale(0.33f), itemPos);
            while (distance < l)
            {
                var d = Dust.NewDustDirect(itemPos + n * distance, 4, 4, ModContent.DustType<MonoDust>(), newColor: Color.Teal.UseA(0) * 2f);
                d.velocity *= 0.3f;
                if (Main.rand.NextBool(3))
                {
                    d = Dust.NewDustDirect(itemPos + n * distance, 4, 4, DustID.Electric, Scale: 0.7f);
                    d.velocity *= 0.3f;
                    d.velocity += n * 2.5f;
                    d.position -= n * 8f;
                    d.noGravity = true;
                    d.fadeIn = d.scale + 0.1f;
                }
                distance += 5f;
            }
        }
    }
}