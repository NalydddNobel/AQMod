using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials {
    public class PossessedShard : ModItem {

        private int _shake;
        private int _soundDelay;
        private int _applyCollision;
        private Vector2 fleeVelocity;

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 10;
            ItemID.Sets.ItemNoGravity[Type] = true;
        }

        public override void SetDefaults() {
            Item.width = 14;
            Item.height = 14;
            Item.maxStack = 9999;
            Item.rare = ItemDefaults.RarityEarlyHardmode - 1;
            Item.value = Item.sellPrice(silver: 7);
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.Lerp(Color.White, lightColor, Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0.66f, 1f));
        }

        public override void UpdateInventory(Player player) {
            fleeVelocity = Vector2.Zero;
            _applyCollision = 16;
            _soundDelay = 0;
            _shake = 0;
            Item.timeSinceItemSpawned = 0;
        }

        public override bool CanStackInWorld(Item decrease) {
            return false;
        }

        private void UpdateCollision() {
            var oldFleeVelocity = fleeVelocity;
            fleeVelocity = Collision.TileCollision(Item.position, fleeVelocity, Item.width, Item.height, false, false);
            var vector4 = Collision.SlopeCollision(Item.position, fleeVelocity, Item.width, Item.height, 0f, fall: true);
            if (vector4.Z != fleeVelocity.X || vector4.W != fleeVelocity.Y || fleeVelocity.X != oldFleeVelocity.X || fleeVelocity.Y != oldFleeVelocity.Y) {
                if (fleeVelocity.X != oldFleeVelocity.X) {
                    fleeVelocity.X = -oldFleeVelocity.X;
                }
                if (fleeVelocity.X != oldFleeVelocity.Y) {
                    fleeVelocity.Y = -oldFleeVelocity.Y;
                }
                fleeVelocity = fleeVelocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f));
                if (fleeVelocity.Length() < 7f) {
                    fleeVelocity.Normalize();
                    fleeVelocity *= 7f;
                }


                int dustSizeAdd = 10;
                var dustPosition = Item.position - Vector2.Normalize(fleeVelocity) * (Item.width + 4) - new Vector2(dustSizeAdd / 2f);
                for (int i = 0; i < 8; i++) {
                    var d = Dust.NewDustDirect(dustPosition, Item.width + dustSizeAdd, Item.height + dustSizeAdd, DustID.PurpleTorch, Scale: Main.rand.NextFloat(2f));
                    d.velocity = Vector2.Normalize(fleeVelocity) * Main.rand.NextFloat(3f);
                    d.noGravity = true;
                    d.fadeIn = d.scale + 0.5f;
                }

                if (_soundDelay <= 0) {
                    SoundEngine.PlaySound(AequusSounds.shardHit with { Volume = 0.8f, Pitch = -0.3f, PitchVariance = 0.1f, }, Item.position);
                }
                if (Main.netMode == NetmodeID.Server) {
                    NetMessage.SendData(MessageID.SyncItem, number: Item.whoAmI);
                }

                _shake = 8;
                _applyCollision = -8;
                _soundDelay = 8;
            }
        }

        public override void Update(ref float gravity, ref float maxFallSpeed) {

            if (_shake > 0 && Item.timeSinceItemSpawned % 3 == 0) {
                _shake--;
            }

            if (Item.timeSinceItemSpawned < 50 || Item.shimmered) {
                return;
            }

            if (Item.timeSinceItemSpawned % 8 == 0) {
                var d = Dust.NewDustDirect(Item.position, Item.width, Item.height, DustID.PurpleTorch, Alpha: 100);
                d.velocity = Vector2.Normalize(d.position - Item.Center) * (Main.rand.NextFloat(2f) + 1f);
                d.noGravity = true;
                d.fadeIn = d.scale + 0.5f;
            }

            if (_soundDelay > 0) {
                _soundDelay--;
            }
            if (!Helper.CheckForSolidGroundBelow(Item.Center.ToTileCoordinates(), 12, out var _)) {
                Item.velocity.Y += 0.4f;
            }
            if (!Helper.CheckForSolidRoofAbove(Item.Center.ToTileCoordinates(), 4, out var _)) {
                Item.velocity.Y -= 0.14f;
            }

            float moveSpeed = Main.expertMode ? 0.4f : 0.2f;
            float viewDistance = (fleeVelocity.X != 0f || fleeVelocity.Y != 0f) ? 300f : 150f;
            bool fleeing = false;
            if (_applyCollision >= 0) {
                for (int i = 0; i < Main.maxPlayers; i++) {
                    if (Main.player[i].active && Item.Distance(Main.player[i].Center) < viewDistance) {

                        var dir = Item.DirectionFrom(Main.player[i].Center);
                        if (fleeVelocity == Vector2.Zero) {
                            fleeVelocity = dir * 10f;
                            SoundEngine.PlaySound(AequusSounds.dash);
                            if (Main.netMode == NetmodeID.Server) {
                                NetMessage.SendData(MessageID.SyncItem, number: Item.whoAmI);
                            }

                            for (int j = 0; j < 8; j++) {
                                var d = Dust.NewDustDirect(Item.position, Item.width, Item.height, DustID.PurpleTorch, Scale: Main.rand.NextFloat(2f));
                                d.velocity = Vector2.Normalize(fleeVelocity) * Main.rand.NextFloat(3f);
                                d.noGravity = true;
                                d.fadeIn = d.scale + 0.5f;
                            }
                        }

                        fleeVelocity += dir * moveSpeed;
                        fleeing = true;
                    }
                }
            }

            fleeVelocity *= fleeing ? 0.96f : 0.86f;
            if (Math.Abs(fleeVelocity.X) < 0.1f && Math.Abs(fleeVelocity.Y) < 0.1f) {
                fleeVelocity = Vector2.Zero;
            }

            if (Collision.SolidCollision(Item.position, Item.width, Item.height)) {
                if (_applyCollision < 16) {
                    _applyCollision++;
                }
            }
            else if (_applyCollision > 0) {
                _applyCollision--;
            }
            else if (_applyCollision < 0) {
                _applyCollision++;
            }

            if (_applyCollision < 16) {
                UpdateCollision();
            }
            else {
                fleeVelocity += Item.DirectionFrom(Main.player[Player.FindClosest(Item.position, Item.width, Item.height)].Center);

                if (Main.netMode == NetmodeID.Server) {
                    NetMessage.SendData(MessageID.SyncItem, number: Item.whoAmI);
                }
            }

            if (fleeVelocity.HasNaNs()) {
                fleeVelocity = Vector2.Zero;
            }
            Item.position += fleeVelocity;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {

            var texture = TextureAssets.Item[Type].Value;
            var drawCoordinates = Item.Center - Main.screenPosition;
            Item.GetItemDrawData(out var frame);
            var origin = frame.Size() / 2f;

            float progress = MathF.Pow(1f - Main.GlobalTimeWrappedHourly % 1f, 2f);
            var glowColor = Color.BlueViolet with { A = 0 };
            var vector = Helper.CircularVector(4, progress * MathHelper.PiOver2);

            if (_shake > 0) {
                drawCoordinates += new Vector2(Main.rand.Next(-_shake, _shake), Main.rand.Next(-_shake, _shake)) / 1.5f;
            }

            if (progress > 0.5f) {

                float glowProgress = (progress - 0.5f) / 0.5f;
                foreach (var v in vector) {
                    spriteBatch.Draw(
                        texture,
                        drawCoordinates + v * 2f * scale,
                        frame,
                        glowColor * glowProgress,
                        rotation,
                        origin,
                        scale, SpriteEffects.None, 0f
                    );
                }
            }

            foreach (var v in vector) {
                spriteBatch.Draw(
                    texture,
                    drawCoordinates + v * (progress * 24f + 2f) * scale,
                    frame,
                    glowColor * (1f - progress),
                    rotation,
                    origin,
                    scale, SpriteEffects.None, 0f
                );
            }

            spriteBatch.Draw(
                texture,
                drawCoordinates,
                frame,
                Item.GetAlpha(lightColor),
                rotation,
                origin,
                scale, SpriteEffects.None, 0f
            );
            return false;
        }

        public override void NetSend(BinaryWriter writer) {
            writer.Write(fleeVelocity.X);
            writer.Write(fleeVelocity.Y);
        }

        public override void NetReceive(BinaryReader reader) {
            fleeVelocity.X = reader.ReadSingle();
            fleeVelocity.Y = reader.ReadSingle();
        }
    }
}