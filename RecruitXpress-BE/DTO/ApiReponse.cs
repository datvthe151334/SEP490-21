namespace RecruitXpress_BE.DTO
{
    public class ApiResponse<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }

        public ApiResponse(IEnumerable<T> items, int totalCount)
        {
            Items = items;
            TotalCount = totalCount;
        }
        public ApiResponse()
        {
         
        }


    }
}
