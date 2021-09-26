using AQMod.Assets.ItemOverlays;
using AQMod.Assets.Textures;
using AQMod.Walls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable.Walls
{
    public class MoonlightWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                ItemOverlayLoader.Register(new DynamicGlowmask(GlowID.MoonlightWall, getColor), item.type);
        }

        private static Color getColor() => MoonlightWallWall.GetColor(0f);

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.useTime = 7;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.createWall = ModContent.WallType<MoonlightWallWall>();
            item.consumable = true;
            item.autoReuse = true;
            item.useTurn = true;
        }

        public override void CaughtFishStack(ref int stack)
        {
            stack = Main.rand.Next(80, stack + 120);
        }
    }
}