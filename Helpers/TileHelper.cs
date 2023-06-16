using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace PipetteMod.Helpers
{
    internal static class TileHelper
    {
        internal struct GroupedTileInfo
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
        internal static readonly GroupedTileInfo[] groupedTilesWithFrameLengths = new GroupedTileInfo[]
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

        internal static readonly int[] distanceBlocks = new int[]
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

        internal static bool TryGetTileGroupId(Tile tile, out int tileGroupId)
        {
            tileGroupId = groupedTilesWithFrameLengths.SingleOrDefault(x => x.Id == tile.TileType).Id;
            return tileGroupId != 0;
        }
    }
}
