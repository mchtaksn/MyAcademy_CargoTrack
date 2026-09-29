namespace CargoTrack.Dto.Dtos.CargoDtos
{
    public class PagedCargoResultDto
    {
        public List<ResultCargoDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

        public int AllCount { get; set; }
        public int TransitCount { get; set; }
        public int DeliveredCount { get; set; }
        public int ProblemCount { get; set; }
    }
}