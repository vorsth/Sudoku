using System;

namespace Sudoku
{
    public class SudokuBoardPrinter
    {
		public void Print(SudokuBoard board, PrintStyle Style = PrintStyle.Blanks)
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
					if(board.Board[row][col].Locked) {
						Console.Write(board.Board[row][col].ToString());
					} else {
						switch(Style) {
							case PrintStyle.CandidateCount:
								Console.ForegroundColor = ConsoleColor.Yellow;
								Console.Write(board.Board[row][col].CandidateValues.Count);
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

		public void PrintUploadString(SudokuBoard board)
		{
			for(int row = 0; row < 9; row++) {
				for(int col = 0; col < 9; col++) {
					if(board.Board[row][col].Locked){
						Console.Write(board.Board[row][col].Value);
					}else{
						Console.Write(".");
					}
				}
			}
			Console.WriteLine();
		}

		public void PrintDetails(SudokuBoard board)
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

					board.Board[CellRow][CellCol].PrintDetailValue(CellRowPosition, CellColPosition);

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

    }
}
