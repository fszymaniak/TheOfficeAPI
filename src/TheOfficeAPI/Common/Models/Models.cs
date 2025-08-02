namespace TheOfficeAPI.Common.Models
{
    public class Episode
    {
        public int? Season { get; set; }
        public int? EpisodeNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ReleasedDate { get; set; } = string.Empty;
    }

    public class Season
    {
        public string SeasonNumber { get; set; } = string.Empty;
        public int EpisodeCount { get; set; }
    }

    public class ApiRequest
    {
        public string Action { get; set; } = string.Empty;
        public int? Season { get; set; }
        public int? Episode { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Error { get; set; }
    }
}