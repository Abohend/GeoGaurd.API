namespace GeoGaurd.API.Models.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; set; }
}

public static class ApiResponse
{
    public static ApiResponse<T> Ok<T>(T data, string? message = null)
    {
        return new() { Success = true, Data = data, StatusCode = 200, Message = message };
    }

    public static ApiResponse<object?> Ok(string? message = null)
    {
        return new() { Success = true, Data = null, StatusCode = 200, Message = message };
    }

    public static ApiResponse<object?> Fail(int statusCode, string message)
    {
        return new() { Success = false, StatusCode = statusCode, Message = message };
    }

    public static ApiResponse<T> Fail<T>(int statusCode, string message)
    {
        return new() { Success = false, StatusCode = statusCode, Message = message };
    }
}