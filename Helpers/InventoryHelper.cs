using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace PipetteMod.Helpers
{
    internal static class InventoryHelper
    {
        private const int hotbarLength = 10;

        private struct GroupedTileInfo
        {
            internal int Id { get; private set; }
            internal short FrameLength { get; private set; }
            internal char Axis { get; private set; }

            internal GroupedTileInfo(int _id, short _frameLength, char _axis)
            {
                Id = _id;
                FrameLength = _frameLength;
                Axis = _axis;
            }
        }

        // This is the best way I could find to match (most) grouped tiles with separate items to avoid selecting the default item every time.
        // But it requires a lot of manual labour, so this approach is tentative. I will probably try to improve this in the future
        private static readonly GroupedTileInfo[] groupedTilesWithFrameLengths = new GroupedTileInfo[]
        {
            new GroupedTileInfo(TileID.Torches, 22, 'y'),
            new GroupedTileInfo(TileID.Containers, 36, 'x'),
            new GroupedTileInfo(TileID.Containers2, 36, 'x'),
            new GroupedTileInfo(TileID.FakeContainers, 36, 'x'),
            new GroupedTileInfo(TileID.FakeContainers2, 36, 'x'),
            new GroupedTileInfo(TileID.GemSaplings, 54, 'x'),
            new GroupedTileInfo(TileID.WorkBenches, 36, 'x'),
            new GroupedTileInfo(TileID.Tables, 54, 'x'),
            new GroupedTileInfo(TileID.Bookcases, 54, 'x'),
            new GroupedTileInfo(TileID.BeachPiles, 18, 'y')
        };

        private static readonly int[] distanceBlocks = new int[]
        {
            TileID.Containers,
            TileID.Containers2,
            TileID.FakeContainers,
            TileID.FakeContainers2,
            TileID.PiggyBank,
            TileID.Safes,
            TileID.DefendersForge,
            TileID.VoidVault
        };

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

        internal static bool TryGetTileAtMousePosition(Player player, Vector2 mouseWorldPosition, bool pickWalls, bool disablePickWhileInteracting, out Tile matchingTile)
        {
            Tile tileAtMousePosition = Framing.GetTileSafely(mouseWorldPosition);
            matchingTile = tileAtMousePosition;

            if (IsTileAir(tileAtMousePosition) ||
                (IsTileWallOnly(tileAtMousePosition) && !pickWalls) ||
                (disablePickWhileInteracting && distanceBlocks.Any(id => id == tileAtMousePosition.TileType) && player.Distance(mouseWorldPosition) < 100)) // Don't pick chests if the player is in range to interact with them (depending on config)
                return false;

            return true;
        }

        internal static bool TryGetMatchingItem(Player player, Tile targetTile, bool isAWall, out Slot matchingItemSlot)
        {
            if (!isAWall && TryGetTileGroupId(targetTile, out int tileGroupId)) // Make sure the right item is picked for variations of the same tile
            {
                if (tileGroupId == 0)
                {
                    matchingItemSlot = GetSlotFromItem(player, player.inventory.FirstOrDefault(x => x.createTile == targetTile.TileType));

                    return matchingItemSlot.Item is not null;
                }
                else
                {
                    GroupedTileInfo info = groupedTilesWithFrameLengths.Single(x => x.Id == tileGroupId);
                    int frameLength = info.FrameLength;
                    short axis = info.Axis == 'x' ? targetTile.TileFrameX : targetTile.TileFrameY;

                    int tilePlaceStyle = axis / frameLength;

                    matchingItemSlot = GetSlotFromItem(player, player.inventory.FirstOrDefault(x =>
                    x.createTile == info.Id &&
                    x.placeStyle == tilePlaceStyle));

                    return matchingItemSlot.Item is not null && player.HeldItem != matchingItemSlot.Item;
                }
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

        internal static void SwapTwoItems(Player player, Slot x, Slot y)
        {
            (player.inventory[x.Index], player.inventory[y.Index]) = (player.inventory[y.Index], player.inventory[x.Index]);
        }

        internal static bool IsTileAir(Tile tile)
        {
            bool tileHasWallOnly = IsTileWallOnly(tile);
            return !tile.HasTile && !tileHasWallOnly;
        }

        internal static bool IsTileWallOnly(Tile tile)
        {
            // Dirt blocks and walls are both type 0. So using tile.WallType > 0 or tile.TileType > 0 won't help you discern whether the tile or wall is air.
            // tile.HasTile helps here, but there is no equivalent for walls, so dirt walls are not supported by this.
            // The mod should still work for dirt walls though, because TryGetMatchingItem() takes care of the rest on its own.

            bool hasAnyWallThatIsNotDirt = tile.WallType > 0;
            return !tile.HasTile && hasAnyWallThatIsNotDirt;  // Always prioritize picking tiles over walls
        }

        internal static bool TryGetHeldItemSlot(Player player, out Slot heldItemSlot)
        {
            heldItemSlot = GetSlotFromItem(player, player.HeldItem);
            return heldItemSlot.Item is not null;
        }

        private static bool TryGetTileGroupId(Tile tile, out int tileGroupId)
        {
            tileGroupId = groupedTilesWithFrameLengths.SingleOrDefault(x => x.Id == tile.TileType).Id;
            return tileGroupId != 0;
        }

        private static Slot GetSlotFromItem(Player player, Item item) => new Slot(item, Array.IndexOf(player.inventory, item));
    }
}