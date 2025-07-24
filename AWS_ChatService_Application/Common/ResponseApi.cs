namespace AWS_ChatService_Application.Common;

public class ResponseApi<T>
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public static ResponseApi<T> Success(T data, int statusCode = 200, string message = "Ok")
    {
        return new ResponseApi<T>
        {
            IsSuccess = true,
            StatusCode = statusCode,
            Message = message,
            Data = data
        };
    }

    public static ResponseApi<T> Fail(int statusCode, string message, T data = default)
    {
        return new ResponseApi<T>
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Message = message,
            Data = data
        };
    }
}