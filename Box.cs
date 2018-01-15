using System;
using System.Collections.Generic;

namespace Sudoku
{
	class Box
	{

		/// <summary>
		/// Converts a board coordinate (0-8) to a box coordinate (0-2)
		/// </summary>
		/// <param name="BoardCoordinate">Board coordinate to convert</param>
		/// <returns>Box of corresponding board coordinate</returns>
		public static int ToBoxCoordinate(int BoardCoordinate)
		{
			return (int)Math.Floor((double)BoardCoordinate/3);
		}

		/// <summary>
		/// Converts a box coordinate to a board coordinate
		/// </summary>
		/// <param name="BoxCoordinate">Box coordiante to convert</param>
		/// <returns>Board coordinate of upper right cell in the box</returns>
		public static int ToBoardCoordinate(int BoxCoordinate){
			return BoxCoordinate * 3;
		}

		/// <summary>
		/// Returns a list of board coordinates given a box coordinate
		/// </summary>
		/// <param name="Bi">The box coordinate to convert</param>
		/// <returns></returns>
		public static List<int> BoardCoordinates(int Bi)
		{
			if(Bi < 0 || Bi > 2) {
				throw new ArgumentOutOfRangeException("Bi", Bi, "Must be between 0 and 2");
			}
			int BoardStart = Box.ToBoardCoordinate(Bi);
			return new List<int>() { BoardStart, BoardStart+1, BoardStart+2 };
		}
	}
}
