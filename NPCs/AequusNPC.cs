using Aequus.Buffs.Debuffs;
using Aequus.Common.GlobalNPCs;
using Aequus.Common.ItemDrops;
using Aequus.Common.ModPlayers;
using Aequus.Content.Necromancy;
using Aequus.Graphics;
using Aequus.Items;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Vanity.Cursors;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Misc.Festive;
using Aequus.Items.Placeable;
using Aequus.Items.Weapons.Summon.Necro.Candles;
using Aequus.Particles;
using Aequus.Projectiles.Summon.Necro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.NPCs
{
    public class AequusNPC : GlobalNPC
    {
        public struct OnKillPlayerInfo
        {
            public Player player;
            public AequusPlayer aequus;
            public float distance;
        }

        public static FieldInfo NPC_waterMovementSpeed { get; private set; }
        public static FieldInfo NPC_lavaMovementSpeed { get; private set; }
        public static FieldInfo NPC_honeyMovementSpeed { get; private set; }

        public static HashSet<int> HeatDamage { get; private set; }

        public override bool InstancePerEntity => true;

        public bool heatDamage;
        public bool noHitEffect;
        public bool disabledContactDamage;

        public int oldLife;
        public byte mindfungusStacks;
        public byte corruptionHellfireStacks;
        public byte crimsonHellfireStacks;
        public byte locustStacks;

        public override void Load()
        {
            HeatDamage = new HashSet<int>()
            {
                NPCID.Lavabat,
                NPCID.LavaSlime,
                NPCID.FireImp,
                NPCID.MeteorHead,
                NPCID.HellArmoredBones,
                NPCID.HellArmoredBonesMace,
                NPCID.HellArmoredBonesSpikeShield,
                NPCID.HellArmoredBonesSword,
                NPCID.BlazingWheel,
            };
            NPC_waterMovementSpeed = typeof(NPC).GetField("waterMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            NPC_lavaMovementSpeed = typeof(NPC).GetField("lavaMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            NPC_honeyMovementSpeed = typeof(NPC).GetField("honeyMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance);

            AddHooks();
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                case NPCID.Scarecrow1:
                case NPCID.Scarecrow2:
                case NPCID.Scarecrow3:
                case NPCID.Scarecrow4:
                case NPCID.Scarecrow5:
                case NPCID.Scarecrow6:
                case NPCID.Scarecrow7:
                case NPCID.Scarecrow8:
                case NPCID.Scarecrow9:
                case NPCID.Scarecrow10:
                    npcLoot.Add(ItemDropRule.ByCondition(new IsHalloweenCondition(), ModContent.ItemType<HalloweenEnergy>()));
                    npcLoot.Add(new NameTagDropRule(new ItemDrop(ItemID.Ale, 1), "Press B.", new NameTagCondition("birdy", "beardy")));
                    break;

                case NPCID.Bunny:
                case NPCID.ExplosiveBunny:
                case NPCID.BunnyXmas:
                case NPCID.BunnySlimed:
                    npcLoot.Add(new NameTagDropRule(new ItemDrop(ModContent.ItemType<RabbitsFoot>(), 1), "You're a Monster.", new NameTagCondition("toast")));
                    break;

                case NPCID.Moth:
                case NPCID.Mothron:
                case NPCID.MothronSpawn:
                    npcLoot.Add(ItemDropRule.ByCondition(new NameTagCondition("cata", "cataclysmic", "armageddon", "cataclysmicarmageddon", "cataclysmic armageddon"), ModContent.ItemType<MothmanMask>()));
                    break;

                case NPCID.Unicorn:
                    npcLoot.Add(new NameTagDropRule(new ItemDrop(ModContent.ItemType<RabbitsFoot>(), 1), "Tattered Pegasus Wings", new NameTagCondition("pegasus")));
                    break;

                case NPCID.Crab:
                    npcLoot.Add(new NameTagDropRule(new ItemDrop(ItemID.GoldCoin, 1), "Me first dollar!", new NameTagCondition("mr krabs", "krabs", "mr. krabs")));
                    break;

                case NPCID.Pumpking:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.FromCertainWaveAndAbove(15), ModContent.ItemType<HalloweenEnergy>()));
                    npcLoot.Add(ItemDropRule.ByCondition(new IsHalloweenCondition(), ModContent.ItemType<HalloweenEnergy>()));
                    break;
                case NPCID.MourningWood:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.FromCertainWaveAndAbove(15), ModContent.ItemType<HalloweenEnergy>(), 5));
                    npcLoot.Add(ItemDropRule.ByCondition(new IsHalloweenCondition(), ModContent.ItemType<HalloweenEnergy>()));
                    break;

                case NPCID.SlimeMasked:
                case NPCID.ZombieDoctor:
                case NPCID.ZombieSuperman:
                case NPCID.ZombiePixie:
                case NPCID.DemonEyeOwl:
                case NPCID.DemonEyeSpaceship:
                case NPCID.Raven:
                case NPCID.SkeletonTopHat:
                case NPCID.SkeletonAstonaut:
                case NPCID.SkeletonAlien:
                case NPCID.Ghost:
                case NPCID.HoppinJack:
                case NPCID.Splinterling:
                case NPCID.Hellhound:
                case NPCID.HeadlessHorseman:
                case NPCID.Poltergeist:
                    npcLoot.Add(ItemDropRule.ByCondition(new IsHalloweenCondition(), ModContent.ItemType<HalloweenEnergy>()));
                    break;

                case NPCID.Demon:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DemonCursor>(), 100));
                    break;

                case NPCID.QueenBee:
                    npcLoot.Add(ItemDropRule.ByCondition(DropRulesBuilder.NotExpertCondition, ModContent.ItemType<OrganicEnergy>(), 1, 3, 3));
                    break;

                case NPCID.Pixie:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PixieCandle>(), 100));
                    break;

                case NPCID.BloodZombie:
                case NPCID.Drippler:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodMoonCandle>(), 100));
                    break;

                case NPCID.DevourerHead:
                case NPCID.GiantWormHead:
                case NPCID.BoneSerpentHead:
                case NPCID.TombCrawlerHead:
                case NPCID.DiggerHead:
                case NPCID.DuneSplicerHead:
                case NPCID.SeekerHead:
                case NPCID.BloodEelHead:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpicyEel>(), 25));
                    break;

                case NPCID.CultistBoss:
                    npcLoot.Add(ItemDropRule.ByCondition(DropRulesBuilder.FlawlessCondition, ModContent.ItemType<MothmanMask>()));
                    break;

                case NPCID.WallofFlesh:
                    npcLoot.Add(ItemDropRule.ByCondition(new FuncConditional(() => !AequusWorld.downedEventDemon, "DemonSiege", "Mods.Aequus.DropCondition.NotBeatenDemonSiege"), ModContent.ItemType<GoreNest>()));
                    break;
            }
        }

        public override void SetDefaults(NPC npc)
        {
            if (HeatDamage.Contains(npc.type))
            {
                heatDamage = true;
            }
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            return !disabledContactDamage;
        }

        public override bool? CanHitNPC(NPC npc, NPC target)
        {
            return disabledContactDamage ? false : null;
        }

        public override void ResetEffects(NPC npc)
        {
            disabledContactDamage = false;
            if (npc.TryGetGlobalNPC<NPCNameTag>(out var nameTag) && nameTag.HasNameTag)
            {
            }
        }

        public void PostAI_VelocityBoostHack(NPC npc)
        {
        }
        public void PostAI_DoDebuffEffects(NPC npc)
        {
            if (npc.HasBuff<AethersWrath>())
            {
                var colors = new Color[] { new Color(80, 180, 255, 10), new Color(255, 255, 255, 10), new Color(100, 255, 100, 10), };
                int amt = (int)(npc.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                {
                    var spawnLocation = Main.rand.NextCircularFromRect(npc.getRect()) + Main.rand.NextVector2Unit() * 8f;
                    spawnLocation.Y -= 4f;
                    var color = AequusHelpers.LerpBetween(colors, spawnLocation.X / 32f + Main.GlobalTimeWrappedHourly * 4f);
                    EffectsSystem.BehindPlayers.Add(new BloomParticle(spawnLocation, -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        color, color.UseA(0).HueAdd(Main.rand.NextFloat(0.02f)) * 0.1f, Main.rand.NextFloat(1f, 2f), 0.2f, Main.rand.NextFloat(MathHelper.TwoPi)));
                    if (Main.rand.NextBool(12))
                    {
                        var velocity = -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-3f, 3f), -Main.rand.NextFloat(4f, 7f));
                        float scale = Main.rand.NextFloat(1f, 2f);
                        float rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                        EffectsSystem.BehindPlayers.Add(new AethersWrathParticle(spawnLocation, velocity, color, scale, rotation));
                    }
                }
            }
            if (npc.HasBuff<BlueFire>())
            {
                int amt = (int)(npc.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                    EffectsSystem.BehindPlayers.Add(new BloomParticle(Main.rand.NextCircularFromRect(npc.getRect()) + Main.rand.NextVector2Unit() * 8f, -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        new Color(60, 100, 160, 10) * 0.5f, new Color(5, 20, 40, 10), Main.rand.NextFloat(1f, 2f), 0.2f, Main.rand.NextFloat(MathHelper.TwoPi)));
            }
            if (npc.HasBuff<CorruptionHellfire>())
            {
                int amt = (int)(npc.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                    EffectsSystem.BehindPlayers.Add(new BloomParticle(Main.rand.NextCircularFromRect(npc.getRect()) + Main.rand.NextVector2Unit() * 8f, -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        CorruptionHellfire.FireColor, CorruptionHellfire.BloomColor * 0.6f, Main.rand.NextFloat(1f, 2f), 0.2f, Main.rand.NextFloat(MathHelper.TwoPi)));
            }
            if (npc.HasBuff<CrimsonHellfire>())
            {
                int amt = (int)(npc.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                    EffectsSystem.BehindPlayers.Add(new BloomParticle(Main.rand.NextCircularFromRect(npc.getRect()) + Main.rand.NextVector2Unit() * 8f, -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        CrimsonHellfire.FireColor, CrimsonHellfire.BloomColor * 0.2f, Main.rand.NextFloat(1f, 2f), 0.2f, Main.rand.NextFloat(MathHelper.TwoPi)));
            }
        }
        public override void PostAI(NPC npc)
        {
            oldLife = npc.life;

            PostAI_VelocityBoostHack(npc);

            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            PostAI_DoDebuffEffects(npc);
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (npc.HasBuff<Weakness>())
            {
                byte a = drawColor.A;
                drawColor = (drawColor * 0.9f).UseA(a);
                if (npc.life >= 0 && Main.rand.NextBool(20))
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
            if (npc.life >= 0 && npc.HasBuff<Bleeding>())
            {
                if (Main.rand.NextBool(3))
                {
                    if (Main.rand.NextBool(5))
                        npc.HitEffect(0, 4);
                    bool foodParticle = Main.rand.NextBool();
                    var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, foodParticle ? DustID.Blood : DustID.FoodPiece, newColor: foodParticle ? new Color(200, 20, 30, 100) : default);
                    d.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f, 2f);
                    d.velocity += npc.velocity * 0.5f;
                }
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.HasBuff<AethersWrath>())
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 50;
                damage += 5;
            }
            if (npc.HasBuff<MindfungusDebuff>())
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 16 + 8 * mindfungusStacks;
                damage += 9;
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
                damage += 3;
            }
            if (npc.HasBuff<Bleeding>())
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 8;
                damage++;
            }
            UpdateDebuffStack(npc, npc.HasBuff<CorruptionHellfire>(), ref corruptionHellfireStacks, ref damage, 20, 1f);
            UpdateDebuffStack(npc, npc.HasBuff<CrimsonHellfire>(), ref crimsonHellfireStacks, ref damage, 20, 1.1f);
            UpdateDebuffStack(npc, npc.HasBuff<LocustDebuff>(), ref locustStacks, ref damage, 20, 1f);
        }
        public void UpdateDebuffStack(NPC npc, bool has, ref byte stacks, ref int damageNumbers, byte cap = 20, float dotMultiplier = 1f)
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
                    npc.AddRegen(-dot);
                    if (damageNumbers < dot)
                        damageNumbers = dot;
                }
            }
        }

        public override bool SpecialOnKill(NPC npc)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient
                || npc.SpawnedFromStatue || NPCID.Sets.BelongsToInvasionOldOnesArmy[npc.type])
            {
                return false;
            }

            if (Main.netMode != NetmodeID.Server && npc.HasBuff<SnowgraveDebuff>())
            {
                DeathEffect_SnowgraveFreeze(npc);
            }

            var players = GetCloseEnoughPlayers(npc);

            if (npc.realLife == -1 || npc.realLife == npc.whoAmI)
            {
                if (npc.HasBuff<SoulStolen>())
                {
                    CheckSouls(npc, players);
                }
                var info = NecromancyDatabase.TryGet(npc, out var g) ? g : default(GhostInfo);
                var zombie = npc.GetGlobalNPC<NecromancyNPC>();
                if ((info.PowerNeeded != 0f || zombie.zombieDebuffTier >= 100f) && GhostKill(npc, zombie, info, players))
                {
                    zombie.SpawnZombie(npc);
                }
                int ammoBackpackChance = (int)Math.Max(10f - npc.value / 1000f, 1f);
                foreach (var tuple in players)
                {
                    if (!npc.playerInteraction[tuple.player.whoAmI])
                    {
                        continue;
                    }
                    if (npc.value > (Item.copper * 20) && tuple.aequus.accAmmoRenewalPack != null && (ammoBackpackChance <= 1 || Main.rand.NextBool(ammoBackpackChance)))
                    {
                        int stacks = tuple.aequus.accAmmoRenewalPack.Aequus().accStacks;
                        for (int i = 0; i < stacks; i++)
                        {
                            AmmoBackpack.DropAmmo(tuple.player, npc, tuple.aequus.accAmmoRenewalPack);
                        }
                    }
                }
            }
            return false;
        }
        public void CheckSouls(NPC npc, List<OnKillPlayerInfo> players)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                foreach (var p in players)
                {
                    if (p.aequus.candleSouls < p.aequus.heldSoulCandle)
                    {
                        Projectile.NewProjectile(npc.GetSource_Death(), npc.Center, Main.rand.NextVector2Unit() * 1.5f, ModContent.ProjectileType<SoulAbsorbProj>(), 0, 0f, p.player.whoAmI);
                        p.aequus.candleSouls++;
                    }
                }
            }
            else
            {
                var candlePlayers = new List<int>();
                foreach (var p in players)
                {
                    if (p.aequus.candleSouls < p.aequus.heldSoulCandle)
                    {
                        candlePlayers.Add(p.player.whoAmI);
                    }
                }

                if (candlePlayers.Count > 0)
                {
                    PacketSystem.Send((p) =>
                    {
                        p.Write(candlePlayers.Count);
                        p.WriteVector2(npc.Center);
                        for (int i = 0; i < candlePlayers.Count; i++)
                        {
                            p.Write(candlePlayers[i]);
                        }
                    }, PacketType.CandleSouls);
                }
            }
        }
        public bool GhostKill(NPC npc, NecromancyNPC zombie, GhostInfo info, List<OnKillPlayerInfo> players)
        {
            if (zombie.ghostDebuffDOT > 0 && info.EnoughPower(zombie.zombieDebuffTier))
            {
                return true;
            }
            if (zombie.conversionChance > 0 && Main.rand.NextBool(zombie.conversionChance))
            {
                return true;
            }
            //for (int i = 0; i < players.Count; i++)
            //{
            //    if (players[i].Aequus().dreamMask && Main.rand.NextBool(4))
            //    {
            //        zombie.zombieOwner = players[i].whoAmI;
            //        zombie.zombieDebuffTier = info.PowerNeeded;
            //        return true;
            //    }
            //}
            return false;
        }
        public void DeathEffect_SnowgraveFreeze(NPC npc)
        {
            if (SnowgraveCorpse.CanFreezeNPC(npc))
            {
                EffectsSystem.BehindProjs.Add(new SnowgraveCorpse(npc.Center, npc));
            }
        }
        public List<OnKillPlayerInfo> GetCloseEnoughPlayers(NPC npc)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                return new List<OnKillPlayerInfo>() { new OnKillPlayerInfo { player = Main.LocalPlayer, aequus = Main.LocalPlayer.Aequus(), distance = npc.Distance(Main.LocalPlayer.Center) }, };
            }
            var list = new List<OnKillPlayerInfo>();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead)
                {
                    float d = npc.Distance(Main.player[i].Center);
                    if (d < 1000f)
                    {
                        list.Add(new OnKillPlayerInfo { player = Main.player[i], aequus = Main.player[i].Aequus(), distance = d, });
                    }
                }
            }
            return list;
        }

        public override void OnKill(NPC npc)
        {
            if (npc.SpawnedFromStatue || npc.friendly || npc.lifeMax < 5)
                return;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (npc.playerInteraction[i])
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.player[i].Aequus().OnKillEffect(npc.netID, npc.position, npc.width, npc.height, npc.lifeMax);
                        continue;
                    }

                    var p = Aequus.GetPacket(PacketType.OnKillEffect);
                    p.Write(i);
                    p.Write(npc.netID);
                    p.WriteVector2(npc.position);
                    p.Write(npc.width);
                    p.Write(npc.height);
                    p.Write(npc.lifeMax);
                    p.Send(toClient: i);
                }
            }
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
                        if (AequusItem.LegendaryFishIDs.Contains(inv[i].type))
                        {
                            Main.LocalPlayer.GetModPlayer<AnglerQuestRewards>().LegendaryFishRewards(npc, inv[i], i);
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
            binaryWriter.Write(locustStacks);
            binaryWriter.Write(corruptionHellfireStacks);
            binaryWriter.Write(crimsonHellfireStacks);
            binaryWriter.Write(mindfungusStacks);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            locustStacks = binaryReader.ReadByte();
            corruptionHellfireStacks = binaryReader.ReadByte();
            crimsonHellfireStacks = binaryReader.ReadByte();
            mindfungusStacks = binaryReader.ReadByte();
        }

        #region Hooks
        private static void AddHooks()
        {
            On.Terraria.NPC.Transform += NPC_Transform;
            On.Terraria.NPC.UpdateNPC_Inner += NPC_UpdateNPC_Inner; // fsr detouring NPC.Update(int) doesn't work, but this does
            On.Terraria.NPC.VanillaHitEffect += Hook_PreHitEffect;
        }

        private static void NPC_Transform(On.Terraria.NPC.orig_Transform orig, NPC npc, int newType)
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

        private static void NPC_UpdateNPC_Inner(On.Terraria.NPC.orig_UpdateNPC_Inner orig, NPC self, int i)
        {
            if (self.TryGetGlobalNPC<BitCrushedGlobalNPC>(out var bitCrushed) && !bitCrushed.CheckUpdateNPC(self, i))
                return;
            orig(self, i);
        }

        private static void Hook_PreHitEffect(On.Terraria.NPC.orig_VanillaHitEffect orig, NPC self, int hitDirection, double dmg)
        {
            try
            {
                if (self.TryGetGlobalNPC<AequusNPC>(out var aequus))
                {
                    if (aequus.noHitEffect)
                    {
                        return;
                    }
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    goto Orig;
                }

                if (self.HasBuff<Bleeding>())
                {
                    int amt = (int)Math.Min(4.0 + dmg / 20.0, 20.0);
                    for (int i = 0; i < amt; i++)
                    {
                        bool foodParticle = Main.rand.NextBool();
                        var d = Dust.NewDustDirect(self.position, self.width, self.height, foodParticle ? DustID.Blood : DustID.FoodPiece, newColor: foodParticle ? new Color(200, 20, 30, 100) : default);
                        d.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 5f);
                        d.velocity += self.velocity * 0.5f;
                        if (Main.rand.NextBool(3))
                        {
                            d.noGravity = true;
                        }
                    }
                }

                if (self.life <= 0 && self.HasBuff<SnowgraveDebuff>()
                    && SnowgraveCorpse.CanFreezeNPC(self))
                {
                    SoundEngine.PlaySound(SoundID.Item30, self.Center);
                    return;
                }
            }
            catch
            {
            }
        Orig:
            orig(self, hitDirection, dmg);
        }
        #endregion
    }
}