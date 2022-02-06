using AQMod.NPCs.Friendly;
using AQMod.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools
{
    public sealed class BlackBookofUntoldLegends : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 28;
            item.rare = ItemRarityID.Orange;
            item.value = Item.buyPrice(gold: 20);
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item8;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<HermitCrab>());
        }

        public override bool UseItem(Player player)
        {
            var rect = new Rectangle((int)(player.position.X + player.width / 2f) / 16 - 10, (int)(player.position.Y + player.head / 2f) / 16 - 10, 20, 20).KeepInWorld(fluff: 10);
            for (int i = 0; i < 100; i++)
            {
                int x = rect.X + Main.rand.Next(rect.Width);
                int y = rect.Y + Main.rand.Next(rect.Height);
                if (Main.tile[x, y] == null)
                {
                    Main.tile[x, y] = new Tile();
                }
                if (Main.tile[x, y + 1] == null)
                {
                    Main.tile[x, y + 1] = new Tile();
                }
                if ((!Main.tile[x, y].active() || !Main.tile[x, y].Solid())
                        && Main.tile[x, y + 1].active() && (Main.tile[x, y + 1].Solid() || Main.tile[x, y].SolidTop()))
                {
                    int n = NPC.NewNPC(x * 16, y * 16, ModContent.NPCType<HermitCrab>());
                    Main.npc[n].position.X = x * 16f - Main.npc[n].width / 2f + 8f;
                    Main.npc[n].position.Y = y * 16f - Main.npc[n].height + 16f;
                    Main.npc[n].alpha = 255;
                    return true;
                }
            }
            return false;
        }
    }
}