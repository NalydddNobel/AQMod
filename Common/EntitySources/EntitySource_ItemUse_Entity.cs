using Terraria;
using Terraria.DataStructures;

namespace Aequus.Common.EntitySources {
    public class EntitySource_ItemUse_WithEntity : EntitySource_Parent, IEntitySource_WithStatsFromItem {
        public Player Player { get; private set; }

        public Item Item { get; private set; }

        public EntitySource_ItemUse_WithEntity(Entity entity, Player player, Item item, string? context = null) : base(entity, context) {
            Player = player;
            Item = item;
        }
    }
}
