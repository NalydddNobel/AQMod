using Aequus.Common;
using Aequus.Projectiles;
using Aequus.Projectiles.Misc;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Summon.Sentry
{
    public class SantankSentry : ModItem, Hooks.IUpdateItemDye
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.value = Item.sellPrice(gold: 9);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().sentryInheritItem = Item;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return CheckMechsSentry(equippedItem) && CheckMechsSentry(incomingItem);
        }
        public bool CheckMechsSentry(Item item)
        {
            return item.type != ModContent.ItemType<MechsSentry>();
        }

        void Hooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            if (!isSetToHidden || !isNotInVanitySlot)
            {
                player.Aequus().equippedEyes = Type;
                player.Aequus().cEyes = dyeItem.dye;
            }
        }
    }

    public class SantankSentryProjectile : GlobalProjectile
    {
        public Player dummyPlayer;
        public bool appliedItemStatChanges;

        public override bool InstancePerEntity => true;

        public override GlobalProjectile Clone(Projectile projectile, Projectile projectileClone)
        {
            var clone = (SantankSentryProjectile)base.Clone(projectile, projectileClone);
            if (dummyPlayer != null)
                clone.dummyPlayer = AequusPlayer.ProjectileClone(dummyPlayer);
            return clone;
        }

        public override void SetDefaults(Projectile projectile)
        {
            dummyPlayer = null;
            appliedItemStatChanges = false;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (!projectile.friendly || projectile.hostile)
            {
                return;
            }

            if (ProjectileID.Sets.SentryShot[projectile.type])
            {
                if (source is EntitySource_Parent parent)
                {
                    if (parent.Entity is Projectile parentProj)
                    {
                        if (parentProj.owner == Main.myPlayer && !parentProj.hostile
                        && parentProj.sentry && Main.player[projectile.owner].active && Main.player[parentProj.owner].Aequus().sentryInheritItem != null)
                        {
                            var aequus = Main.player[projectile.owner].Aequus();
                            var parentSentry = parentProj.GetGlobalProjectile<SantankSentryProjectile>();
                            AequusProjectile.pWhoAmI = projectile.whoAmI;
                            AequusProjectile.pIdentity = projectile.identity;
                            try
                            {
                                foreach (var i in AequusPlayer.GetEquips(Main.player[projectile.owner]))
                                {
                                    if (SantankInteractions.OnShoot.TryGetValue(i.type, out var onShoot))
                                    {
                                        onShoot(source, projectile, this, parentProj, parentSentry, i, Main.player[projectile.owner], aequus);
                                    }
                                }
                            }
                            catch
                            {
                            }
                            AequusProjectile.pIdentity = -1;
                            AequusProjectile.pWhoAmI = -1;
                        }
                    }
                }
            }
        }

        public override void PostAI(Projectile projectile)
        {
            if (projectile.hostile || projectile.owner < 0 || projectile.owner >= Main.maxPlayers || Main.player[projectile.owner].Aequus().sentryInheritItem == null)
            {
                dummyPlayer = null;
            }
        }

        public void UpdateInheritance(Projectile projectile)
        {
            if (projectile.hostile || !projectile.sentry || projectile.TurretShouldPersist() || projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
            {
                appliedItemStatChanges = false;
                return;
            }

            if (dummyPlayer == null)
            {
                dummyPlayer = AequusPlayer.ProjectileClone(Main.player[projectile.owner]);
            }
            dummyPlayer.active = true;
            dummyPlayer.dead = false;
            dummyPlayer.Center = projectile.Center;
            dummyPlayer.velocity = projectile.velocity;
            PlayerLoader.PreUpdate(dummyPlayer);
            dummyPlayer.ResetEffects();
            dummyPlayer.UpdateDyes();
            dummyPlayer.whoAmI = projectile.owner;
            dummyPlayer.Aequus().projectileIdentity = projectile.identity;
            dummyPlayer.wet = projectile.wet;
            dummyPlayer.lavaWet = projectile.lavaWet;
            dummyPlayer.honeyWet = projectile.honeyWet;
            AequusProjectile.pWhoAmI = projectile.whoAmI;
            AequusProjectile.pIdentity = projectile.identity;

            try
            {
                var aequus = Main.player[projectile.owner].Aequus();
                dummyPlayer.Aequus().accExpertBoost = aequus.accExpertBoost;
                foreach (var i in AequusPlayer.GetEquips(Main.player[projectile.owner], armor: false))
                {
                    if (SantankInteractions.OnAI.TryGetValue(i.type, out var ai))
                    {
                        ai(projectile, this, i.Clone(), Main.player[projectile.owner], aequus);
                    }
                    else if (aequus.accExpertBoost)
                    {
                        MechsSentry.ExpertEffect_UpdateAccessory(i, dummyPlayer);
                    }
                }
                appliedItemStatChanges = true;
            }
            catch
            {

            }

            PlayerLoader.PostUpdate(dummyPlayer);
            dummyPlayer.numMinions = 0;
            dummyPlayer.slotsMinions = 0f;
            AequusProjectile.pIdentity = -1;
            AequusProjectile.pWhoAmI = -1;
        }
    }

    public class SantankInteractions : IAddRecipes
    {
        /// <summary>
        /// <para>1) Projectile - the projectile</para>
        /// <para>2) SantankSentryProjectile/ModProjectile - the projectile's SantankSentryProjectile instance</para>
        /// <para>3) Item - the accessory</para>
        /// <para>4) Player - the player owner of both projectiles</para>
        /// <para>5) AequusPlayer/ModPlayer - the AequusPlayer instance on the player owner</para>
        /// </summary>
        public static Dictionary<int, Action<Projectile, SantankSentryProjectile, Item, Player, AequusPlayer>> OnAI { get; private set; }
        /// <summary>
        /// <para>1) IEntitySource - the entity source</para>
        /// <para>2) Projectile - the projectile</para>
        /// <para>3) SantankSentryProjectile/ModProjectile - the projectile's SantankSentryProjectile instance</para>
        /// <para>4) Projectile - the parent projectile, aka shooter</para>
        /// <para>5) SantankSentryProjectile/ModProjectile - the projectile/shooter's SantankSentryProjectile instance</para>
        /// <para>6) Item - the accessory</para>
        /// <para>7) Player - the player owner of both projectiles</para>
        /// <para>8) AequusPlayer/ModPlayer - the AequusPlayer instance on the player owner</para>
        /// </summary>
        public static Dictionary<int, Action<IEntitySource, Projectile, SantankSentryProjectile, Projectile, SantankSentryProjectile, Item, Player, AequusPlayer>> OnShoot { get; private set; }

        public static MethodInfo Player_SpawnHallucination;

        void ILoadable.Load(Mod mod)
        {
            Player_SpawnHallucination = typeof(Player).GetMethod("SpawnHallucination", BindingFlags.NonPublic | BindingFlags.Instance);

            // Players get info effects from nearby players, maybe inherited info items should do the same?
            OnAI = new Dictionary<int, Action<Projectile, SantankSentryProjectile, Item, Player, AequusPlayer>>()
            {
                [ItemID.BrainOfConfusion] = ApplyEquipFunctional_AI,
                [ItemID.SporeSac] = SporeSac_AI,
                [ItemID.TerrasparkBoots] = WaterWalkingBoots_AI,
                [ItemID.LavaWaders] = WaterWalkingBoots_AI,
                [ItemID.ObsidianWaterWalkingBoots] = WaterWalkingBoots_AI,
                [ItemID.WaterWalkingBoots] = WaterWalkingBoots_AI,
                [ItemID.FireGauntlet] = ApplyEquipFunctional_AI,
                [ItemID.ArcticDivingGear] = ApplyEquipFunctional_AI,
                [ItemID.JellyfishDivingGear] = ApplyEquipFunctional_AI,
                [ItemID.JellyfishNecklace] = ApplyEquipFunctional_AI,
                [ItemID.Magiluminescence] = ApplyEquipFunctional_AI,
                [ItemID.FloatingTube] = InnerTube_AI,
                [ItemID.BoneHelm] = BoneHelm_AI,
                [ItemID.VolatileGelatin] = VolatileGelatin_AI,
                [ItemID.BoneGlove] = BoneGlove_AI,
            };

            OnShoot = new Dictionary<int, Action<IEntitySource, Projectile, SantankSentryProjectile, Projectile, SantankSentryProjectile, Item, Player, AequusPlayer>>()
            {
                [ItemID.SharkToothNecklace] = SharkToothNecklace_OnShoot,
                [ItemID.BoneGlove] = BoneGlove_OnShoot,
            };
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
        }

        void ILoadable.Unload()
        {
            Player_SpawnHallucination = null;

            OnAI?.Clear();
            OnAI = null;
            OnShoot?.Clear();
            OnShoot = null;
        }

        public static void SporeSac_AI(Projectile projectile, SantankSentryProjectile sentry, Item item, Player player, AequusPlayer aequus)
        {
            List<Projectile> sporeSacProjs = new List<Projectile>();
            int myCount = 0;
            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == projectile.owner && (Main.projectile[i].type == ProjectileID.SporeTrap || Main.projectile[i].type == ProjectileID.SporeTrap2
                    || Main.projectile[i].type == ModContent.ProjectileType<NaniteSpore>()))
                {
                    int identity = Main.projectile[i].Aequus().sourceProjIdentity;
                    if (identity >= 0)
                    {
                        sporeSacProjs.Add(Main.projectile[i]);
                        Main.projectile[i].owner = -1;
                        if (AequusHelpers.FindProjectileIdentity(projectile.owner, identity) == projectile.whoAmI)
                        {
                            myCount++;
                            if (myCount > 10)
                            {
                                goto Reset;
                            }
                        }
                    }
                }
            }
            sentry.dummyPlayer.SporeSac(item);
        Reset:
            foreach (var p in sporeSacProjs)
            {
                p.owner = projectile.owner;
            }
        }
        public static void WaterWalkingBoots_AI(Projectile projectile, SantankSentryProjectile sentry, Item item, Player player, AequusPlayer aequus)
        {
        }
        public static void InnerTube_AI(Projectile projectile, SantankSentryProjectile sentry, Item item, Player player, AequusPlayer aequus)
        {
        }
        public static void BoneHelm_AI(Projectile projectile, SantankSentryProjectile sentry, Item item, Player player, AequusPlayer aequus)
        {
            Player_SpawnHallucination.Invoke(sentry.dummyPlayer, new object[] { item });
        }
        public static void VolatileGelatin_AI(Projectile projectile, SantankSentryProjectile sentry, Item item, Player player, AequusPlayer aequus)
        {
            sentry.dummyPlayer.VolatileGelatin(item);
        }
        public static void BoneGlove_AI(Projectile projectile, SantankSentryProjectile sentry, Item item, Player player, AequusPlayer aequus)
        {
            sentry.dummyPlayer.boneGloveTimer--;
        }
        public static void ApplyEquipFunctional_AI(Projectile projectile, SantankSentryProjectile sentry, Item item, Player player, AequusPlayer aequus)
        {
            sentry.dummyPlayer.ApplyEquipFunctional(item, false);
        }

        public static void SharkToothNecklace_OnShoot(IEntitySource source, Projectile projectile, SantankSentryProjectile sentry, Projectile parent, SantankSentryProjectile parentSentry,
            Item item, Player player, AequusPlayer aequus)
        {
            projectile.ArmorPenetration += 5;
        }
        public static void BoneGlove_OnShoot(IEntitySource source, Projectile projectile, SantankSentryProjectile sentry, Projectile parent, SantankSentryProjectile parentSentry,
            Item item, Player player, AequusPlayer aequus)
        {
            if (Main.myPlayer != player.whoAmI && parentSentry.dummyPlayer != null)
            {
                return;
            }

            parentSentry.dummyPlayer.boneGloveTimer = 60;
            var center = parent.Center;
            var vector = parent.DirectionTo(player.ApplyRangeCompensation(0.2f, center, center + Vector2.Normalize(projectile.velocity) * 100f)) * 10f;
            Projectile.NewProjectile(player.GetSource_Accessory(item), center.X, center.Y, vector.X, vector.Y, 532, 25, 5f, player.whoAmI);
        }
    }
}