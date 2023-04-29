using Aequus.Items;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.Gravity {
    public class GravityChest : ModItem {
        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<GravityChestTile>());
            Item.value = Item.sellPrice(silver: 10);
        }
    }

    public class GravityChestTile : BaseChest<GravityChest> {
        public static readonly HashSet<int> BlackistPickup = new();

        public override Color MapColor => Color.Cyan;

        public override void Load() {
            BlackistPickup.Add(ItemID.Heart);
            BlackistPickup.Add(ItemID.Star);
            BlackistPickup.Add(ItemID.CandyCane);
            BlackistPickup.Add(ItemID.CandyApple);
            BlackistPickup.Add(ItemID.SoulCake);
            BlackistPickup.Add(ItemID.SugarPlum);
            BlackistPickup.Add(ItemID.ManaCloakStar);
        }

        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            DustType = DustID.Electric;
        }

        public override void Unload() {
            BlackistPickup.Clear();
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
            DrawBasicGlowmask(i, j, spriteBatch, AequusTextures.GravityChestTile_Glow, Color.White);
        }

        public virtual bool PickupItemLogic(int i, int j, int chestID, Item item, AequusItem aequus) {
            return !item.IsACoin && !BlackistPickup.Contains(item.type);
        }

        public static void ItemPickupEffect(Vector2 itemPos, Vector2 chestPos) {
            var c = chestPos - itemPos;
            var n = Vector2.Normalize(c);
            var l = c.Length().UnNaN();
            float distance = 0f;
            itemPos -= new Vector2(2f);
            SoundEngine.PlaySound(SoundID.Item114.WithPitchOffset(0.6f), itemPos);
            SoundEngine.PlaySound(SoundID.Grab.WithPitchOffset(0.8f).WithVolumeScale(0.33f), itemPos);
            while (distance < l) {
                var d = Dust.NewDustDirect(itemPos + n * distance, 4, 4, ModContent.DustType<MonoDust>(), newColor: Color.Teal.UseA(0) * 2f);
                d.velocity *= 0.3f;
                if (Main.rand.NextBool(3)) {
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