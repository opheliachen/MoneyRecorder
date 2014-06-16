using System;
using System.Collections.Generic;

namespace TwoPairs.MoneyRecorder.Data
{
    public class Currency
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public int UpdatedBy { get; set; }

        public virtual ICollection<Statement> Statements { get; set; }
    }
}