using Aequus.Common;
using Aequus;
using Aequus.Items.Misc.Energies;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Utility
{
    [AutoloadEquip(EquipType.Back)]
    public class AmmoBackpack : ModItem
    {
        public static HashSet<int> AmmoBlacklist { get; private set; }

        public override void Load()
        {
            AmmoBlacklist = new HashSet<int>()
            {
                AmmoID.FallenStar,
                AmmoID.Gel,
                AmmoID.Solution,
            };
        }

        public override void Unload()
        {
            AmmoBlacklist?.Clear();
            AmmoBlacklist = null;
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(20, 20);
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accAmmoRenewalPack = Item;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AmmoBox)
                .AddIngredient<AquaticEnergy>()
                .AddTile(TileID.Anvils)
                .TryRegisterBefore(ItemID.CursedBullet);
        }

        public static void Proc(Player player, AequusPlayer aequus, EnemyKillInfo npc)
        {
            if (aequus.accAmmoRenewalPack != null)
            {
                int ammoBackpackChance = 3;
                if (npc.value > Item.copper * 20 && (ammoBackpackChance <= 1 || Main.rand.NextBool(ammoBackpackChance)))
                {
                    if (Main.myPlayer == player.whoAmI)
                    {
                        int stacks = aequus.accAmmoRenewalPack.Aequus().accStacks;
                        for (int i = 0; i < stacks; i++)
                        {
                            DropAmmo(player, npc, aequus.accAmmoRenewalPack);
                        }
                    }
                }
            }
        }
        public static void DropAmmo(Player player, EnemyKillInfo npc, Item ammoBackpack)
        {
            var neededAmmoTypes = GetAmmoTypesToSpawn(player, npc, ammoBackpack);
            if (neededAmmoTypes.Count > 0)
            {
                int chosenType = Main.rand.Next(neededAmmoTypes);
                int stack = DetermineStack(chosenType, player, npc, ammoBackpack);
                int i = Item.NewItem(player.GetSource_Accessory(ammoBackpack), npc.Rect, chosenType, stack);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 1f);
                }
            }
        }
        public static int DetermineStack(int itemToSpawn, Player player, EnemyKillInfo npc, Item ammoBackpack)
        {
            float m = StackMultiplier(itemToSpawn, player, npc, ammoBackpack);
            return (int)Math.Max((Main.rand.Next(50) + 1) * m, 1);
        }
        public static float StackMultiplier(int itemToSpawn, Player player, EnemyKillInfo npc, Item ammoBackpack)
        {
            return 1f - Math.Clamp(ContentSamples.ItemsByType[itemToSpawn].value / (Item.silver * (Math.Max(npc.value, Item.silver * 2.5f) / (Item.silver * 5f))), 0f, 1f);
        }
        public static List<int> GetAmmoTypesToSpawn(Player player, EnemyKillInfo npc, Item ammoBackpack)
        {
            var l = new List<int>();
            bool fullSlots = !player.inventory[Main.InventoryAmmoSlotsStart].IsAir && !player.inventory[Main.InventoryAmmoSlotsStart + 1].IsAir
                && !player.inventory[Main.InventoryAmmoSlotsStart + 2].IsAir && !player.inventory[Main.InventoryAmmoSlotsStart + 3].IsAir;

            for (int i = Main.InventoryAmmoSlotsStart; i < Main.InventoryAmmoSlotsStart + Main.InventoryAmmoSlotsCount; i++)
            {
                var item = player.inventory[i];
                if (item.IsAir || !item.consumable || item.makeNPC > 0 || item.damage == 0 || item.ammo <= ItemID.None || ContentSamples.ItemsByType[item.ammo].makeNPC > 0 || item.bait > 0 || item.stack >= item.maxStack)
                {
                    continue;
                }
                if ((!fullSlots || item.type == item.ammo) && !AmmoBlacklist.Contains(item.ammo) && !l.Contains(item.ammo) && Main.rand.NextBool(3))
                    l.Add(item.ammo);
                if (!AmmoBlacklist.Contains(item.type) && !l.Contains(item.type))
                    l.Add(item.type);
            }

            for (int i = Main.InventoryAmmoSlotsStart; i < Main.InventoryAmmoSlotsStart + Main.InventoryAmmoSlotsCount; i++)
            {
                if (!player.inventory[i].consumable)
                {
                    l.Remove(player.inventory[i].ammo);
                    l.Remove(player.inventory[i].type);
                }
            }
            return l;
        }
    }
}