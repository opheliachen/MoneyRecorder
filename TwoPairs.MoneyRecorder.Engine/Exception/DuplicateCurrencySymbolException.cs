using System;

namespace TwoPairs.MoneyRecorder.Exceptions
{
    public class DuplicateCurrencySymbolException : Exception
    {
        public DuplicateCurrencySymbolException()
        {
        }

        public DuplicateCurrencySymbolException(string message)
            : base(message)
        {
        }
    }
}