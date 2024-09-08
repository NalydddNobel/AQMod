namespace Aequus.Content.Items.Accessories.RespawnFeather;

/*
public class GoldenFeatherDrop {
    private readonly Rectangle InactiveRange = new Rectangle(NPC.safeRangeX * -16, NPC.safeRangeY * -16, NPC.safeRangeX * 32, NPC.safeRangeX * 32);
    private readonly Rectangle ActiveRange = new Rectangle(NPC.safeRangeX * -24, NPC.safeRangeY * -24, NPC.safeRangeX * 32, NPC.safeRangeX * 32);

    private Vector2 Position;

    private LinkedList<ItemReturnInfo> _items = new();

    public void AddItem(ItemReturnInfo item) {
        _items.AddFirst(item);
    }

    internal bool SoftUpdate(Player player) {
        return player.Hitbox.Intersects(InactiveRange);
    }

    internal bool ActiveUpdate(Player player) {
        if (!player.Hitbox.Intersects(ActiveRange)) {
            return false;
        }


        return true;
    }
}

public readonly record struct ItemReturnInfo(Item Item, IReturnSlot? Slot);

public interface IReturnSlot {
    void ReturnToPlayer(Player player, List<Item> ReturnList);
}

public readonly record struct ReturnToInventory(int Slot) : IReturnSlot {
    readonly void IReturnSlot.ReturnToPlayer(Player player, List<Item> ReturnList) {
        Item itemInSlot = player.inventory[Slot];

        if (itemInSlot.IsAir) {
            ReturnList.Add(itemInSlot);

        }
    }
}

public class GoldenFeatherPlayer : ModPlayer {
    private readonly LinkedList<GoldenFeatherDrop> _active = new();
    private readonly LinkedList<GoldenFeatherDrop> _drops = new();

    private LinkedListNode<GoldenFeatherDrop>? _currentNode;

    public override void Load() {
        //On_Player.dropitem
    }

    public override void PostUpdate() {
        if (_drops.Count == 0) {
            _currentNode = null;
            return;
        }

        if (_currentNode != null) {
            LinkedListTools.Step(ref _currentNode);
        }

        _currentNode ??= _drops.First!;

        if (_currentNode.Value.SoftUpdate(Player)) {
            _active.AddFirst(_currentNode.Value);
        }

        LinkedListTools.Where(_active, ActiveUpdate);
    }

    bool ActiveUpdate(GoldenFeatherDrop drop) {
        return drop.ActiveUpdate(Player);
    }
}
*/