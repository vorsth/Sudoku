using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
	class BoardReader
	{
		private string FileName { get; set; }

		public List<SudokuBoard> Boards { get; private set; }

		public BoardReader(string file)
		{
			this.FileName = file;
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
			this.Boards.Add(new SudokuBoard(Board, Name));
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
