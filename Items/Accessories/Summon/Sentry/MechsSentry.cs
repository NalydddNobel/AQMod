using Aequus.Projectiles.Misc;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Summon.Sentry
{
    public class MechsSentry : ModItem
    {
        public override void SetStaticDefaults()
        {
            SantankInteractions.OnAI.Add(Type, OnAI);
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 12);
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accInheritTurrets = true;
            player.Aequus().accExpertItemBoost = true;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return CheckSantankSentry(equippedItem) && CheckSantankSentry(incomingItem);
        }
        public bool CheckSantankSentry(Item item)
        {
            return item.type != ModContent.ItemType<SantankSentry>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MechanicalWheelPiece)
                .AddIngredient(ItemID.MechanicalWagonPiece)
                .AddIngredient(ItemID.MechanicalBatteryPiece)
                .AddIngredient(ModContent.ItemType<SantankSentry>())
                .AddTile(TileID.MythrilAnvil)
                .Register((r) => r.SortAfterFirstRecipesOf(ItemID.PapyrusScarab));
        }

        public static void ExpertEffect_UpdateAccessory(Item item, Player player)
        {
            var aequus = player.Aequus();
            if (item.type == ItemID.BrainOfConfusion)
            {
                ExpertEffect_BrainOfConfusion(item, player, aequus);
            }
            else if (item.type == ItemID.WormScarf)
            {
                ExpertEffect_WormScarf(item, player, aequus);
            }
        }
        public static void ExpertEffect_WormScarf(Item item, Player player, AequusPlayer aequus)
        {
            if (aequus.accExpertItemBoostWormScarfTimer <= 0)
            {
                int target = AequusHelpers.FindTargetWithLineOfSight(player.position, player.width, player.height);

                if (target != -1)
                {
                    aequus.accExpertItemBoostWormScarfTimer = 90;
                    if (Main.myPlayer == player.whoAmI)
                    {
                        var v = player.DirectionTo(Main.npc[target].Center);
                        Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center + v * 20f, v * 20f, ModContent.ProjectileType<WormScarfLaser>(), 60, 1f, player.whoAmI);
                    }
                }
            }
        }
        public static void ExpertEffect_BrainOfConfusion(Item item, Player player, AequusPlayer aequus)
        {
            aequus.accExpertItemBoostBoCProbesDefenseTimer++;
            if (aequus.accExpertItemBoostBoCProbesDefenseTimer > 30)
            {
                aequus.accExpertItemBoostBoCProbesDefenseTimer = 0;
                aequus.accExpertItemBoostBoCProbesDefense += 5;
                if (aequus.accExpertItemBoostBoCProbesDefense > ProtectiveProbe.MaxDefenseSlices)
                {
                    aequus.accExpertItemBoostBoCProbesDefense = ProtectiveProbe.MaxDefenseSlices;
                }
            }

            if (Main.myPlayer != player.whoAmI)
            {
                return;
            }

            if (player.Aequus()
                .ProjectilesOwned_ConsiderProjectileIdentity(ModContent.ProjectileType<ProtectiveProbe>()) < aequus.accExpertItemBoostBoCProbesDefense / ProtectiveProbe.DefenseSlices)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, player.velocity, ModContent.ProjectileType<ProtectiveProbe>(), 0, 1f, player.whoAmI,
                     player.Aequus().projectileIdentity + 1);
            }
        }

        public static void OnAI(Projectile projectile, SantankSentryProjectile sentry, Item item, Player player, AequusPlayer aequus)
        {
            aequus.accExpertItemBoost = true;
        }
    }

    public class MechsSentryItem : GlobalItem
    {
        public static Dictionary<int, string> TooltipItems { get; private set; }

        public override void Load()
        {
            TooltipItems = new Dictionary<int, string>();
            AddEntry(ItemID.EoCShield);
            AddEntry(ItemID.WormScarf);
            AddEntry(ItemID.BrainOfConfusion);
            AddEntry(ItemID.BoneGlove);
            AddEntry(ItemID.HiveBackpack);
            AddEntry(ItemID.BoneHelm);
            AddEntry(ItemID.VolatileGelatin);
            AddEntry(ItemID.SporeSac);
        }

        internal static string EntryKey(int itemID)
        {
            return $"Mods.Aequus.Tooltips.{nameof(MechsSentry)}.{AequusText.CreateKeyNameFromSearch(ItemID.Search.GetName(itemID))}";
        }
        internal static void AddEntry(int itemID)
        {
            TooltipItems.Add(itemID, EntryKey(itemID));
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            try
            {
                if (!Main.LocalPlayer.Aequus().accExpertItemBoost || !TooltipItems.TryGetValue(item.type, out var text))
                {
                    return;
                }
                int add = 1;
                if (item.type == ItemID.BoneHelm)
                {
                    add = 0;
                }
                tooltips.Insert(AequusTooltips.GetIndex(tooltips, "Tooltip#") + add, new TooltipLine(Mod, "MechsTooltip", Language.GetTextValue(text)) 
                { OverrideColor = Color.Lerp(Color.Red, Color.White, 0.35f), });
            }
            catch
            {

            }
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.Aequus().accExpertItemBoost)
            {
                MechsSentry.ExpertEffect_UpdateAccessory(item, player);
            }
        }
    }

    public class MechsSentryProjectile : GlobalProjectile
    {
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.type == ProjectileID.BoneGloveProj && source is EntitySource_ItemUse itemUse && itemUse.Entity is Player player && player.GetModPlayer<AequusPlayer>().AccExpertItemBoost)
            {
                projectile.Transform(ModContent.ProjectileType<Bonesaw>());
                projectile.velocity *= 1.25f;
                projectile.damage = (int)(projectile.damage * 1.5f);
            }
        }
    }
}