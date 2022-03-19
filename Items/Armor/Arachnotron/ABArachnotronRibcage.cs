using AQMod.Common.Graphics.PlayerEquips;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor.Arachnotron
{
    [AutoloadEquip(EquipType.Body)]
    public class ABArachnotronRibcage : ModItem
    {
        public override bool Autoload(ref string name)
        {
            name = "ArachnotronRibcage";
            return base.Autoload(ref name);
        }

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                if (GlowmaskData.ItemToGlowmask == null)
                {
                    GlowmaskData.ItemToGlowmask = new Dictionary<int, GlowmaskData>();
                }
                var glow = new AQUtils.ItemGlowmask(() => new Color(250, 250, 250, 0));
                GlowmaskData.ItemToGlowmask.Add(item.type, new GlowmaskData(mod.GetTexture("Items/Armor/Arachnotron/ArachnotronRibcage_Glow"), glow));
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
            player.meleeSpeed += 0.1f;
            player.minionDamage += 0.1f;
            player.endurance += 0.05f;
            player.maxMinions += 1;
            player.GetModPlayer<AQPlayer>().arachnotronArms = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.SpiderBreastplate);
            r.AddIngredient(ItemID.HallowedBar, 18);
            r.AddIngredient(ItemID.SoulofFright, 6);
            r.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}