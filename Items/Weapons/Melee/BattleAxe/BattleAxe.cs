using Aequus.Buffs.Debuffs;
using Aequus.Common.GlobalTiles;
using Aequus.Common.Net.Sounds;
using Aequus.Projectiles.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.BattleAxe {
    public class BattleAxe : ModItem {
        public override void SetDefaults() {
            Item.DefaultToAequusSword<BattleAxeProj>(34);
            Item.useTime /= 2;
            Item.SetWeaponValues(14, 5f, 16);
            Item.width = 30;
            Item.height = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.axe = 10;
            Item.tileBoost = 1;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.reuseDelay = 4;
        }

        public override bool? UseItem(Player player) {
            Item.FixSwing(player);
            return true;
        }

        public override bool MeleePrefix() {
            return true;
        }

        public override bool CanShoot(Player player) {
            return player.ownedProjectileCounts[Item.shoot] <= 0;
        }
    }

    public class BattleAxeProj : HeldSlashingSwordProjectile {
        public override void SetDefaults() {
            base.SetDefaults();
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.extraUpdates = 2;
            swordHeight = 60;
            gfxOutOffset = -18;
        }

        public override void AI() {
            base.AI();
            if (Main.player[Projectile.owner].itemAnimation <= 1) {
                Main.player[Projectile.owner].Aequus().itemCombo = (ushort)(combo == 0 ? 64 : 0);
            }
            if (!playedSound && AnimProgress > 0.4f) {
                playedSound = true;
                SoundEngine.PlaySound(SoundID.Item1.WithPitchOffset(-1f), Projectile.Center);
            }
        }

        public override Vector2 GetOffsetVector(float progress) {
            return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.75f) - MathHelper.PiOver2 * 1.75f) * -swingDirection * (0.9f + 0.1f * Math.Min(Main.player[Projectile.owner].Aequus().itemUsage / 300f, 1f)));
        }


        public override float SwingProgress(float progress) {
            return SwingProgressSplit(progress);
        }

        public override float GetScale(float progress) {
            float scale = base.GetScale(progress);
            if (progress > 0.1f && progress < 0.9f) {
                return scale + 0.3f * Helper.Wave(SwingProgress((progress - 0.1f) / 0.8f), 0f, 1f);
            }
            return scale;
        }

        public override float GetVisualOuter(float progress, float swingProgress) {
            return 0f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
            freezeFrame = 4;
            if (Main.rand.NextBool(5)) {
                ModContent.GetInstance<BleedingDebuffSound>().Play(target.Center);
                target.AddBuff(ModContent.BuffType<BattleAxeBleeding>(), 240);
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
            GetSwordDrawInfo(out var texture, out var handPosition, out var frame, out var rotationOffset, out var origin, out var effects);

            float colorIntensity = drawColor.ToVector3().Length() * Projectile.Opacity * 0.2f;
            Color swishColor = new(colorIntensity, colorIntensity, colorIntensity, 0f);

            DrawSwordAfterImages(texture, handPosition, frame, swishColor * 0.33f, rotationOffset, origin, effects);
            DrawSword(texture, handPosition, frame, drawColor, rotationOffset, origin, effects);
            return false;
        }
    }

    public class BattleAxeTile : ModTile {
        public override string Texture => AequusTextures.BattleAxe.Path;

        public override void SetStaticDefaults() {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            DustType = -1;
            TileID.Sets.DisableSmartCursor[Type] = true;
            AddMapEntry(new Color(136, 136, 148), TextHelper.GetItemName<BattleAxe>());
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
            if (!TileID.Sets.IsATreeTrunk[Framing.GetTileSafely(i - 1, j).TileType]
                && !TileID.Sets.IsATreeTrunk[Framing.GetTileSafely(i + 1, j).TileType]) {
                WorldGen.KillTile(i, j);
            }
            return false;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j) {
            return new[] { new Item(ModContent.ItemType<BattleAxe>()) };
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
            var texture = TextureAssets.Tile[Type].Value;
            var drawCoords = (this.GetDrawPosition(i, j) + Helper.TileDrawOffset + new Vector2(8f)).Floor();
            var effects = SpriteEffects.None;
            var frame = new Rectangle(0, 0, texture.Width / 2 + 10, texture.Height);
            if (TileID.Sets.IsATreeTrunk[Main.tile[i - 1, j].TileType]) {
                effects = SpriteEffects.FlipHorizontally;
                drawCoords.X += 14f;
            }
            spriteBatch.Draw(
                texture,
                drawCoords,
                frame,
                Lighting.GetColor(i, j),
                0f,
                new Vector2(texture.Width / 2f, texture.Height / 4f),
                1f,
                effects,
                0f
            );
            return false;
        }

        public static bool TrySpawnBattleAxe(in GlobalRandomTileUpdateParams info) {
            int battleAxeTile = ModContent.TileType<BattleAxeTile>();
            int x = info.X + (WorldGen.genRand.NextBool() ? -1 : 1);
            var tile = Framing.GetTileSafely(x, info.Y);
            var tree = Framing.GetTileSafely(info.X, info.Y);
            if (!TileID.Sets.IsATreeTrunk[info.TileTypeCache]
                || tile.HasTile
                || tree.TileFrameX > 22
                || tree.TileFrameY > 110
                || !Helper.InOuterX(info.X, info.Y, 3)) {
                return false;
            }
            //Helper.DebugDust(i, j);
            if (Helper.FindPlayerWithin(info.X, info.Y) != -1
                || TileHelper.ScanTiles(new(info.X - 200, info.Y - 50, 400, 100), TileHelper.HasTileAction(battleAxeTile))
                || TileHelper.ScanTiles(new(x, info.Y - 2, 1, 7), TileHelper.IsTree, TileHelper.IsSolid)
                ) {
                return false;
            }
            tile.Active(value: true);
            tile.TileType = (ushort)battleAxeTile;
            //WorldGen.PlaceTile(x, j, battleAxeTile, mute: true);
            return tile.TileType == battleAxeTile;
        }
    }
}