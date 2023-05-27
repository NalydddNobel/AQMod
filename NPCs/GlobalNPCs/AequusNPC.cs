using Aequus.Buffs.Debuffs;
using Aequus.Buffs.Necro;
using Aequus.Common;
using Aequus.Common.Preferences;
using Aequus.Content.Necromancy;
using Aequus.Items;
using Aequus.Items.Potions;
using Aequus.NPCs.GlobalNPCs;
using Aequus.NPCs.Monsters.Sky.GaleStreams;
using Aequus.Particles;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.NPCs {
    public partial class AequusNPC : GlobalNPC, IPostSetupContent, IAddRecipes {
        public static float spawnNPCYOffset;

        public static HashSet<int> HeatDamage { get; private set; }

        public override bool InstancePerEntity => true;

        public int oldLife;

        public byte miscTimer;

        /// <summary>
        /// A flag determining whether or not this NPC inflicts "Heat Damage"
        /// <para>Used for the <see cref="FrostPotion"/>.</para>
        /// </summary>
        public bool heatDamage;

        /// <summary>
        /// Disables hit effects.
        /// <para>Used to make the Chomper minion look like it's swallowing the enemy it hit.</para>
        /// </summary>
        public bool noHitEffect;
        /// <summary>
        /// Disables contact damage.
        /// </summary>
        public bool noContactDamage;
        /// <summary>
        /// Disables on-kill effects like loot and player accessory effects.
        /// </summary>
        public bool noOnKill;
        /// <summary>
        /// Disables typical AI from running, only used for testing.
        /// </summary>
        public bool noAI;
        /// <summary>
        /// Effectively the same as <see cref="NPC.hide"/>, except refreshed to false in ResetEffects().
        /// <para>This is used on the slimes that are attached to <see cref="StreamingBalloon"/>, to make the appear hidden and properly be visable again after leaving the balloon.</para>
        /// </summary>
        public bool noVisible;
        /// <summary>
        /// Effectively the same as <see cref="NPC.dontTakeDamage"/>, except it ticks down before allowing the target to take damage again.
        /// <para>This is used on the Healer Drone to make town NPCs immortal for a couple seconds</para>
        /// </summary>
        public byte noTakingDamage;

        /// <summary>
        /// If the NPC came from a hereditary source, like being spawned from another NPC, or a projectile, or whatever else, this will be true.
        /// </summary>
        public bool isChildNPC;

        /// <summary>
        /// How long since this NPC has been striked.
        /// </summary>
        public uint lastHit;
        /// <summary>
        /// Flat life regen damage addition.
        /// </summary>
        public byte debuffDamage;

        /// <summary>
        /// Can be used to increase/decrease an NPC's damage without directly touching <see cref="NPC.damage"/>.
        /// </summary>
        public float statAttackDamage;

        public float dropRerolls;

        public bool frozen;

        public byte syncedTimer;

        public override void Load() {
            HeatDamage = new HashSet<int>();

            Load_Drops();
            LoadHooks();
        }

        public void PostSetupContent(Aequus mod) {
            var contentFile = new ContentArrayFile("HeatDamage", NPCID.Search);
            contentFile.AddToHashSet("NPCs", HeatDamage);
        }

        public void AddRecipes(Aequus aequus) {
            AddRecipes_PatchMimicLoot();
        }

        public override void Unload() {
            Unload_MimicEdits();
            Unload_Elites();
            HeatDamage?.Clear();
        }

        public override void SetDefaults(NPC npc) {
            if (HeatDamage.Contains(npc.type)) {
                heatDamage = true;
            }
            statAttackDamage = 1f;
            noAI = false;
            noContactDamage = false;
            noOnKill = false;
            noVisible = false;
            if (!Main.hardMode) {
                if (npc.type == NPCID.GreenJellyfish && GameplayConfig.Instance.EarlyGreenJellyfish) {
                    npc.damage /= 3;
                    npc.defense /= 3;
                }
                if (npc.type == NPCID.AnglerFish && GameplayConfig.Instance.EarlyAnglerFish) {
                    npc.lifeMax /= 2;
                    npc.defense /= 3;
                    npc.damage /= 2;
                }
                SetDefaults_PreHardmodeMimicEdits(npc);
            }
            ResetElitePrefixes();
        }

        public override void OnSpawn(NPC npc, IEntitySource source) {
            if (spawnNPCYOffset != 0f) {
                npc.position.Y += spawnNPCYOffset;
                var tileLocation = npc.Center.ToTileCoordinates();
                int y = Helper.FindFloor(tileLocation.X, tileLocation.Y);
                if (y != -1) {
                    npc.position.Y = y * 16f - npc.height;
                }
            }
            if (Helper.HereditarySource(source, out var ent)) {
                isChildNPC = true;
                if (ent is NPC parentNPC) {
                    friendship = parentNPC.Aequus().friendship;
                }
                NecromancyNPC.InheritFromParent(npc, ent);
            }
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot) {
            return !noContactDamage;
        }

        public override bool CanHitNPC(NPC npc, NPC target) {
            return !(friendship || target.Aequus().friendship || noContactDamage);
        }

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item) {
            return noTakingDamage > 0 ? false : null;
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile) {
            return noTakingDamage > 0 || (friendship && (projectile.IsMinionProj() || projectile.hostile)) ? false : null;
        }

        public override void ResetEffects(NPC npc) {
            ResetEffects_Meathook();
            ResetEffects_CheckSoulHealth(npc);
            debuffWindFan = false;
            debuffSnowgrave = false;
            debuffMindfungus = false;
            debuffLocust = false;
            debuffIronLotus = false;
            debuffCrimsonFire = false;
            debuffCorruptionFire = false;
            debuffBoneRing = false;
            debuffBoneHelmEmpowered = false;
            debuffBlueFire = false;
            debuffBattleAxe = false;
            debuffAetherFire = false;
            frozen = false;
            lagDebuff = 0;
            dropRerolls = 0f;
            noContactDamage = false;
            statAttackDamage = 1f;
            if (noTakingDamage > 0)
                noTakingDamage--;
        }

        public override Color? GetAlpha(NPC npc, Color drawColor) {
            if (lagDebuff > 0) {
                return Color.White;
            }
            return null;
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            if (noVisible) {
                noVisible = false;
                return false;
            }
            if (!npc.IsABestiaryIconDummy) {
                PreDraw_Elites(npc, spriteBatch, screenPos, drawColor);
                PreDraw_Gamestar(npc, screenPos);
            }
            return PreDraw_MimicEdits(npc, spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            if (!npc.IsABestiaryIconDummy) {
                PostDraw_Elites(npc, spriteBatch, screenPos, drawColor);
            }
        }

        public override bool PreAI(NPC npc) {
            if (friendship) {
                npc.StatSpeed() *= 0.9f;
            }
            if (npc.type == NPCID.Probe) {
                npc.buffImmune[ModContent.BuffType<OsirisDebuff>()] = false;
                npc.buffImmune[ModContent.BuffType<InsurgentDebuff>()] = false;
            }
            bool output = noAI;
            output |= !PreAI_Elites(npc);
            return !output;
        }
        private void DebuffEffects(NPC npc) {
            DebuffEffects_Snowgrave(npc);
            DebuffEffect_IronLotus(npc);
            DebuffEffect_CrimsonHellfire(npc);
            DebuffEffect_CorruptionHellfire(npc);
            DebuffEffects_BoneRing(npc);
            DebuffEffect_BoneHelmEmpowered(npc);
            DebuffEffect_BlueFire(npc);
            DebuffEffect_BattleAxeDebuff(npc);
            DebuffEffect_AethersWrath(npc);
        }

        public override void PostAI(NPC npc) {
            oldLife = npc.life;

            if (npc.lifeRegen + debuffDamage >= 0) {
                debuffDamage = 0;
            }
            PostAI_IronLotusSpread(npc);
            PostAI_UpdateFriendship(npc);
            PostAI_Elites(npc);
            if (Main.netMode == NetmodeID.Server) {
                return;
            }
            DebuffEffects(npc);
        }

        public override void DrawEffects(NPC npc, ref Color drawColor) {
            DrawEffects_Snowgrave(npc, ref drawColor);
            DrawEffects_BoneRingDebuff(npc, ref drawColor);
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage) {
            LifeRegenModifiers modifiers = new();
            UpdateLifeRegen_WindFanDebuff(npc, ref modifiers);
            UpdateLifeRegen_Mindfungus(npc, this, ref modifiers);
            UpdateLifeRegen_Locusts(npc, this, ref modifiers);
            UpdateLifeRegen_IronLotus(npc, ref modifiers);
            UpdateLifeRegen_CrimsonHellfire(npc, this, ref modifiers);
            UpdateLifeRegen_CorruptionHellfire(npc, this, ref modifiers);
            UpdateLifeRegen_BoneHelmEmpowered(npc, ref modifiers);
            UpdateLifeRegen_BlueFire(npc, ref modifiers);
            UpdateLifeRegen_BattleAxeDebuff(npc, ref modifiers);
            UpdateLifeRegen_AethersWrath(npc, ref modifiers);

            if (npc.oiled && modifiers.ApplyOiled && !(npc.onFire || npc.onFire2 || npc.onFire3 || npc.onFrostBurn || npc.onFrostBurn2 || npc.shadowFlame)) {
                modifiers.LifeRegen -= 50;
                damage = Math.Max(damage, 10);
            }

            if (modifiers.LifeRegen < 0 && npc.lifeRegen > 0) {
                npc.lifeRegen = 0;
            }
            npc.lifeRegen += modifiers.LifeRegen;
            damage = Math.Max(modifiers.DamageNumber, damage);

            if (debuffDamage > 0) {
                npc.lifeRegen -= debuffDamage;
                if (damage < 1)
                    damage = 1;
                damage += debuffDamage / 2;
            }
        }

        private void ModifyHit(NPC npc, Player player, ref NPC.HitModifiers modifiers) {
            ModifyHit_ProcMeathook(npc, ref modifiers);
        }
        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers) {
            ModifyHit(npc, player, ref modifiers);
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers) {
            ModifyHit(npc, Main.player[projectile.owner], ref modifiers);
        }

        private void OnHitBy(NPC npc, Player player, NPC.HitInfo hit) {
            OnHit_PlayMeathookSound(npc);
        }
        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone) {
            OnHitBy(npc, player, hit);
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) {
            OnHitBy(npc, Main.player[projectile.owner], hit);
        }

        public override bool SpecialOnKill(NPC npc) {
            if (noOnKill
                || (npc.TryGetGlobalNPC<NecromancyNPC>(out var zombie) && zombie.isZombie)) {
                return true;
            }

            if (Main.netMode != NetmodeID.Server) {
                try {
                    if (npc.HasBuff<SnowgraveDebuff>() && SnowgraveCorpse.CanFreezeNPC(npc)) {
                        ParticleSystem.New<SnowgraveCorpse>(ParticleLayer.BehindAllNPCs).Setup(npc.Center, npc);
                    }
                }
                catch {

                }
            }
            return false;
        }

        public override void OnKill(NPC npc) {
            if (npc.SpawnedFromStatue || npc.friendly || npc.lifeMax < 5)
                return;

            if (npc.TryGetGlobalNPC<NecromancyNPC>(out var zombie)) {
                zombie.CreateGhost(npc);
            }

            for (int i = 0; i < Main.maxPlayers; i++) {
                if (npc.playerInteraction[i]) {
                    var info = new EnemyKillInfo(npc);
                    if (Main.netMode == NetmodeID.SinglePlayer) {
                        Main.player[i].Aequus().OnKillEffect(info);
                        continue;
                    }

                    var p = Aequus.GetPacket(PacketType.OnKillEffect);
                    p.Write(i);
                    info.WriteData(p);
                    p.Send(toClient: i);
                }
            }
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers) {
            lastHit = 0;
        }

        public override bool PreChatButtonClicked(NPC npc, bool firstButton) {
            if (npc.type == NPCID.Angler) {
                if (firstButton) {
                    var inv = Main.LocalPlayer.inventory;
                    for (int i = 0; i < Main.InventoryItemSlotsCount; i++) {
                        if (!inv[i].IsAir && AequusItem.LegendaryFishIDs.Contains(inv[i].type)) {
                            if (Main.npcChatCornerItem != inv[i].type) {
                                Main.npcChatCornerItem = inv[i].type;
                                Main.npcChatText = TextHelper.GetTextValue("Chat.Angler.LegendaryFish");
                                return false;
                            }
                            Main.npcChatCornerItem = 0;
                            Main.npcChatText = TextHelper.GetTextValue("Chat.Angler.LegendaryFishReward");
                            Main.LocalPlayer.Aequus().LegendaryFishRewards(npc, inv[i], i);
                            inv[i].stack--;
                            if (inv[i].stack <= 0) {
                                inv[i].TurnToAir();
                            }
                            SoundEngine.PlaySound(SoundID.Grab);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
            binaryWriter.Write(miscTimer);
            binaryWriter.Write(lastHit);
            binaryWriter.Write(syncedTimer);

            var bb = new BitsByte(locustStacks > 0, corruptionHellfireStacks > 0, crimsonHellfireStacks > 0, mindfungusStacks > 0, friendship, isChildNPC, noTakingDamage > 0, debuffDamage > 0);
            binaryWriter.Write(bb);
            if (bb[0]) {
                binaryWriter.Write(locustStacks);
            }
            if (bb[1]) {
                binaryWriter.Write(corruptionHellfireStacks);
            }
            if (bb[2]) {
                binaryWriter.Write(crimsonHellfireStacks);
            }
            if (bb[3]) {
                binaryWriter.Write(mindfungusStacks);
            }
            if (bb[6]) {
                binaryWriter.Write(noTakingDamage);
            }
            if (bb[7]) {
                binaryWriter.Write(debuffDamage);
            }
            SendExtraAI_Elites(npc, bitWriter, binaryWriter);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
            miscTimer = binaryReader.ReadByte();
            lastHit = binaryReader.ReadUInt32();
            syncedTimer = binaryReader.ReadByte();
            var bb = (BitsByte)binaryReader.ReadByte();
            if (bb[0]) {
                locustStacks = binaryReader.ReadByte();
            }
            if (bb[1]) {
                corruptionHellfireStacks = binaryReader.ReadByte();
            }
            if (bb[2]) {
                crimsonHellfireStacks = binaryReader.ReadByte();
            }
            if (bb[3]) {
                mindfungusStacks = binaryReader.ReadByte();
            }
            friendship = bb[4];
            isChildNPC = bb[5];
            if (bb[6]) {
                noTakingDamage = binaryReader.ReadByte();
            }
            if (bb[7]) {
                debuffDamage = binaryReader.ReadByte();
            }
            ReceiveExtraAI_Elites(npc, bitReader, binaryReader);
        }

        private struct LifeRegenModifiers {
            private bool applyOiled;
            private int damageNumber;

            public int LifeRegen { get; set; }
            public int DamageNumber { get => damageNumber; set => damageNumber = Math.Max(damageNumber, value); }
            public bool ApplyOiled { get => applyOiled; set => applyOiled |= value; }

            public LifeRegenModifiers() {
                damageNumber = -1;
                applyOiled = false;
                LifeRegen = 0;
            }
        }

        #region Hooks
        private static void LoadHooks() {
            On_Chest.SetupShop_int += On_Chest_SetupShop_int;
            On_NPC.Transform += NPC_Transform;
            On_NPC.UpdateNPC_Inner += NPC_UpdateNPC_Inner; // fsr detouring NPC.Update(int) doesn't work, but this does
            On_NPC.HitEffect_HitInfo += On_NPC_HitEffect_HitInfo;
            On_NPC.SpawnNPC += NPC_SpawnNPC;
        }

        private static void On_NPC_HitEffect_HitInfo(On_NPC.orig_HitEffect_HitInfo orig, NPC self, NPC.HitInfo hit) {
            try {
                if (self.TryGetGlobalNPC<AequusNPC>(out var aequus)) {
                    if (aequus.noHitEffect) {
                        return;
                    }
                }

                if (Main.netMode == NetmodeID.Server) {
                    goto Orig;
                }

                if (self.HasBuff<BattleAxeBleeding>()) {
                    int amt = (int)Math.Min(4.0 + hit.Damage / 20.0, 20.0);
                    for (int i = 0; i < amt; i++) {
                        bool foodParticle = Main.rand.NextBool();
                        var d = Dust.NewDustDirect(self.position, self.width, self.height, foodParticle ? DustID.Blood : DustID.FoodPiece, newColor: foodParticle ? new Color(200, 20, 30, 100) : default);
                        d.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 5f);
                        d.velocity += self.velocity * 0.5f;
                        if (Main.rand.NextBool(3)) {
                            d.noGravity = true;
                        }
                    }
                }

                if (self.life <= 0 && self.HasBuff<SnowgraveDebuff>()
                    && SnowgraveCorpse.CanFreezeNPC(self)) {
                    SoundEngine.PlaySound(SoundID.Item30, self.Center);
                    return;
                }
            }
            catch {
            }
        Orig:
            orig(self, hit);
        }

        private static void On_Chest_SetupShop_int(On_Chest.orig_SetupShop_int orig, Chest self, int type) {
            bool hardMode = Main.hardMode;
            Main.hardMode |= Aequus.HardmodeTier;

            try {
                orig(self, type);
            }
            catch {

            }

            Main.hardMode = hardMode;
        }

        private static void NPC_SpawnNPC(On_NPC.orig_SpawnNPC orig) {
            SpawnsManagerSystem.PreCheckCreatureSpawns();
            try {
                orig();
            }
            catch {
            }
            SpawnsManagerSystem.PostCheckCreatureSpawns();
        }

        private static void NPC_Transform(On_NPC.orig_Transform orig, NPC npc, int newType) {
            string nameTag = null;
            if (npc.TryGetGlobalNPC<NPCNameTag>(out var nameTagNPC)) {
                nameTag = nameTagNPC.NameTag;
                switch (npc.type) {
                    case NPCID.Bunny:
                    case NPCID.BunnySlimed:
                    case NPCID.BunnyXmas:
                    case NPCID.ExplosiveBunny:
                        if (nameTagNPC.HasNameTag && nameTagNPC.NameTag.ToLower() == "toast") {
                            return;
                        }
                        break;
                }
            }

            var info = GhostSyncInfo.GetInfo(npc);

            orig(npc, newType);

            if (info.IsZombie) {
                info.SetZombieNPCInfo(npc, npc.GetGlobalNPC<NecromancyNPC>());
            }
            if (npc.TryGetGlobalNPC(out nameTagNPC)) {
                nameTagNPC.NameTag = nameTag;
            }
        }

        private static void UpdateNPC_Frozen(NPC npc, bool lowerBuffTime = true) {
            npc.lifeRegen = 0;
            npc.lifeRegenExpectedLossPerSecond = -1;
            NPCLoader.ResetEffects(npc);
            npc.UpdateNPC_BuffSetFlags(lowerBuffTime: lowerBuffTime);
        }
        private static void NPC_UpdateNPC_Inner(On_NPC.orig_UpdateNPC_Inner orig, NPC npc, int i) {
            if (!npc.TryGetGlobalNPC<AequusNPC>(out var aequus)) {
                goto Origin;
            }

            aequus.miscTimer++;

            if (aequus.frozen) {
                aequus.frozen = false;
                UpdateNPC_Frozen(npc, lowerBuffTime: true);
                return;
            }

            if (Helper.iterations == 0 && aequus.lagDebuff > 0) {
                aequus.UpdateNPC_Gamestar(npc, i);
                return;
            }

            if (Helper.iterations == 0 && npc.TryGetGlobalNPC<NecromancyNPC>(out var zombie) && zombie.isZombie) {
                var aequusPlayer = Main.player[zombie.zombieOwner].Aequus();
                if (aequusPlayer.ghostShadowDash > 0) {
                    if (zombie.shadowDashTimer > 240 - aequusPlayer.ghostShadowDash * 10) {
                        Helper.iterations++;
                        for (int k = 0; k < 3; k++) {
                            for (int l = 0; l < 2; l++) {
                                var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Smoke, Alpha: 100, newColor: Color.Black, Scale: Main.rand.NextFloat(1f, 1.5f));
                                d.velocity *= 0.25f;
                                d.velocity -= npc.velocity * 0.25f;
                            }
                            if (npc.velocity.Length() < 3f)
                                npc.velocity = Vector2.Normalize(npc.velocity).UnNaN() * 3f;
                            var v = npc.velocity;
                            orig(npc, i);
                            npc.velocity = Vector2.Lerp(v, npc.velocity, 0.2f);
                            if (Vector2.Distance(npc.oldPosition, npc.position).UnNaN() < 1f) {
                                break;
                            }
                        }
                        Helper.iterations = 0;
                    }
                }
                else {
                    zombie.shadowDashTimer = 0;
                }
            }

        Origin:
            orig(npc, i);
        }
        #endregion
    }
}