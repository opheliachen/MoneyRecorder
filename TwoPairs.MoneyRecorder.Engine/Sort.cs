namespace TwoPairs.MoneyRecorder
{
    public class Sort<T>
    {
        public Sort(T sortBy, SortType sortType)
        {
            By = sortBy;
            Type = sortType;
        }

        public SortType Type { get; private set; }

        public T By { get; private set; }

        public override string ToString()
        {
            return string.Format("By={0}&Type={1}", By, Type);
        }
    }
}