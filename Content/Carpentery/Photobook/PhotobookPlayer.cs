using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

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

        public override void SaveData(TagCompound tag)
        {
            if (photos == null)
                return;

            try
            {
                var photos = new List<TagCompound>();
                for (int i = 0; i < this.photos.Length; i++)
                {
                    if (this.photos[i].HasData)
                    {
                        photos.Add(this.photos[i].SerializeData());
                    }
                }
                if (photos.Count < 0)
                    return;

                tag["Photos"] = photos;
            }
            catch (Exception ex)
            {
                Mod.Logger.Error($"{ex.Message}\n{ex.StackTrace}");
            }
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet<List<TagCompound>>("Photos", out var photoSaveData))
            {
                if (photos.Length < photoSaveData.Count)
                {
                    Array.Resize(ref photos, photoSaveData.Count);
                }
                for (int i = 0; i < photoSaveData.Count; i++)
                {
                    photos[i] = PhotoData.DeserializeData(photoSaveData[i]);
                }
            }
            return;
        }
    }
}