using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Solvers
{
    public class HiddenSingleCellsSolver
    {
        private NakedSingleCellsSolver _nakedSingleCellsSolver;

        public HiddenSingleCellsSolver(NakedSingleCellsSolver nakedSingleCellsSolver)
        {
            this._nakedSingleCellsSolver = nakedSingleCellsSolver;
        }

        public void ProcessBoard(SudokuBoard board)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    this._nakedSingleCellsSolver.ProcessBoard(board);
                    Cell c = board.Board[row][col];
                    if (!c.Locked)
                    {
                        List<int> CandidateInBox = GetBoxCandidateValues(board, Box.ToBoxCoordinate(row), Box.ToBoxCoordinate(col), row, col);
                        List<int> CandidateInRow = GetRowCandidateValues(board, row, col);
                        List<int> CandidateInCol = GetColumnCandidateValues(board, row, col);

                        foreach (int v in c.CandidateValues)
                        {
                            // Find if the value v is available in other cells in the box
                            if (!CandidateInBox.Contains(v))
                            {
                                board.LockCell(c, row, col, v);
                                break;
                            }
                            if (!CandidateInRow.Contains(v))
                            {
                                board.LockCell(c, row, col, v);
                                break;
                            }
                            if (!CandidateInCol.Contains(v))
                            {
                                board.LockCell(c, row, col, v);
                                break;
                            }
                        }
                    }
                }
            }
        }

		/// <summary>
		/// Get a list of the available values in non-locked cells in the box, but not from the given cell at (row,col)
		/// </summary>
		/// <param name="Bx">Box x coordinate</param>
		/// <param name="By">Box y coordinate</param>
		/// <param name="row">Row coordinate of cell to exclude</param>
		/// <param name="col">Column coordinate of cell to exclude</param>
		/// <returns></returns>
		private List<int> GetBoxCandidateValues(SudokuBoard board, int Bx, int By, int row, int col)
		{
			List<int> CandidateValuesInBox = new List<int>();
			for(int i = Box.ToBoardCoordinate(Bx); i < Box.ToBoardCoordinate(Bx) + 3; i++) {
				for(int j = Box.ToBoardCoordinate(By); j < Box.ToBoardCoordinate(By) + 3; j++){
					// Skip any locked cells and the given cell
					if(!board.Board[i][j].Locked && !(i == row && j == col)){
						CandidateValuesInBox.AddRange(board.Board[i][j].CandidateValues);
					}
				}
			}
			CandidateValuesInBox = CandidateValuesInBox.Distinct().ToList();
			CandidateValuesInBox.Sort();
			return CandidateValuesInBox;
		}

		/// <summary>
		/// Get a list of the available values in non-locked cells in the row, but not from the given cell at (row,col)
		/// </summary>
		/// <param name="row">Row coordinate of cell to exclude (also the row to get values from)</param>
		/// <param name="col">Column coordinate of cell to exclude</param>
		/// <returns></returns>
		private List<int> GetRowCandidateValues(SudokuBoard board, int row, int col, BlockScope Scope = BlockScope.FullBlock)
		{
			List<int> CandidateValuesInRow = new List<int>();
			int BoardBoxMin = Box.ToBoardCoordinate(Box.ToBoxCoordinate(col));
			int BoardBoxMax = Box.ToBoardCoordinate(Box.ToBoxCoordinate(col))+2;

			for(int c = 0; c < 9; c++) {
				if(c != col && !board.Board[row][c].Locked) {
					if( (Scope.HasFlag(BlockScope.InsideBlock) && (c >= BoardBoxMin && c <= BoardBoxMax)) || // If we want inside the block, and this column is inside
						(Scope.HasFlag(BlockScope.OutsideBlock) && (c < BoardBoxMin || c >BoardBoxMax))      // If we want outside the block, and this column is outside
					) {
						CandidateValuesInRow.AddRange(board.Board[row][c].CandidateValues);
					}
				}
			}
			CandidateValuesInRow = CandidateValuesInRow.Distinct().ToList();
			CandidateValuesInRow.Sort();
			return CandidateValuesInRow;
		}

		/// <summary>
		/// Get a list of the available values in non-locked cells in the column, but not from the given cell at (row,col)
		/// </summary>
		/// <param name="row">Row coordinate of cell to exclude (also the row to get values from)</param>
		/// <param name="col">Column coordinate of cell to exclude</param>
		/// <returns></returns>
		private List<int> GetColumnCandidateValues(SudokuBoard board, int row, int col, BlockScope Scope = BlockScope.FullBlock)
		{
			List<int> CandidateValuesInColumn = new List<int>();
			int BoardBoxMin = Box.ToBoardCoordinate(Box.ToBoxCoordinate(row));
			int BoardBoxMax = Box.ToBoardCoordinate(Box.ToBoxCoordinate(row))+2;

			for(int r = 0; r < 9; r++) {
				if(r != row && !board.Board[r][col].Locked) {
					if( (Scope.HasFlag(BlockScope.InsideBlock) && (r >= BoardBoxMin && r <= BoardBoxMax)) || // If we want inside the block, and this column is inside
						(Scope.HasFlag(BlockScope.OutsideBlock) && (r < BoardBoxMin || r > BoardBoxMax))     // If we want outside the block, and this column is outside
					) {
						CandidateValuesInColumn.AddRange(board.Board[r][col].CandidateValues);
					}
				}
			}
			CandidateValuesInColumn = CandidateValuesInColumn.Distinct().ToList();
			CandidateValuesInColumn.Sort();
			return CandidateValuesInColumn;
		}

		private void LockCell(SudokuBoard board, Cell cell, int row, int col, int v = 0)
		{
			if(v == 0) {
				LockCell(board, cell, row, col, cell.CandidateValues[0]);
				return;
			}
			// Get all the set values in the row, column and box check if one of those values is what we're trying to set this cell to
			if(board.GetRow(row).Contains((int)v)) {
				board.Print(PrintStyle.CandidateCount);
				throw new Exception(String.Format("ROW Confilct at ({0},{1}) with value {2}", row, col, v));
			}
			if(board.GetCol(col).Contains((int)v)){
				board.Print(PrintStyle.CandidateCount);
				throw new Exception(String.Format("COLUMN Conflict at ({0},{1}) with value {2}", row, col, v));
			}
			if(board.GetBox( Box.ToBoxCoordinate(row), Box.ToBoxCoordinate(col)).Contains((int)v)){
				board.Print(PrintStyle.CandidateCount);
				throw new Exception(String.Format("BOX Conflict at ({0},{1}) with value {2}", row, col, v));
			}

			cell.LockValue(v);
			// Remove v from the available values in the row
			for(int c = 0; c < 9; c++) {
				board.Board[row][c].RemoveCandidate(v);
			}
			// Remove v from the available values in the column
			for(int r = 0; r < 9; r++) {
				board.Board[r][col].RemoveCandidate(v);
			}
			// Remove v from the available values in the box
			foreach(int r in Box.BoardCoordinates(Box.ToBoxCoordinate(row))) {
				foreach(int c in Box.BoardCoordinates(Box.ToBoxCoordinate(col))){
					board.Board[r][c].RemoveCandidate(v);
				}
			}
		}
    }
}
