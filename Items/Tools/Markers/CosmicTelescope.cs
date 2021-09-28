using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.UserInterface;
using AQMod.Common.Utilities;
using AQMod.Content;
using AQMod.Content.WorldEvents;
using AQMod.Items.Energies;
using AQMod.Items.Misc.Markers;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.Markers
{
    public class CosmicTelescope : MapMarker
    {
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item4;
            item.consumable = true;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(gold: 2);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Glass);
            recipe.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            recipe.AddIngredient(ItemID.CobaltBar, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Glass);
            recipe.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            recipe.AddIngredient(ItemID.PalladiumBar, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public static string ApplyCosmicTelescope(Player player, AQPlayer aQPlayer, string mouseText)
        {
            if (GlimmerEvent.ActuallyActive && Main.Map[GlimmerEvent.X, GlimmerEvent.Y].Light > 40)
            {
                var texture = DrawUtils.Textures.Extras[ExtraID.GlimmerEventMapIcon];
                var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                var drawPos = MapInterface.MapPos(new Vector2(GlimmerEvent.X + 0.5f, GlimmerEvent.Y - 3f));
                var hitbox = Utils.CenteredRectangle(drawPos, new Vector2(texture.Width, texture.Height) * Main.UIScale);
                var scale = Main.UIScale;
                if (hitbox.Contains(Main.mouseX, Main.mouseY))
                {
                    mouseText = Language.GetTextValue(AQText.Key + "Common.GlimmerEvent");
                    scale += 0.5f;
                }
                Main.spriteBatch.Draw(texture, drawPos, frame, new Color(255, 255, 255, 255), 0f, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
                for (int i = 0; i < 2; i++)
                {
                    int b = i == 1 ? -1 : 1;
                    var pos = new Vector2(GlimmerEvent.X + 0.5f + GlimmerEvent.HyperStariteDistance * b, 160f);
                    if (pos.X < 0f || pos.X > Main.maxTilesX)
                        continue;
                    texture = DrawUtils.Textures.Extras[ExtraID.StariteMapIcon];
                    frame = new Rectangle(0, 0, texture.Width, texture.Height);
                    drawPos = MapInterface.MapPos(pos);
                    hitbox = Utils.CenteredRectangle(drawPos, new Vector2(texture.Width, texture.Height) * Main.UIScale);
                    scale = Main.UIScale;
                    if (hitbox.Contains(Main.mouseX, Main.mouseY))
                    {
                        mouseText = AQText.ModText("NPCName.HyperStarite").Value;
                        scale += 0.5f;
                    }
                    Main.spriteBatch.Draw(texture, drawPos, frame, new Color(255, 255, 255, 255), 0f, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
                    pos = new Vector2(GlimmerEvent.X + 0.5f + GlimmerEvent.SuperStariteDistance * b, 160f);
                    if (pos.X < 0f || pos.X > Main.maxTilesX)
                        continue;
                    drawPos = MapInterface.MapPos(pos);
                    hitbox = Utils.CenteredRectangle(drawPos, new Vector2(texture.Width, texture.Height) * Main.UIScale);
                    scale = Main.UIScale;
                    if (hitbox.Contains(Main.mouseX, Main.mouseY))
                    {
                        mouseText = AQText.ModText("NPCName.SuperStarite").Value;
                        scale += 0.5f;
                    }
                    Main.spriteBatch.Draw(texture, drawPos, frame, new Color(255, 255, 255, 255), 0f, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
                }
            }
            return mouseText;
        }

        public override int GetID() => MapMarkerPlayer.ID.CosmicMarker;

        public override string Apply(Player player, AQPlayer aQPlayer, string mouseText, MapMarkerPlayer mapMarkerPlayer)
        {
            return ApplyCosmicTelescope(player, aQPlayer, mouseText);
        }
    }
}