using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

namespace PipetteMod.Helpers
{
    internal static class TileHelper
    {
        internal static bool TryGetTileAtMousePosition(Player player, Vector2 mouseWorldPosition, bool pickWalls, bool disablePickWhileInteracting, out Tile matchingTile)
        {
            Tile tileAtMousePosition = Framing.GetTileSafely(mouseWorldPosition);
            matchingTile = tileAtMousePosition;

            if (IsTileAir(tileAtMousePosition) ||
                (IsTileWallOnly(tileAtMousePosition) && !pickWalls) ||
                (disablePickWhileInteracting && Main.tileContainer[matchingTile.TileType] && player.Distance(mouseWorldPosition) < 100)) // Don't pick chests if the player is in range to interact with them (depending on config)
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

        internal static bool TryGetFrameImportantStyle(Tile tile, out int style)
        {
            bool frameImportant = Main.tileFrameImportant[tile.TileType];

            style = TileObjectData.GetTileStyle(tile); // Well that was easier than I expected...

            return frameImportant;
        }
    }
}
