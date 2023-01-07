using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Carpentery.Photobook
{
    public class PhotobookPlayer : ModPlayer
    {
        public bool hasPhotobook;
        public int maxPhotos;
        public PhotoData[] photos;
        public PhotoData[] Photos 
        { 
            get
            {
                if (photos.Length < maxPhotos)
                {
                    Array.Resize(ref photos, maxPhotos);
                }
                return photos;
            }
            private set 
            {
                photos = value;
            } 
        }

        public static int MyMaxPhotos => Main.LocalPlayer.GetModPlayer<PhotobookPlayer>().maxPhotos;

        public override void Initialize()
        {
            maxPhotos = 20;
            photos = new PhotoData[maxPhotos];
        }

        public override void ResetEffects()
        {
            hasPhotobook = false;
            maxPhotos = 20;
            if (photos == null)
                photos = new PhotoData[maxPhotos];
        }

        public void UpgradePhotos(int newMax)
        {
            maxPhotos = Math.Max(maxPhotos, newMax);
        }
    }
}