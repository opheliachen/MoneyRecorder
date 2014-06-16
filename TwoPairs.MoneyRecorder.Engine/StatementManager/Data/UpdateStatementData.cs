using System;

namespace TwoPairs.MoneyRecorder
{
    public class UpdateStatementData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid CurrencyId { get; set; }

        public int UpdatedBy { get; set; }
    }
}