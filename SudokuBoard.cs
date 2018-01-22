using Sudoku.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku
{
    public class SudokuBoard
    {
        private SudokuBoardPrinter _printer;

        public string Name { get; private set; }
        public List<List<Cell>> Board;

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

        public SudokuBoard(List<List<int>> b, SudokuBoardPrinter printer, string name = "UNKNOWN")
        {
            this._printer = printer;
            this.Name = name;
            this.Board = new List<List<Cell>>();

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
        public List<int?> GetRow(int r)
        {
            return this.Board[r].Select(x => x.Value).ToList();
        }

        /// <summary>
        /// Get the values that exist in the column
        /// </summary>
        /// <param name="c">Column to look at</param>
        /// <returns></returns>
        public List<int?> GetCol(int c)
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
        public List<int?> GetBox(int Bx, int By)
        {
            List<int?> CellValues = new List<int?>();
            for(int i = Box.ToBoardCoordinate(Bx); i < Box.ToBoardCoordinate(Bx)+3; i++) {
                for(int j = 3*By; j < 3*By+3; j++) {
                    CellValues.Add(this.Board[i][j].Value);
                }
            }
            return CellValues;
        }

        public void LockCell(Cell cell, int row, int col, int v = 0)
        {
            if(v == 0) {
                LockCell(cell, row, col, cell.CandidateValues[0]);
                return;
            }
            // Get all the set values in the row, column and box check if one of those values is what we're trying to set this cell to
            if(this.GetRow(row).Contains((int)v)) {
                this._printer.Print(this, PrintStyle.CandidateCount);
                throw new Exception(String.Format("ROW Confilct at ({0},{1}) with value {2}", row, col, v));
            }
            if(this.GetCol(col).Contains((int)v)){
                this._printer.Print(this, PrintStyle.CandidateCount);
                throw new Exception(String.Format("COLUMN Conflict at ({0},{1}) with value {2}", row, col, v));
            }
            if(this.GetBox( Box.ToBoxCoordinate(row), Box.ToBoxCoordinate(col)).Contains((int)v)){
                this._printer.Print(this, PrintStyle.CandidateCount);
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

        /// <summary>
        /// The hash code is used to determine the current state of the board, often for the purposes to see
        /// if anything has changed since a previous solving iteration
        /// </summary>
        /// <returns></returns>
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
    }
}
