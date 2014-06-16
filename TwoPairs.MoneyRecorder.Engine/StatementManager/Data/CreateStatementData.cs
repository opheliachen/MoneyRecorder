using System;

namespace TwoPairs.MoneyRecorder
{
    public class CreateStatementData
    {
        public string Name { get; set; }

        public Guid CurrencyId { get; set; }

        public int CreatedBy { get; set; }
    }
}