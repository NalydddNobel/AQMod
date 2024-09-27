using Aequus.Common.Entities.Banners;
using Aequus.Common.Entities.Bestiary;
using Aequus.Common.Entities.NPCs.AI;
using Aequus.Common.NPCs;
using Aequus.Common.NPCs.Global;
using Aequus.Common.Utilities.Helpers;
using Aequus.Content.Biomes.PollutedOcean;
using Aequus.Content.Items.Accessories.Backpacks;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Aequus.Content.Monsters.PollutedOcean.Scavenger;

[AutoloadBanner]
//[AutoloadStatue]
[BestiaryBiome<PollutedOceanUnderground>()]
public partial class Scavenger : ModNPC, IFighterAIProvider, IPostPopulateItemDropDatabase {
    public const int Slot_Head = 0;
    public const int Slot_Body = 1;
    public const int Slot_Legs = 2;
    public const int Slot_Accs = 3;
    public const int ARMOR_COUNT = 4;

    public static readonly int ExtraEquipChance = 2;
    public static readonly int ItemDropChance = 8;
    public static readonly int TravelingMerchantBuilderItemChance = 30;

    private int serverWhoAmI = Main.maxPlayers;
    private Player playerDummy;
    public Item[] armor;
    public Item weapon;
    public int attackAnimation;

    NPC IFighterAIProvider.NPC => NPC;

    float IFighterAIProvider.SpeedCap => FighterAI.DefaultSpeedCap + runSpeedCap;

    float IFighterAIProvider.Acceleration => FighterAI.DefaultAcceleration + acceleration;

#if !POLLUTED_OCEAN
    public override bool IsLoadingEnabled(Mod mod) {
        return false;
    }
#endif

    #region Initialization
    public Scavenger() {
        weapon = new();
        armor = new Item[ARMOR_COUNT];
        for (int i = 0; i < armor.Length; i++) {
            armor[i] = new();
        }
    }

    public override void SetStaticDefaults() {
        SetupAccessoryUsages();
        Main.npcFrameCount[Type] = 20;
        NPCID.Sets.NPCBestiaryDrawOffset[Type] = new() {
            Velocity = -1f,
            Scale = 1f,
        };
        NPCID.Sets.StatueSpawnedDropRarity[Type] = 0.01f;
        LegacyPushableEntities.NPCIDs.Add(Type);
#if ELEMENTS
        NPCDataSet.NoDropElementInheritence.Add(Type);
#endif
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScavengerBag>(), 50));
        npcLoot.Add(ItemDropRule.OneFromOptions(TravelingMerchantBuilderItemChance, ItemID.PaintSprayer, ItemID.PortableCementMixer, ItemID.ExtendoGrip, ItemID.BrickLayer));
#if COPPER_CHEST
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CopperKey>(), chanceDenominator: CopperKey.DropRate));
#endif
    }

    public virtual void PostPopulateItemDropDatabase(ItemDropDatabase database) {
        LootUtils.InheritDropRules(NPCID.Skeleton, Type, database);
    }

    public override void SetDefaults() {
        NPC.width = 18;
        NPC.height = 40;
        NPC.aiStyle = 3;
        NPC.damage = 20;
        NPC.defense = 8;
        NPC.lifeMax = 60;
        NPC.HitSound = SoundID.NPCHit2;
        NPC.DeathSound = SoundID.NPCDeath2;
        NPC.knockBackResist = 0.5f;
        NPC.value = Item.silver;
        AnimationType = NPCID.Skeleton;
        NPC.aiStyle = -1;
        NPC.npcSlots = 2f;
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
        playerDummy.armor[0] = armor[Slot_Head];
        playerDummy.armor[1] = armor[Slot_Body];
        playerDummy.armor[2] = armor[Slot_Legs];
        playerDummy.armor[3] = armor[Slot_Accs];
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
        ScavengerItemChoices choices = Instance<ScavengerItemChoices>();
        SetItem(ref weapon, choices.Weapons, random);
        var options = new List<int>() { Slot_Head, Slot_Body, Slot_Legs, Slot_Accs };
        while (options.Count > 0) {
            int choice = random.Next(options);

            bool value = choice switch {
                Slot_Head => SetItem(ref armor[Slot_Head], choices.Head, random),
                Slot_Body => SetItem(ref armor[Slot_Body], choices.Body, random),
                Slot_Legs => SetItem(ref armor[Slot_Legs], choices.Legs, random),
                Slot_Accs => SetItem(ref armor[Slot_Accs], choices.Accs, random),
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
            RandomizeArmor(Main.rand);
            InitPlayer();
        }
        playerDummy!.Bottom = NPC.Bottom;
        acceleration = -0.08f;
        runSpeedCap = 0f;
        playerDummy.ResetEffects();
        NPC.defense = NPC.defDefense;
        for (int i = 0; i < armor.Length; i++) {
            NPC.defense += armor[i].defense;
        }
        PassDownStatsToPlayer();
        if (!armor[Slot_Accs].IsAir) {
            playerDummy.ApplyEquipFunctional(armor[Slot_Accs], hideVisual: false);
        }
        if (NPC.TryGetGlobalNPC(out StatSpeedGlobalNPC speed)) {
            acceleration += playerDummy.runAcceleration;
            speed.statSpeed += playerDummy.moveSpeed - 1f;
            runSpeedCap += playerDummy.accRunSpeed / 1.5f;
            if (NPC.velocity.Y < 0f) {
                if (playerDummy.jumpBoost) {
                    speed.statSpeedJumpSpeedMultiplier += 0.5f;
                }
                speed.statSpeedJumpSpeedMultiplier += playerDummy.jumpSpeedBoost / 4f;
            }
        }

        //bool attacking = DoAttack();
        bool attacking = false;
        if (!armor[Slot_Accs].IsAir && CustomAccessoryUsage.TryGetValue(armor[Slot_Accs].type, out var accessoryUpdate)) {
            accessoryUpdate(this, attacking);
        }

        if (attacking) {
            return;
        }

        FighterAI.AI(this);
    }

    public override bool? CanFallThroughPlatforms() {
        return NPC.HasValidTarget && Main.player[NPC.target].position.Y > NPC.Bottom.Y;
    }

    /*
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

        //void TryDroppingItem(Item item, UnifiedRandom random) {
        //    if (item != null && !item.IsAir && random.NextBool(ItemDropChance)) {
        //        Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), item.type, item.stack, prefixGiven: -1);
        //    }
        //}
    }
    */

    #region IO
    public override void SaveData(TagCompound tag) {
        TrySaveItem("Head", armor[Slot_Head]);
        TrySaveItem("Body", armor[Slot_Body]);
        TrySaveItem("Legs", armor[Slot_Legs]);
        TrySaveItem("Acc", armor[Slot_Accs]);
        TrySaveItem("Weapon", weapon);

        void TrySaveItem(string name, Item item) {
            if (item != null && !item.IsAir) {
                tag[name] = item;
            }
        }
    }

    public override void LoadData(TagCompound tag) {
        TryLoadItem("Head", ref armor[Slot_Head]);
        TryLoadItem("Body", ref armor[Slot_Body]);
        TryLoadItem("Legs", ref armor[Slot_Legs]);
        TryLoadItem("Acc", ref armor[Slot_Accs]);
        TryLoadItem("Weapon", ref weapon);

        void TryLoadItem(string name, ref Item item) {
            if (tag.TryGet<Item>(name, out var loadedItem)) {
                item = loadedItem;
            }
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
    #endregion
}
