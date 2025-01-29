public class ErrorViewModel
{
    public string? RequestId { get; set; } // Nullable

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
