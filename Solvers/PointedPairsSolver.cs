using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Solvers
{
    public class PointedPairsSolver : ISolver
    {
        private NakedSingleCellsSolver _nakedSingleCellsSolver;

        public PointedPairsSolver(NakedSingleCellsSolver nakedSingleCellsSolver)
        {
            this._nakedSingleCellsSolver = nakedSingleCellsSolver;
        }

        public void ProcessBoard(SudokuBoard board)
        {
            for(int row = 0; row < 9; row++) {
                for(int col = 0; col < 9; col++) {
                    Cell cell = board.Board[row][col];
                    int Br = Box.ToBoxCoordinate(row);
                    int Bc = Box.ToBoxCoordinate(col);
                    if(!cell.Locked) {
                        foreach(int v in cell.CandidateValues) {
                            // Look in row of this box for the same candidate
                            List<int> BoxCandidateValues = GetRowCandidateValues(board, row,col,BlockScope.InsideBlock);
                            List<int> RowCandidateValues = GetRowCandidateValues(board, row,col,BlockScope.OutsideBlock);
                            if(BoxCandidateValues.Contains(v) && !RowCandidateValues.Contains(v)){
                                RowPointedPairClearBox(board, v, Br, Bc, row);
                            }

                            // Look in column of this box for the same candidate
                            BoxCandidateValues = GetColumnCandidateValues(board, row, col,BlockScope.InsideBlock);
                            List<int> ColumnCandidateValues = GetColumnCandidateValues(board, row, col, BlockScope.OutsideBlock);
                            if(BoxCandidateValues.Contains(v) && !ColumnCandidateValues.Contains(v)) {
                                ColumnPointedPairClearBox(board, v, Br, Bc, col);
                            }
                        }
                        this._nakedSingleCellsSolver.ProcessBoard(board);
                    }
                }
            }
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

        /// <summary>
        /// Clears a value fromt the box when given a row pointed pair
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Br"></param>
        /// <param name="Bc"></param>
        /// <param name="row"></param>
        public void RowPointedPairClearBox(SudokuBoard board, int Value, int Br, int Bc, int row)
        {
            #if DEBUG
            Console.WriteLine(String.Format("ROW Pointed Pair on {0} in box ({1},{2})", Value, Br, Bc));
            #endif
            for (int r = Box.ToBoardCoordinate(Br); r < Box.ToBoardCoordinate(Br) + 3; r++)
            {
                for (int c = Box.ToBoardCoordinate(Bc); c < Box.ToBoardCoordinate(Bc) + 3; c++)
                {
                    if (r != row)
                    {
                        #if DEBUG
                        Console.WriteLine(String.Format("CLEAR {0} from ({1},{2})", Value, r, c));
                        #endif
                        board.Board[r][c].RemoveCandidate(Value);
                    }
                }
            }
        }

        /// <summary>
        /// Clears a value from a box when given a column pointed pair
        /// </summary>
        /// <param name="Value">Value to clear in the box</param>
        /// <param name="Br">Box column index</param>
        /// <param name="Bc">Box row index</param>
        /// <param name="col">Column that the pointed pair is in, don't clear the value from this column</param>
        public void ColumnPointedPairClearBox(SudokuBoard board, int Value, int Br, int Bc, int col)
        {
            #if DEBUG
            Console.WriteLine(String.Format("COLUMN Pointed Pair on {0} in box({1},{2})", Value, Br, Bc));
            #endif
            for (int r = Box.ToBoardCoordinate(Br); r < Box.ToBoardCoordinate(Br) + 3; r++)
            {
                for (int c = Box.ToBoardCoordinate(Bc); c < Box.ToBoardCoordinate(Bc) + 3; c++)
                {
                    if (c != col)
                    {
                        #if DEBUG
                        Console.WriteLine(String.Format("CLEAR {0} from ({1},{2})", Value, r, c));
                        #endif
                        board.Board[r][c].RemoveCandidate(Value);
                    }
                }
            }
        }

    }
}
