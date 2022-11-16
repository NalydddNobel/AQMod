using Aequus.Graphics.RenderTargets;
using Aequus.Items;
using Aequus.Items.Tools.Camera;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public class ShutterstockerProj : ModProjectile
    {
        public Vector2 mouseWorld;

        public float PhotoSize { get => Projectile.ai[0]; set => Projectile.ai[0] = MathHelper.Clamp(value, 3f, 36f); }
        public int CalculatedPhotoSize => (int)Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if ((int)Projectile.ai[0] == 0)
            {
                PhotoSize = 20f;
            }

            var player = Main.player[Projectile.owner];

            if (Main.myPlayer == Projectile.owner)
            {
                if (Main.mouseRight && Main.mouseRightRelease)
                {
                    Projectile.Kill();
                    return;
                }

                var oldMouseWorld = player.MouseWorld();
                mouseWorld = player.MouseWorld();
                if (mouseWorld.X != oldMouseWorld.X || mouseWorld.Y != oldMouseWorld.Y)
                {
                    Projectile.netUpdate = true;
                }
                int scrollWheel = PlayerInput.ScrollWheelDelta / 120;
                if (scrollWheel != 0)
                {
                    PhotoSize += scrollWheel;
                }
            }

            var targetMouseWorld = mouseWorld.ToTileCoordinates().ToWorldCoordinates();
            if (CalculatedPhotoSize % 2 == 0)
                targetMouseWorld -= new Vector2(8f);
            var diff = targetMouseWorld - Projectile.Center;
            float distance = diff.Length();
            if (distance <= 2f)
            {
                Projectile.Center = targetMouseWorld;
            }
            else
            {
                Projectile.velocity = Vector2.Normalize(diff) * Math.Max(distance / 4f, 1f);
            }

            if (!player.channel || !player.controlUseItem)
            {
                SnapPhoto();
                Projectile.Kill();
                return;
            }

            player.itemTime = 2;
            player.itemAnimation = 2;
            player.heldProj = Projectile.whoAmI;

            AequusHelpers.ShootRotation(Projectile, MathHelper.WrapAngle((Projectile.Center - Main.player[Projectile.owner].Center).ToRotation() + (float)Math.PI / 2f));
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public void SnapPhoto()
        {
            var player = Main.player[Projectile.owner];
            if (!player.ConsumeItem(player.HeldItem.useAmmo))
            {
                return;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                Item item;
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    item = AequusItem.SetDefaults<ShutterstockerClip>(checkMaterial: false);
                }
                else
                {
                    int i = Item.NewItem(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, player.HeldItem.useAmmo, "Shutterstock Photo Creation"), player.getRect(),
                        ModContent.ItemType<ShutterstockerClip>());
                    if (i == -1)
                    {
                        return;
                    }
                    item = Main.item[i];
                }

                int size = CalculatedPhotoSize + ShutterstockerSceneRenderer.TilePadding;
                var coords = Projectile.Center.ToTileCoordinates();
                var rect = new Rectangle(coords.X - size / 2, coords.Y - size / 2, size, size);
                item.ModItem<ShutterstockerClip>().SetClip(rect);
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    ShutterstockerSceneRenderer.RenderRequests.Add(item.ModItem<ShutterstockerClip>());
                }
                else
                {
                    var p = Aequus.GetPacket(PacketType.SpawnShutterstockerClip);
                    p.Write(Projectile.owner);
                    item.ModItem<ShutterstockerClip>().NetSend(p);
                    p.Send();
                }
            }

            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            ScreenCulling.SetPadding(20);
            if (ScreenCulling.OnScreenWorld(Projectile.getRect()))
            {
                Main.BlackFadeIn = 400;
                SoundEngine.PlaySound(SoundID.Camera);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ShutterstockerInterface.DrawList.Add(this);
            DrawHeldCamera();
            return false;
        }

        public void DrawHeldCamera()
        {
            var texture = TextureAssets.Projectile[Type];

            var position = Main.GetPlayerArmPosition(Projectile);
            var difference = position - mouseWorld;
            var dir = Vector2.Normalize(difference);
            var drawCoords = position + dir * -8f;
            float rotation = difference.ToRotation() + (Main.player[Projectile.owner].direction == -1 ? 0f : MathHelper.Pi);
            var origin = texture.Value.Size() / 2f;
            var spriteEffects = Main.player[Projectile.owner].direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture.Value, drawCoords - Main.screenPosition, null, AequusHelpers.GetColor(drawCoords),
                 rotation, origin, Projectile.scale, spriteEffects, 0);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(mouseWorld.X);
            writer.Write(mouseWorld.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            mouseWorld.X = reader.ReadSingle();
            mouseWorld.Y = reader.ReadSingle();
        }
    }
}