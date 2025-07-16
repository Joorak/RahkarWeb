using System;

namespace Application.Models;
public class RequestResponse
{
    public bool Successful { get; set; } = false;

    public string? Error { get; set; } = null;


    public int EntityId { get; set; } = -1;
    public NullResponse? Item { get; set; } = null;

    public List<NullResponse?>? Items { get; set; } = null;

    public static RequestResponse Success(int id = 0)
    {
        return new RequestResponse { Successful = true, EntityId = id };
    }

    public static RequestResponse Failure(string? error = null)
    {
        return new RequestResponse { Successful = false, Error = error };
    }
}
public class RequestResponse<T> where T : class
{
    public bool Successful { get; set; } = false;

    public string? Error { get; set; } = null;


    public int EntityId { get; set; } = -1;
    public T? Item { get; set; } = null;

    public List<T>? Items { get; set; } = null;

    public static RequestResponse<T> Success(T? item)
    {
        return new RequestResponse<T> { Successful = true, Item = item };
    }

    public static RequestResponse<T> Failure(string? error = null)
    {
        return new RequestResponse<T> { Successful = false, Error = error };
    }

} 
