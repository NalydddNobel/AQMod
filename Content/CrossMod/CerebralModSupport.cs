using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Summon.Necro;
using Aequus.Items.Accessories.Vanity.Cursors;
using Aequus.Items.Tools;
using Aequus.Items.Weapons.Melee;
using Aequus.Items.Weapons.Ranged;
using Aequus.Items.Weapons.Summon.Necro.Candles;
using Aequus.Items.Weapons.Summon.Necro.Scepters;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    public class CerebralModSupport : ModSystem
    {
        public static Mod CerebralMod { get; private set; }

        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModLoader.HasMod("CerebralMod");
        }

        public override void AddRecipes()
        {
            if (!ModLoader.TryGetMod("CerebralMod", out var cerebralMod))
            {
                return;   
            }
            CerebralMod = cerebralMod;

            if (CerebralMod.TryFind<ModItem>("GoldenChestCrafter", out var goldenChestCrafter))
            {
                Recipe.Create(ModContent.ItemType<BattleAxe>())
                    .AddIngredient(goldenChestCrafter.Type)
                    .AddTile(TileID.Anvils)
                    .Register();
                Recipe.Create(ModContent.ItemType<Bellows>())
                    .AddIngredient(goldenChestCrafter.Type)
                    .AddTile(TileID.Anvils)
                    .Register();
                Recipe.Create(ModContent.ItemType<BoneRing>())
                    .AddIngredient(goldenChestCrafter.Type)
                    .AddTile(TileID.Anvils)
                    .Register();
                Recipe.Create(ModContent.ItemType<GlowCore>())
                    .AddIngredient(goldenChestCrafter.Type)
                    .AddTile(TileID.Anvils)
                    .Register();
                Recipe.Create(ModContent.ItemType<SwordCursor>())
                    .AddIngredient(goldenChestCrafter.Type)
                    .AddTile(TileID.Anvils)
                    .Register();
            }
            if (CerebralMod.TryFind<ModItem>("FrozenChestCrafter", out var frozenChestCrafter))
            {
                Recipe.Create(ModContent.ItemType<CrystalDagger>())
                    .AddIngredient(frozenChestCrafter.Type)
                    .AddTile(TileID.Anvils)
                    .Register();
            }
            if (CerebralMod.TryFind<ModItem>("SkywareChestCrafter", out var skywareChestCrafter))
            {
                Recipe.Create(ModContent.ItemType<Slingshot>())
                    .AddIngredient(skywareChestCrafter.Type)
                    .AddTile(TileID.Anvils)
                    .Register();
            }
            if (CerebralMod.TryFind<ModItem>("ShadowOrbCrafter", out var shadowOrbCrafter))
            {
                Recipe.Create(ModContent.ItemType<CorruptionCandle>())
                    .AddIngredient(shadowOrbCrafter.Type)
                    .AddTile(TileID.Anvils)
                    .Register();
            }
            if (CerebralMod.TryFind<ModItem>("CrimsonHeartCrafter", out var crimsonHeartCrafter))
            {
                Recipe.Create(ModContent.ItemType<CrimsonCandle>())
                    .AddIngredient(crimsonHeartCrafter.Type)
                    .AddTile(TileID.Anvils)
                    .Register();
            }
            if (CerebralMod.TryFind<ModItem>("DungeonChestCrafter", out var dungeonOrbCrafter))
            {
                Recipe.Create(ModContent.ItemType<Valari>())
                    .AddIngredient(dungeonOrbCrafter.Type)
                    .AddTile(TileID.Anvils)
                    .Register();
                Recipe.Create(ModContent.ItemType<Revenant>())
                    .AddIngredient(dungeonOrbCrafter.Type)
                    .AddTile(TileID.Anvils)
                    .Register();
                Recipe.Create(ModContent.ItemType<DungeonCandle>())
                    .AddIngredient(dungeonOrbCrafter.Type)
                    .AddTile(TileID.Anvils)
                    .Register();
                Recipe.Create(ModContent.ItemType<PandorasBox>())
                    .AddIngredient(dungeonOrbCrafter.Type)
                    .AddTile(TileID.Anvils)
                    .Register();
            }
        }

        public override void Unload()
        {
            CerebralMod = null;
        }
    }
}