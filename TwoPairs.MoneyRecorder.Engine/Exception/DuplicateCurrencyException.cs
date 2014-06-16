using System;

namespace TwoPairs.MoneyRecorder.Exceptions
{
    public class DuplicateCurrencyException : Exception
    {
        public DuplicateCurrencyException()
        {
        }

        public DuplicateCurrencyException(string message)
            : base(message)
        {
        }
    }
}