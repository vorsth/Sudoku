using System;
using System.Collections.Generic;
using Sudoku.Solvers;

namespace Sudoku
{
    class Program
    {
        static void Main(string[] args)
        {
            var printer = new SudokuBoardPrinter();
            var BR = new SudokuBoardReader("Puzzles.txt", printer);
            var BoardsToRun = BR.Boards;

            int SolvedBoards = 0;

            var strategies = GetStrategies(printer);

            foreach(SudokuBoard B in BoardsToRun) {
                var solver = new SudokuBoardSolver(B, strategies, printer);
                #if DEBUG
                printer.Print(solver.Board);
                printer.PrintUploadString(solver.Board);
                #endif

                solver.Solve();

                if(solver.IsBoardSolved) {
                    Console.WriteLine(String.Format("{0} Solved: {1} iterations", solver.Board.Name, solver.SolveAttempts));
                    SolvedBoards += 1;
                } else {
                    Console.WriteLine(String.Format("{0} Not Solved", solver.Board.Name));
                    printer.Print(solver.Board, PrintStyle.Blanks);
                    printer.PrintDetails(solver.Board);
                }
            }

            Console.WriteLine(String.Format("Solved {0} boards", SolvedBoards));
            Console.ReadLine();
        }

        private static List<ISolver> GetStrategies(SudokuBoardPrinter printer)
        {
            var nakedSingleCellsSolver = new NakedSingleCellsSolver();
            var hiddenSingleCellsSolver = new HiddenSingleCellsSolver(nakedSingleCellsSolver, printer);
            var pointedPairsSolver = new PointedPairsSolver(nakedSingleCellsSolver);

            return new List<ISolver>()
            {
                nakedSingleCellsSolver,
                hiddenSingleCellsSolver,
                pointedPairsSolver
            };
        }
    }
}
