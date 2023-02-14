using Aequus;
using Aequus.Buffs;
using Aequus.Buffs.Misc;
using Aequus.Common;
using Aequus.Common.ItemDrops;
using Aequus;
using Aequus.Common.Preferences;
using Aequus.Content.CrossMod;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Debuff;
using Aequus.Items.Accessories.Summon.Necro;
using Aequus.Items.Accessories.Utility;
using Aequus.Items.Accessories.Vanity.Cursors;
using Aequus.Items.Consumables.Bait;
using Aequus.Items.Misc;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Pets.Light;
using Aequus.Items.Tools;
using Aequus.Items.Weapons.Melee;
using Aequus.Items.Weapons.Ranged;
using Aequus.Items.Weapons.Summon.Candles;
using Aequus.Items.Weapons.Summon.Scepters;
using Aequus.NPCs.Monsters.Sky.GaleStreams;
using Aequus.Particles;
using Aequus.Projectiles.Misc.Friendly;
using Aequus.Tiles;
using Aequus.Tiles.Furniture.Gravity;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Aequus.Tiles.CraftingStation;

namespace Aequus.Items
{
    public partial class AequusItem : GlobalItem, IPostSetupContent, IAddRecipes
    {
        public delegate bool CustomCoatingFunction(int x, int y, Player player);

        public static HashSet<int> SummonStaff { get; private set; }
        public static HashSet<int> CritOnlyModifier { get; private set; }

        public static List<int> ClassOrderedPillarFragments { get; private set; }
        public static List<int> RainbowOrderPillarFragments { get; private set; }
        public static List<int> FruitIDs { get; private set; }
        public static List<int> LegendaryFishIDs { get; private set; }

        public static Dictionary<int, CustomCoatingFunction> ApplyCustomCoating { get; private set; }
        public static List<CustomCoatingFunction> RemoveCustomCoating { get; private set; }

        public static Dictionary<int, string> RarityNames { get; private set; }

        private static Dictionary<int, int> ItemToBannerCache;

        public static int ReversedGravityCheck;
        public static int SuctionChestCheck;
        public static int suctionChestCheckAmt;

        public bool reversedGravity;
        public byte noGravityTime;
        public int accStacks;
        public int defenseChange;
        public bool naturallyDropped;
        public bool unOpenedChestItem;
        public bool prefixPotionsBounded;
        public bool luckyDrop;

        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public override void Load()
        {
            FruitIDs = new List<int>()
            {
                ItemID.Apple,
                ItemID.Apricot,
                ItemID.Banana,
                ItemID.BlackCurrant,
                ItemID.BloodOrange,
                ItemID.Cherry,
                ItemID.Coconut,
                ItemID.Dragonfruit,
                ItemID.Elderberry,
                ItemID.Grapefruit,
                ItemID.Lemon,
                ItemID.Mango,
                ItemID.Peach,
                ItemID.Pineapple,
                //ItemID.Pomegranate,
                ItemID.Plum,
                ItemID.Rambutan,
                //ItemID.SpicyPepper,
                ItemID.Starfruit,
            };
            ItemToBannerCache = new Dictionary<int, int>();
            RemoveCustomCoating = new List<CustomCoatingFunction>();
            ApplyCustomCoating = new Dictionary<int, CustomCoatingFunction>();
            LegendaryFishIDs = new List<int>();
            SummonStaff = new HashSet<int>();
            CritOnlyModifier = new HashSet<int>()
            {
                PrefixID.Keen,
                PrefixID.Zealous,
            };
            RarityNames = new Dictionary<int, string>()
            {
                [ItemRarityID.Master] = "Mods.Aequus.ItemRarity.-13",
                [ItemRarityID.Expert] = "Mods.Aequus.ItemRarity.-12",
                [ItemRarityID.Quest] = "Mods.Aequus.ItemRarity.-11",
                [ItemRarityID.Gray] = "Mods.Aequus.ItemRarity.-1",
                [ItemRarityID.White] = "Mods.Aequus.ItemRarity.0",
                [ItemRarityID.Blue] = "Mods.Aequus.ItemRarity.1",
                [ItemRarityID.Green] = "Mods.Aequus.ItemRarity.2",
                [ItemRarityID.Orange] = "Mods.Aequus.ItemRarity.3",
                [ItemRarityID.LightRed] = "Mods.Aequus.ItemRarity.4",
                [ItemRarityID.Pink] = "Mods.Aequus.ItemRarity.5",
                [ItemRarityID.LightPurple] = "Mods.Aequus.ItemRarity.6",
                [ItemRarityID.Lime] = "Mods.Aequus.ItemRarity.7",
                [ItemRarityID.Yellow] = "Mods.Aequus.ItemRarity.8",
                [ItemRarityID.Cyan] = "Mods.Aequus.ItemRarity.9",
                [ItemRarityID.Red] = "Mods.Aequus.ItemRarity.10",
                [ItemRarityID.Purple] = "Mods.Aequus.ItemRarity.11",
            };
            On.Terraria.NPC.NPCLoot_DropHeals += NPCLoot_DropHeals;
        }

