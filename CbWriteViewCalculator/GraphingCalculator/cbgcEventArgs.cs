using System;

namespace CbWriteViewCalculator.GraphingCalculator
{

	public class DrawFunctionEventArgs : EventArgs
	{
		public DrawFunction DF;
		public DrawFunctionEventArgs(DrawFunction sDF)
		{
			this.DF = sDF;
		}
	}









}

