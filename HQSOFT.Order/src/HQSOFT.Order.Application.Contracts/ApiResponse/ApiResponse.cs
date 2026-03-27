using System;
using System.Collections.Generic;
using System.Linq;

namespace HQSOFT.Order.ApiResponse;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public ApiResponseMeta? Meta { get; set; }
    public List<ApiError> Errors { get; set; } = new();

    public static ApiResponse<T> Ok(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message,
        Meta = new ApiResponseMeta()
    };

    public static ApiResponse<T> Fail(string message, string code = "ERROR") => new()
    {
        Success = false,
        Message = message,
        Errors = new List<ApiError>
        {
            new ApiError { Code = code, Message = message }
        },
        Meta = new ApiResponseMeta()
    };

    public static ApiResponse<T> Fail(string message, string code, Dictionary<string, object>? details) => new()
    {
        Success = false,
        Message = message,
        Errors = new List<ApiError>
        {
            new ApiError { Code = code, Message = message, Details = details }
        },
        Meta = new ApiResponseMeta()
    };

    public static ApiResponse<T> Fail(List<ApiError> errors) => new()
    {
        Success = false,
        Message = errors.Select(p => p.Message).JoinAsString(", "),
        Errors = errors,
        Meta = new ApiResponseMeta()
    };
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public ApiResponseMeta? Meta { get; set; }
    public List<ApiError> Errors { get; set; } = new();

    public static ApiResponse Ok(string? message = null) => new()
    {
        Success = true,
        Message = message,
        Meta = new ApiResponseMeta()
    };

    public static ApiResponse Fail(string message, string code = "ERROR") => new()
    {
        Success = false,
        Message = message,
        Errors = new List<ApiError>
        {
            new ApiError { Code = code, Message = message }
        },
        Meta = new ApiResponseMeta()
    };
}

public class ApiResponseMeta
{
    public string Version { get; set; } = "1.0";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string RequestId { get; set; } = Guid.NewGuid().ToString("N")[..8];
}

public class ApiError
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Field { get; set; }
    public object? Details { get; set; }
}

public class PagedApiResponse<T> : ApiResponse<List<T>>
{
    public PaginationInfo? Pagination { get; set; }

    public new static PagedApiResponse<T> Ok(
        List<T> items,
        long totalCount,
        int pageNumber,
        int pageSize,
        string? message = null) => new()
    {
        Success = true,
        Data = items,
        Message = message,
        Meta = new ApiResponseMeta(),
        Pagination = new PaginationInfo
        {
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber * pageSize < totalCount
        }
    };
}

public class PaginationInfo
{
    public long TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}