        private static void NPCLoot_DropHeals(On.Terraria.NPC.orig_NPCLoot_DropHeals orig, NPC self, Player closestPlayer)
        {
            if (closestPlayer.HasBuff<ManathirstBuff>())
            {
                Item.NewItem(self.GetSource_Loot(), self.getRect(), ItemID.Star);
                return;
            }
            if (closestPlayer.HasBuff<BloodthirstBuff>())
            {
                Item.NewItem(self.GetSource_Loot(), self.getRect(), ItemID.Heart);
                return;
            }
            orig(self, closestPlayer);
        }

        public void PostSetupContent(Aequus aequus)
        {
            var contentArray = new ContentArrayFile("ItemSets", ItemID.Search);

            ClassOrderedPillarFragments = contentArray.ReadIntList("ClassOrderedPillarFragments");
            RainbowOrderPillarFragments = contentArray.ReadIntList("RainbowOrderPillarFragments");
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
            if (Aequus.LogMore)
                Aequus.Instance.Logger.Info("Loading rarity name translations...");
            for (int i = ItemRarityID.Purple + 1; i < RarityLoader.RarityCount; i++)
            {
                try
                {
                    var rare = RarityLoader.GetRarity(i);
                    string key = $"Mods.Aequus.ItemRarity.{rare.Mod.Name}.{rare.Name}";
                    if (TextHelper.ContainsKey(key))
                    {
                        RarityNames.Add(rare.Type, key);
                        if (Aequus.LogMore)
                            Aequus.Instance.Logger.Info($"Autoloaded rarity key: {key}");
                    }
                    //else if (Aequus.LogMore)
                    //{
                    //    Aequus.Instance.Logger.Info($"Key not found: {key}");
                    //}
                }
                catch
                {
                }
            }

            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                var item = ContentSamples.ItemsByType[i];
                if (item.damage > 0 && item.DamageType == DamageClass.Summon && item.shoot > ProjectileID.None && item.useStyle > ItemUseStyleID.None && (ContentSamples.ProjectilesByType[item.shoot].minionSlots > 0f || ContentSamples.ProjectilesByType[item.shoot].sentry))
                {
                    SummonStaff.Add(i);
                }
            }
        }

        public override void Unload()
        {
            ItemToBannerCache?.Clear();
            ItemToBannerCache = null;
            RemoveCustomCoating?.Clear();
            RemoveCustomCoating = null;
            ApplyCustomCoating?.Clear();
            ApplyCustomCoating = null;
            RarityNames?.Clear();
            RarityNames = null;
            LegendaryFishIDs?.Clear();
            LegendaryFishIDs = null;
            SummonStaff?.Clear();
            SummonStaff = null;
            CritOnlyModifier?.Clear();
            CritOnlyModifier = null;
        }

        public override void OnCreate(Item item, ItemCreationContext context)
        {
            if (context is RecipeCreationContext recipeContext && Main.LocalPlayer.adjTile[ModContent.TileType<RecyclingMachineTile>()] && ItemScrap.ScrappableRarities.Contains(item.rare) && Main.LocalPlayer.RollLuck(4) == 0)
            {
                if (recipeContext.recipe.requiredItem.Count == 1 && ItemHelper.CanBeCraftedInto(item.type, recipeContext.recipe.requiredItem[0].type))
                {
                    return;
                }
                var scrap = SetDefaults<ItemScrap>();
                scrap.ModItem<ItemScrap>().Rarity = item.OriginalRarity;
                scrap.ModItem<ItemScrap>().UpdateRarity();
                Main.LocalPlayer.QuickSpawnClonedItemDirect(item.GetSource_FromThis("Aequus: Recipe Scrap"), scrap, 1);
            }
        }

