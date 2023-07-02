using Aequus.Common;
using Aequus.Common.Items.EquipmentBooster;
using Aequus.Items.Armor.SetGravetender;
using Aequus.Items.Armor.SetNecromancer;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Materials.SoulGem;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.SetNecromancer {
    [AutoloadEquip(EquipType.Head)]
    public class NecromancerHood : ModItem {

        public int EnemyDamage;
        public int[] EnemySpawn;

        public override void Load() {
            if (!Main.dedServ) {
                GlowMasksHandler.AddGlowmask(AequusTextures.NecromancerHood_Head_Glow.Path);
            }
        }

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.defense = 4;
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 1);
            EnemyDamage = 100;
            EnemySpawn = new int[]
            {
                NPCID.Skeleton,
                NPCID.ArmoredSkeleton,
                NPCID.SkeletonArcher,
            };
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) {
            return body.type == ModContent.ItemType<NecromancerRobe>();
        }

        public override void UpdateArmorSet(Player player) {
            player.setBonus = TextHelper.GetTextValue("ArmorSetBonus.Necromancer");
            var aequus = player.Aequus();
            var legModifiers = aequus.equipModifiers.Legs();
            aequus.armorNecromancerBattle = this;
            legModifiers.textColor = EquipBoostManager.BasicEmpowermentColor;
            legModifiers.Boost |= EquipBoostType.Defense | EquipBoostType.Abilities;
        }

        public override void UpdateEquip(Player player) {
            player.GetDamage<MagicDamageClass>() += 0.08f;
            player.GetDamage<SummonDamageClass>() += 0.08f;
            player.Aequus().ghostSlotsMax++;
        }

        public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) {
            glowMask = GlowMasksHandler.GetID(AequusTextures.NecromancerHood_Head_Glow.Path);
            glowMaskColor = Color.White with { A = 0 } * (1f - shadow);
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<GravetenderHood>()
                .AddIngredient<DemonicEnergy>(1)
                .AddIngredient<SoulGemFilled>(3)
                .AddTile(TileID.Loom)
                .TryRegisterBefore(ItemID.GravediggerShovel);
        }
    }
}

namespace Aequus {
    public partial class AequusPlayer {
        public NecromancerHood armorNecromancerBattle;

        public void CheckNecromancerSetbonus() {
            if (Main.myPlayer != Player.whoAmI || ghostSlots > 0 || armorNecromancerBattle == null) {
                return;
            }

            Projectile.NewProjectile(
                Player.GetSource_Accessory(armorNecromancerBattle.Item),
                Player.Center,
                Vector2.Zero,
                armorNecromancerBattle.Item.shoot,
                armorNecromancerBattle.EnemyDamage,
                0f,
                Player.whoAmI,
                Main.rand.Next(armorNecromancerBattle.EnemySpawn));
        }
    }
}