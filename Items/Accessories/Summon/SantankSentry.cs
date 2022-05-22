using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Summon
{
    public sealed class SantankSentry : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 9);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accInheritTurrets = true;
        }
    }

    public class SantankSentryProjectile : GlobalProjectile
    {
        /// <summary>
        /// <para>1) Projectile - the projectile</para>
        /// <para>2) SantankSentryProjectile/ModProjectile - the projectile's SantankSentryProjectile instance</para>
        /// <para>3) Item - the accessory</para>
        /// <para>4) Player - the player owner of both projectiles</para>
        /// <para>5) AequusPlayer/ModPlayer - the AequusPlayer instance on the player owner</para>
        /// </summary>
        public static Dictionary<int, Action<Projectile, SantankSentryProjectile, Item, Player, AequusPlayer>> SantankAccessoryInteraction_AI { get; private set; }
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
        public static Dictionary<int, Action<IEntitySource, Projectile, SantankSentryProjectile, Projectile, SantankSentryProjectile, Item, Player, AequusPlayer>> SantankAccessoryInteraction_OnShoot { get; private set; }

        public Player dummyPlayer;
        public bool appliedItemStatChanges;
        public int[] hasBounded;

        public override void Load()
        {
            SantankInteractions.Load();
            // Players get info effects from nearby players, maybe inherited info items should do the same?
            SantankAccessoryInteraction_AI = new Dictionary<int, Action<Projectile, SantankSentryProjectile, Item, Player, AequusPlayer>>()
            {
                [ItemID.TerrasparkBoots] = SantankInteractions.WaterWalkingBoots_AI,
                [ItemID.LavaWaders] = SantankInteractions.WaterWalkingBoots_AI,
                [ItemID.ObsidianWaterWalkingBoots] = SantankInteractions.WaterWalkingBoots_AI,
                [ItemID.WaterWalkingBoots] = SantankInteractions.WaterWalkingBoots_AI,
                [ItemID.FireGauntlet] = SantankInteractions.ApplyEquipFunctional_AI,
                [ItemID.ArcticDivingGear] = SantankInteractions.ApplyEquipFunctional_AI,
                [ItemID.JellyfishDivingGear] = SantankInteractions.ApplyEquipFunctional_AI,
                [ItemID.JellyfishNecklace] = SantankInteractions.ApplyEquipFunctional_AI,
                [ItemID.Magiluminescence] = SantankInteractions.ApplyEquipFunctional_AI,
                [ItemID.StingerNecklace] = SantankInteractions.SharkToothNecklace_AI,
                [ItemID.SharkToothNecklace] = SantankInteractions.SharkToothNecklace_AI,
                [ItemID.FloatingTube] = SantankInteractions.InnerTube_AI,
                [ItemID.BoneHelm] = SantankInteractions.BoneHelm_AI,
                [ItemID.VolatileGelatin] = SantankInteractions.VolatileGelatin_AI,
                [ItemID.BoneGlove] = SantankInteractions.BoneGlove_AI,
            };

            SantankAccessoryInteraction_OnShoot = new Dictionary<int, Action<IEntitySource, Projectile, SantankSentryProjectile, Projectile, SantankSentryProjectile, Item, Player, AequusPlayer>>()
            {
                [ItemID.SharkToothNecklace] = SantankInteractions.SharkToothNecklace_OnShoot,
                [ItemID.BoneGlove] = SantankInteractions.BoneGlove_OnShoot,
            };
        }

        public override void Unload()
        {
            SantankAccessoryInteraction_AI?.Clear();
            SantankAccessoryInteraction_AI = null;
            SantankAccessoryInteraction_OnShoot?.Clear();
            SantankAccessoryInteraction_OnShoot = null;
        }

        public override bool InstancePerEntity => true;

        public override GlobalProjectile Clone(Projectile projectile, Projectile projectileClone)
        {
            var clone = (SantankSentryProjectile)base.Clone(projectile, projectileClone);
            if (dummyPlayer != null)
                clone.dummyPlayer = AequusPlayer.SantankAccClone(dummyPlayer);
            return clone;
        }

        public override void SetDefaults(Projectile projectile)
        {
            dummyPlayer = null;
            appliedItemStatChanges = false;
            hasBounded = null;
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
                        && parentProj.sentry && Main.player[projectile.owner].active && Main.player[parentProj.owner].Aequus().accInheritTurrets)
                        {
                            var aequus = Main.player[projectile.owner].Aequus();
                            var parentSentry = parentProj.GetGlobalProjectile<SantankSentryProjectile>();
                            SantankInteractions.RunningProj = projectile.whoAmI;
                            SantankInteractions.RunningProjParent = parentProj.whoAmI;
                            try
                            {
                                foreach (var i in AequusPlayer.GetEquips(Main.player[projectile.owner]))
                                {
                                    if (SantankAccessoryInteraction_OnShoot.TryGetValue(i.type, out var onShoot))
                                    {
                                        onShoot(source, projectile, this, parentProj, parentSentry, i, Main.player[projectile.owner], aequus);
                                    }
                                }
                            }
                            catch
                            {
                            }
                            SantankInteractions.RunningProjParent = -1;
                            SantankInteractions.RunningProj = -1;
                        }
                    }
                }
            }
        }

        public override void PostAI(Projectile projectile)
        {
            if (projectile.hostile || !projectile.sentry)
            {
                return;
            }

            var aequus = Main.player[projectile.owner].Aequus();
            if (aequus.accInheritTurrets)
            {
                if (dummyPlayer == null)
                {
                    dummyPlayer = AequusPlayer.SantankAccClone(Main.player[projectile.owner]);
                }
                dummyPlayer.Center = projectile.Center;
                dummyPlayer.velocity = projectile.velocity;
                dummyPlayer.ResetEffects();
                dummyPlayer.whoAmI = projectile.owner;
                if (hasBounded == null)
                {
                    hasBounded = Array.Empty<int>();
                }
                SantankInteractions.RunningProj = projectile.whoAmI;
                try
                {
                    foreach (var i in AequusPlayer.GetEquips(Main.player[projectile.owner]))
                    {
                        if (SantankAccessoryInteraction_AI.TryGetValue(i.type, out var ai))
                        {
                            ai(projectile, this, i, Main.player[projectile.owner], aequus);
                        }
                    }
                    appliedItemStatChanges = true;
                }
                catch
                {

                }

                if (hasBounded.Length > 0)
                {
                    var l = new List<int>();
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].owner == projectile.owner && hasBounded.ContainsAny(Main.projectile[i].type))
                        {
                            int proj = (int)Main.projectile[i].ai[0] - 1;
                            if (AequusHelpers.FindProjectileIdentity(projectile.owner, proj) == projectile.whoAmI)
                            {
                                l.Add(Main.projectile[i].type);
                            }
                        }
                    }
                    hasBounded = l.ToArray();
                }

                SantankInteractions.RunningProj = -1;
            }
            else
            {
                dummyPlayer = null;
                hasBounded = null;
            }
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (dummyPlayer != null)
            {
                dummyPlayer.GetModPlayer<HyperCrystalPlayer>().CalcDamage(target.getRect(), ref damage);
            }
        }
    }

    public class SantankInteractions
    {
        public static int RunningProj;
        public static int RunningProjParent;
        public static MethodInfo Player_SpawnHallucination;

        internal static void Load()
        {
            RunningProj = -1;
            RunningProjParent = -1;
            Player_SpawnHallucination = typeof(Player).GetMethod("SpawnHallucination", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static void WaterWalkingBoots_AI(Projectile projectile, SantankSentryProjectile sentry, Item item, Player player, AequusPlayer aequus)
        {
        }
        public static void SharkToothNecklace_AI(Projectile projectile, SantankSentryProjectile sentry, Item item, Player player, AequusPlayer aequus)
        {
            if (!sentry.appliedItemStatChanges)
            {
                projectile.ArmorPenetration += 5;
            }
        }
        public static void ApplyEquipFunctional_AI(Projectile projectile, SantankSentryProjectile sentry, Item item, Player player, AequusPlayer aequus)
        {
            sentry.dummyPlayer.ApplyEquipFunctional(item, false);
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