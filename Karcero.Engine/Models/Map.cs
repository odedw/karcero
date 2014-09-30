using System;
using System.Collections.Generic;
using System.Linq;
using Karcero.Engine.Contracts;
using Karcero.Engine.Helpers;

namespace Karcero.Engine.Models
{
    /// <summary>
    /// A matrix of cells.
    /// </summary>
    /// <typeparam name="T">The type of cells the map is comprised of.</typeparam>
    public class Map<T> where T : class, IBaseCell, new()
    {
        #region Properties
        /// <summary>
        /// The height of the map.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// The width of the map.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// A collection of rooms that the are contained in the map.
        /// </summary>
        public List<Room> Rooms { get; set; }

        /// <summary>
        /// A collection of all of the cells in the map generated with a LINQ operation (order not guaranteed).
        /// </summary>
        public IEnumerable<T> AllCells
        {
            get
            {
                return mMap.SelectMany(cells => cells);
            }
        }

        private readonly T[][] mMap;
        #endregion

        #region Constructor
        /// <summary>
        /// Returns an instance initialized to the sizes specified.
        /// </summary>
        /// <param name="width">The desired width of the map.</param>
        /// <param name="height">The desired height of the map.</param>
        public Map(int width, int height)
        {
            Height = height;
            Width = width;
            mMap = new T[Height][];
            for (int i = 0; i < Height; i++)
            {
                mMap[i] = new T[Width];
                for (int j = 0; j < Width; j++)
                {
                    mMap[i][j] = new T() { Row = i, Column = j };
                }
            }

            Rooms = new List<Room>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns an the adjacent cell of the specified cell to a certain direction and distance.
        /// </summary>
        /// <param name="cell">The initial cell.</param>
        /// <param name="direction">The direction of the desired adjacent cell.</param>
        /// <param name="distance">How many cells apart the adjacent cell returned is (defaults to 1).</param>
        /// <returns>The adjacent cell to the direction and distance specified.</returns>
        public T GetAdjacentCell(T cell, Direction direction, int distance = 1)
        {
            switch (direction)
            {
                case Direction.South:
                    return cell.Row + distance >= Height ? null : GetCell(cell.Row + distance, cell.Column);
                case Direction.East:
                    return cell.Column + distance >= Width ? null : GetCell(cell.Row, cell.Column + distance);
                case Direction.North:
                    return cell.Row - distance < 0 ? null : GetCell(cell.Row - distance, cell.Column);
                case Direction.West:
                    return cell.Column - distance < 0 ? null : GetCell(cell.Row, cell.Column-distance);
            }
            return null;
        }

        /// <summary>
        /// Try and get the adjacent cell of the specified cell to a certain direction and distance.
        /// </summary>
        /// <param name="cell">The initial cell.</param>
        /// <param name="direction">The direction of the desired adjacent cell.</param>
        /// <param name="adjacentCell">Will be set to the adjacent cell if exists, null otherwise.</param>
        /// <returns>True if the desired adjacent exists.</returns>
        public bool TryGetAdjacentCell(T cell, Direction direction, out T adjacentCell)
        {
            adjacentCell = GetAdjacentCell(cell, direction);
            return adjacentCell != null;
        }

        /// <summary>
        /// Returns the cell in a specified location.
        /// </summary>
        /// <param name="row">The row of the desired cell.</param>
        /// <param name="column">The column of the desired cell.</param>
        /// <returns>The desired cell if exists, null otherwise.</returns>
        public T GetCell(int row, int column)
        {
            return row >= 0 && column >= 0 && row < Height && column < Width ? mMap[row][column] : null;
        }

        /// <summary>
        /// Get the cells the specified room is comprised of.
        /// </summary>
        /// <param name="room">The desired room.</param>
        /// <returns>The cells the specified room is comprised of.</returns>
        public IEnumerable<T> GetRoomCells(Room room)
        {
            var cells = new List<T>();
            for (var i = room.Row; i < Math.Min(room.Bottom, Height); i++)
            {
                for (var j = room.Column; j < Math.Min(room.Right, Width); j++)
                {
                    cells.Add(GetCell(i,j));
                }    
            }
            return cells;
        }

        /// <summary>
        /// Get the cells adjacent to all of the edges of the room specified in a certain distance.
        /// </summary>
        /// <param name="room">The desired room.</param>
        /// <param name="distance">The desired distance (defaults to 1).</param>
        /// <returns>A collection of all of the cells adjacent to all of the edges of the room specified in the desired distance.</returns>
        public IEnumerable<T> GetCellsAdjacentToRoom(Room room, int distance = 1)
        {
            var cells = new List<T>();
            for (var j = room.Column; j < Math.Min(room.Right, Width); j++)
            {
                if (room.Row >= distance) cells.Add(GetAdjacentCell(GetCell(room.Row, j), Direction.North, distance));
                if (room.Bottom <= Height - distance) cells.Add(GetAdjacentCell(GetCell(room.Bottom - 1, j), Direction.South, distance));
            }

            for (var i = room.Row; i < Math.Min(room.Bottom, Height); i++)
            {

                if (room.Column >= distance) cells.Add(GetAdjacentCell(GetCell(i, room.Column), Direction.West, distance));
                if (room.Right <= Width - distance) cells.Add(GetAdjacentCell(GetCell(i, room.Right - 1), Direction.East, distance));

            }
            return cells;
        } 

        /// <summary>
        /// Returns true if a cell location is inside any room on the map.
        /// </summary>
        /// <param name="row">The desired row.</param>
        /// <param name="column">The desired column.</param>
        /// <returns>True if the specified location is in any room on the map.</returns>
        public bool IsLocationInRoom(int row, int column)
        {
            return Rooms.Any(room => room.IsLocationInRoom(row, column));
        }

        /// <summary>
        /// Returns all of the adjacent cells of the specified cell.
        /// </summary>
        /// <param name="cell">The desired cell.</param>
        /// <param name="includeDiagonalCells">True if diagonal adjacent cells should be included (defaults to false).</param>
        /// <returns>All of the adjacent cells of the specified cell.</returns>
        public IEnumerable<T> GetAllAdjacentCells(T cell, bool includeDiagonalCells = false)
        {
            var cells = GetAll.ValuesOf<Direction>()
                .Where(direction => GetAdjacentCell(cell, direction) != null)
                .Select(direction => GetAdjacentCell(cell, direction)).ToList();

            if (includeDiagonalCells)
            {
                if (GetCell(cell.Row - 1, cell.Column - 1) != null) cells.Add(GetCell(cell.Row - 1, cell.Column - 1));
                if (GetCell(cell.Row + 1, cell.Column - 1) != null) cells.Add(GetCell(cell.Row + 1, cell.Column - 1));
                if (GetCell(cell.Row - 1, cell.Column + 1) != null) cells.Add(GetCell(cell.Row - 1, cell.Column + 1));
                if (GetCell(cell.Row + 1, cell.Column + 1) != null) cells.Add(GetCell(cell.Row + 1, cell.Column + 1));
            }
            return cells;
        }

        /// <summary>
        /// Returns all of the adjacent cells of the specified cell by cardinal direction.
        /// </summary>
        /// <param name="cell">The desired cell.</param>
        /// <returns>All of the adjacent cells of the specified cell by cardinal direction..</returns>
        public Dictionary<Direction, T> GetAllAdjacentCellsByDirection(T cell)
        {
            return GetAll.ValuesOf<Direction>()
                .ToDictionary(direction => direction, direction => GetAdjacentCell(cell, direction));
        }
        #endregion
    }
}
