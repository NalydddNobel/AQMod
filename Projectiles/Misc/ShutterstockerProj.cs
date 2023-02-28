using Aequus.Content.Town.CarpenterNPC.Quest;
using Aequus.Content.Town.CarpenterNPC.Quest.Bounties;
using Aequus.Content.Town.CarpenterNPC.Quest.Bounties.Steps;
using Aequus.Content.Town.CarpenterNPC.Quest.Photobook;
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

        public virtual float PhotoSize { get => Projectile.ai[0]; set => Projectile.ai[0] = MathHelper.Clamp(value, 3f, 60f); }
        public virtual int PhotoSizeX => (int)Projectile.ai[0];
        public virtual int PhotoSizeY => (int)Projectile.ai[0];
        public virtual int ClipPaddingX => PhotoRenderer.TilePadding;
        public virtual int ClipPaddingY => PhotoRenderer.TilePadding;

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
                    Projectile.netUpdate = true;
                }
            }

            var targetMouseWorld = mouseWorld.ToTileCoordinates().ToWorldCoordinates();
            if (PhotoSizeX % 2 == 0)
                targetMouseWorld.X -= 8f;
            if (PhotoSizeY % 2 == 0)
                targetMouseWorld.Y -= 8f;
            var diff = targetMouseWorld - Projectile.Center;
            float distance = diff.Length();
            if (distance <= 2f)
            {
                Projectile.Center = targetMouseWorld;
            }
            else
            {
                Projectile.velocity = Vector2.Normalize(diff) * Math.Max(distance / 2f, 1f);
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

            Helper.ShootRotation(Projectile, MathHelper.WrapAngle((Projectile.Center - Main.player[Projectile.owner].Center).ToRotation() + (float)Math.PI / 2f));
        }

        public virtual void SpawnClipItem(Rectangle tilesCaptured)
        {
            var carpenter = Main.player[Projectile.owner].GetModPlayer<CarpenterBountyPlayer>();
            foreach (var b in CarpenterSystem.BountiesByID)
            {
                if (!CarpenterSystem.CompletedBounties.Contains(b.FullName))
                {
                    var result = b.CheckConditions(new StepInfo(tilesCaptured));
                    if (b.Type == carpenter.SelectedBounty)
                    {
                        CarpenterBountyPlayer.LastPhotoTakenResults = result.perStepResults;
                    }
                    if (!result.Success())
                        continue;
                    if (b.BuildingBuff > 0)
                    {
                        CarpenterSystem.AddBuildingBuffLocation(b.Type, tilesCaptured);
                    }
                    SoundEngine.PlaySound(SoundID.Item129);
                    int text = CombatText.NewText(Main.player[Projectile.owner].getRect(), Color.Green, 1, dramatic: true);
                    Main.combatText[text].text = $"Bounty '{b.DisplayName}' Completed!";
                    Main.combatText[text].position = Main.player[Projectile.owner].Top + new Vector2(FontAssets.CombatText[0].Value.MeasureString(Main.combatText[text].text).X / -2f, -10f);
                    CarpenterSystem.CompleteCarpenterBounty(b);
                }
            }

            var photoPlayer = Main.player[Projectile.owner].GetModPlayer<PhotobookPlayer>();
            if (!photoPlayer.hasPhotobook)
                return;
            for (int i = 0; i < photoPlayer.Photos.Length; i++)
            {
                if (photoPlayer.photos[i].tileMap == null)
                {
                    photoPlayer.photos[i] = PhotoData.TakeASnap(tilesCaptured);
                    photoPlayer.photos[i].LoadTexture();
                    int text = CombatText.NewText(Main.player[Projectile.owner].getRect(), Color.Lerp(Color.White, Color.Lime * 2f, 0.2f), 1);
                    Main.combatText[text].text = $"Saved in slot {i + 1}!";
                    Main.combatText[text].position = Main.player[Projectile.owner].Top + new Vector2(FontAssets.CombatText[0].Value.MeasureString(Main.combatText[text].text).X / -2f, -60f);
                    Main.combatText[text].velocity *= 0.25f;
                    Main.combatText[text].velocity.Y = -Main.combatText[text].velocity.Y;
                    Main.combatText[text].lifeTime *= 2;
                    Main.combatText[text].scale *= 0.1f;
                    return;
                }
            }

            //Item item;
            //if (Main.netMode != NetmodeID.SinglePlayer)
            //{
            //    item = AequusItem.SetDefaults<ShutterstockerClip>(checkMaterial: false);
            //}
            //else
            //{
            //    int i = Item.NewItem(Main.player[Projectile.owner].GetSource_ItemUse_WithPotentialAmmo(Main.player[Projectile.owner].HeldItem, Main.player[Projectile.owner].HeldItem.useAmmo, "Shutterstock Photo Creation"), Main.player[Projectile.owner].getRect(),
            //        ModContent.ItemType<ShutterstockerClip>());
            //    if (i == -1)
            //    {
            //        return;
            //    }
            //    item = Main.item[i];
            //}

            //item.ModItem<ShutterstockerClip>().SetClip(tilesCaptured);
            //if (Main.netMode == NetmodeID.SinglePlayer)
            //{
            //    PhotoRenderer.RenderRequests.Add(PhotoData.FromLegacyClip(item.ModItem<ShutterstockerClip>()));
            //}
            //else
            //{
            //    var p = Aequus.GetPacket(PacketType.SpawnShutterstockerClip);
            //    p.Write(Projectile.owner);
            //    item.ModItem<ShutterstockerClip>().NetSend(p);
            //    p.Send();
            //}
        }
        public virtual void SnapPhoto()
        {
            if (!Main.player[Projectile.owner].ConsumeItem(Main.player[Projectile.owner].HeldItem.useAmmo))
            {
                return;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                int sizeX = PhotoSizeX + ClipPaddingX;
                int sizeY = PhotoSizeY + ClipPaddingY;
                var coords = Projectile.Center.ToTileCoordinates();
                SpawnClipItem(new Rectangle(coords.X - sizeX / 2, coords.Y - sizeY / 2, sizeX, sizeY));
            }

            Main.player[Projectile.owner].Aequus().SetCooldown(300, ignoreStats: false, Main.player[Projectile.owner].HeldItemFixed());
            if (Main.netMode != NetmodeID.Server)
            {
                ScreenCulling.SetPadding(20);
                if (ScreenCulling.OnScreenWorld(Projectile.getRect()))
                {
                    Main.BlackFadeIn = 400;
                    SoundEngine.PlaySound(SoundID.Camera);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ShutterstockerInterface.DrawList.Add(this);
            DrawHeldCamera();
            return false;
        }

        public virtual void DrawHeldCamera()
        {
            var texture = TextureAssets.Projectile[Type];

            var position = Main.GetPlayerArmPosition(Projectile);
            position.Y += Main.player[Projectile.owner].gfxOffY;
            var difference = position - mouseWorld;
            var dir = Vector2.Normalize(difference);
            var drawCoords = position + dir * -8f;
            drawCoords.Y += Main.player[Projectile.owner].gfxOffY;
            float rotation = difference.ToRotation() + (Main.player[Projectile.owner].direction == -1 ? 0f : MathHelper.Pi);
            var origin = texture.Value.Size() / 2f;
            var spriteEffects = Main.player[Projectile.owner].direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture.Value, drawCoords - Main.screenPosition, null, Helper.GetColor(drawCoords),
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