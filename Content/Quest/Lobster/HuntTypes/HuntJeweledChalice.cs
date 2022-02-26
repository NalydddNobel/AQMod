using AQMod.NPCs.Friendly;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Content.Quest.Lobster.HuntTypes
{
    public class HuntJeweledChalice : RobsterHunt
    {
        public HuntJeweledChalice(Mod mod, string name) : base(mod, name)
        {
        }

        public override int GetQuestItem() => ModContent.ItemType<AQMod.Items.Misc.ExporterQuest.JeweledChalice>();

        public override void Setup()
        {
        }

        public override bool CanStart(Player player)
        {
            for (int i = 0; i < 1000; i++)
            {
                byte npc = (byte)Main.rand.Next(Main.maxNPCs);
                if (Main.npc[npc].active && Main.npc[npc].townNPC && !Main.npc[npc].homeless && Main.npc[npc].type != ModContent.NPCType<Robster>())
                {
                    int x = Main.npc[npc].homeTileX;
                    int y = Main.npc[npc].homeTileY;
                    var checkRectangle = new Rectangle(x - 8, y - 8, 16, 16);
                    for (int k = checkRectangle.X; k < checkRectangle.X + checkRectangle.Width; k++)
                    {
                        for (int l = checkRectangle.Y; l < checkRectangle.Y + checkRectangle.Height; l++)
                        {
                            int randomX = checkRectangle.X + Main.rand.Next(checkRectangle.Width);
                            int randomY = checkRectangle.Y + Main.rand.Next(checkRectangle.Height);
                            if (!Framing.GetTileSafely(randomX, randomY).active() && Framing.GetTileSafely(randomX, randomY + 1).active() && Main.tileSolidTop[Main.tile[randomX, randomY + 1].type])
                            {
                                WorldGen.PlaceTile(randomX, randomY, ModContent.TileType<AQMod.Tiles.ExporterQuest.JeweledChaliceTile>(), true, false, -1, 0);
                                if (Framing.GetTileSafely(randomX, randomY).type == ModContent.TileType<AQMod.Tiles.ExporterQuest.JeweledChaliceTile>())
                                {
                                    HuntSystem.SetNPCTarget(npc, npcID: false);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public override void OnStart(Player player)
        {
            HuntSystem.SetNPCTarget(NPCID.Guide);
        }

        public override string QuestChat()
        {
            return "Mods.AQMod.Common.HuntJeweledChalice";
        }

        public override void RemoveHunt()
        {
            RemoveQuestTiles();
        }
    }
}