using Aequus.Common;
using Aequus.Items.Armor.Gravetender;
using Aequus.Items.Armor.Necromancer;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Materials.Gems;
using Microsoft.Xna.Framework;
using System.Reflection.Metadata.Ecma335;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.Necromancer {
    [AutoloadEquip(EquipType.Head)]
    public class NecromancerHood : ModItem {

        public int EnemyDamage;
        public int[] EnemySpawn;

        public override void Load() {
            GlowMasksHandler.AddGlowmask(AequusTextures.NecromancerHood_Head_Glow.Path);
        }

        public override void SetStaticDefaults() {
            SacrificeTotal = 1;
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
            player.Aequus().armorNecromancerBattle = this;
            player.Aequus().empoweredLegs = true;
        }

        public override void UpdateEquip(Player player) {
            player.GetDamage<SummonDamageClass>() += 0.2f;
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

        public int EnemySummon { get => (int)Projectile.ai[0]; }

        public override void SetDefaults() {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.CadetBlue;
        }

        public override bool? CanDamage() {
            return false;
        }

        public override void AI() {

            if ((int)Projectile.localAI[0] == 0) {
                SoundEngine.PlaySound(AequusSounds.necromancySpawn, Projectile.Center);
                Projectile.localAI[0] = 1f;
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
                    Projectile.position.X = pos.X;
                    Projectile.position.Y = y * 16f - Projectile.height;
                    break;
                }
                Projectile.timeLeft++;
                return;
            }
        }

        public override void Kill(int timeLeft) {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                
            }
        }

        public override bool PreDraw(ref Color lightColor) {

            var color = GetAlpha(lightColor);

            return false;
        }
    }
}

namespace Aequus {
    public partial class AequusPlayer {
        public NecromancerHood armorNecromancerBattle;

        public void OnHitByEffect_NecromancerSetbonus() {
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