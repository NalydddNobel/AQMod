using AQMod.Common.Graphics.PlayerEquips;
using AQMod.Items.DrawOverlays;
using AQMod.Items.Materials.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor.Arachnotron
{
    [AutoloadEquip(EquipType.Body)]
    public class ABArachnotronRibcage : ModItem, IItemOverlaysWorldDraw
    {
        private static readonly GlowmaskOverlay _overlay = new GlowmaskOverlay(AQUtils.GetPath2<ABArachnotronRibcage>("_Glow"));
        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;

        public override bool Autoload(ref string name)
        {
            name = "ArachnotronRibcage";
            return base.Autoload(ref name);
        }

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                AQMod.ArmorOverlays.AddBodyOverlay<ABArachnotronRibcage>(new ArachnotronRibcageOverlay());
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.defense = 20;
            item.rare = ItemRarityID.LightPurple;
            item.value = Item.sellPrice(gold: 10);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<AQPlayer>().arachnotronArms = true;
            player.maxMinions += 1;
            player.meleeSpeed += 0.1f;
            player.endurance += 0.05f;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.SpiderBreastplate);
            r.AddIngredient(ItemID.BandofRegeneration);
            r.AddIngredient(ItemID.HallowedBar, 18);
            r.AddIngredient(ItemID.SoulofFright, 6);
            r.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}