using System;

namespace Sudoku
{
    class Program
    {
        static void Main(string[] args)
        {
            var printer = new SudokuBoardPrinter();
            var BR = new SudokuBoardReader("../../Puzzles.txt", printer);
            var BoardsToRun = BR.Boards;

            int SolvedBoards = 0;

            foreach(SudokuBoard B in BoardsToRun) {
                #if DEBUG
                printer.Print(B);
                printer.PrintUploadString(B);
                #endif

                int LastHash = 0;
                while(!B.IsSolved() && B.GetHashCode() != LastHash) {
                    LastHash = B.GetHashCode();
                    B.Solve();
                    #if DEBUG
                    Console.WriteLine(String.Format("Attempt: {0} - {1} solved cells", B.SolveAttempts, B.SolvedCells));
                    printer.Print(B, PrintStyle.CandidateCount);
                    #endif
                }

                if(B.IsSolved()) {
                    Console.WriteLine(String.Format("{0} Solved: {1} iterations", B.Name, B.SolveAttempts));
                    SolvedBoards += 1;
                } else {
                    Console.WriteLine(String.Format("{0} Not Solved", B.Name));
                    printer.Print(B, PrintStyle.Blanks);
                    printer.PrintDetails(B);
                }
            }

            Console.WriteLine(String.Format("Solved {0} boards", SolvedBoards));
            Console.ReadLine();
        }
    }
}
