using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku
{
	class Cell
	{
		public List<int> CandidateValues { get; set; }
		public int? Value { get; set; }
		public bool Locked { get; set; }
		public SudokuBoard Board { get; private set; }
		public static readonly List<int> AllValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

		public Cell(SudokuBoard b, int value)
		{
			this.Board = b;
			if(value == 0) {
				this.Value = null;
				this.CandidateValues = Cell.AllValues.ToList();
				this.Locked = false;
			} else {
				this.Value = value;
				this.CandidateValues = new List<int>(value);
				this.Locked = true;
			}
		}

		/// <summary>
		/// Removes the value from this list of candidate values
		/// </summary>
		/// <param name="v">Value to remove</param>
		public void RemoveCandidate(int v)
		{
			this.CandidateValues.Remove(v);
			if(this.CandidateValues.Count == 1) {
				LockValue(this.CandidateValues[0]);
			}
		}

		public void RemoveCandidates(List<int?> NonValidValues)
		{
			foreach(int? v in NonValidValues){
				if(v.HasValue) {
					this.RemoveCandidate((int)v);
				}
			}
		}

		public void LockValue(int value)
		{
			this.Value = value;
			this.Locked = true;
			this.CandidateValues = new List<int>(){value};
		}

		public void PrintDetailValue(int CellRowPosition, int CellColPosition)
		{
			if(this.Locked) {
				if(CellRowPosition == 1 && CellColPosition == 1) {
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write(this.Value);
					Console.ResetColor();
				} else {
					Console.Write(" ");
				}
			}else{
				int Candidate = (CellRowPosition*3 + CellColPosition) + 1; // Add 1 since candidates are 1-based
				if(this.CandidateValues.Contains(Candidate)) {
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.Write(Candidate);
					Console.ResetColor();
				} else {
					Console.Write(" ");
				}
			}
		}

		public override string ToString()
		{
			if(this.Value.HasValue) {
				return Convert.ToString(this.Value);
			} else {
				return "_";
			}
		}

		public override int GetHashCode()
		{
			return this.Value.HasValue ? this.Value.Value : -1;
		}

	}
}
