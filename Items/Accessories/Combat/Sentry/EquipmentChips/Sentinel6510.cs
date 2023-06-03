using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Combat.Sentry.EquipmentChips {
    [LegacyName("MechsSentry")]
    public class Sentinel6510 : Sentry6502 {
        public override void SetDefaults() {
            base.SetDefaults();
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(gold: 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            var aequus = player.Aequus();
            aequus.accSentrySlot = true;
            aequus.accSentryInheritence = Item;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Sentry6502>())
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddTile(TileID.MythrilAnvil)
                .TryRegisterAfter(ItemID.PapyrusScarab);
        }
    }

    public class Sentinel6510AccessorySlot : ModAccessorySlot {
        private static Color _inventoryBackHack;

        public override bool DrawVanitySlot => false;

        public override bool IsEnabled() {
            if (Main.gameMenu) {
                return false;
            }

            try {
                return Player.Aequus().accSentrySlot || (FunctionalItem != null && !FunctionalItem.IsAir && FunctionalItem.type == ModContent.ItemType<Sentinel6510>());
            }
            catch {
            }

            return false;
        }

        public override bool PreDraw(AccessorySlotType context, Item item, Vector2 position, bool isHovered) {
            int slot = (int)context - 10;
            if (slot < 0 || slot > 2) {
                return true;
            }

            var texture = AequusTextures.InventoryBack_EquipmentChips;
            var frame = texture.Frame(verticalFrames: 3, frameY: slot);
            Main.spriteBatch.Draw(texture, position, frame, Main.inventoryBack, 0f, Vector2.Zero, Main.inventoryScale, SpriteEffects.None, 0f);

            _inventoryBackHack = Main.inventoryBack;
            Main.inventoryBack = Color.Transparent;
            return true;
        }

        public override void PostDraw(AccessorySlotType context, Item item, Vector2 position, bool isHovered) {
            Main.inventoryBack = _inventoryBackHack;
        }

        public override void ApplyEquipEffects() {
            int proj = Player.Aequus().projectileIdentity;
            if (proj != -1) {
                int projWhoAmI = Helper.FindProjectileIdentity(Player.whoAmI, proj);
                if (projWhoAmI != -1 && Main.projectile[projWhoAmI].sentry) {
                    base.ApplyEquipEffects();
                }
            }
        }
    }
}