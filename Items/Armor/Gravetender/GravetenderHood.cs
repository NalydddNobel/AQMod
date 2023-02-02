using Aequus.Buffs.Minion;
using Aequus.Common.ModPlayers;
using Aequus.Projectiles.Summon.Misc;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.Gravetender
{
    [AutoloadEquip(EquipType.Head)]
    public class GravetenderHood : ModItem, ItemHooks.ISetbonusDoubleTap
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.defense = 2;
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<GravetenderWisp>();
            Item.buffType = ModContent.BuffType<GravetenderMinionBuff>();
            Item.value = Item.sellPrice(silver: 20);
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<GravetenderRobes>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = AequusText.GetText("ArmorSetBonus.Gravetender", AequusText.ArmorSetBonusKey);
            var aequus = player.Aequus();
            aequus.setbonusRef = Item;
            aequus.selectGhostNPC = -2;
            aequus.setGravetender = Item;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 50)
                .AddIngredient(ItemID.RottenChunk, 5)
                .AddTile(TileID.Loom)
                .AddCondition(Recipe.Condition.InGraveyardBiome)
                .TryRegisterBefore(ItemID.GravediggerShovel);
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 50)
                .AddIngredient(ItemID.Vertebrae, 5)
                .AddTile(TileID.Loom)
                .AddCondition(Recipe.Condition.InGraveyardBiome)
                .TryRegisterBefore(ItemID.GravediggerShovel);
        }

        public void OnDoubleTap(Player player, AequusPlayer aequus, int keyDir)
        {
            if (aequus.selectGhostNPC > -1 && Main.npc[aequus.selectGhostNPC].IsZombieAndInteractible(Main.myPlayer))
            {
                aequus.gravetenderGhost = aequus.selectGhostNPC;
                SoundEngine.PlaySound(SoundID.Item4, player.Center);
            }
        }
    }
}