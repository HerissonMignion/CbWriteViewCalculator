using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbWriteViewCalculator
{
	public class wvStringReader
	{
		private string zzzStrToRead;
		public string StrToRead { get { return this.zzzStrToRead; } }
		public int Pos = 0; //position gauche du caractere actuel

		public string Read()
		{
			string rep = "";
			if (this.Pos < this.StrToRead.Length)
			{
				rep = this.StrToRead.Substring(this.Pos, 1);
				this.Pos++;
			}
			return rep;
		}
		public string Read(int length = 1)
		{
			string rep = "";
			if (this.Pos + length - 1 < this.StrToRead.Length)
			{
				rep = this.StrToRead.Substring(this.Pos, length);
				this.Pos += length;
			}
			return rep;
		}
		public string ReadUntil(string c = ";")
		{
			string rep = "";
			while (!this.EndOfString)
			{
				string actuel = this.Read();
				if (actuel == c) { break; }
				rep += actuel;
			}
			return rep;
		}

		public bool EndOfString
		{
			get
			{
				return this.Pos >= this.StrToRead.Length;
			}
		}

		//void new()
		public wvStringReader(string str)
		{
			this.zzzStrToRead = str;
			this.Pos = 0;
		}
	}
}
