using Aequus.Content.DataSets;
using System;
using System.IO;
using System.Linq;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Aequus.Old.Content.Potions.Prefixes.SplashPotions;

public class SplashPotionProj : ModProjectile {
    public override string Texture => AequusTextures.Item(ItemID.RegenerationPotion);

    public int ItemType { get => Math.Abs((int)Projectile.ai[0]); set => Projectile.ai[0] = value; }
    public int BuffTime { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
    public virtual int AreaOfEffect => 120;

    public override void SetDefaults() {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 600;
        ItemType = ItemID.RegenerationPotion;
    }

    public override void OnSpawn(IEntitySource source) {
        if (source is not EntitySource_ItemUse itemUse || itemUse.Item == null || itemUse.Item.IsAir) {
            ItemType = Main.rand.Next(ItemMetadata.Potions.Where(e => e.ValidEntry).Select(e => e.Id).ToArray());
            return;
        }

        Projectile.ai[0] = itemUse.Item.type;
        Projectile.ai[1] = itemUse.Item.buffTime;
        Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        Projectile.netUpdate = true;
        Projectile.velocity = Vector2.Normalize(Main.MouseWorld - Projectile.Center) * 10f;
    }

    public void EnterSplashState() {
        Projectile.ai[0] = -Projectile.ai[0];
        Projectile.timeLeft = (AreaOfEffect / 16 + 1) * 4;
        Projectile.velocity = Vector2.Zero;
        Projectile.netUpdate = true;
        Projectile.hide = true;
        Projectile.tileCollide = false;
    }

    public void HandleSplashState() {
        int buffID = ContentSamples.ItemsByType[ItemType].buffType;

        for (int i = 0; i < Main.maxPlayers; i++) {
            if (Main.player[i].active && !Main.player[i].dead && !Main.player[i].ghost && Projectile.Distance(Main.player[i].getRect().ClosestPointInRect(Projectile.Center)) < AreaOfEffect) {
                Main.player[i].AddBuff(buffID, BuffTime);
            }
        }

        int progress = (Projectile.timeLeft - 4) / 4;

        Color color = GetPotionColor();
        int startX = (int)(Projectile.Center.X - AreaOfEffect) / 16;
        int startY = (int)(Projectile.Center.Y - AreaOfEffect) / 16;
        int endX = (int)(Projectile.Center.X + AreaOfEffect) / 16;
        int endY = (int)(Projectile.Center.Y + AreaOfEffect) / 16;
        int middleX = (int)Projectile.Center.X / 16;
        for (int i = startX; i < endX; i++) {
            for (int j = startY; j < endY; j++) {
                if (!WorldGen.InWorld(i, j, 4))
                    continue;

                if (Main.tile[i, j].IsFullySolid() && !Main.tile[i, j - 1].IsFullySolid()) {
                    int x = AreaOfEffect / 16 - Math.Abs(i - middleX);
                    if (progress == x) {
                        for (int m = 0; m < x; m++) {
                            if (Main.rand.NextBool(2)) {
                                var d = Dust.NewDustPerfect(new Vector2(i * 16f + Main.rand.NextFloat(16f), j * 16f + Main.rand.NextFloat(-2f, 2f)), DustID.SilverFlame, Vector2.Zero, newColor: color with { A = (byte)Main.rand.Next(200) } * Math.Min(x / 4f, 1f), Scale: Main.rand.NextFloat(0.75f, 1.5f));
                                d.velocity.X = Math.Sign(d.position.X - Projectile.Center.X) * Main.rand.NextFloat(3f) * x / 3f;
                                d.velocity.Y -= 16f / Math.Max(x, 1f) / 2f * Main.rand.NextFloat(1f);
                                if (d.velocity.Length() > 4f) {
                                    d.velocity.Normalize();
                                    d.velocity *= 4f;
                                }
                                d.fadeIn = d.scale + 0.4f;
                                d.noGravity = true;
                            }
                        }
                    }
                }
            }
        }

        if (Projectile.localAI[0] == 0f) {
            for (int i = 0; i < 16; i++) {
                Vector2 normal = Main.rand.NextVector2Unit();
                Dust d = Dust.NewDustPerfect(Projectile.Center + normal * Main.rand.NextFloat(AreaOfEffect / 8f), DustID.SilverFlame, normal * Main.rand.NextFloat(2f), newColor: color with { A = (byte)Main.rand.Next(200) }, Scale: Main.rand.NextFloat(0.5f, 1f));
                d.fadeIn = d.scale + 0.4f;
                d.noGravity = true;
            }
            for (int i = 0; i < 16; i++) {
                var normal = Main.rand.NextVector2Unit();
                normal.Y = -Math.Abs(normal.Y) * 2f;
                Dust d= Dust.NewDustPerfect(Projectile.Center + normal * Main.rand.NextFloat(16f), DustID.Glass, normal * Main.rand.NextFloat(2f), Scale: Main.rand.NextFloat(0.5f, 1f));
                d.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Shatter.WithPitchOffset(Main.rand.NextFloat(0.2f, 0.4f)));
            Projectile.localAI[0] = 1f;
        }

        for (int i = 0; i < 4; i++) {
            if (Main.rand.NextBool(2)) {
                Vector2 normal = Main.rand.NextVector2Unit();
                Dust d = Dust.NewDustPerfect(Projectile.Center + normal * Main.rand.NextFloat(progress * 4f), DustID.SilverFlame,
                    normal * Main.rand.NextFloat(progress * 1f), newColor: color with { A = (byte)Main.rand.Next(200) }, Scale: Main.rand.NextFloat(0.5f, 1f));
                d.fadeIn = d.scale + 0.4f;
                d.noGravity = true;
            }
        }
    }

    public override void AI() {
        if (Projectile.ai[0] < 0f) {
            Projectile.hide = true;
            Projectile.tileCollide = false;
            HandleSplashState();
            return;
        }
        if (ItemType == 0) {
            ItemType = ItemID.RegenerationPotion;
        }

        if (BuffTime == 0) {
            BuffTime = 3600;
        }

        Rectangle rect = Projectile.getRect();
        rect.Inflate(16, 16);
        for (int i = 0; i < Main.maxPlayers; i++) {
            if (Main.player[i].active && !Main.player[i].dead && !Main.player[i].ghost && Projectile.Colliding(rect, Main.player[i].getRect())) {
                if (i == Projectile.owner && Projectile.timeLeft > 580) {
                    continue;
                }
                EnterSplashState();
                return;
            }
        }
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && Projectile.Colliding(rect, Main.npc[i].getRect())) {
                EnterSplashState();
                return;
            }
        }
        Projectile.velocity.Y += 0.25f;
        Projectile.velocity.X *= 0.99f;
        Projectile.rotation += Projectile.velocity.X * 0.02f;
        if (Main.GameUpdateCount % 7 == 0 || Main.rand.NextBool(12)) {
            Color color = GetPotionColor();
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SilverFlame, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, newColor: color with { A = 0 } * Main.rand.NextFloat(0.66f, 1f), Scale: Main.rand.NextFloat(0.8f, 1.5f));
            d.velocity *= 0.1f;
            d.velocity += Projectile.velocity * 0.1f;
            d.fadeIn = d.scale + 0.4f;
            d.noGravity = true;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        EnterSplashState();
        return false;
    }

    public override void OnKill(int timeLeft) {
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(Projectile.timeLeft);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        Projectile.timeLeft = reader.ReadInt32();
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 8;
        height = 8;
        return true;
    }

    public override bool PreDraw(ref Color lightColor) {
        Main.instance.LoadItem(ItemType);

        Main.GetItemDrawFrame(ItemType, out Texture2D texture, out Rectangle frame);
        var origin = frame.Size() / 2;
        Main.EntitySpriteDraw(TextureAssets.Item[ItemType].Value, Projectile.Center - Main.screenPosition, frame, ExtendLight.Get(Projectile.Center), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        return false;
    }

    private Color GetPotionColor() {
        Color[] TCommonColor = ItemSets.DrinkParticleColors.IndexInRange(ItemType) ? ItemSets.DrinkParticleColors[ItemType] : null;

        return TCommonColor == null || TCommonColor.Length == 0 ? Color.White : Main.rand.Next(TCommonColor);
    }
}
