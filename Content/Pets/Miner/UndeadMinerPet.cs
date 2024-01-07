using Aequus.Common.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequus.Content.Pets.Miner;

public class UndeadMinerPet : ModPet {
    public int swingPick;
    public int tileX;
    public int tileY;

    public int SwingTime => 24;
    public int frame;
    public float frameCounter;

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 15;
        Main.projPet[Type] = true;
        ProjectileID.Sets.LightPet[Type] = true;
    }

    public override void SetDefaults() {
        Projectile.width = 20;
        Projectile.height = 14;
        Projectile.friendly = true;
        Projectile.aiStyle = ProjAIStyleID.Pet;
        DrawOriginOffsetY = -8;
        AIType = ProjectileID.BlackCat;
    }

    public override bool PreAI() {
        if (!base.PreAI()) {
            return false;
        }

        Lighting.AddLight(Projectile.Top, new Vector3(0.4f, 0.3f, 0.1f));
        var tileCoords = ((Projectile.Center + Main.player[Projectile.owner].Center) / 2f).ToTileCoordinates();
        float closest = float.MaxValue;
        var pick = Main.player[Projectile.owner].GetBestPickaxe();
        pick ??= ContentSamples.ItemsByType[ItemID.CopperPickaxe];
        for (int j = 10; j > -10; j--) {
            for (int i = -10; i < 10; i++) {
                int checkTileX = tileCoords.X + i;
                int checkTileY = tileCoords.Y + j;
                if (WorldGen.InWorld(checkTileX, checkTileY, 50) && Main.tile[checkTileX, checkTileY].IsFullySolid()
                    && Main.tileSpelunker[Main.tile[checkTileX, checkTileY].TileType]
                    && Main.player[Projectile.owner].CheckPickPower(pick, checkTileX, checkTileY)
                    && WorldGen.CanKillTile(checkTileX, checkTileY)) {
                    var worldCoords = new Vector2(checkTileX * 16f, checkTileY * 16f);
                    float c = Vector2.Distance(Projectile.Center, worldCoords);
                    if (c < closest && Collision.CanHitLine(Projectile.position + Main.rand.NextVector2Unit() * Main.rand.NextFloat(6f), Projectile.width, Projectile.height, worldCoords + Vector2.Normalize(Projectile.Center - worldCoords).RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f)) * Main.rand.Next(8, 20), 16, 16)) {
                        tileX = checkTileX;
                        tileY = checkTileY;
                        closest = c;
                        Projectile.netUpdate = true;
                    }
                }
            }
        }
        if (swingPick > 0) {
            frame = 11 + swingPick / (SwingTime / 4);
            if (frame > 14) {
                frame = 14;
            }
            swingPick++;
            if (swingPick > SwingTime) {
                swingPick = 0;
            }
        }
        else {
            swingPick--;
            if (Projectile.velocity.X == 0f) {
                frame = 0;
            }
            if (Projectile.velocity.Y != 0f || !Projectile.tileCollide) {
                frame = 1;
            }
            else if (Math.Abs(Projectile.velocity.X) > 0f) {
                if (frame < 2) {
                    frame = 2;
                }

                frameCounter += Math.Abs(Projectile.velocity.X) * 0.7f;
                if (frameCounter >= 6f) {
                    frameCounter = 0f;
                    frame++;
                    if (frame > 10) {
                        frame = 2;
                    }
                }
            }
        }
        if (tileX > 0 && tileY > 0) {
            if (Projectile.Distance(Main.player[Projectile.owner].Center) > 1000f) {
                Projectile.Center = Main.player[Projectile.owner].Center;
                Projectile.velocity *= 0.1f;
                tileX = 0;
                tileY = 0;
                return false;
            }
            Projectile.shouldFallThrough = false;
            Projectile.rotation = 0f;
            Projectile.tileCollide = true;
            var tileWorld = new Vector2(tileX * 16f + 8f, tileY * 16f + 8f);
            ProjectileAISnippets.GenericPetGroundMovement(Projectile, tileWorld);
            Projectile.spriteDirection = -Projectile.direction;
            if (Projectile.getRect().ClosestPointInRect(tileWorld).Distance(tileWorld) < 40f) {
                if (swingPick <= 0) {
                    swingPick = 1;
                    if (pick.UseSound != null)
                        SoundEngine.PlaySound(pick.UseSound, Projectile.Center);
                    if (Main.myPlayer == Projectile.owner) {
                        Main.player[Projectile.owner].PickTile(tileX, tileY, pick.pick);
                    }
                    if (!Main.tile[tileX, tileY].IsFullySolid() || !Main.player[Projectile.owner].CheckPickPower(pick, tileX, tileY)) {
                        tileX = 0;
                        tileY = 0;
                        return false;
                    }
                }
            }
            else if (Projectile.velocity.Y == 0f && swingPick <= -18) {
                Projectile.velocity.X += Projectile.direction;
                if (Projectile.position.Y > tileWorld.Y) {
                    Projectile.velocity.Y = Math.Max((Projectile.position.Y - tileWorld.Y) * -0.057f + Main.rand.NextFloat(-1f, 0f), -10f);
                }
                swingPick = 0;
            }
            Projectile.velocity.Y += 0.3f;
            return false;
        }
        return true;
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(swingPick);
        writer.Write(tileX);
        writer.Write(tileY);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        swingPick = reader.ReadInt32();
        tileX = reader.ReadInt32();
        tileY = reader.ReadInt32();
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        if (tileY > 0 && tileY * 16 + 8 > Projectile.position.Y) {
            fallThrough = true;
        }
        return true;
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var texture, out var offset, out var _, out var origin, out int _);
        var frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: this.frame);
        Main.EntitySpriteDraw(texture, Projectile.position + offset + new Vector2(0f, DrawOriginOffsetY) - Main.screenPosition, frame, LightHelper.GetLightColor(Projectile.Center), Projectile.rotation, origin, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        if (swingPick > 0) {
            float rotation = (MathHelper.Pi + 0.1f) / SwingTime * swingPick - MathHelper.PiOver2;
            var pick = Main.player[Projectile.owner].GetBestPickaxe();
            int pickaxeItemID = pick == null ? ItemID.CopperPickaxe : pick.type;
            Main.GetItemDrawFrame(pickaxeItemID, out var itemTexture, out var itemFrame);
            var pickaxeDrawCoords = GetPickaxeHandPosition();
            if (Projectile.spriteDirection == -1) {
                origin.X = itemFrame.Width;
            }
            else {
                rotation = -rotation;
            }
            Main.EntitySpriteDraw(itemTexture, pickaxeDrawCoords - Main.screenPosition, itemFrame, LightHelper.GetLightColor(pickaxeDrawCoords), rotation - MathHelper.PiOver4, new Vector2(0f, itemFrame.Height), Projectile.scale, SpriteEffects.None, 0);
            if (pick?.glowMask > 0) {
                Main.EntitySpriteDraw(TextureAssets.GlowMask[pick.glowMask].Value, pickaxeDrawCoords - Main.screenPosition, itemFrame, new Color(250, 250, 250, 0), rotation - MathHelper.PiOver4, new Vector2(0f, itemFrame.Height), Projectile.scale, SpriteEffects.None, 0);
            }
        }
        return false;
    }

    public Vector2 GetPickaxeHandPosition() {
        var pos = Projectile.Center + new Vector2(0f, DrawOriginOffsetY);
        switch (frame) {
            case 11: {
                    pos.X += 4f * Projectile.spriteDirection;
                }
                break;
            case 12: {
                    pos.X += 2f * Projectile.spriteDirection;
                    pos.Y += 2f;
                }
                break;
            case 13: {
                    pos.X += 2f * Projectile.spriteDirection;
                    pos.Y += 9f;
                }
                break;
            case 14: {
                    pos.X += 4f * Projectile.spriteDirection;
                    pos.Y += 11f;
                }
                break;
        }
        return pos;
    }

    internal override InstancedPetBuff CreatePetBuff() {
        return new(this, (p) => ref p.GetModPlayer<AequusPlayer>().petUndeadMiner, lightPet: true);
    }

    internal override InstancedPetItem CreatePetItem() {
        return new(this, ItemRarityID.Green);
    }
}