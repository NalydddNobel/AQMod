﻿using Aequus.Common;
using Aequus.Items.Equipment.Armor.SetGravetender;
using Aequus.Items.Equipment.Armor.SetNecromancer;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Materials.SoulGem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Armor.SetNecromancer {
    [AutoloadEquip(EquipType.Head)]
    [WorkInProgress]
    public class NecromancerHood : ModItem {
        public int EnemyDamage;
        public int[] EnemySpawn;

        public override void Load() {
            if (!Main.dedServ) {
                GlowMasksHandler.AddGlowmask(AequusTextures.NecromancerHood_Head_Glow.Path);
            }
        }

        public override void SetDefaults() {
            Item.defense = 4;
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 1);
            EnemyDamage = 100;
            EnemySpawn = new int[] {
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
            aequus.armorNecromancerBattle = this;
            //var legModifiers = aequus.equipModifiers.Legs();
            //legModifiers.Boost |= EquipBoostType.Defense | EquipBoostType.Abilities;
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
#if DEBUG
            CreateRecipe()
                .AddIngredient<GravetenderHood>()
                .AddIngredient<DemonicEnergy>(1)
                .AddIngredient<SoulGemFilled>(3)
                .AddTile(TileID.Loom)
                .TryRegisterBefore(ItemID.GravediggerShovel);
#endif
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