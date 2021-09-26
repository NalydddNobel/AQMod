using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.UserInterface;
using AQMod.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace AQMod.Items.Misc.Markers
{
    public class DungeonMap : MapMarker
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item4;
            item.consumable = true;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(gold: 2);
        }

        public static string ApplyDungeonMap(Player player, AQPlayer aQPlayer, string mouseText)
        {
            if (Main.Map[Main.dungeonX, Main.dungeonY].Light <= 40 && !NPC.downedBoss3 && !Main.hardMode)
                return mouseText;
            var mapIcon = SpriteUtils.Textures.Extras[ExtraID.MapIcons];
            int iconFrame;
            switch (Framing.GetTileSafely(Main.dungeonX, Main.dungeonY).type)
            {
                default:
                iconFrame = 3;
                break;

                case TileID.BlueDungeonBrick:
                iconFrame = 0;
                break;

                case TileID.PinkDungeonBrick:
                iconFrame = 1;
                break;

                case TileID.GreenDungeonBrick:
                iconFrame = 2;
                break;
            }
            var frame = new Rectangle(MapInterface.MapIconWidth * iconFrame, 0, MapInterface.TrueMapIconWidth, MapInterface.MapIconHeight);
            var drawPos = MapInterface.MapPos(new Vector2(Main.dungeonX + 0.5f, Main.dungeonY - 2.5f));
            var hitbox = Utils.CenteredRectangle(drawPos, new Vector2(frame.Width, frame.Height) * Main.UIScale);
            var scale = Main.UIScale;
            bool hovering = hitbox.Contains(Main.mouseX, Main.mouseY);
            if (hovering)
            {
                mouseText = "Dungeon";
                scale += 0.1f;
            }
            Main.spriteBatch.Draw(mapIcon, drawPos, frame, new Color(255, 255, 255, 255), 0f, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
            MapInterface.UnityTeleport((Main.dungeonX + 0.5f) * 16f, Main.dungeonY * 16f - player.height, drawPos + new Vector2(frame.Width / 2f + 2f, frame.Height / 2f) * scale, player, (NPC.downedBoss3 || Main.hardMode) && hovering);
            return mouseText;
        }

        public override int GetID() => MapMarkerPlayer.ID.DungeonMarker;

        public override string Apply(Player player, AQPlayer aQPlayer, string mouseText, MapMarkerPlayer mapMarkerPlayer)
        {
            return ApplyDungeonMap(player, aQPlayer, mouseText);
        }
    }
}