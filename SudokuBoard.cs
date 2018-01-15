using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku
{
	class SudokuBoard
	{
		public string Name { get; private set; }
		public List<List<Cell>> Board;
		public int SolveAttempts { get; set; }
		public int SolvedCells
		{
			get
			{
				int SolvedCount = 0;
				for(int i = 0; i < 9; i++) {
					for(int j = 0; j < 9; j++) {
						if(this.Board[i][j].Locked) {
							SolvedCount++;
						}
					}
				}
				return SolvedCount;
			}
		}

		public SudokuBoard(List<List<int>> b, string name = "UNKNOWN")
		{
			this.Name = name;
			this.Board = new List<List<Cell>>();
			this.SolveAttempts = 0;

			if(b.Count != 9) {
				throw new Exception("Row count off");
			}
			for(int row = 0; row < 9; row++){
				if(b[row].Count != 9) {
					throw new Exception(String.Format("Column count off Col:{0}", row));
				}
				this.Board.Add(new List<Cell>());
				for(int column = 0; column < 9; column++){
					this.Board[row].Add(new Cell(this,b[row][column]));
				}
			}

		}

		public bool IsSolved()
		{
			List<int?> RequiredValues = new List<int?>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

			for(int i = 0; i < 9; i++) {
				// Check each row for all values
				List<int?> RowData = this.GetRow(i);
				List<int?> ColData = this.GetCol(i);

				RowData.Sort();
				ColData.Sort();

				for(int j = 0; j < 9; j++) {
					if(RowData[j] != RequiredValues[j]) {
						return false;
					}
					if(ColData[j] != RequiredValues[j]) {
						return false;
					}
				}
			}

			for(int i = 0; i < 3; i++) {
				for(int j = 0; j < 3; j++) {
					List<int?> BoxData = this.GetBox(i, j);
					BoxData.Sort();
					for(int k = 0; k < 9; k++) {
						if(BoxData[k] != RequiredValues[k]) {
							return false;
						}
					}
				}
			}

			return true;
		}

		public void SetCell(int x, int y, int? value){
			if(x < 0 || x > 8 || y < 0 || y > 8){
				throw new Exception(String.Format("Coordinates ({0},{1}) out of range", x,y));
			}
			this.Board[x][y].Value = value;
		}

		/// <summary>
		/// Get the values that exist in the row
		/// </summary>
		/// <param name="r">Row to look at</param>
		/// <returns></returns>
		private List<int?> GetRow(int r)
		{
			return this.Board[r].Select(x => x.Value).ToList();
		}

		/// <summary>
		/// Get the values that exist in the column
		/// </summary>
		/// <param name="c">Column to look at</param>
		/// <returns></returns>
		private List<int?> GetCol(int c)
		{
			List<int?> CellValues = new List<int?>();
			foreach(List<Cell> row in this.Board) {
				CellValues.Add(row[c].Value);
			}
			return CellValues;
		}

		/// <summary>
		/// Get the values in the box
		/// </summary>
		/// <param name="Bx">Box coordinate x (0-2)</param>
		/// <param name="By">Box coordinate y (0-2)</param>
		/// <returns></returns>
		private List<int?> GetBox(int Bx, int By)
		{
			List<int?> CellValues = new List<int?>();
			for(int i = Box.ToBoardCoordinate(Bx); i < Box.ToBoardCoordinate(Bx)+3; i++) {
				for(int j = 3*By; j < 3*By+3; j++) {
					CellValues.Add(this.Board[i][j].Value);
				}
			}
			return CellValues;
		}

		/// <summary>
		/// Get a list of the available values in non-locked cells in the box, but not from the given cell at (row,col)
		/// </summary>
		/// <param name="Bx">Box x coordinate</param>
		/// <param name="By">Box y coordinate</param>
		/// <param name="row">Row coordinate of cell to exclude</param>
		/// <param name="col">Column coordinate of cell to exclude</param>
		/// <returns></returns>
		private List<int> GetBoxCandidateValues(int Bx, int By, int row, int col)
		{
			List<int> CandidateValuesInBox = new List<int>();
			for(int i = Box.ToBoardCoordinate(Bx); i < Box.ToBoardCoordinate(Bx) + 3; i++) {
				for(int j = Box.ToBoardCoordinate(By); j < Box.ToBoardCoordinate(By) + 3; j++){
					// Skip any locked cells and the given cell
					if(!this.Board[i][j].Locked && !(i == row && j == col)){
						CandidateValuesInBox.AddRange(this.Board[i][j].CandidateValues);
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
		private List<int> GetRowCandidateValues(int row, int col, BlockScope Scope = BlockScope.FullBlock)
		{
			List<int> CandidateValuesInRow = new List<int>();
			int BoardBoxMin = Box.ToBoardCoordinate(Box.ToBoxCoordinate(col));
			int BoardBoxMax = Box.ToBoardCoordinate(Box.ToBoxCoordinate(col))+2;

			for(int c = 0; c < 9; c++) {
				if(c != col && !this.Board[row][c].Locked) {
					if( (Scope.HasFlag(BlockScope.InsideBlock) && (c >= BoardBoxMin && c <= BoardBoxMax)) || // If we want inside the block, and this column is inside
						(Scope.HasFlag(BlockScope.OutsideBlock) && (c < BoardBoxMin || c >BoardBoxMax))      // If we want outside the block, and this column is outside
					) {
						CandidateValuesInRow.AddRange(this.Board[row][c].CandidateValues);
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
		private List<int> GetColumnCandidateValues(int row, int col, BlockScope Scope = BlockScope.FullBlock)
		{
			List<int> CandidateValuesInColumn = new List<int>();
			int BoardBoxMin = Box.ToBoardCoordinate(Box.ToBoxCoordinate(row));
			int BoardBoxMax = Box.ToBoardCoordinate(Box.ToBoxCoordinate(row))+2;

			for(int r = 0; r < 9; r++) {
				if(r != row && !this.Board[r][col].Locked) {
					if( (Scope.HasFlag(BlockScope.InsideBlock) && (r >= BoardBoxMin && r <= BoardBoxMax)) || // If we want inside the block, and this column is inside
						(Scope.HasFlag(BlockScope.OutsideBlock) && (r < BoardBoxMin || r > BoardBoxMax))     // If we want outside the block, and this column is outside
					) {
						CandidateValuesInColumn.AddRange(this.Board[r][col].CandidateValues);
					}
				}
			}
			CandidateValuesInColumn = CandidateValuesInColumn.Distinct().ToList();
			CandidateValuesInColumn.Sort();
			return CandidateValuesInColumn;
		}

		/// <summary>
		/// Gets the values that are still required in this column
		/// </summary>
		/// <param name="col"></param>
		/// <returns></returns>
		private List<int> GetColumnRequiredValues(int col)
		{
			List<int> RequiredValues = null;
			Cell.AllValues.CopyTo(RequiredValues.ToArray());
			for(int r = 0; r < 9; r++) {
				if(this.Board[r][col].Locked) {
					RequiredValues.Remove((int)(this.Board[r][col].Value));
				}
			}
			return RequiredValues;
		}

		/// <summary>
		/// Gets the values that are still required in this row
		/// </summary>
		/// <param name="col"></param>
		/// <returns></returns>
		public List<int> GetRowRequriedValues(int row)
		{
			List<int> RequiredValues = null;
			Cell.AllValues.CopyTo(RequiredValues.ToArray());
			for(int c = 0; c < 9; c++) {
				if(this.Board[row][c].Locked) {
					RequiredValues.Remove((int)(this.Board[row][c].Value));
				}
			}
			return RequiredValues;
		}

		/// <summary>
		/// Gets the values that are still required in the box
		/// </summary>
		/// <param name="Bx"></param>
		/// <param name="By"></param>
		/// <returns></returns>
		public List<int> GetBoxRequiredValues(int Bx, int By)
		{
			List<int> RequiredValues = null;
			Cell.AllValues.CopyTo(RequiredValues.ToArray());
			foreach(int r in Box.BoardCoordinates(Bx)) {
				foreach(int c in Box.BoardCoordinates(By)) {
					if(this.Board[r][c].Locked) {
						RequiredValues.Remove((int)(this.Board[r][c].Value));
					}
				}
			}
			return RequiredValues;
		}

		public void Solve()
		{
			this.SolveAttempts++;
			FindNakedSingleCells();
			FindHiddenSingleCells();
			FindPointedPairs();
		}

		/// <summary>
		/// Find available numbers in each cell. If, when removing available numbers, there is only one left, that now becomes the "locked" value
		/// </summary>
		public void FindNakedSingleCells()
		{
			int HashCode = -1;
			while(HashCode != this.GetHashCode()) {
				HashCode = this.GetHashCode();
				for(int row = 0; row < 9; row++) {
					for(int col = 0; col < 9; col++) {
						if(!this.Board[row][col].Locked) {
							List<int?> NonValidValues = new List<int?>();
							NonValidValues.AddRange(this.GetRow(row));
							NonValidValues.AddRange(this.GetCol(col));
							NonValidValues.AddRange(this.GetBox(Box.ToBoxCoordinate(row), Box.ToBoxCoordinate(col)));
							NonValidValues = NonValidValues.Distinct().ToList();
							NonValidValues.Sort();
							this.Board[row][col].RemoveCandidates(NonValidValues);
						}
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void FindHiddenSingleCells()
		{
			for(int row = 0; row < 9; row++) {
				for(int col = 0; col < 9; col++) {
					FindNakedSingleCells();
					Cell c = this.Board[row][col];
					if(!c.Locked) {
						List<int> CandidateInBox = GetBoxCandidateValues(Box.ToBoxCoordinate(row), Box.ToBoxCoordinate(col), row, col);
						List<int> CandidateInRow = GetRowCandidateValues(row, col);
						List<int> CandidateInCol = GetColumnCandidateValues(row, col);

						foreach(int v in c.CandidateValues) {
							// Find if the value v is available in other cells in the box
							if(!CandidateInBox.Contains(v)) {
								this.LockCell(c,row,col,v);
								break;
							}
							if(!CandidateInRow.Contains(v)) {
								this.LockCell(c,row,col,v);
								break;
							}
							if(!CandidateInCol.Contains(v)) {
								this.LockCell(c,row,col,v);
								break;
							}
						}
					}
				}
			}
		}

		public void FindPointedPairs()
		{
			for(int row = 0; row < 9; row++) {
				for(int col = 0; col < 9; col++) {
					Cell cell = this.Board[row][col];
					int Br = Box.ToBoxCoordinate(row);
					int Bc = Box.ToBoxCoordinate(col);
					if(!cell.Locked) {
						foreach(int v in cell.CandidateValues) {
							// Look in row of this box for the same candidate
							List<int> BoxCandidateValues = GetRowCandidateValues(row,col,BlockScope.InsideBlock);
							List<int> RowCandidateValues = GetRowCandidateValues(row,col,BlockScope.OutsideBlock);
							if(BoxCandidateValues.Contains(v) && !RowCandidateValues.Contains(v)){
								RowPointedPairClearBox(v, Br, Bc, row);
							}

							// Look in column of this box for the same candidate
							BoxCandidateValues = GetColumnCandidateValues(row, col,BlockScope.InsideBlock);
							List<int> ColumnCandidateValues = GetColumnCandidateValues(row, col, BlockScope.OutsideBlock);
							if(BoxCandidateValues.Contains(v) && !ColumnCandidateValues.Contains(v)) {
								ColumnPointedPairClearBox(v, Br, Bc, col);
							}
						}
						FindNakedSingleCells();
					}
				}
			}
		}

		/// <summary>
		/// Clears a value fromt the box when given a row pointed pair
		/// </summary>
		/// <param name="Value"></param>
		/// <param name="Br"></param>
		/// <param name="Bc"></param>
		/// <param name="row"></param>
		public void RowPointedPairClearBox(int Value, int Br, int Bc, int row)
		{
			#if DEBUG
			Console.WriteLine(String.Format("ROW Pointed Pair on {0} in box ({1},{2})", Value, Br, Bc));
			#endif
			for(int r = Box.ToBoardCoordinate(Br); r < Box.ToBoardCoordinate(Br) + 3; r++) {
				for(int c = Box.ToBoardCoordinate(Bc); c < Box.ToBoardCoordinate(Bc) + 3; c++) {
					if(r != row) {
						#if DEBUG
						Console.WriteLine(String.Format("CLEAR {0} from ({1},{2})", Value, r, c));
						#endif
						this.Board[r][c].RemoveCandidate(Value);
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
		public void ColumnPointedPairClearBox(int Value, int Br, int Bc, int col){
			#if DEBUG
			Console.WriteLine(String.Format("COLUMN Pointed Pair on {0} in box({1},{2})",Value, Br, Bc));
			#endif
			for(int r = Box.ToBoardCoordinate(Br); r < Box.ToBoardCoordinate(Br) + 3; r++) {
				for(int c = Box.ToBoardCoordinate(Bc); c < Box.ToBoardCoordinate(Bc) + 3; c++) {
					if(c != col) {
						#if DEBUG
						Console.WriteLine(String.Format("CLEAR {0} from ({1},{2})", Value, r, c));
						#endif
						this.Board[r][c].RemoveCandidate(Value);
					}
				}
			}
		}

		private void LockCell(Cell cell, int row, int col, int v = 0)
		{
			if(v == 0) {
				LockCell(cell, row, col, cell.CandidateValues[0]);
				return;
			}
			// Get all the set values in the row, column and box check if one of those values is what we're trying to set this cell to
			if(this.GetRow(row).Contains((int)v)) {
				this.Print(PrintStyle.CandidateCount);
				throw new Exception(String.Format("ROW Confilct at ({0},{1}) with value {2}", row, col, v));
			}
			if(this.GetCol(col).Contains((int)v)){
				this.Print(PrintStyle.CandidateCount);
				throw new Exception(String.Format("COLUMN Conflict at ({0},{1}) with value {2}", row, col, v));
			}
			if(this.GetBox( Box.ToBoxCoordinate(row), Box.ToBoxCoordinate(col)).Contains((int)v)){
				this.Print(PrintStyle.CandidateCount);
				throw new Exception(String.Format("BOX Conflict at ({0},{1}) with value {2}", row, col, v));
			}

			cell.LockValue(v);
			// Remove v from the available values in the row
			for(int c = 0; c < 9; c++) {
				this.Board[row][c].RemoveCandidate(v);
			}
			// Remove v from the available values in the column
			for(int r = 0; r < 9; r++) {
				this.Board[r][col].RemoveCandidate(v);
			}
			// Remove v from the available values in the box
			foreach(int r in Box.BoardCoordinates(Box.ToBoxCoordinate(row))) {
				foreach(int c in Box.BoardCoordinates(Box.ToBoxCoordinate(col))){
					this.Board[r][c].RemoveCandidate(v);
				}
			}
		}

		public override int GetHashCode()
		{
			int hash = 13;
			foreach(List<Cell> row in this.Board) {
				foreach(Cell c in row) {
					hash += ((c.GetHashCode() * 13) + 7) % Int32.MaxValue;
				}
			}
			return hash;
		}

		public override string ToString()
		{
			string b = "";
			for(int row = 0; row < 9; row++){
				for(int col = 0; col < 9; col++){
					if(col % 3 == 0) {
						b += " | ";
					}
					b += " " + this.Board[row][col].ToString();
				}
				b += "|\n";
				if((row + 1) % 3 == 0) {
					b += "\n";
				}
			}
			return b;
		}

		public void Print(PrintStyle Style = PrintStyle.Blanks)
		{
			Console.Write("    ");
			for(int i = 0; i < 9; i++) {
				Console.Write(String.Format("{0} ", i));
				if((i+1) % 3 == 0) {
					Console.Write("  ");
				}
			}
			Console.WriteLine();

			for(int row = 0; row < 9; row++){
				if(row%3 == 0) {
					Console.WriteLine("  -------------------------");
				}
				Console.Write(row);
				for(int col = 0; col < 9; col++){
					if(col % 3 == 0) {
						Console.Write(" |");
					}
					Console.Write(" ");
					if(this.Board[row][col].Locked) {
						Console.Write(this.Board[row][col].ToString());
					} else {
						switch(Style) {
							case PrintStyle.CandidateCount:
								Console.ForegroundColor = ConsoleColor.Yellow;
								Console.Write(this.Board[row][col].CandidateValues.Count);
								Console.ResetColor();
								break;
							default:
								Console.Write(" ");
								break;
						}
					}
				}
				Console.Write(" |\n");
			}
			Console.WriteLine("  -------------------------");
		}

		public void PrintDetails()
		{
			Console.WriteLine("     0   1   2   3   4   5   6   7   8");
			Console.WriteLine("   XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
			for(int r = 0; r < 27; r++) {
				if((r-1) % 3 == 0) {
					Console.Write(String.Format(" {0} ", r/3 ));
				} else {
					Console.Write("   ");
				}
				for(int c = 0; c < 27; c++) {

					int CellRow = Box.ToBoxCoordinate(r);
					int CellCol = Box.ToBoxCoordinate(c);

					int CellRowPosition = r % 3;
					int CellColPosition = c % 3;

					// Bar between boxes
					if(CellCol % 3 == 0 && CellColPosition == 0) {
						Console.Write("X");
					}

					this.Board[CellRow][CellCol].PrintDetailValue(CellRowPosition, CellColPosition);

					if(CellCol % 3 != 2 && CellColPosition == 2) {
						Console.Write("|");
					}
				}
				// Column between boxes
				Console.WriteLine("X");
				if((r+1) % 3 == 0 && (r+1)%9 != 0) {
					Console.WriteLine("   X---+---+---X---+---+---X---+---+---X");
				}
				// Row between boxes
				if((r+1) % 9 == 0) {
					Console.WriteLine("   XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
				}
			}
		}

		public void PrintUploadString()
		{
			for(int row = 0; row < 9; row++) {
				for(int col = 0; col < 9; col++) {
					if(this.Board[row][col].Locked){
						Console.Write(this.Board[row][col].Value);
					}else{
						Console.Write(".");
					}
				}
			}
			Console.WriteLine();
		}

	}
}
