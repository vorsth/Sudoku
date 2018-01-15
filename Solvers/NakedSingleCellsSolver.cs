using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Solvers
{
	/// <summary>
	/// Find available numbers in each cell. If, when removing available numbers, there is only one left, that now becomes the "locked" value
	/// </summary>
    public class NakedSingleCellsSolver
    {
        public void ProcessBoard(SudokuBoard board)
        {
			int HashCode = -1;
			while(HashCode != board.GetHashCode()) {
				HashCode = board.GetHashCode();
				for(int row = 0; row < 9; row++) {
					for(int col = 0; col < 9; col++) {
						if(!board.Board[row][col].Locked) {
							List<int?> NonValidValues = new List<int?>();
							NonValidValues.AddRange(board.GetRow(row));
							NonValidValues.AddRange(board.GetCol(col));
							NonValidValues.AddRange(board.GetBox(Box.ToBoxCoordinate(row), Box.ToBoxCoordinate(col)));
							NonValidValues = NonValidValues.Distinct().ToList();
							NonValidValues.Sort();
							board.Board[row][col].RemoveCandidates(NonValidValues);
						}
					}
				}
			}
        }
    }
}
