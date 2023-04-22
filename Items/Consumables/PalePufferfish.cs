using Aequus.Buffs.Debuffs;
using Aequus.Common.Recipes;
using Aequus.Content.Vampirism.Buffs;
using Aequus.Content.Vampirism.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables {
    public class PalePufferfish : ModItem
    {
        public static HashSet<int> RemovableBuffs { get; private set; }

        public override void Load()
        {
            RemovableBuffs = new HashSet<int>()
            {
                BuffID.OnFire,
                BuffID.OnFire3,
                BuffID.Ichor,
                BuffID.CursedInferno,
                BuffID.Frostburn,
                BuffID.Frostburn2,
                BuffID.Chilled,
                BuffID.Bleeding,
                BuffID.Confused,
                BuffID.Poisoned,
                BuffID.Venom,
                BuffID.Darkness,
                BuffID.Blackout,
                BuffID.Silenced,
                BuffID.Cursed,
                BuffID.Slow,
                BuffID.Weak,
                BuffID.WitheredArmor,
                BuffID.Electrified,
                BuffID.Rabies,
                BuffID.OgreSpit,
                BuffID.VortexDebuff,
                BuffID.Tipsy,
                ModContent.BuffType<VampirismBuff>(),
                ModContent.BuffType<BlueFire>(),
                ModContent.BuffType<PickBreak>(),
            };
        }

        public override void Unload()
        {
            RemovableBuffs?.Clear();
            RemovableBuffs = null;
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            ItemID.Sets.FoodParticleColors[Type] = new[] { Color.Red, Color.DarkRed };
            ItemID.Sets.DrinkParticleColors[Type] = new Color[] { Color.Red, };
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = true;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item2;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.maxStack = Item.CommonMaxStack;
        }

        public override bool? UseItem(Player player)
        {
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                if (player.buffTime[i] > 0 && RemovableBuffs.Contains(player.buffType[i]))
                {
                    player.DelBuff(i);
                    i--;
                }
            }
            player.GetModPlayer<AequusPlayer>()._vampirismData = 0;
            return true;
        }

        public override void AddRecipes()
        {
            AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<VampireSquid>());
        }
    }
}