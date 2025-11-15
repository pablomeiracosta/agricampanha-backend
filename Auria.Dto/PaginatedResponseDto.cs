using System.Collections.Generic;

namespace Auria.Dto
{
    public class PaginatedResponseDto<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public PaginatedResponseDto()
        {
            Items = new List<T>();
        }

        public PaginatedResponseDto(IEnumerable<T> items, int totalCount, int currentPage, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)System.Math.Ceiling(totalCount / (double)pageSize);
        }
    }
}
