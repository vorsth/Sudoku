using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sudoku
{
    class SudokuBoardReader
    {
        private string FileName;
        private SudokuBoardPrinter _printer;

        public List<SudokuBoard> Boards { get; private set; }

        public SudokuBoardReader(string file, SudokuBoardPrinter printer)
        {
            this.FileName = file;
            this._printer = printer;
            this.Boards = new List<SudokuBoard>();

            List<string> Lines = File.ReadAllLines(file).ToList();

            for(int i = 0; i < Lines.Count; i += 10) {
                ReadGrid(Lines.Skip(i).Take(10).ToList());
            }
        }

        private void ReadGrid(List<string> Lines)
        {
            List<List<int>> Board = new List<List<int>>();
            // Skip the first line since it's the grid number
            string Name = Lines[0];
            for(int i = 1; i < 10; i++) {
                Board.Add(ReadGridLine(Lines[i]));
            }
            this.Boards.Add(new SudokuBoard(Board, this._printer, Name));
        }

        private List<int> ReadGridLine(string GridLine)
        {
            List<int> Cells = new List<int>();
            for(int i = 0; i < 9; i++) {
                Cells.Add(Convert.ToInt32(GridLine.Substring(i,1)));
            }
            return Cells;
        }


    }
}
