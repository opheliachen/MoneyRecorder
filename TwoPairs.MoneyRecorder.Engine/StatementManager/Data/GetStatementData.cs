using System;
using TwoPairs.MoneyRecorder.Data;

namespace TwoPairs.MoneyRecorder
{
    public class GetStatementData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }

        public Currency Currency { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }
    }
}