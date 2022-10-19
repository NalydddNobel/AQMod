using Aequus.Projectiles.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.UI
{
    public class ShutterstockerInterface : ILoadable
    {
        public const int InterfaceFramesX = 4;
        public const int InterfaceFramesY = 4;

        public static Asset<Texture2D> Texture { get; private set; }

        public static List<ShutterstockerProj> DrawList { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            DrawList = new List<ShutterstockerProj>();
            if (!Main.dedServ)
            {
                Texture = ModContent.Request<Texture2D>(Aequus.AssetsPath + "UI/ShutterstockerInterface", AssetRequestMode.ImmediateLoad);
            }
        }

        void ILoadable.Unload()
        {
            Texture = null;
            DrawList?.Clear();
            DrawList = null;
        }

        public static void Render(SpriteBatch spriteBatch)
        {
            if (DrawList.Count == 0)
                return;

            foreach (var proj in DrawList)
            {
                if (proj.Projectile == null)
                    continue;

                var referenceFrame = Frame(0, 0);
                var photoRect = Utils.CenteredRectangle(proj.Projectile.Center, new Vector2(proj.CalculatedPhotoSize * 16));
                photoRect.X -= (int)Main.screenPosition.X;
                photoRect.Y -= (int)Main.screenPosition.Y;
                var texture = Texture.Value;
                spriteBatch.Draw(texture, new Vector2(photoRect.X, photoRect.Y), referenceFrame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, new Vector2(photoRect.X + photoRect.Width - referenceFrame.Width, photoRect.Y), Frame(2, 0), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, new Vector2(photoRect.X, photoRect.Y + photoRect.Height - referenceFrame.Height), Frame(0, 2), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, new Vector2(photoRect.X + photoRect.Width - referenceFrame.Width, photoRect.Y + photoRect.Height - referenceFrame.Height), Frame(2, 2), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                var segmentFrame = Frame(1, 0);
                var segmentFrame2 = Frame(1, 2);
                for (int i = 1; i < photoRect.Width / 16 - 1; i++)
                {
                    int x = photoRect.X + i * 16;
                    spriteBatch.Draw(texture, new Vector2(x, photoRect.Y), segmentFrame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture, new Vector2(x, photoRect.Y + photoRect.Height - referenceFrame.Height), segmentFrame2, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }

                segmentFrame = Frame(0, 1);
                segmentFrame2 = Frame(2, 1);
                for (int i = 1; i < photoRect.Width / 16 - 1; i++)
                {
                    int y = photoRect.Y + i * 16;
                    spriteBatch.Draw(texture, new Vector2(photoRect.X, y), segmentFrame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture, new Vector2(photoRect.X + photoRect.Width - referenceFrame.Width, y), segmentFrame2, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
            DrawList.Clear();
        }

        public static Rectangle Frame(int x, int y)
        {
            return Texture.Frame(InterfaceFramesX, 3, x, y);
        }
    }
}