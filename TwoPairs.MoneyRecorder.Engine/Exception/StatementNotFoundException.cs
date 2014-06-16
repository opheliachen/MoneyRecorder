using System;

namespace TwoPairs.MoneyRecorder.Exceptions
{
    public class StatementNotFoundException : Exception
    {
        public StatementNotFoundException()
        {
        }

        public StatementNotFoundException(string message)
            : base(message)
        {
        }
    }
}