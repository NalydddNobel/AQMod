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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.NPCs
{
    public partial class AequusNPC : GlobalNPC, IPostSetupContent, IAddRecipes
    {
        public static float spawnNPCYOffset;

        public static HashSet<int> HeatDamage { get; private set; }

        public override bool InstancePerEntity => true;

        public int oldLife;

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

        public byte mindfungusStacks;
        public byte corruptionHellfireStacks;
        public byte crimsonHellfireStacks;
        public byte locustStacks;
        public byte syncedTickUpdate;

        public override void Load()
        {
            HeatDamage = new HashSet<int>();

            Load_Drops();
            LoadHooks();
        }

        public void PostSetupContent(Aequus mod)
        {
            var contentFile = new ContentArrayFile("HeatDamage", NPCID.Search);
            contentFile.AddToHashSet("NPCs", HeatDamage);
        }

        public void AddRecipes(Aequus aequus)
        {
            AddRecipes_PatchMimicLoot();
        }

        public override void Unload()
        {
            Unload_MimicEdits();
            Unload_Elites();
            HeatDamage?.Clear();
        }

        public override void SetDefaults(NPC npc)
        {
            if (HeatDamage.Contains(npc.type))
            {
                heatDamage = true;
            }
            statAttackDamage = 1f;
            noAI = false;
            noContactDamage = false;
            noOnKill = false;
            noVisible = false;
            if (!Main.hardMode) {
                if (npc.type == NPCID.GreenJellyfish && GameplayConfig.Instance.EarlyGreenJellyfish) {
                    npc.defense /= 3;
                }
                if (npc.type == NPCID.AnglerFish && GameplayConfig.Instance.EarlyAnglerFish) {
                    npc.lifeMax /= 2;
                    npc.defense /= 3;
                }
                SetDefaults_PreHardmodeMimicEdits(npc);
            }
            ResetElitePrefixes();
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (spawnNPCYOffset != 0f)
            {
                npc.position.Y += spawnNPCYOffset;
                var tileLocation = npc.Center.ToTileCoordinates();
                int y = Helper.FindFloor(tileLocation.X, tileLocation.Y);
                if (y != -1) {
                    npc.position.Y = y * 16f - npc.height;
                }
            }
            if (Helper.HereditarySource(source, out var ent))
            {
                isChildNPC = true;
                if (ent is NPC parentNPC) {
                    friendship = parentNPC.Aequus().friendship;
                }
                NecromancyNPC.InheritFromParent(npc, ent);
            }
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            return !noContactDamage;
        }

        public override bool CanHitNPC(NPC npc, NPC target)
        {
            return !(friendship || target.Aequus().friendship || noContactDamage);
        }

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            return noTakingDamage > 0 ? false : null;
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            return noTakingDamage > 0 || (friendship && (projectile.IsMinionProj() || projectile.hostile)) ? false : null;
        }

        public override void ResetEffects(NPC npc)
        {
            ResetEffects_Meathook();
            dropRerolls = 0f;
            noContactDamage = false;
            statAttackDamage = 1f;
            if (noTakingDamage > 0)
                noTakingDamage--;
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (noVisible)
            {
                noVisible = false;
                return false;
            }
            if (!npc.IsABestiaryIconDummy)
            {
                PreDraw_Elites(npc, spriteBatch, screenPos, drawColor);
            }
            return PreDraw_MimicEdits(npc, spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            if (!npc.IsABestiaryIconDummy) {
                PostDraw_Elites(npc, spriteBatch, screenPos, drawColor);
            }
        }

        public override bool PreAI(NPC npc)
        {
            if (friendship) {
                npc.StatSpeed() *= 0.9f;
            }
            if (npc.type == NPCID.Probe)
            {
                npc.buffImmune[ModContent.BuffType<OsirisDebuff>()] = false;
                npc.buffImmune[ModContent.BuffType<InsurgentDebuff>()] = false;
            }
            bool output = noAI;
            output |= !PreAI_Elites(npc);
            return !output;
        }
        public void DebuffEffects(NPC npc)
        {
            if (npc.HasBuff<IronLotusDebuff>())
            {
                int amt = (int)(npc.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                {
                    ParticleSystem.New<BloomParticle>(ParticleLayer.BehindPlayers).Setup(Main.rand.NextCircularFromRect(npc.getRect()) + Main.rand.NextVector2Unit() * 8f, -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        new Color(180, 90, 40, 60) * 0.5f, new Color(20, 2, 10, 10), Main.rand.NextFloat(1f, 2f), 0.2f, Main.rand.NextFloat(MathHelper.TwoPi));
                }
            }
            if (npc.HasBuff<AethersWrath>())
            {
                var colors = new Color[] { new Color(80, 180, 255, 10), new Color(255, 255, 255, 10), new Color(100, 255, 100, 10), };
                int amt = (int)(npc.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                {
                    var spawnLocation = Main.rand.NextCircularFromRect(npc.getRect()) + Main.rand.NextVector2Unit() * 8f;
                    spawnLocation.Y -= 4f;
                    var color = Helper.LerpBetween(colors, spawnLocation.X / 32f + Main.GlobalTimeWrappedHourly * 4f);
                    ParticleSystem.New<BloomParticle>(ParticleLayer.BehindPlayers).Setup(spawnLocation, -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        color, color.UseA(0).HueAdd(Main.rand.NextFloat(0.02f)) * 0.1f, Main.rand.NextFloat(1f, 2f), 0.2f, Main.rand.NextFloat(MathHelper.TwoPi));
                    if (i == 0 && Main.GameUpdateCount % 12 == 0)
                    {
                        var velocity = -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-6f, 6f), -Main.rand.NextFloat(4f, 7f));
                        float scale = Main.rand.NextFloat(1.2f, 2.2f);
                        float rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                        ParticleSystem.New<AethersWrathParticle>(ParticleLayer.BehindPlayers).Setup(spawnLocation, velocity, 10, color, scale * 1.2f, rotation);
                    }
                }
            }
            if (npc.HasBuff<BlueFire>())
            {
                int amt = (int)(npc.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                    ParticleSystem.New<BloomParticle>(ParticleLayer.BehindPlayers).Setup(Main.rand.NextCircularFromRect(npc.getRect()) + Main.rand.NextVector2Unit() * 8f, -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        new Color(60, 100, 160, 10) * 0.5f, new Color(5, 20, 40, 10), Main.rand.NextFloat(1f, 2f), 0.2f, Main.rand.NextFloat(MathHelper.TwoPi));
            }
            if (npc.HasBuff<CorruptionHellfire>())
            {
                int amt = (int)(npc.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                    ParticleSystem.New<BloomParticle>(ParticleLayer.BehindPlayers).Setup(Main.rand.NextCircularFromRect(npc.getRect()) + Main.rand.NextVector2Unit() * 8f, -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        CorruptionHellfire.FireColor, CorruptionHellfire.BloomColor * 0.6f, Main.rand.NextFloat(1f, 2f), 0.2f, Main.rand.NextFloat(MathHelper.TwoPi));
            }
            if (npc.HasBuff<CrimsonHellfire>())
            {
                int amt = (int)(npc.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                    ParticleSystem.New<BloomParticle>(ParticleLayer.BehindPlayers).Setup(Main.rand.NextCircularFromRect(npc.getRect()) + Main.rand.NextVector2Unit() * 8f, -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        CrimsonHellfire.FireColor, CrimsonHellfire.BloomColor * 0.2f, Main.rand.NextFloat(1f, 2f), 0.2f, Main.rand.NextFloat(MathHelper.TwoPi));
            }
        }

        public override void PostAI(NPC npc)
        {
            oldLife = npc.life;

            if (npc.lifeRegen + debuffDamage >= 0)
            {
                debuffDamage = 0;
            }
            int debuff = npc.FindBuffIndex(ModContent.BuffType<IronLotusDebuff>());
            if (debuff != -1)
            {
                int plr = Player.FindClosest(npc.position, npc.width, npc.height);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && (Main.npc[i].type == NPCID.TargetDummy || Main.npc[i].CanBeChasedBy(Main.player[plr])) && Main.npc[i].Distance(npc.Center) < 100f)
                    {
                        Main.npc[i].AddBuff(ModContent.BuffType<IronLotusDebuff>(), npc.buffTime[debuff]);
                    }
                }
            }
            PostAI_UpdateFriendship(npc);
            PostAI_Elites(npc);
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            DebuffEffects(npc);
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (npc.HasBuff<SnowgraveDebuff>())
            {
                if (Main.GameUpdateCount % 9 == 0)
                {
                    var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.IceRod, Scale: Main.rand.NextFloat(0.6f, 1f));
                    d.velocity = npc.velocity * 0.5f;
                    d.noGravity = true;
                    d.fadeIn = d.scale + 0.5f;
                }
            }
            if (npc.HasBuff<BoneRingWeakness>())
            {
                byte a = drawColor.A;
                drawColor = (drawColor * 0.8f).UseA(a);
                if (Main.GameUpdateCount % 10 == 0)
                {
                    var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Slush, Alpha: 160, Scale: Main.rand.NextFloat(0.6f, 1f));
                    d.velocity = npc.velocity.SafeNormalize(Vector2.Zero);
                    d.noGravity = true;
                    d.fadeIn = d.scale + 0.5f;
                }
                if (npc.life >= 0 && Main.GameUpdateCount % 30 == 0)
                {
                    npc.HitEffect(0, 10);
                }
                if (!npc.noTileCollide)
                {
                    int x = (int)((npc.position.X + npc.width / 2f) / 16f);
                    int checkTilesTop = (int)((npc.position.Y + npc.height) / 16f);
                    int checkTilesBottom = (int)(npc.position.Y / 16f);
                    for (int j = checkTilesBottom; j <= checkTilesTop; j++)
                    {
                        if (Main.tile[x, j].IsFullySolid())
                        {
                            var d = Dust.NewDustPerfect(new Vector2(npc.position.X + Main.rand.NextFloat(npc.width), npc.position.Y + npc.height), DustID.Slush, Vector2.Zero, 160, Scale: Main.rand.NextFloat(0.6f, 1f));
                            d.noGravity = true;
                            d.fadeIn = d.scale + 0.5f;
                            break;
                        }
                    }
                }
            }
            if (npc.HasBuff<BattleAxeBleeding>())
            {
                if (Main.GameUpdateCount % 10 == 0)
                {
                    bool foodParticle = Main.rand.NextBool();
                    var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, foodParticle ? DustID.Blood : DustID.FoodPiece, newColor: foodParticle ? new Color(200, 20, 30, 100) : default);
                    d.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f, 2f);
                    d.velocity += npc.velocity * 0.5f;
                }
                if (npc.life >= 0 && Main.GameUpdateCount % 50 == 0)
                {
                    npc.HitEffect(0, 10);
                }
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            bool applyAequusOiled = false;
            if (npc.HasBuff<IronLotusDebuff>())
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 100;
                applyAequusOiled = true;
                damage = Math.Max(damage, 5);
            }
            if (npc.HasBuff<AethersWrath>())
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 50;
                applyAequusOiled = true;
                damage = Math.Max(damage, 10);
            }
            if (npc.HasBuff<MindfungusDebuff>())
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 16 + 8 * mindfungusStacks;
                damage = Math.Max(damage, 5 + mindfungusStacks);
            }
            else
            {
                mindfungusStacks = 0;
            }
            if (npc.HasBuff<BlueFire>())
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 30;
                applyAequusOiled = true;
                damage = Math.Max(damage, 6);
            }
            if (npc.HasBuff<BattleAxeBleeding>())
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 16;
            }
            if (npc.HasBuff<WindFanDebuff>())
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 25;
                damage = Math.Max(damage, 5);
            }
            bool debuff = npc.HasBuff<CorruptionHellfire>();
            applyAequusOiled |= debuff;
            UpdateDebuffStack(npc, debuff, ref corruptionHellfireStacks, ref damage, 20, 1f);
            debuff = npc.HasBuff<CrimsonHellfire>();
            applyAequusOiled |= debuff;
            UpdateDebuffStack(npc, debuff, ref crimsonHellfireStacks, ref damage, 20, 1.1f);

            UpdateDebuffStack(npc, npc.HasBuff<LocustDebuff>(), ref locustStacks, ref damage, 20, 1f);

            if (npc.oiled && applyAequusOiled && !(npc.onFire || npc.onFire2 || npc.onFire3 || npc.onFrostBurn || npc.onFrostBurn2 || npc.shadowFlame))
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 50;
                damage = Math.Max(damage, 10);
            }

            if (debuffDamage > 0)
            {
                npc.lifeRegen -= debuffDamage;
                if (damage < 1)
                    damage = 1;
                damage += debuffDamage / 4;
            }
        }
        public void UpdateDebuffStack(NPC npc, bool has, ref byte stacks, ref int twoSecondsDamageNumbers, byte cap = 20, float dotMultiplier = 1f)
        {
            if (!has)
            {
                stacks = 0;
            }
            else
            {
                stacks = Math.Min(stacks, cap);
                int dot = (int)(stacks * dotMultiplier);

                if (dot >= 0)
                {
                    npc.AddRegen(-dot * 8);
                    twoSecondsDamageNumbers = Math.Max(twoSecondsDamageNumbers, (int)(dot * dotMultiplier));
                }
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

        public override bool SpecialOnKill(NPC npc)
        {
            if (noOnKill
                || (npc.TryGetGlobalNPC<NecromancyNPC>(out var zombie) && zombie.isZombie))
            {
                return true;
            }

            if (Main.netMode != NetmodeID.Server)
            {
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

        public override void OnKill(NPC npc)
        {
            if (npc.SpawnedFromStatue || npc.friendly || npc.lifeMax < 5)
                return;

            if (npc.TryGetGlobalNPC<NecromancyNPC>(out var zombie))
            {
                zombie.CreateGhost(npc);
            }

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (npc.playerInteraction[i])
                {
                    var info = new EnemyKillInfo(npc);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
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

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            lastHit = 0;
        }

        public override bool PreChatButtonClicked(NPC npc, bool firstButton)
        {
            if (npc.type == NPCID.Angler)
            {
                if (firstButton)
                {
                    var inv = Main.LocalPlayer.inventory;
                    for (int i = 0; i < Main.InventoryItemSlotsCount; i++)
                    {
                        if (!inv[i].IsAir && AequusItem.LegendaryFishIDs.Contains(inv[i].type))
                        {
                            if (Main.npcChatCornerItem != inv[i].type)
                            {
                                Main.npcChatCornerItem = inv[i].type;
                                Main.npcChatText = TextHelper.GetTextValue("Chat.Angler.LegendaryFish");
                                return false;
                            }
                            Main.npcChatCornerItem = 0;
                            Main.npcChatText = TextHelper.GetTextValue("Chat.Angler.LegendaryFishReward");
                            Main.LocalPlayer.Aequus().LegendaryFishRewards(npc, inv[i], i);
                            inv[i].stack--;
                            if (inv[i].stack <= 0)
                            {
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

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(lastHit);
            binaryWriter.Write(syncedTickUpdate);

            var bb = new BitsByte(locustStacks > 0, corruptionHellfireStacks > 0, crimsonHellfireStacks > 0, mindfungusStacks > 0, friendship, isChildNPC, noTakingDamage > 0, debuffDamage > 0);
            binaryWriter.Write(bb);
            if (bb[0])
            {
                binaryWriter.Write(locustStacks);
            }
            if (bb[1])
            {
                binaryWriter.Write(corruptionHellfireStacks);
            }
            if (bb[2])
            {
                binaryWriter.Write(crimsonHellfireStacks);
            }
            if (bb[3])
            {
                binaryWriter.Write(mindfungusStacks);
            }
            if (bb[6])
            {
                binaryWriter.Write(noTakingDamage);
            }
            if (bb[7])
            {
                binaryWriter.Write(debuffDamage);
            }
            SendExtraAI_Elites(npc, bitWriter, binaryWriter);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            lastHit = binaryReader.ReadUInt32();
            syncedTickUpdate = binaryReader.ReadByte();
            var bb = (BitsByte)binaryReader.ReadByte();
            if (bb[0])
            {
                locustStacks = binaryReader.ReadByte();
            }
            if (bb[1])
            {
                corruptionHellfireStacks = binaryReader.ReadByte();
            }
            if (bb[2])
            {
                crimsonHellfireStacks = binaryReader.ReadByte();
            }
            if (bb[3])
            {
                mindfungusStacks = binaryReader.ReadByte();
            }
            friendship = bb[4];
            isChildNPC = bb[5];
            if (bb[6])
            {
                noTakingDamage = binaryReader.ReadByte();
            }
            if (bb[7])
            {
                debuffDamage = binaryReader.ReadByte();
            }
            ReceiveExtraAI_Elites(npc, bitReader, binaryReader);
        }

        #region Hooks
        private static void LoadHooks()
        {
            Terraria.On_Chest.SetupShop_int += On_Chest_SetupShop_int;
            Terraria.On_NPC.Transform += NPC_Transform;
            Terraria.On_NPC.UpdateNPC_Inner += NPC_UpdateNPC_Inner; // fsr detouring NPC.Update(int) doesn't work, but this does
            Terraria.On_NPC.HitEffect_HitInfo += On_NPC_HitEffect_HitInfo;
            Terraria.On_NPC.SpawnNPC += NPC_SpawnNPC;
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

        private static void NPC_SpawnNPC(Terraria.On_NPC.orig_SpawnNPC orig)
        {
            SpawnsManagerSystem.PreCheckCreatureSpawns();
            try
            {
                orig();
            }
            catch
            {
            }
            SpawnsManagerSystem.PostCheckCreatureSpawns();
        }

        private static void NPC_Transform(Terraria.On_NPC.orig_Transform orig, NPC npc, int newType)
        {
            string nameTag = null;
            if (npc.TryGetGlobalNPC<NPCNameTag>(out var nameTagNPC))
            {
                nameTag = nameTagNPC.NameTag;
                switch (npc.type)
                {
                    case NPCID.Bunny:
                    case NPCID.BunnySlimed:
                    case NPCID.BunnyXmas:
                    case NPCID.ExplosiveBunny:
                        if (nameTagNPC.HasNameTag && nameTagNPC.NameTag.ToLower() == "toast")
                        {
                            return;
                        }
                        break;
                }
            }

            var info = GhostSyncInfo.GetInfo(npc);

            orig(npc, newType);

            if (info.IsZombie)
            {
                info.SetZombieNPCInfo(npc, npc.GetGlobalNPC<NecromancyNPC>());
            }
            if (npc.TryGetGlobalNPC(out nameTagNPC))
            {
                nameTagNPC.NameTag = nameTag;
            }
        }

        private static void NPC_UpdateNPC_Inner(Terraria.On_NPC.orig_UpdateNPC_Inner orig, NPC self, int i)
        {
            if (self.TryGetGlobalNPC<BitCrushedGlobalNPC>(out var bitCrushed) && !bitCrushed.CheckUpdateNPC(self, i))
                return;
            if (Helper.iterations == 0 && self.TryGetGlobalNPC<NecromancyNPC>(out var zombie) && zombie.isZombie)
            {
                var aequus = Main.player[zombie.zombieOwner].Aequus();
                if (aequus.ghostShadowDash > 0)
                {
                    if (zombie.shadowDashTimer > 240 - aequus.ghostShadowDash * 10)
                    {
                        Helper.iterations++;
                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 2; l++)
                            {
                                var d = Dust.NewDustDirect(self.position, self.width, self.height, DustID.Smoke, Alpha: 100, newColor: Color.Black, Scale: Main.rand.NextFloat(1f, 1.5f));
                                d.velocity *= 0.25f;
                                d.velocity -= self.velocity * 0.25f;
                            }
                            if (self.velocity.Length() < 3f)
                                self.velocity = Vector2.Normalize(self.velocity).UnNaN() * 3f;
                            var v = self.velocity;
                            orig(self, i);
                            self.velocity = Vector2.Lerp(v, self.velocity, 0.2f);
                            if (Vector2.Distance(self.oldPosition, self.position).UnNaN() < 1f)
                            {
                                break;
                            }
                        }
                        Helper.iterations = 0;
                    }
                }
                else
                {
                    zombie.shadowDashTimer = 0;
                }
            }
            orig(self, i);
        }

        private static void Hook_PreHitEffect(Terraria.On_NPC.orig_VanillaHitEffect orig, NPC self, int hitDirection, double dmg)
        {
        }
        #endregion
    }
}