using Terraria.ModLoader.IO;

namespace Aequus.Content.StatSheet
{
    public interface IStatTracker
    {
        string Name { get; set; }
        string DisplayName { get; set; }

        void Update(IStatUpdateInfo info);
        string ProvideStatText();
        object ProvideSaveData();
        void LoadSaveData(TagCompound tag);
    }
}
