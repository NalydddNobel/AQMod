using Aequus.Common.NPCs;
using Aequus.Common.NPCs.Components;
using Aequus.Content.DataSets;
using Aequus.Content.Items.Equipment.Accessories.Inventory;
using Aequus.Core.Autoloading;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Content.Enemies.PollutedOcean.Scavenger;

public partial class Scavenger : AIFighterLegacy, IPreDropItems, IPostPopulateItemDropDatabase {
    public const int HeadSlot = 0;
    public const int BodySlot = 1;
    public const int LegSlot = 2;
    public const int AccSlot = 3;
    public const int ArmorCount = 4;

    public static int ExtraEquipChance = 2;
    public static int ItemDropChance = 4;
    public static int TravelingMerchantBuilderItemChance = 20;

    private int serverWhoAmI = Main.maxPlayers;
    private Player playerDummy;
    public Item[] armor;
    public Item weapon;
    public int attackAnimation;

    public override float SpeedCap => base.SpeedCap + runSpeedCap;

    public override float Acceleration => base.Acceleration + acceleration;

    #region Initialization
    public Scavenger() {
        weapon = new();
        armor = new Item[ArmorCount];
        for (int i = 0; i < armor.Length; i++) {
            armor[i] = new();
        }
    }

    public override void SetStaticDefaults() {
        SetupAccessoryUsages();
        SetupDrawLookups();
        Main.npcFrameCount[Type] = 20;
        NPCID.Sets.NPCBestiaryDrawOffset[Type] = new() {
            Velocity = -1f,
            Scale = 1f,
        };
        NPCSets.PushableByTypeId.Add(Type);
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry)
            .AddMainSpawn(BestiaryBuilder.CavernsBiome)
            .AddSpawn(BestiaryBuilder.OceanBiome);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScavengerBag>(), ScavengerLootBag.BackpackDropRate));
        npcLoot.Add(ItemDropRule.Common(ItemID.PaintSprayer, TravelingMerchantBuilderItemChance));
        npcLoot.Add(ItemDropRule.Common(ItemID.PortableCementMixer, TravelingMerchantBuilderItemChance));
        npcLoot.Add(ItemDropRule.Common(ItemID.ExtendoGrip, TravelingMerchantBuilderItemChance));
        npcLoot.Add(ItemDropRule.Common(ItemID.BrickLayer, TravelingMerchantBuilderItemChance));
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
        playerDummy.armor[0] = armor[HeadSlot];
        playerDummy.armor[1] = armor[BodySlot];
        playerDummy.armor[2] = armor[LegSlot];
        playerDummy.armor[3] = armor[AccSlot];
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
        List<int> options = new() { HeadSlot, BodySlot, LegSlot, AccSlot };
        while (options.Count > 0) {
            int choice = random.Next(options);

            bool value = choice switch {
                HeadSlot => SetItem(ref armor[HeadSlot], NPCSets.ScavengerHelmets, random),
                BodySlot => SetItem(ref armor[BodySlot], NPCSets.ScavengerBreastplates, random),
                LegSlot => SetItem(ref armor[LegSlot], NPCSets.ScavengerLeggings, random),
                AccSlot => SetItem(ref armor[AccSlot], NPCSets.ScavengerAccessories, random),
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

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        var source = NPC.GetSource_FromThis();

        if (NPC.life <= 0) {
            for (int i = 0; i < 20; i++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Bone, 2.5f * hit.HitDirection, -2.5f);
            }
            Gore.NewGore(source, NPC.position, NPC.velocity, Mod.Find<ModGore>(nameof(AequusTextures.ScavengerGoreHead)).Type, NPC.scale);
            for (int i = 0; i < 2; i++) {
                Gore.NewGore(source, NPC.position + new Vector2(0f, 20f), NPC.velocity, 43, NPC.scale);
                Gore.NewGore(source, NPC.position + new Vector2(0f, 34f), NPC.velocity, 44, NPC.scale);
            }
        }
        else {
            for (int i = 0; i < hit.Damage / (double)NPC.lifeMax * 50f; i++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Bone, hit.HitDirection, -1f);
            }
        }
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
        if (playerDummy == null) {
            InitPlayer();
        }
        playerDummy.Bottom = NPC.Bottom;
        acceleration = -0.08f;
        runSpeedCap = 0f;
        playerDummy.ResetEffects();
        NPC.defense = NPC.defDefense;
        for (int i = 0; i < armor.Length; i++) {
            NPC.defense += armor[i].defense;
        }
        PassDownStatsToPlayer();
        if (!armor[AccSlot].IsAir) {
            playerDummy.ApplyEquipFunctional(armor[AccSlot], hideVisual: false);
        }
        if (NPC.TryGetGlobalNPC<AequusNPC>(out var aequusNPC)) {
            acceleration += playerDummy.runAcceleration;
            aequusNPC.statSpeedX += playerDummy.moveSpeed - 1f;
            runSpeedCap += playerDummy.accRunSpeed / 1.5f;
            if (NPC.velocity.Y < 0f) {
                if (playerDummy.jumpBoost) {
                    aequusNPC.statSpeedY += 0.5f;
                }
                aequusNPC.statSpeedY += playerDummy.jumpSpeedBoost / 4f;
            }
        }

        //bool attacking = DoAttack();
        bool attacking = false;
        if (!armor[AccSlot].IsAir && CustomAccessoryUsage.TryGetValue(armor[AccSlot].type, out var accessoryUpdate)) {
            accessoryUpdate(this, attacking);
        }

        if (attacking) {
            return;
        }

        base.AI();
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
        //for (int i = 0; i < armor.Length; i++) {
        //    TryDroppingItem(armor[i], Main.rand);
        //}

        var dropsRegisterList = new List<Item>();
        for (int i = 0; i < armor.Length; i++) {
            if (armor[i] != null && !armor[i].IsAir && Main.rand.NextBool(ItemDropChance)) {
                dropsRegisterList.Add(armor[i].Clone());
            }
        }

        ScavengerLootBag.AddDropsToList(NPC, dropsRegisterList);

        dropsRegisterList.RemoveAll((i) => i.ModItem is ScavengerBag);
        
        if (dropsRegisterList.Count <= 0) {
            return;
        }

        int bag = NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ScavengerLootBag>());
        if (bag == Main.maxNPCs || Main.npc[bag].ModNPC is not ScavengerLootBag lootBagNPC) {
            return;
        }

        foreach (var drop in dropsRegisterList) {
            drop.Prefix(-1);
        }
        lootBagNPC.drops = dropsRegisterList.ToArray();
        Main.npc[bag].velocity.X += Main.rand.NextFloat(-3f, 3f);
        Main.npc[bag].velocity.Y = -4f;
        Main.npc[bag].netUpdate = true;
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

    public bool PreDropItems(Player closestPlayer) {
        return false;
    }
}