        public override void SetDefaults(Item item)
        {
            if (item.type >= Main.maxItemTypes)
            {
                short id = GlowMasks.GetID(item.type);
                if (id > 0)
                {
                    item.glowMask = id;
                }
            }

            if (item.type == ItemID.ShadowKey)
            {
                item.rare = ItemRarityID.Blue;
                item.value = Item.buyPrice(gold: 15);
            }
            else if (item.type == ItemID.PortalGun)
            {
                if (GameplayConfig.Instance.EarlyPortalGun)
                {
                    item.expert = false;
                    item.value = Item.buyPrice(gold: 10);
                }
            }
            else if (item.type == ItemID.GravityGlobe)
            {
                if (GameplayConfig.Instance.EarlyGravityGlobe)
                {
                    item.expert = false;
                    item.value = Item.buyPrice(gold: 5);
                }
            }

            prefixPotionsBounded = false;
            accStacks = 1;
            noGravityTime = 0;
            reversedGravity = false;
        }

        public override void OnSpawn(Item item, IEntitySource source)
        {
            noGravityTime = 0;
            reversedGravity = false;
            if (source is EntitySource_Loot)
            {
                naturallyDropped = true;
            }
            if (AequusPlayer.doLuckyDropsEffect && Main.netMode != NetmodeID.Server && !item.IsACoin)
            {
                luckyDrop = true;
                int amt = Math.Clamp(item.value / Item.gold, 1, 10);
                for (int i = 0; i < amt; i++)
                {
                    float intensity = (float)Math.Pow(0.9f, i + 1);

                    ParticleSystem.New<ShinyFlashParticle>(ParticleLayer.AboveDust).Setup(AequusHelpers.NextFromRect(Main.rand, item.getRect()), Vector2.Zero, Color.Yellow.UseA(0), Color.White * 0.33f, Main.rand.NextFloat(0.5f, 1f) * intensity, 0.2f, 0f);
                }
                for (int i = 0; i < amt * 2; i++)
                {
                    var d = Dust.NewDustDirect(item.position, item.width, item.height, DustID.SpelunkerGlowstickSparkle);
                    d.velocity *= 0.5f;
                    d.velocity = item.DirectionTo(d.position) * d.velocity.Length();
                }
            }
        }

