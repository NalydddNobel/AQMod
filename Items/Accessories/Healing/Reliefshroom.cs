using Aequus.Buffs;
using Aequus.Items.Misc.Energies;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Healing
{
    public sealed class Reliefshroom : ModItem, ItemHooks.IUpdateItemDye
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.healingMushroomItem = Item;
            aequus.healingMushroomRegeneration += 12;

            if (aequus.idleTime <= 5 && player.velocity.X.Abs() > 2f)
            {
                Lighting.AddLight(player.Center, Color.Violet.ToVector3() * 0.5f);
                player.AddBuff(ModContent.BuffType<ReliefshroomBuff>(), 2);
                if (Main.rand.NextBool(12))
                {
                    var v = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                    var d = Dust.NewDustPerfect(aequus.Player.Center + v * Main.rand.NextFloat(player.width * 0.8f, player.width * 2f), ModContent.DustType<ReliefshroomDustSpore>(), -v * Main.rand.NextFloat(0.1f, 1f), 255, Scale: Main.rand.NextFloat(0.6f, 0.7f));
                    if (aequus.cHealingMushroom != 0)
                    {
                        d.shader = GameShaders.Armor.GetSecondaryShader(aequus.cHealingMushroom, player);
                    }
                }
                if (Main.GameUpdateCount % 60 == 0)
                {
                    foreach (var v in AequusHelpers.CircularVector(10, Main.rand.NextFloat(MathHelper.TwoPi)))
                    {
                        var d = Dust.NewDustPerfect(aequus.Player.Center + v * Main.rand.NextFloat(player.width * 2.4f, player.width * 2.6f), ModContent.DustType<ReliefshroomDustSpore>(), -v * Main.rand.NextFloat(0.9f, 1.1f), 255);
                        d.customData = player;
                        if (aequus.cHealingMushroom != 0)
                        {
                            d.shader = GameShaders.Armor.GetSecondaryShader(aequus.cHealingMushroom, player);
                        }
                    }
                }
            }
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return CheckMendshroom(equippedItem) && CheckMendshroom(incomingItem);
        }
        public bool CheckMendshroom(Item item)
        {
            return item.type != ModContent.ItemType<Mendshroom>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Mendshroom>()
                .AddIngredient<DemonicEnergy>()
                .AddTile(TileID.DemonAltar)
                .Register((r) => r.SortAfterFirstRecipesOf(ItemID.CharmofMyths));
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            player.Aequus().cHealingMushroom = dyeItem.dye;
        }
    }
}