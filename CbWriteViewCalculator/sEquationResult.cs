namespace CbWriteViewCalculator
{


    //lorsqu'on call le calcul d'une equation, l'equation va retourner cette structure qui contiendra le resultat ou l'erreur s'il y en a une

    public struct sEquationResult
    {
        public double TheResult;

        public bool AnErrorOccurred;
        public enum ErrorType
        {
			Unknown,

            NoError,
            EmptyEquation,
            DivisionByZero,

            UndefinedVariable,
			UndefinedTrigFunction,
			InvalidVariableName,
			UndefinedFunction,
			InvalidFunctionParamCount,
			
            NotAFunction,
			OutOfDomain,
            RootOfNegativeNumber,
			NaN
        }
        public ErrorType ActualError;


        public sEquationResult(double StartResult)
        {
            this.TheResult = StartResult;
            this.AnErrorOccurred = false;
            this.ActualError = ErrorType.NoError;
        }
		public sEquationResult(ErrorType TheError)
		{
			this.ActualError = TheError;
			this.AnErrorOccurred = true;
			this.TheResult = 0d;
		}
    }




	public struct sCompiledEquationResult
	{
		public double TheResult;

		public bool AnErrorOccurred;
		public string ErrorMessage;


		public sCompiledEquationResult(double StartResult)
		{
			this.TheResult = StartResult;
			this.AnErrorOccurred = false;
			this.ErrorMessage = "";
		}

	}










}