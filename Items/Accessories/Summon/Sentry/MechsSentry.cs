using Aequus.Projectiles;
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
    public class MechsSentry : ModItem, ItemHooks.IUpdateItemDye
    {
        public override void Load()
        {
            On.Terraria.Player.SporeSac += Hook_NaniteBombs;
            On.Terraria.Player.VolatileGelatin += Hook_ThermiteGel;
        }

        private static void Hook_NaniteBombs(On.Terraria.Player.orig_SporeSac orig, Player self, Item sourceItem)
        {
            if (!self.Aequus().ExpertBoost)
            {
                orig(self, sourceItem);
                return;
            }

            SpawnNaniteBombs(self, sourceItem);
        }
        public static void SpawnNaniteBombs(Player player, Item sourceItem)
        {
            int damage = 70;
            float knockBack = 1.5f;
            if (!Main.rand.NextBool(15))
            {
                return;
            }
            int num = 0;
            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && (Main.projectile[i].type == ModContent.ProjectileType<NaniteSpore>()))
                {
                    num++;
                }
            }
            if (Main.rand.Next(15) < num || num >= 10)
            {
                return;
            }
            int num2 = 50;
            int num3 = 24;
            int num4 = 90;
            int num5 = 0;
            Vector2 center;
            while (true)
            {
                if (num5 >= num2)
                {
                    return;
                }
                int num6 = Main.rand.Next(200 - num5 * 2, 400 + num5 * 2);
                center = player.Center;
                center.X += Main.rand.Next(-num6, num6 + 1);
                center.Y += Main.rand.Next(-num6, num6 + 1);
                if (!Collision.SolidCollision(center, num3, num3) && !Collision.WetCollision(center, num3, num3))
                {
                    center.X += num3 / 2;
                    center.Y += num3 / 2;
                    if (Collision.CanHit(new Vector2(player.Center.X, player.position.Y), 1, 1, center, 1, 1) || Collision.CanHit(new Vector2(player.Center.X, player.position.Y - 50f), 1, 1, center, 1, 1))
                    {
                        int num7 = (int)center.X / 16;
                        int num8 = (int)center.Y / 16;
                        bool flag = false;
                        if (Main.rand.NextBool(3)&& Main.tile[num7, num8] != null && Main.tile[num7, num8].WallType > 0)
                        {
                            flag = true;
                        }
                        else
                        {
                            center.X -= num4 / 2;
                            center.Y -= num4 / 2;
                            if (Collision.SolidCollision(center, num4, num4))
                            {
                                center.X += num4 / 2;
                                center.Y += num4 / 2;
                                flag = true;
                            }
                            else if (Main.tile[num7, num8] != null && Main.tile[num7, num8].HasTile && Main.tile[num7, num8].TileType == 19)
                            {
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            for (int j = 0; j < 1000; j++)
                            {
                                if (Main.projectile[j].active && Main.projectile[j].owner == player.whoAmI && Main.projectile[j].aiStyle == 105)
                                {
                                    var val = center - Main.projectile[j].Center;
                                    if (val.Length() < 48f)
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                            if (flag && Main.myPlayer == player.whoAmI)
                            {
                                break;
                            }
                        }
                    }
                }
                num5++;
            }
            Projectile.NewProjectile(player.GetSource_Accessory(sourceItem), center.X, center.Y, 0f, 0f, ModContent.ProjectileType<NaniteSpore>(), damage, knockBack, player.whoAmI);
        }

        private static void Hook_ThermiteGel(On.Terraria.Player.orig_VolatileGelatin orig, Player self, Item sourceItem)
        {
            if (!self.Aequus().ExpertBoost)
            {
                orig(self, sourceItem);
                return;
            }

            if (Main.myPlayer != self.whoAmI)
            {
                return;
            }
            self.volatileGelatinCounter++;
            if (self.volatileGelatinCounter <= 50)
            {
                return;
            }
            self.volatileGelatinCounter = 0;
            SpawnThermiteGel(self, sourceItem);
        }
        public static void SpawnThermiteGel(Player player, Item sourceItem)
        {
            int damage = 65;
            float knockBack = 7f;
            float num = 800f;
            NPC nPC = null;
            for (int i = 0; i < 200; i++)
            {
                NPC nPC2 = Main.npc[i];
                if (nPC2 != null && nPC2.active && nPC2.CanBeChasedBy(player) && Collision.CanHit(player, nPC2))
                {
                    float num2 = Vector2.Distance(nPC2.Center, player.Center);
                    if (num2 < num)
                    {
                        num = num2;
                        nPC = nPC2;
                    }
                }
            }
            if (nPC != null)
            {
                Vector2 v = nPC.Center - player.Center;
                v = v.SafeNormalize(Vector2.Zero) * 9f;
                v.Y -= 3f;
                Projectile.NewProjectile(player.GetSource_Accessory(sourceItem), player.Center, v, ModContent.ProjectileType<ThermiteGel>(), damage, knockBack, player.whoAmI);
            }
        }

        public override void SetStaticDefaults()
        {
            SantankInteractions.OnAI.Add(Type, OnAI);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(gold: 35);
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().sentryInheritItem = Item;
            player.Aequus().accExpertBoost = true;
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
            else if (item.type == ItemID.EoCShield)
            {
                ExpertEffect_ShieldOfCthulhu(item, player, aequus);
            }
        }
        public static void ExpertEffect_ShieldOfCthulhu(Item item, Player player, AequusPlayer aequus)
        {
            if (player.eocDash > 0 && Main.myPlayer == player.whoAmI && player.ownedProjectileCounts[ModContent.ProjectileType<ShieldOfCthulhuBoost>()] <= 0)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, new Vector2(player.direction, 0f), ModContent.ProjectileType<ShieldOfCthulhuBoost>(), player.GetWeaponDamage(item), 1f, player.whoAmI);
            }
        }
        public static void ExpertEffect_WormScarf(Item item, Player player, AequusPlayer aequus)
        {
            if (aequus.expertBoostWormScarfTimer <= 0)
            {
                int target = AequusHelpers.FindTargetWithLineOfSight(player.position, player.width, player.height);

                if (target != -1)
                {
                    aequus.expertBoostWormScarfTimer = 90;
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
            aequus.expertBoostBoCTimer++;
            if (aequus.expertBoostBoCTimer > 30)
            {
                aequus.expertBoostBoCTimer = 0;
                aequus.expertBoostBoCDefense += 5;
                if (aequus.expertBoostBoCDefense > ProtectiveProbe.MaxDefenseSlices)
                {
                    aequus.expertBoostBoCDefense = ProtectiveProbe.MaxDefenseSlices;
                }
            }

            if (Main.myPlayer == player.whoAmI && player.Aequus()
                .ProjectilesOwned(ModContent.ProjectileType<ProtectiveProbe>()) < aequus.expertBoostBoCDefense / ProtectiveProbe.DefenseSlices)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, player.velocity, ModContent.ProjectileType<ProtectiveProbe>(), 0, 1f, player.whoAmI,
                     player.Aequus().projectileIdentity + 1);
            }
        }

        public static void OnAI(Projectile projectile, SantankSentryProjectile sentry, Item item, Player player, AequusPlayer aequus)
        {
            aequus.accExpertBoost = true;
        }

        void ItemHooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            if (!isSetToHidden || !isNotInVanitySlot)
            {
                player.Aequus().equippedEyes = Type;
                player.Aequus().cEyes = dyeItem.dye;
            }
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
            //AddEntry(ItemID.HiveBackpack);
            //AddEntry(ItemID.BoneHelm);
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
                if (!Main.LocalPlayer.Aequus().accExpertBoost || !TooltipItems.TryGetValue(item.type, out var text))
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
            if (player.Aequus().ExpertBoost)
            {
                MechsSentry.ExpertEffect_UpdateAccessory(item, player);
            }
        }
    }

    public class MechsSentryProjectile : GlobalProjectile
    {
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
        }
    }
}