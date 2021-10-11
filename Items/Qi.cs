using AQMod.Common.ItemOverlays;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items
{
    public class Qi : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new QiOverlayData(), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.rare = ItemRarityID.Red;
            item.value = Item.sellPrice(silver: 20);
            item.maxStack = 999;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return false;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(item.Center, new Vector3(0.8f, 0.8f, 0.8f));
        }
    }
}