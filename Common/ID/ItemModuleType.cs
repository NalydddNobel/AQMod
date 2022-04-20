using Terraria.ModLoader;

namespace Aequus.Common.ID
{
    public sealed class ItemModuleType : ILoadable
    {
        public const int BarbDamaging = 0;
        public const int BarbDebuff = 1;
        public const int BarbMovementOverhaul = 2;
        public const int BarbMeathook = 3;
        public const int MaxModuleTypes = 4;

        private static int reservedID;

        public static int Count => reservedID;

        /// <summary>
        /// Gives you an ID to classify a specific item module type
        /// <para>
        /// It may be confusing to wrap around your head what this ID is used for, but this example should help:
        /// <code>public static int MyCustomModuleType;</code>
        /// <code>...Load()</code>
        /// <code>MyCustomEquipType = ItemModuleType.GetReservedID();</code>
        /// <code>...SetStaticDefaults() in a Grappling Hook Item...</code>
        /// <code>ModularItemManager.Catalogue.AllowEquipType(Type, MyCustomEquipType);</code>
        /// </para>
        /// </summary>
        /// <returns></returns>
        public static int GetReservedID()
        {
            reservedID++;
            return reservedID - 1;
        }

        void ILoadable.Load(Mod mod)
        {
            ResetReservedIDToCount();
        }

        void ILoadable.Unload()
        {
            ResetReservedIDToCount();
        }

        private static void ResetReservedIDToCount()
        {
            reservedID = MaxModuleTypes;
        }
    }
}