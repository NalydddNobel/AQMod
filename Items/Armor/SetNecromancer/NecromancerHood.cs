using Aequus.Common;
using Aequus.Common.ModPlayers;
using Aequus.Content.Necromancy;
using Aequus.Content.Necromancy.Renderer;
using Aequus.Items.Armor.SetGravetender;
using Aequus.Items.Armor.SetNecromancer;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Materials.Gems;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.SetNecromancer {
    [AutoloadEquip(EquipType.Head)]
    public class NecromancerHood : ModItem {

        public int EnemyDamage;
        public int[] EnemySpawn;

        public override void Load() {
            if (!Main.dedServ) {
                GlowMasksHandler.AddGlowmask(AequusTextures.NecromancerHood_Head_Glow.Path);
            }
        }

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.defense = 4;
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 1);
            Item.shoot = ModContent.ProjectileType<NecromancerHoodSpawnerProj>();
            EnemyDamage = 100;
            EnemySpawn = new int[]
            {
                NPCID.Skeleton,
                NPCID.ArmoredSkeleton,
                NPCID.SkeletonArcher,
            };
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) {
            return body.type == ModContent.ItemType<NecromancerRobe>();
        }

        public override void UpdateArmorSet(Player player) {
            player.setBonus = TextHelper.GetTextValue("ArmorSetBonus.Necromancer");
            var aequus = player.Aequus();
            var legModifiers = aequus.equipModifiers.Legs();
            aequus.armorNecromancerBattle = this;
            legModifiers.addedStacks += 1;
            legModifiers.bonusColor = EquipEmpowermentManager.BasicEmpowermentColor;
            legModifiers.type |= EquipEmpowermentParameters.Defense | EquipEmpowermentParameters.Abilities;
        }

        public override void UpdateEquip(Player player) {
            player.GetDamage<MagicDamageClass>() += 0.08f;
            player.GetDamage<SummonDamageClass>() += 0.08f;
            player.Aequus().ghostSlotsMax++;
        }

        public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) {
            glowMask = GlowMasksHandler.GetID(AequusTextures.NecromancerHood_Head_Glow.Path);
            glowMaskColor = Color.White with { A = 0 } * (1f - shadow);
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<GravetenderHood>()
                .AddIngredient<DemonicEnergy>(1)
                .AddIngredient<SoulGemFilled>(3)
                .AddTile(TileID.Loom)
                .TryRegisterBefore(ItemID.GravediggerShovel);
        }
    }

    public class NecromancerHoodSpawnerProj : ModProjectile {
        public override string Texture => AequusTextures.None.Path;

        public int EnemySummon { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public virtual int GhostRendererID => ColorTargetID.ZombieScepter;

        public override void SetDefaults() {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
        }

        public override Color? GetAlpha(Color lightColor) {
            return Main.netMode == NetmodeID.Server ? null : 
                GhostRenderer.GetColorTarget(Main.player[Projectile.owner], GhostRendererID).getDrawColor();
        }

        public override bool? CanDamage() {
            return false;
        }

        public override void AI() {

            if ((int)Projectile.localAI[1] == 0) {
                SoundEngine.PlaySound(AequusSounds.necromancySpawn with { PitchVariance = 0.2f, }, Projectile.Center);
            }
            Projectile.localAI[1]++;

            if (Projectile.timeLeft == 20) {

                if (Main.netMode != NetmodeID.Server) {
                    var color = Projectile.GetAlpha(Color.White);
                    for (int i = 0; i < 12; i++) {
                        ParticleSystem.New<DashBlurParticle>(ParticleLayer.AboveDust).Setup(
                            Projectile.position + new Vector2(Main.rand.NextFloat(Projectile.width), Main.rand.NextFloat(Projectile.height) - 12f),
                            Vector2.UnitY * Main.rand.NextFloat(-8f, -1f),
                            color with { A = 0, } * Main.rand.NextFloat(0.1f, 1f)
                        );
                    }
                }

                if (Main.netMode != NetmodeID.MultiplayerClient) {
                    var n = NPC.NewNPCDirect(Projectile.GetSource_Death(), Projectile.Center, EnemySummon);
                    var zombie = n.GetGlobalNPC<NecromancyNPC>();
                    zombie.ghostDamage = Projectile.damage;
                    zombie.renderLayer = GhostRendererID;
                    zombie.SpawnZombie_SetZombieStats(n, Projectile.Bottom + new Vector2(0f, -n.height), Vector2.UnitY * -32f, 0, 0, out bool _);
                }
            }

            if ((int)Projectile.ai[1] == 0) {

                var player = Main.player[Projectile.owner];
                Projectile.Center = player.Center;

                for (int i = 0; i < 25; i++) {
                    var pos = Projectile.position + Main.rand.NextVector2Unit() * Main.rand.NextFloat(10f, 360f);

                    if (!Collision.CanHitLine(pos, Projectile.width, Projectile.height, 
                        player.position, player.width, player.height)) {
                        continue;
                    }

                    int y = Helper.FindFloor((int)pos.X / 16, (int)pos.Y / 16, distance: 20);
                    if (y == -1) {
                        continue;
                    }

                    Projectile.position.X = pos.X;
                    Projectile.position.Y = y * 16f - Projectile.height;
                    Projectile.ai[1] = 1f;

                    // When spawned via cheats
                    if (EnemySummon == 0) {
                        EnemySummon = Main.rand.Next(NPCLoader.NPCCount);
                    }
                    break;
                }
                Projectile.timeLeft++;
                return;
            }
        }

        public override bool PreDraw(ref Color lightColor) {

            var drawCoordinates = Projectile.Center - Main.screenPosition;
            var color = Projectile.GetAlpha(lightColor);

            int timer = (int)Projectile.localAI[1];

            Main.instance.LoadItem(ItemID.Tombstone);
            var tombTexture = TextureAssets.Item[ItemID.Tombstone].Value;
            int graveY = Math.Clamp((int)(timer * 1.5f) - 10, 0, tombTexture.Height);
            if (graveY > 0) {

                float graveOpacity = graveY / (float)tombTexture.Height;
                if (Projectile.timeLeft < 20) {
                    graveOpacity *= Projectile.timeLeft / 20f;
                }

                var graveCoordinates = drawCoordinates + new Vector2(0f, -graveY + tombTexture.Height + Projectile.height / 2f);
                Rectangle graveFrame = new(0, 0, tombTexture.Width, (int)graveY / 2 * 2);
                Vector2 graveOrigin = new(tombTexture.Width / 2f, tombTexture.Height);
                Color graveOutlineColor = color with { A = 0 } * graveOpacity;
                foreach (var v in Helper.CircularVector(4)) {
                    Main.EntitySpriteDraw(
                        tombTexture,
                        graveCoordinates + v * 4f * Projectile.scale,
                        graveFrame,
                        graveOutlineColor * graveOpacity,
                        0f,
                        graveOrigin,
                        Projectile.scale,
                        SpriteEffects.None, 0
                    );
                }

                Main.EntitySpriteDraw(
                    tombTexture,
                    graveCoordinates,
                    graveFrame,
                    lightColor * graveOpacity,
                    0f,
                    graveOrigin,
                    Projectile.scale,
                    SpriteEffects.None, 0
                );
            }
            return false;
        }
    }
}

namespace Aequus {
    public partial class AequusPlayer {
        public NecromancerHood armorNecromancerBattle;

        public void CheckNecromancerSetbonus() {
            if (Main.myPlayer != Player.whoAmI || ghostSlots > 0 || armorNecromancerBattle == null) {
                return;
            }

            Projectile.NewProjectile(
                Player.GetSource_Accessory(armorNecromancerBattle.Item), 
                Player.Center, 
                Vector2.Zero, 
                armorNecromancerBattle.Item.shoot, 
                armorNecromancerBattle.EnemyDamage,
                0f, 
                Player.whoAmI,
                Main.rand.Next(armorNecromancerBattle.EnemySpawn));
        }
    }
}