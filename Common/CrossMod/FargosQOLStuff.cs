using AQMod.Items.BossItems.Crabson;
using AQMod.Items.BossItems.Starite;
using AQMod.Items.Placeable.Banners;
using AQMod.Items.Vanities;
using AQMod.Items.Vanities.CursorDyes;
using AQMod.Items.Vanities.Dyes;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common.CrossMod
{
    internal sealed class FargosQOLStuff
    {
        private readonly AQMod _aQMod1;
        private readonly Mod _fargowiltas;
        private FargosQOLStuff(Mod fargos, AQMod aQMod)
        {
            _fargowiltas = fargos;
            _aQMod1 = aQMod;
        }

        public static bool FargowiltasActive { get; private set; }

        public static void Setup(AQMod aQMod)
        {
            FargosQOLStuff f = null;
            try
            {
                var fargos = ModLoader.GetMod("Fargowiltas");
                if (fargos == null)
                    return;
                f = new FargosQOLStuff(fargos, aQMod);
                FargowiltasActive = true;
            }
            catch (Exception)
            {
            }
            if (FargowiltasActive)
            {
                f.createBossSummons();
                f.createRecipes();
            }
        }

        private void createBossSummons()
        {
            try
            {
                addSummon(1.5f, "MushroomClam", () => WorldDefeats.DownedCrabson, Item.buyPrice(gold: 8));
            }
            catch (Exception e)
            {
                _aQMod1.Logger.Warn("Error occured when setting up fargo boss summons");
                _aQMod1.Logger.Warn(e.Message);
                _aQMod1.Logger.Warn(e.StackTrace);
            }
        }

        private void createRecipes()
        {
            createFargoRecipe<HermitCrabBanner, Items.Armor.HermitShell>();
            createFargoRecipe<HermitCrabBanner, FishyFins>();
            createFargoRecipe<StriderCrabBanner, Items.Armor.StriderCarapace>();
            createFargoRecipe<StriderCrabBanner, Items.Armor.StriderPalms>();
            createFargoRecipe<StriderCrabBanner, Items.Tools.GrapplingHooks.StriderHook>();
            createFargoRecipe<StriderCrabBanner, FishyFins>();

            createFargoRecipe<StariteBanner, CelesitalEightBall>();
            createFargoRecipe<StariteBanner, HypnoDye>();
            createFargoRecipe<SuperStariteBanner>(ItemID.Nazar);
            createFargoRecipe<SuperStariteBanner, OutlineDye>();
            createFargoRecipe<HyperStariteBanner>(ItemID.Nazar);
            createFargoRecipe<HyperStariteBanner, ScrollDye>();

            createFargoRecipe<CinderaBanner, Items.Accessories.DegenerationRing>();
            createFargoRecipe<CinderaBanner>(ItemID.MagmaStone);
            createFargoRecipe<CinderaBanner, HellBeamDye>();
            createFargoRecipe<MagmabubbleBanner, Items.Accessories.DegenerationRing>();
            createFargoRecipe<MagmabubbleBanner>(ItemID.LavaCharm);
            createFargoRecipe<TrapperImpBanner>(ItemID.ObsidianRose);
            createFargoRecipe<TrapperImpBanner, Items.Weapons.Melee.PowPunch>();
            createFargoRecipe<TrapperImpBanner, DemonicCursorDye>();

            createFargoRecipeBannerInput<Items.Weapons.Melee.CrystalDagger>(ItemID.UndeadVikingBanner);

            createFargoRecipe(ItemID.MimicBanner, ItemID.AdhesiveBandage);
            createFargoRecipe(ItemID.MimicBanner, ItemID.SharkToothNecklace);
            createFargoRecipe(ItemID.MimicBanner, ItemID.MoneyTrough);
            var r = new ModRecipe(_aQMod1);
            r.AddIngredient(ItemID.MimicBanner);
            r.AddIngredient(ItemID.Bone, 5);
            r.AddTile(TileID.Solidifier);
            r.SetResult(ModContent.ItemType<Items.Tools.ATM>());
            r.AddRecipe();

            createFargoRecipeBannerInput<Items.Accessories.Breadsoul>(ItemID.DungeonSpiritBanner);
            createFargoRecipeBannerInput<Items.Dedicated.ContentCreators.Dreadsoul>(ItemID.DungeonSpiritBanner);

            createFargoRecipe<CrabsonTrophy, Items.Weapons.Melee.JerryClawFlail>();
            createFargoRecipe<CrabsonTrophy, Items.Weapons.Ranged.CinnabarBow>();
            createFargoRecipe<CrabsonTrophy, Items.Weapons.Magic.Bubbler>();

            createFargoRecipe<OmegaStariteTrophy, Items.Weapons.Ranged.Raygun>();
            createFargoRecipe<OmegaStariteTrophy, Items.Weapons.Magic.MagicWand>();
            int[] itemArray = new int[] { ModContent.ItemType<EnchantedDye>(), ModContent.ItemType<RainbowOutlineDye>(), ModContent.ItemType<DiscoDye>(), };
            int item = ModContent.ItemType<OmegaStariteTrophy>();
            for (int i = 0; i < itemArray.Length; i++)
            {
                for (int j = 0; j < itemArray.Length; j++)
                {
                    if (j != i)
                    {
                        r = new ModRecipe(_aQMod1);
                        r.AddIngredient(itemArray[i]);
                        r.AddIngredient(item);
                        r.AddTile(TileID.Solidifier);
                        r.SetResult(itemArray[j]);
                        r.AddRecipe();
                    }
                }
            }
        }

        private void createFargoRecipe<TModItem, TModItem2>() where TModItem : ModItem where TModItem2 : ModItem
        {
            createFargoRecipe<TModItem>(ModContent.ItemType<TModItem2>());
        }

        private void createFargoRecipeBannerInput<TModItem>(int banner) where TModItem : ModItem
        {
            createFargoRecipe(banner, ModContent.ItemType<TModItem>());
        }

        private void createFargoRecipe<TModItem>(int result) where TModItem : ModItem
        {
            createFargoRecipe(ModContent.ItemType<TModItem>(), result);
        }

        private void createFargoRecipe(int banner, int result)
        {
            var r = new ModRecipe(_aQMod1);
            r.AddIngredient(banner);
            r.AddTile(TileID.Solidifier);
            r.SetResult(result);
            r.AddRecipe();
        }

        private void addSummon(float sort, string itemName, Func<bool> checkFlag, int price)
        {
            _fargowiltas.Call("AddSummon", sort, "AQMod", itemName, checkFlag, price);
        }
    }
}