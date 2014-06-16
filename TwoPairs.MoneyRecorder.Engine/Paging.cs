using System.Diagnostics.Contracts;

namespace TwoPairs.MoneyRecorder
{
    public class Paging
    {
        public Paging(int currentPage, int pageSize)
        {
            Contract.Requires(currentPage > 0, "currentPage must be be greater than zero.");
            Contract.Requires(pageSize > 0, "pageSize must be be greater than zero.");

            CurrentPage = currentPage;
            PageSize = pageSize;
        }

        public int CurrentPage { get; private set; }

        public int PageSize { get; private set; }

        public override string ToString()
        {
            return string.Format("CurrentPage={0}&PageSize={1}",
                CurrentPage, PageSize);
        }
    }
}