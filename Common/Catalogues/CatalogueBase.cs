using Terraria.ModLoader;

namespace Aequus.Common.Catalogues
{
    /// <summary>
    /// A base for a sets provider related class.
    /// </summary>
    public abstract class CatalogueBase : ModType
    {
        protected sealed override void Register()
        {
        }

        public sealed override void Load()
        {
            InitalizeMiscEntries();
            SetupVanillaEntries();
        }

        public sealed override void SetupContent()
        {
            LoadCrossModEntries();
            LoadAutomaticEntries();
            RemoveUnwantedEntries();
        }

        public sealed override void SetStaticDefaults()
        {
        }

        public virtual void InitalizeMiscEntries()
        {
        }

        public virtual void SetupVanillaEntries()
        {
        }

        public virtual void LoadCrossModEntries()
        {
        }

        public virtual void LoadAutomaticEntries()
        {
        }

        public virtual void RemoveUnwantedEntries()
        {
        }
    }
}