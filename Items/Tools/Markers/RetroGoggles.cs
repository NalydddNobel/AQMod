using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.Config;
using AQMod.Common.UserInterface;
using AQMod.Common.Utilities;
using AQMod.Content;
using AQMod.Items.Misc.Markers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace AQMod.Items.Tools.Markers
{
    public class RetroGoggles : MapMarker
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

        public static string ApplyRetroGoggles(Player player, AQPlayer aQPlayer, string mouseText)
        {
            var texture = DrawUtils.Textures.Extras[ExtraID.OldGenPinkEnemyBlip];
            int frameHeight = texture.Height / 2;
            int frameNumber = (int)(Main.GameUpdateCount % 24 / 12);
            var frame = new Rectangle(0, frameHeight * frameNumber, texture.Width, frameHeight);
            Vector2 origin = frameNumber == 1 ? new Vector2(3f, 5f) : new Vector2(4f, 4f);
            var scale = Main.UIScale + 4f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && !AQNPC.Sets.NoMapBlip[Main.npc[i].type] && Main.npc[i].GetBossHeadTextureIndex() == -1 && !Main.npc[i].townNPC)
                {
                    var drawPos = MapInterface.MapPos(Main.npc[i].Center / 16f);
                    var color = AQConfigClient.Instance.MapBlipColor;
                    byte r = color.R;
                    if (r < 10)
                        r = 10;
                    byte g = color.G;
                    if (g < 10)
                        g = 10;
                    byte b = color.B;
                    if (b < 10)
                        b = 10;
                    Main.spriteBatch.Draw(texture, drawPos, frame, new Color(r, g, b, 150), 0f, origin, scale, SpriteEffects.None, 0f);
                }
            }
            return mouseText;
        }

        public override int GetID() => MapMarkerPlayer.ID.RetroMarker;

        public override string Apply(Player player, AQPlayer aQPlayer, string mouseText, MapMarkerPlayer mapMarkerPlayer)
        {
            return ApplyRetroGoggles(player, aQPlayer, mouseText);
        }
    }
}