using Aequus.Common;
using Aequus.Items.Equipment.Armor.SetNecromancer;
using Aequus.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Armor.SetSeraphim {
    [AutoloadEquip(EquipType.Head)]
    [WorkInProgress]
    public class SeraphimHood : NecromancerHood {
        public override void SetDefaults() {
            Item.defense = 9;
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 1);
            EnemySpawn = new int[] {
                NPCID.BlueArmoredBones,
                NPCID.BlueArmoredBonesMace,
                NPCID.BlueArmoredBonesNoPants,
                NPCID.BlueArmoredBonesSword,
                NPCID.HellArmoredBones,
                NPCID.HellArmoredBonesMace,
                NPCID.HellArmoredBonesSpikeShield,
                NPCID.HellArmoredBonesSword,
                NPCID.RustyArmoredBonesAxe,
                NPCID.RustyArmoredBonesFlail,
                NPCID.RustyArmoredBonesSword,
                NPCID.RustyArmoredBonesSwordNoArmor,
            };
            EnemyDamage = 200;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) {
            return body.type == ModContent.ItemType<SeraphimRobes>();
        }

        public override void UpdateArmorSet(Player player) {
            player.setBonus = TextHelper.GetTextValue("ArmorSetBonus.Seraphim");
            player.Aequus().armorNecromancerBattle = this;
        }

        public override void UpdateEquip(Player player) {
            player.GetDamage<SummonDamageClass>() += 0.2f;
            player.Aequus().ghostSlotsMax++;
        }

        public override void AddRecipes() {
#if DEBUG
            CreateRecipe()
                .AddIngredient<NecromancerHood>()
                .AddIngredient<Hexoplasm>(8)
                .AddTile(TileID.Loom)
                .TryRegisterBefore(ItemID.GravediggerShovel);
#endif
        }
    }
}