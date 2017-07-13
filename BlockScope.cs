using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
	[Flags]
	public enum BlockScope
	{
		None         = 0x00,
		InsideBlock  = 0x01,
		OutsideBlock = 0x02,
		FullBlock    = 0x03
	}
}
