using Aequus.Common.Items;
using Aequus.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Misc.SickBeat {
    public class SickBeat : ModItem, ItemHooks.IDrawSpecialItemDrop {
        public override void SetDefaults() {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 44;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = ItemDefaults.ValueEarlyHardmode;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 2f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<SickBeatProj>();
            Item.shootSpeed = 10.5f;
            Item.autoReuse = true;
        }

        public override bool CanUseItem(Player player) {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public void OnPreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            npc.Opacity = 0.5f;
            Main.instance.LoadItem(Type);
            var texture = TextureAssets.Item[Type].Value;
            Helper.GetItemDrawData(Type, out var frame);
            float scale = 1f;
            if (texture.Width > npc.frame.Width / 2) {
                scale = npc.frame.Width / 2 / (float)texture.Width;
            }
            spriteBatch.Draw(texture, npc.Center - screenPos, frame, drawColor, npc.rotation, frame.Size() / 2f, scale * npc.scale, SpriteEffects.None, 0f);
        }
    }
}