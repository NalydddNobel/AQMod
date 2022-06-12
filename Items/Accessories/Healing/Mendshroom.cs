using Aequus.Buffs;
using Aequus.Items.Accessories.Summon.Sentry;
using Aequus.Projectiles.Misc;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Healing
{
    public sealed class Mendshroom : ModItem, Hooks.IUpdateItemDye
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);

            SantankInteractions.OnAI.Add(Type, SantankInteractions.ApplyEquipFunctional_AI);
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.rare = ItemDefaults.RarityCrabCrevice;
            Item.value = ItemDefaults.CrabCreviceValue;
            Item.buffType = ModContent.BuffType<MendshroomBuff>();
            Item.shoot = ModContent.ProjectileType<MendshroomAuraProj>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.healingMushroomItem = Item;
            aequus.mendshroomDiameter += 280f;
            aequus.healingMushroomRegeneration += 30;
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            player.Aequus().cHealingMushroom = dyeItem.dye;
        }
    }
}