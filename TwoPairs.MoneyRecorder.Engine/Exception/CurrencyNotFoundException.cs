using System;

namespace TwoPairs.MoneyRecorder.Exceptions
{
    public class CurrencyNotFoundException : Exception
    {
        public CurrencyNotFoundException()
        {
        }

        public CurrencyNotFoundException(string message)
            : base(message)
        {
        }
    }
}