namespace BookkeeperAPI.Model
{
    public class PaginatedResult<T>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string FirstPage { get; set; }

        public string LastPage { get; set; }

        public int PageCount { get; set; }

        public int TotalCount { get; set; }

        public string? NextPage { get; set; }

        public string? PreviousPage { get; set; }

        public IEnumerable<T> Data { get; set; }
    }
}
