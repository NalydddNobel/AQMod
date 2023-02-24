using Aequus.Content.CrossMod;
using Aequus.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Aequus.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AetherialCrown : ModItem, ItemHooks.IHoverSlot
    {
        public int armorSetChecks;
        public int pretendToBeItem;
        private int body;
        private int legs;
        private static bool iterating;
        private Item _itemCache;

        protected override bool CloneNewInstances => true;

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToHeadgear(20, 20, Item.headSlot);
            Item.defense = 0;
            Item.rare = ItemRarityID.Red;
            _itemCache = null;
        }

        private void CheckSetbonus(Player player)
        {
            pretendToBeItem = 0;
            int foundSetbonuses = 0;
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                var item = AequusItem.SetDefaults(i);
                if (item.headSlot > 0 && !item.vanity)
                {
                    player.armor[0] = item;
                    item.Prefix(Item.prefix);
                    item.defense = 0;
                    player.head = ContentSamples.ItemsByType[i].headSlot;
                    player.UpdateArmorSets(player.whoAmI);
                    if (!string.IsNullOrEmpty(player.setBonus))
                    {
                        _itemCache = player.armor[0];
                        pretendToBeItem = i;
                        foundSetbonuses++;
                    }
                    player.armor[0] = Item;
                    if (foundSetbonuses > armorSetChecks)
                        return;
                }
            }
            if (foundSetbonuses - 1 != armorSetChecks)
            {
                armorSetChecks = 0;
                body = 0;
                legs = 0;
            }
        }

        private void CheckForRefresh(Player player)
        {
            bool updateSet = false;
            if (player.armor[1] == null || player.armor[1].IsAir)
            {
                body = 0;
            }
            else if (player.armor[1].type != body)
            {
                updateSet = true;
                body = player.armor[1].type;
            }
            if (player.armor[2] == null || player.armor[2].IsAir)
            {
                body = 0;
            }
            else if (player.armor[2].type != legs)
            {
                updateSet = true;
                legs = player.armor[2].type;
            }
            if (updateSet)
            {
                CheckSetbonus(player);
            }
        }

        public override ModItem Clone(Item newEntity)
        {
            var clone = (AetherialCrown)base.Clone(newEntity);
            if (_itemCache != null)
                clone._itemCache = _itemCache.Clone();
            return clone;
        }

        public void SetToFakeHelmet(Player player)
        {
            if (_itemCache == null)
            {
                _itemCache = AequusItem.SetDefaults(pretendToBeItem);
                _itemCache.Prefix(Item.prefix);
                _itemCache.defense = 0;
            }
            player.head = ContentSamples.ItemsByType[pretendToBeItem].headSlot;
            player.armor[0] = _itemCache;
        }

        public override void UpdateEquip(Player player)
        {
            iterating = true;
            try
            {
                player.lifeRegen += 2;
                player.Aequus().ammoAndThrowingCost33 = true;
                player.manaCost *= 0.9f;
                player.statManaMax2 += 20;
                player.maxMinions += 1;
                player.maxTurrets += 1;
                player.Aequus().ghostSlotsMax += 1;
                try
                {
                    if (ThoriumMod.Instance != null)
                    {
                        ThoriumMod.Call("BonusBardInspirationMax", player, 1);
                        ThoriumMod.Call("BonusHealerHealBonus", player, 1);
                    }
                }
                catch
                {
                }
                if (pretendToBeItem > 0)
                {
                    try
                    {
                        SetToFakeHelmet(player);
                        player.VanillaUpdateEquip(player.armor[0]);
                    }
                    catch
                    {
                    }
                    player.armor[0] = Item;
                }
            }
            catch
            {
            }
            iterating = false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string clrString = TextHelper.ColorCommandStart(Color.Lerp(Color.Lime, Color.White, 0.8f), alphaPulse: true);
            foreach (var t in tooltips)
            {
                if (t.Name.StartsWith("Tooltip"))
                {
                    t.Text = t.Text.Replace("((", clrString);
                    t.Text = t.Text.Replace("))", "]");
                }
            }
            for (int i = 0; i < tooltips.Count; i++)
            {
                var s = tooltips[i].Text.Split(';');
                if (s.Length > 1)
                {
                    if (!ModLoader.HasMod(s[0]))
                    {
                        tooltips.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        tooltips[i].Text = s[1];
                    }
                }
            }
            if (pretendToBeItem <= 0 || _itemCache == null)
                return;
            tooltips.AddTooltip(new TooltipLine(Mod, "AetherialTooltip", $"Equipped as {_itemCache.Name}") { OverrideColor = Color.Lerp(Color.Orange, Color.White, 0.8f) });
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return true;
        }

        public override void UpdateArmorSet(Player player)
        {
            if (iterating)
                return;

            iterating = true;
            try
            {
                CheckForRefresh(player);
                if (pretendToBeItem > 0)
                {
                    try
                    {
                        SetToFakeHelmet(player);
                        player.UpdateArmorSets(player.whoAmI);
                    }
                    catch
                    {
                    }
                    player.armor[0] = Item;
                }
            }
            catch
            {
            }
            iterating = false;
        }

        public override void AddRecipes()
        {
            var r = CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 8)
                .AddIngredient<UltimateEnergy>(3);

            var fragments = AequusItem.GetPreferredAllFragmentList();
            int stack = 6;

            for (int i = 0; i < fragments.Count; i++)
            {
                r.AddIngredient(fragments[i], stack);
            }

            r.AddTile(TileID.LunarCraftingStation)
                .TryRegisterAfter(ItemID.CelestialSigil);
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write((byte)armorSetChecks);
        }

        public override void NetReceive(BinaryReader reader)
        {
            armorSetChecks = reader.ReadByte();
        }

        public override void SaveData(TagCompound tag)
        {
            if (armorSetChecks > 0)
                tag["ArmorSetChecks"] = armorSetChecks;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet("ArmorSetChecks", out int armorSetChecks))
            {
                this.armorSetChecks = armorSetChecks;
            }
        }

        public bool HoverSlot(Item[] inventory, int context, int slot)
        {
            if (context != ItemSlot.Context.EquipArmor)
            {
                body = 0;
                legs = 0;
                _itemCache = null;
                pretendToBeItem = 0;
                return false;
            }
            if (Main.mouseRight && Main.mouseRightRelease)
            {
                SoundEngine.PlaySound(SoundID.Grab);
                armorSetChecks++;
                body = 0;
                legs = 0;
                return true;
            }
            return false;
        }
    }
}