using Aequus.Content.StatSheet;
using Aequus.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items
{
    internal class TesterItem : ModItem
    {
        public override string Texture => AequusHelpers.GetPath<Gamestar>();

        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.width = 20;
            Item.height = 20;
        }

        public override bool? UseItem(Player player)
        {
            int x = AequusHelpers.tileX;
            int y = AequusHelpers.tileY;

            var clr = Color.Red.HueAdd(Main.rand.NextFloat(1f));
            foreach (var s in StatSheetManager.RegisteredStats)
            {
                Main.NewText($"{Language.GetTextValue(s.DisplayName)}: {s.ProvideStatText()}", Color.Lerp(clr, Color.White, 0.75f));
                clr = clr.HueAdd(Main.rand.NextFloat(0.1f));
            }
            return true;
        }
    }
}