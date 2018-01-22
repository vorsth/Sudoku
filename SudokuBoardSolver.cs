using System.Collections.Generic;
using Sudoku.Solvers;

namespace Sudoku
{
    public class SudokuBoardSolver
    {
        private SudokuBoard _board;
        private List<ISolver> _solvers;
        private SudokuBoardPrinter _printer;

        public int SolveAttempts { get; set; }
        public SudokuBoard Board => this._board;
        public bool IsBoardSolved => this._board.IsSolved();

        public SudokuBoardSolver(SudokuBoard board, List<ISolver> solvers, SudokuBoardPrinter printer)
        {
            this._board = board;
            this._solvers = solvers;
            this._printer = printer;
            this.SolveAttempts = 0;
        }

        public void Solve()
        {
            int LastHash = 0;
            while (!this.Board.IsSolved() && this.Board.GetHashCode() != LastHash)
            {
                LastHash = this.Board.GetHashCode();
                this.RunSolvers();

                #if DEBUG
                Console.WriteLine(String.Format("Attempt: {0} - {1} solved cells", this.SolveAttempts, this.Board.SolvedCells));
                this._printer.Print(this.Board, PrintStyle.CandidateCount);
                #endif
            }
            this.SolveAttempts++;
        }

        private void RunSolvers()
        {
            foreach(var solver in this._solvers)
            {
                solver.ProcessBoard(this.Board);
            }
        }
    }
}
