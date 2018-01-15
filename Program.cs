using System;
using System.Collections.Generic;

namespace Sudoku
{
	class Program
	{
		static void Main(string[] args)
		{
			//Easy 1
			//SudokuBoard B = new SudokuBoard(new List<List<int>>(){
			//	new List<int>(){0,9,0,2,0,7,0,4,0},
			//	new List<int>(){3,0,0,5,0,8,0,0,6},
			//	new List<int>(){0,0,8,0,4,0,2,0,0},
			//	new List<int>(){1,4,0,0,0,0,0,8,3},
			//	new List<int>(){0,0,6,0,0,0,4,0,0},
			//	new List<int>(){2,7,0,0,0,0,0,9,1},
			//	new List<int>(){0,0,3,0,6,0,9,0,0},
			//	new List<int>(){4,0,0,3,0,9,0,0,7},
			//	new List<int>(){0,6,0,8,0,5,0,1,0}}
			//);

			// Easy 2
			//SudokuBoard B = new SudokuBoard(new List<List<int>>(){
			//	new List<int>(){0,0,0,4,0,1,9,0,0},
			//	new List<int>(){0,0,0,5,7,0,0,0,0},
			//	new List<int>(){7,0,1,0,0,0,6,0,0},
			//	new List<int>(){2,0,0,0,8,0,0,9,6},
			//	new List<int>(){0,7,0,1,0,4,0,8,0},
			//	new List<int>(){1,8,0,0,5,0,0,0,7},
			//	new List<int>(){0,0,4,0,0,0,2,0,3},
			//	new List<int>(){0,0,0,0,2,5,0,0,0},
			//	new List<int>(){0,0,7,9,0,6,0,0,0}
			//}
			//);

			BoardReader BR = new BoardReader("../../Puzzles.txt");

			int SolvedBoards = 0;

			List<SudokuBoard> BoardsToRun = BR.Boards;
			//List<SudokuBoard> BoardsToRun = new List<SudokuBoard>() { BR.Boards[49] };

			foreach(SudokuBoard B in BoardsToRun) {
				#if DEBUG
				B.Print();
				B.PrintUploadString();
				#endif

				int LastHash = 0;
				while(!B.IsSolved() && B.GetHashCode() != LastHash) {
					LastHash = B.GetHashCode();
					B.Solve();
					#if DEBUG
					Console.WriteLine(String.Format("Attempt: {0} - {1} solved cells", B.SolveAttempts, B.SolvedCells));
					B.Print(PrintStyle.CandidateCount);
					#endif
				}

				if(B.IsSolved()) {
					Console.WriteLine(String.Format("{0} Solved: {1} iterations", B.Name, B.SolveAttempts));
					SolvedBoards += 1;
				} else {
					Console.WriteLine(String.Format("{0} Not Solved", B.Name));
					B.Print(PrintStyle.Blanks);
					B.PrintDetails();
				}
			}

			Console.WriteLine(String.Format("Solved {0} boards", SolvedBoards));
			Console.ReadLine();



		}
	}
}
