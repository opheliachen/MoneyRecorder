using System;

namespace TwoPairs.MoneyRecorder.Data
{
    public class Statement
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid CurrencyId { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public int UpdatedBy { get; set; }

        public virtual Currency Currency { get; set; }
    }
}