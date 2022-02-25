using AQMod.Common.Graphics.PlayerEquips;
using AQMod.Items.DrawOverlays;
using AQMod.Items.Materials.Energies;
using AQMod.Localization;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Items.Armor.Arachnotron
{
    [AutoloadEquip(EquipType.Head)]
    public class AArachnotronVisor : ModItem, IItemOverlaysWorldDraw
    {
        private static readonly GlowmaskOverlay _overlay = new GlowmaskOverlay(AQUtils.GetPath2<AArachnotronVisor>("_Glow"));
        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;

        public override bool Autoload(ref string name)
        {
            name = "ArachnotronVisor";
            return base.Autoload(ref name);
        }

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                AQMod.ArmorOverlays.AddHeadOverlay<AArachnotronVisor>(new ArachnotronVisorOverlay());
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.defense = 10;
            item.rare = ItemRarityID.LightPurple;
            item.value = Item.sellPrice(gold: 7, silver: 50);
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ABArachnotronRibcage>() && legs.type == ModContent.ItemType<ArachnotronRevvers>();
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeCrit += 10;
            player.meleeDamage += 0.1f;
            player.minionDamage += 0.1f;
            player.nightVision = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.AQMod.ArmorSetBonus.Arachnotron", AQText.KeybindText(Keybinds.ArmorSetBonus));
            player.GetModPlayer<AQPlayer>().setArachnotron = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.SpiderMask);
            r.AddIngredient(ItemID.FlareGun);
            r.AddIngredient(ItemID.HallowedBar, 12);
            r.AddIngredient(ItemID.SoulofFright, 4);
            r.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}