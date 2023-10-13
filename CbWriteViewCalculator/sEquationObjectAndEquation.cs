using System;

namespace CbWriteViewCalculator
{
	public struct sEquationObjectAndEquation
	{
		public oEquationObject TheEquObj;
		public oEquation TheEqu;
		
		public enum enumMainObject
		{
			none,
			Equation,
			EquationObject
		}
		public enumMainObject TheMainObject; //indique quel est l'object "principale" entre les deux

		//retourne true si les deux object sont null et que mainobject est sur none
		public bool IsTotalNull()
		{
			if (this.TheEquObj == null && this.TheEqu == null && this.TheMainObject == enumMainObject.none)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public void SetToTotalNull()
		{
			this.TheEquObj = null;
			this.TheEqu = null;
			this.TheMainObject = enumMainObject.none;
		}

		public object MainObject
		{
			get
			{
				switch (this.TheMainObject)
				{
					default:
					case enumMainObject.none:
						return null;
						break;
					case enumMainObject.Equation:
						return this.TheEqu;
						break;
					case enumMainObject.EquationObject:
						return this.TheEquObj;
						break;
				}
			}
		}

		//void new()
		public sEquationObjectAndEquation(oEquationObject StartEquObj, oEquation StartEqu)
		{
			this.TheEquObj = StartEquObj;
			this.TheEqu = StartEqu;
			this.TheMainObject = enumMainObject.none;
		}
		public sEquationObjectAndEquation(oEquationObject StartEquObj, oEquation StartEqu, enumMainObject StartMainObject)
		{
			this.TheEquObj = StartEquObj;
			this.TheEqu = StartEqu;
			this.TheMainObject = StartMainObject;
		}

	}
}