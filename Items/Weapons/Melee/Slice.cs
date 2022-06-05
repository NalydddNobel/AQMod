using Aequus.Items.Recipes;
using Aequus.Projectiles.Melee.Swords;
using Aequus.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Items.Weapons.Melee
{
    public class Slice : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToDopeSword<SliceProj>(10);
            Item.SetWeaponValues(60, 2.5f);
            Item.width = 20;
            Item.height = 20;
            Item.autoReuse = true;
            Item.rare = ItemDefaults.RarityGaleStreams;
            Item.value = ItemDefaults.GaleStreamsValue;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor.MaxRGBA(120);
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            return AequusHelpers.RollSwordPrefix(Item, rand);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void AddRecipes()
        {
            AequusRecipes.SpaceSquidRecipe(this, ModContent.ItemType<CrystalDagger>());
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundHelpers.SwordSwoosh.WithPitch(
                SliceProj.FasterSwings(player.GetModPlayer<AequusPlayer>().itemUsage) ? 1f : 0.8f), position);
            return true;
        }
    }
}