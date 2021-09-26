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
    public class LihzahrdMap : MapMarker
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

        public static string ApplyLihzahrdMap(Player player, AQPlayer aQPlayer, string mouseText)
        {
            if (AQWorldGen.CommonStructureSearchMethods.LihzahrdAltar(out var position))
            {
                if (Main.Map[position.X, position.Y].Light > 40 || NPC.downedPlantBoss)
                {
                    var mapIcon = SpriteUtils.Textures.Extras[ExtraID.MapIcons];
                    var frame = new Rectangle(MapInterface.MapIconWidth * 4, 0, MapInterface.TrueMapIconWidth, MapInterface.MapIconHeight);
                    var drawPos = MapInterface.MapPos(new Vector2(position.X + 1.5f, position.Y - 0.5f));
                    var hitbox = Utils.CenteredRectangle(drawPos, new Vector2(frame.Width, frame.Height) * Main.UIScale);
                    var scale = Main.UIScale;
                    bool hovering = hitbox.Contains(Main.mouseX, Main.mouseY);
                    if (hovering)
                        scale += 0.1f;
                    Main.spriteBatch.Draw(mapIcon, drawPos, frame, new Color(255, 255, 255, 255), 0f, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
                    MapInterface.UnityTeleport((position.X + 1f) * 16f, (position.Y + 2f) * 16f - player.height, drawPos + new Vector2(frame.Width / 2f + 2f, frame.Height / 2f) * scale, player, NPC.downedPlantBoss && hovering);
                }
            }
            return mouseText;
        }

        public override int GetID() => MapMarkerPlayer.ID.LihzahrdMarker;

        public override string Apply(Player player, AQPlayer aQPlayer, string mouseText, MapMarkerPlayer mapMarkerPlayer)
        {
            return ApplyLihzahrdMap(player, aQPlayer, mouseText);
        }
    }
}