        public override void UpdateInventory(Item item, Player player)
        {
            noGravityTime = 0;
            reversedGravity = false;
            luckyDrop = false;
        }

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (luckyDrop && Main.netMode != NetmodeID.Server)
            {
                if (Main.rand.NextBool(20))
                {
                    var d = Dust.NewDustDirect(item.position, item.width, item.height, DustID.SpelunkerGlowstickSparkle);
                    d.velocity *= 0.35f;
                    d.velocity = item.DirectionTo(d.position) * d.velocity.Length();
                    d.velocity += item.velocity * 0.5f;
                }
                if (item.velocity.Length() > 2f && Main.GameUpdateCount % 5 == 0)
                {
                    var d = Dust.NewDustDirect(item.position, item.width, item.height, DustID.SpelunkerGlowstickSparkle);
                    d.velocity *= 0.1f;
                }
            }
            if (noGravityTime > 0)
            {
                item.velocity.Y *= 0.95f;
                gravity = 0f;
                noGravityTime--;
            }
            if (reversedGravity)
            {
                gravity = -gravity;
                if (item.velocity.Y < -maxFallSpeed)
                {
                    item.velocity.Y = -maxFallSpeed;
                }
            }
        }

        public override bool CanStack(Item item1, Item item2)
        {
            return item1.prefix == item2.prefix;
        }

        public override bool CanStackInWorld(Item item1, Item item2)
        {
            return item1.prefix == item2.prefix;
        }

        public override bool OnPickup(Item item, Player player)
        {
            if (naturallyDropped && item.IsACoin && player.Aequus().accFoolsGoldRing > 0)
            {
                int multiplier = player.Aequus().accFoolsGoldRing;
                if (item.value > Item.silver)
                {
                    multiplier++;
                }
                if (item.value > Item.gold)
                {
                    multiplier++;
                }
                if (item.value > Item.platinum)
                {
                    multiplier++;
                }
                player.AddBuff(ModContent.BuffType<FoolsGoldRingBuff>(), 120 * multiplier);
            }
            naturallyDropped = false;
            return true;
        }

        public void CheckGravityTiles(Item item, int i)
        {
            bool old = reversedGravity;
            reversedGravity = AequusTile.GetGravityTileStatus(item.Center) < 0;
            if (reversedGravity != old)
            {
                item.velocity.Y = -item.velocity.Y;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncItem, number: i);
                }
            }
        }

        public bool CheckItemAbsorber(Item item, int i)
        {
            var tileCoords = item.Center.ToTileCoordinates();
            int searchSize = 20;
            for (int k = 0; k < suctionChestCheckAmt; k++)
            {
                int x = tileCoords.X + Main.rand.Next(-searchSize, searchSize);
                int y = tileCoords.Y + Main.rand.Next(-searchSize, searchSize);
                if (!WorldGen.InWorld(x, y) || !Main.tile[x, y].HasTile || !Main.tileContainer[Main.tile[x, y].TileType])
                {
                    continue;
                }
                if (Main.tile[x, y].TileType > Main.maxTileSets && TileLoader.GetTile(Main.tile[x, y].TileType) is GravityChestTile gravityChest)
                {
                    int left = x - Main.tile[x, y].TileFrameX / 18;
                    int top = y - Main.tile[x, y].TileFrameY / 18;
                    int chestID = Chest.FindChest(left, top);
                    if (chestID != -1 && gravityChest.PickupItemLogic(x, y, chestID, item, this))
                    {
                        if (!AequusHelpers.TryStackingInto(Main.chest[chestID].item, Chest.maxItems, item, out int chestItemIndex))
                        {
                            if (Main.netMode != NetmodeID.SinglePlayer)
                                NetMessage.SendData(MessageID.SyncChestItem, number: chestID, number2: chestItemIndex);
                            continue;
                        }
                        var itemPos = item.Center;
                        item.TurnToAir();
                        item.active = false;
                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            NetMessage.SendData(MessageID.SyncItem, number: i);
                            var r = new Rectangle(left - searchSize, top - searchSize, searchSize * 2 + 2, searchSize * 2 + 2).WorldRectangle();
                            for (int m = 0; m < Main.maxPlayers; m++)
                            {
                                if (ScreenCulling.ServerSafeInView(Main.player[m].Center, r))
                                {
                                    var p = Aequus.GetPacket(PacketType.GravityChestPickupEffect);
                                    p.Write(itemPos.X);
                                    p.Write(itemPos.Y);
                                    p.Write(left);
                                    p.Write(top);
                                    p.Send(toClient: m);
                                }
                            }
                        }
                        else
                        {
                            ScreenCulling.SetPadding(padding: 20);
                            if (ScreenCulling.OnScreenWorld(Main.LocalPlayer.Center))
                            {
                                GravityChestTile.ItemPickupEffect(itemPos, new Vector2(left * 16f + 16f, top * 16f + 16f));
                            }
                        }
                        suctionChestCheckAmt += 3;
                        if (suctionChestCheckAmt > 30)
                            suctionChestCheckAmt = 30;
                        return true;
                    }
                }
            }
            return false;
        }

        public static void CheckItemGravity()
        {
            ReversedGravityCheck--;
            if (ReversedGravityCheck <= 0)
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                for (int i = 0; i < Main.maxItems; i++)
                {
                    if (Main.item[i].active && !Main.item[i].IsAir && !ItemID.Sets.ItemNoGravity[Main.item[i].type])
                    {
                        Main.item[i].Aequus().CheckGravityTiles(Main.item[i], i);
                    }
                }
                stopWatch.Stop();
                ReversedGravityCheck = Math.Min((int)stopWatch.ElapsedMilliseconds, 30);
            }
        }
        public static void CheckItemAbsorber()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            SuctionChestCheck--;
            if (SuctionChestCheck <= 0)
            {
                if (Main.rand.NextBool(40))
                {
                    suctionChestCheckAmt--;
                    if (suctionChestCheckAmt <= 2)
                        suctionChestCheckAmt = 2;
                }
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                for (int i = 0; i < Main.maxItems; i++)
                {
                    if (Main.item[i].active && !Main.item[i].IsAir)
                    {
                        if (Main.item[i].Aequus().CheckItemAbsorber(Main.item[i], i))
                            break;
                    }
                }
                stopWatch.Stop();
                SuctionChestCheck = Math.Min((int)stopWatch.ElapsedMilliseconds, 30);
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            UpdateEquip_Prefixes(item, player);
            if (defenseChange < 0)
            {
                player.Aequus().negativeDefense -= defenseChange;
            }
            else
            {
                player.statDefense += defenseChange;
            }
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            UpdateAccessory_Prefixes(item, player, hideVisual);
            if (defenseChange < 0)
            {
                player.Aequus().negativeDefense -= defenseChange;
            }
            if (player.Aequus().accBloodCrownSlot != -2)
            {
                accStacks = 1;
            }
            if (item.type == ItemID.RoyalGel || player.npcTypeNoAggro[NPCID.BlueSlime])
            {
                player.npcTypeNoAggro[ModContent.NPCType<WhiteSlime>()] = true;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (defenseChange != 0)
            {
                for (int i = 0; i < tooltips.Count; i++)
                {
                    if (tooltips[i].Mod == "Terraria" && tooltips[i].Name == "Defense")
                    {
                        if (defenseChange == -item.defense)
                        {
                            tooltips.RemoveAt(i);
                            i--;
                            continue;
                        }
                        if (defenseChange <= -item.defense)
                        {
                            tooltips[i].Text = $"-{tooltips[i].Text}";
                            break;
                        }
                        var text = tooltips[i].Text.Split(' ');
                        text[0] += defenseChange > 0 ? 
                            TextHelper.ColorCommand($"(+{defenseChange})", TextHelper.PrefixGood, alphaPulse: true) : 
                            TextHelper.ColorCommand($"({defenseChange})", TextHelper.PrefixBad, alphaPulse: true);
                        tooltips[i].Text = string.Join(' ', text);
                        break;
                    }
                }
            }
            ModifyTooltips_Prefixes(item, tooltips);
        }

        public override bool? UseItem(Item item, Player player)
        {
            var aequus = player.Aequus();
            if (item.damage > 0 && !item.noUseGraphic && !item.noMelee && !item.IsATool()
                && aequus.accHyperCrystal != null && aequus.hyperCrystalCooldown <= aequus.hyperCrystalCooldownMax && aequus.hyperCrystalCooldownMelee <= 0)
            {
                if (aequus.hyperCrystalCooldown <= 0)
                {
                    aequus.hyperCrystalCooldown = aequus.hyperCrystalCooldownMax;
                    aequus.hyperCrystalCooldownMelee = aequus.hyperCrystalCooldown + item.useTime;
                    if (Main.myPlayer == player.whoAmI)
                    {
                        if (item.useStyle == ItemUseStyleID.Swing || item.channel)
                        {
                            Projectile.NewProjectile(player.GetSource_Accessory(aequus.accHyperCrystal), player.Center + new Vector2(0f, -80f - player.height), new Vector2(3f * player.direction, 2f),
                                ModContent.ProjectileType<HyperCrystalProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI, ai0: 4f);
                        }
                        else
                        {
                            Projectile.NewProjectile(player.GetSource_Accessory(aequus.accHyperCrystal), player.Center, Vector2.Normalize(Main.MouseWorld - player.Center) * 4f,
                                ModContent.ProjectileType<HyperCrystalProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI, ai0: 3f);
                        }
                    }
                }
            }
            if (item.buffType > 0 && item.buffTime > 0)
            {
                if (prefixPotionsBounded && !Main.persistentBuff[item.buffType])
                {
                    player.Aequus().BoundedPotionIDs.Add(item.buffType);
                }
                else
                {
                    player.Aequus().BoundedPotionIDs.Remove(item.buffType);
                }
            }
            return null;
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            if (item.damage > 0 && item.CountsAsClass(DamageClass.Throwing) && player.Aequus().ammoAndThrowingCost33 && Main.rand.NextBool(3))
            {
                return false;
            }
            return true;
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (AequusUI.CurrentItemSlot.Context == ItemSlot.Context.EquipAccessory)
            {
                var aequus = Main.LocalPlayer.GetModPlayer<AequusPlayer>();
                if (aequus.accBloodCrownSlot > -1 && AequusUI.CurrentItemSlot.Slot == aequus.accBloodCrownSlot)
                {
                    var backFrame = TextureAssets.InventoryBack16.Value.Frame();
                    var drawPosition = ItemSlotRenderer.InventoryItemGetCorner(position, frame, scale);
                    var color = new Color(150, 60, 60, 255);

                    spriteBatch.Draw(TextureAssets.InventoryBack16.Value, drawPosition, backFrame, color, 0f, backFrame.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
                }
            }
            return true;
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (reversedGravity)
            {
                rotation = MathHelper.Pi - rotation;
            }
            return true;
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            var bb = new BitsByte(naturallyDropped, reversedGravity, noGravityTime > 0, luckyDrop);
            writer.Write(bb);
            if (bb[2])
                writer.Write(noGravityTime);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            var bb = (BitsByte)reader.ReadByte();
            naturallyDropped = bb[0];
            reversedGravity = bb[1];
            if (bb[2])
                noGravityTime = reader.ReadByte();
            luckyDrop = bb[3];
        }

        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            var wingStats = player.Aequus().wingStats;
            speed = wingStats.horizontalSpeed.ApplyTo(speed);
            acceleration = wingStats.horizontalAcceleration.ApplyTo(acceleration);
        }

        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            var wingStats = player.Aequus().wingStats;
            ascentWhenFalling = wingStats.verticalAscentWhenFalling.ApplyTo(ascentWhenFalling);
            ascentWhenRising = wingStats.verticalAscentWhenRising.ApplyTo(ascentWhenRising);
            maxCanAscendMultiplier = wingStats.verticalMaxCanAscendMultiplier.ApplyTo(maxCanAscendMultiplier);
            maxAscentMultiplier = wingStats.verticalMaxAscentMultiplier.ApplyTo(maxAscentMultiplier);
            constantAscend = wingStats.verticalMaxAscentMultiplier.ApplyTo(constantAscend);
        }

        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            if (player.GetModPlayer<AequusPlayer>().moroSummonerFruit && SummonStaff.Contains(item.type))
            {
                mult = 0f;
            }
        }

        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            switch (item.type)
            {
                case ItemID.EyeOfCthulhuBossBag:
                    {
                        if (!GameplayConfig.Instance.EyeOfCthulhuOres || !GameplayConfig.Instance.EyeOfCthulhuOreDropsDecrease)
                        {
                            break;
                        }
                        if (itemLoot.Find<ItemDropWithConditionRule>((i) => i.itemId == ItemID.DemoniteOre, out var itemDropRule))
                        {
                            itemDropRule.amountDroppedMinimum /= 2;
                            itemDropRule.amountDroppedMaximum /= 2;
                        }
                        if (itemLoot.Find((i) => i.itemId == ItemID.CrimtaneOre, out itemDropRule))
                        {
                            itemDropRule.amountDroppedMinimum /= 2;
                            itemDropRule.amountDroppedMaximum /= 2;
                        }
                    }
                    break;

                case ItemID.BossBagBetsy:
                    {
                        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<IronLotus>()));
                    }
                    break;

                case ItemID.MoonLordBossBag:
                    if (GameplayConfig.Instance.EarlyGravityGlobe)
                        itemLoot.RemoveWhere((itemDrop) => itemDrop is CommonDrop commonDrop && commonDrop.itemId == ItemID.GravityGlobe);
                    if (GameplayConfig.Instance.EarlyPortalGun)
                        itemLoot.RemoveWhere((itemDrop) => itemDrop is CommonDrop commonDrop && commonDrop.itemId == ItemID.PortalGun);
                    break;

                case ItemID.PlanteraBossBag:
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<OrganicEnergy>(), 1, 3, 3));
                    break;

                case ItemID.TwinsBossBag:
                case ItemID.DestroyerBossBag:
                case ItemID.SkeletronPrimeBossBag:
                    itemLoot.Add(ItemDropRule.ByCondition(new FuncConditional(() => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3, "AllMechs", "Mods.Aequus.DropCondition.AllMechs"), ModContent.ItemType<TheReconstruction>()));
                    break;

                case ItemID.GoldenCrate:
                case ItemID.GoldenCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<SwordCursor>(), 4));
                    break;

                case ItemID.IronCrate:
                case ItemID.IronCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<GlowCore>(), 6));
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<MiningPetSpawner>(), 6));
                    itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(2, ModContent.ItemType<BoneRing>(), ModContent.ItemType<BattleAxe>(), ModContent.ItemType<Bellows>()));
                    break;

                case ItemID.DungeonFishingCrate:
                case ItemID.DungeonFishingCrateHard:
                case ItemID.HallowedFishingCrate:
                case ItemID.HallowedFishingCrateHard:
                case ItemID.JungleFishingCrate:
                case ItemID.JungleFishingCrateHard:
                case ItemID.LavaCrate:
                case ItemID.LavaCrateHard:
                case ItemID.OasisCrate:
                case ItemID.OasisCrateHard:
                case ItemID.OceanCrate:
                case ItemID.OceanCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrateBait>()));
                    break;

                case ItemID.CorruptFishingCrate:
                case ItemID.CorruptFishingCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CorruptionCandle>(), 3));
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrateBait>()));
                    break;

                case ItemID.CrimsonFishingCrate:
                case ItemID.CrimsonFishingCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrimsonCandle>(), 3));
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrateBait>()));
                    break;

                case ItemID.FloatingIslandFishingCrate:
                case ItemID.FloatingIslandFishingCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Slingshot>(), 3));
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrateBait>()));
                    break;

                case ItemID.FrozenCrate:
                case ItemID.FrozenCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrystalDagger>(), 3));
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrateBait>()));
                    break;

                case ItemID.LockBox:
                    itemLoot.Add(ItemDropRule.OneFromOptions(1, ModContent.ItemType<Valari>(), ModContent.ItemType<Revenant>(), /*ModContent.ItemType<DungeonCandle>(),*/ ModContent.ItemType<PandorasBox>()));
                    break;
            }
        }

        public static int ItemToBanner(int itemID)
        {
            if (ItemToBannerCache.TryGetValue(itemID, out int banner))
            {
                return banner;
            }
            for (int i = 0; i < NPCLoader.NPCCount; i++)
            {
                int bannerID = Item.NPCtoBanner(i);
                int calcedBanner = Item.BannerToItem(bannerID);
                if (calcedBanner == itemID)
                {
                    ItemToBannerCache.Add(itemID, bannerID);
                    return bannerID;
                }
            }
            ItemToBannerCache.Add(itemID, 0);
            return 0;
        }

        public static Item SetDefaults(int type, bool checkMaterial = true)
        {
            var i = new Item();
            i.SetDefaults(type, noMatCheck: !checkMaterial);
            return i;
        }
        public static Item SetDefaults<T>(bool checkMaterial = true) where T : ModItem
        {
            return SetDefaults(ModContent.ItemType<T>(), checkMaterial);
        }

        public static int NewItemCloned(IEntitySource source, Vector2 pos, Item item)
        {
            int i = Item.NewItem(source, pos, item.type, item.stack);
            Main.item[i] = item.Clone();
            Main.item[i].active = true;
            Main.item[i].whoAmI = i;
            Main.item[i].Center = pos;
            Main.item[i].stack = item.stack;
            return i;
        }

        public static bool IsPotion(Item item)
        {
            return item.buffType > 0 && item.buffTime > 0 && item.consumable && item.useStyle == ItemUseStyleID.DrinkLiquid
                && item.healLife <= 0 && item.healMana <= 0 && item.damage < 0 && !Main.buffNoTimeDisplay[item.buffType] && !Main.meleeBuff[item.buffType] &&
                !AequusBuff.ConcoctibleBuffIDsBlacklist.Contains(item.buffType);
        }

        public static void SaveItemID(TagCompound tag, string key, int itemID)
        {
            if (itemID >= Main.maxItemTypes)
            {
                var modItem = ItemLoader.GetItem(itemID);
                tag[$"{key}Key"] = $"{modItem.Mod.Name}:{modItem.Name}";
                return;
            }
            tag[$"{key}ID"] = itemID;
        }
        public static int LoadItemID(TagCompound tag, string key)
        {
            if (tag.TryGet($"{key}Key", out string buffKey))
            {
                var val = buffKey.Split(":");
                if (ModLoader.TryGetMod(val[0], out var mod))
                {
                    if (mod.TryFind<ModItem>(val[1], out var modItem))
                    {
                        return modItem.Type;
                    }
                }
            }
            return tag.Get<int>($"{key}ID");
        }

        public static List<int> GetPreferredAllFragmentList()
        {
            if (ThoriumMod.Instance != null)
                return RainbowOrderPillarFragments;

            return ClassOrderedPillarFragments;
        }
    }
}