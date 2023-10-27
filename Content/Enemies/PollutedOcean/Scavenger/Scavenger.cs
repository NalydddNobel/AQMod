using Aequus.Common.NPCs;
using Aequus.Content.DataSets;
using Aequus.Content.Items.Equipment.Accessories.Inventory;
using Aequus.Core.Autoloading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Aequus.Content.Graphics.RadonMossFogRenderer;

namespace Aequus.Content.Enemies.PollutedOcean.Scavenger;

public partial class Scavenger : AIFighterLegacy, IPostPopulateItemDropDatabase {
    public const int Helmet = 0;
    public const int Breastplate = 1;
    public const int Leggings = 2;
    public const int Accessory = 3;
    public const int ArmorCount = 4;

    public static int ExtraEquipChance = 4;
    public static int ItemDropChance = 4;

    private int serverWhoAmI = Main.maxPlayers;
    private Player playerDummy;
    public Item[] armor;
    public Item weapon;
    public int attackAnimation;

    #region Initialization
    public Scavenger() {
        weapon = new();
        armor = new Item[ArmorCount];
        for (int i = 0; i < armor.Length; i++) {
            armor[i] = new();
        }
    }

    public override void SetStaticDefaults() {
        LoadAccessoryUsages();
        Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Skeleton];
        NPCSets.PushableByTypeId.Add(Type);
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry)
            .AddMainSpawn(BestiaryBuilder.CavernsBiome)
            .AddSpawn(BestiaryBuilder.OceanBiome);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScavengerBag>(), 10));
    }

    public virtual void PostPopulateItemDropDatabase(Aequus aequus, ItemDropDatabase database) {
        NPCHelper.InheritDropRules(NPCID.Skeleton, Type, database);
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(NPCID.Skeleton);
        AnimationType = NPCID.Skeleton;
        NPC.aiStyle = -1;
    }
    #endregion

    private void PassDownStatsToPlayer() {
        if (Main.netMode == NetmodeID.Server && serverWhoAmI != Main.myPlayer) {
            serverWhoAmI = Main.myPlayer;
            NPC.netUpdate = true;
        }
        playerDummy.statLife = NPC.life;
        playerDummy.statLifeMax = NPC.lifeMax;
        playerDummy.statLifeMax2 = NPC.lifeMax;
        playerDummy.statDefense = Player.DefenseStat.Default + NPC.defense;
        playerDummy.armor[0] = armor[Helmet];
        playerDummy.armor[1] = armor[Breastplate];
        playerDummy.armor[2] = armor[Leggings];
        playerDummy.armor[3] = armor[Accessory];
        playerDummy.selectedItem = 0;
        playerDummy.inventory[0] = weapon;
        playerDummy.whoAmI = serverWhoAmI;
    }

    private void InitPlayer() {
        playerDummy = new();
        PassDownStatsToPlayer();
    }

    private bool SetItem(ref Item item, int itemId, int stack = 1) {
        if (itemId <= 0) {
            item.TurnToAir();
            return false;
        }
        item.SetDefaults(itemId);
        item.stack = stack;
        return true;
    }

    private bool SetItem(ref Item item, List<int> itemList, UnifiedRandom random) {
        return SetItem(ref item, Main.rand.Next(itemList));
    }

    private void RandomizeArmor(UnifiedRandom random) {
        SetItem(ref weapon, NPCSets.ScavengerWeapons, random);

        List<int> options = new() { Helmet, Breastplate, Leggings, Accessory };
        while (options.Count > 0) {
            int choice = random.Next(options);

            bool value = choice switch {
                Helmet => SetItem(ref armor[Helmet], NPCSets.ScavengerHelmets, random),
                Breastplate => SetItem(ref armor[Breastplate], NPCSets.ScavengerBreastplates, random),
                Leggings => SetItem(ref armor[Leggings], NPCSets.ScavengerLeggings, random),
                Accessory => SetItem(ref armor[Accessory], NPCSets.ScavengerAccessories, random),
                _ => false
            };

            if (!random.NextBool(ExtraEquipChance)) {
                break;
            }

            if (value) {
                options.Remove(choice);
            }
        }
    }

    public override void OnSpawn(IEntitySource source) {
        RandomizeArmor(Main.rand);
        InitPlayer();
    }

    private bool DoAttack() {
        float attackDistance = 200f;
        var target = Main.player[NPC.target];
        if (NPC.velocity.Y == 0f && NPC.Distance(target.Center) < attackDistance && Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height)) {
            NPC.velocity.X *= 0.9f;
            NPC.TargetClosest(faceTarget: true);

            attackAnimation++;
            if (attackAnimation > 60) {
                attackAnimation = 0;
                if (weapon.shoot > ProjectileID.None && Main.netMode != NetmodeID.MultiplayerClient) {
                    int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(target.Center) * weapon.shootSpeed, weapon.shoot, weapon.damage, weapon.knockBack, Main.myPlayer);
                    Main.projectile[p].friendly = false;
                    Main.projectile[p].hostile = true;
                    Main.projectile[p].noDropItem = true;
                }
                if (weapon.UseSound != null) {
                    SoundEngine.PlaySound(weapon.UseSound, NPC.Center);
                }
            }
            return true;
        }
        return false;
    }

    public override void AI() {
        playerDummy.ResetEffects();
        NPC.defense = NPC.defDefense;
        for (int i = 0; i < armor.Length; i++) {
            NPC.defense += armor[i].defense;
        }
        if (!armor[Accessory].IsAir) {
            playerDummy.ApplyEquipFunctional(armor[Accessory], hideVisual: false);
        }
        PassDownStatsToPlayer();
        playerDummy.Bottom = NPC.Bottom;

        //bool attacking = DoAttack();
        bool attacking = false;
        if (!armor[Accessory].IsAir && CustomAccessoryUsage.TryGetValue(armor[Accessory].type, out var accessoryUpdate)) {
            accessoryUpdate(this, attacking);
        }

        if (attacking) {
            return;
        }

        base.AI();


        if (NPC.TryGetGlobalNPC<AequusNPC>(out var aequusNPC)) {
            aequusNPC.statSpeedX += playerDummy.accRunSpeed / Speed;
            if (NPC.velocity.Y < 0f) {
                if (playerDummy.jumpBoost) {
                    aequusNPC.statSpeedY += 0.5f;
                }
                aequusNPC.statSpeedY += playerDummy.jumpSpeedBoost / 4f;
            }
        }
    }

    public override void FindFrame(int frameHeight) {
        base.FindFrame(frameHeight);
    }

    private void TryDroppingItem(Item item, UnifiedRandom random) {
        if (item != null && !item.IsAir && random.NextBool(ItemDropChance)) {
            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), item.type, item.stack, prefixGiven: -1);
        }
    }

    public override void OnKill() {
        //TryDroppingItem(weapon, Main.rand);
        for (int i = 0; i < armor.Length; i++) {
            TryDroppingItem(armor[i], Main.rand);
        }
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(serverWhoAmI);
        writer.Write(attackAnimation);
        writer.Write(accessoryUseData);
        writer.Write(weapon.type);
        writer.Write(weapon.stack);
        for (int i = 0; i < armor.Length; i++) {
            writer.Write(armor[i].type);
            writer.Write(armor[i].stack);
        }
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        serverWhoAmI = reader.ReadInt32();
        attackAnimation = reader.ReadInt32();
        accessoryUseData = reader.ReadSingle();
        SetItem(ref weapon, reader.ReadInt32(), reader.ReadInt32());
        for (int i = 0; i < armor.Length; i++) {
            SetItem(ref armor[i], reader.ReadInt32(), reader.ReadInt32());
        }
    }
}