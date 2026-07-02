namespace FcgUsers.Application.Responses;

public record PagedResponse<T>(
    IEnumerable<T> Items,
    int CurrentPage,
    int TotalPages,
    int TotalItems
);
