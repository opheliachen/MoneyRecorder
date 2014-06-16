using System;

namespace TwoPairs.MoneyRecorder
{
    public class UpdateCurrencyData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        public int UpdatedBy { get; set; }
    }
}