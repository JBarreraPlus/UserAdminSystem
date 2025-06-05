namespace UserAdminSystem.Responses;

public record GeneralResponse<T>(
    bool Success,
    string Message = null!,
    int StatusCode = StatusCodes.Status200OK,
    T? Data = default
);