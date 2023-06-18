namespace PipetteMod.Helpers;

internal static class InventoryHelper
{
    private const int hotbarLength = 10;

    internal struct Slot
    {
        internal Item Item { get; private set; }
        internal int Index { get; private set; }

        internal Slot(Item _item, int _index)
        {
            Item = _item;
            Index = _index;
        }
    }

    internal static bool TryGetMatchingItem(Player player, Tile targetTile, bool isAWall, out Slot matchingItemSlot)
    {
        if (!isAWall && TileHelper.TryGetFrameImportantStyle(targetTile, out int style)) // Make sure the right style is picked for FrameImportant tiles
        {
            matchingItemSlot = GetSlotFromItem(player, player.inventory.FirstOrDefault(x => x.createTile == targetTile.TileType && x.placeStyle == style));

            return matchingItemSlot.Item is not null && player.HeldItem != matchingItemSlot.Item;
        }

        bool TilePredicate(Item x) => !x.IsAir && x.createTile == targetTile.TileType;
        bool WallPredicate(Item x) => !x.IsAir && x.createWall == targetTile.WallType;

        Item item = player.inventory.FirstOrDefault(isAWall ? WallPredicate : TilePredicate);
        matchingItemSlot = GetSlotFromItem(player, item);

        if (matchingItemSlot.Item is null) return false;
        else return matchingItemSlot.Item.type != player.HeldItem.type;
    }

    internal static bool TryFindEmptySlotInHotbar(Player player, out Slot emptyHotbarSlot)
    {
        if (player.HeldItem.IsAir)
        {
            emptyHotbarSlot = GetSlotFromItem(player, player.HeldItem);
            return true;
        }

        Item[] inventory = player.inventory;
        Slot slot = new(null, -1);

        for (int i = 0; i < hotbarLength; i++)
        {
            Item item = inventory[i];
            if (item.IsAir)
            {
                slot = GetSlotFromItem(player, item);
                break;
            }
            else continue;
        }

        emptyHotbarSlot = slot;
        return slot.Item is not null;
    }

    internal static bool TryGetIndexOfItemInHotbar(Player player, Item item, out int hotbarItemIndex)
    {
        Item[] inventory = player.inventory;

        for (int i = 0; i < hotbarLength; i++)
        {
            if (inventory[i].type == item.type)
            {
                hotbarItemIndex = i;
                return true;
            }
            else continue;
        }

        hotbarItemIndex = -1;
        return false;
    }

    internal static void MoveItemToSlotAndSwitch(Player player, Slot emptyItem, Slot matchingItem)
    {
        player.inventory[emptyItem.Index] = matchingItem.Item;
        player.inventory[matchingItem.Index].TurnToAir();

        player.changeItem = emptyItem.Index;
    }

    internal static void SwapTwoItems(Player player, Slot x, Slot y) =>
        (player.inventory[x.Index], player.inventory[y.Index]) = 
            (player.inventory[y.Index], player.inventory[x.Index]);

    internal static bool TryGetHeldItemSlot(Player player, out Slot heldItemSlot)
    {
        heldItemSlot = GetSlotFromItem(player, player.HeldItem);
        return heldItemSlot.Item is not null;
    }

    private static Slot GetSlotFromItem(Player player, Item item) => 
        new Slot(item, Array.IndexOf(player.inventory, item));
}