using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Types.Shared
{
    public class ApiResponse
    {
        public ApiResponse(bool success) { Success = success; }
        public ApiResponse(string error)
        {
            Error = error;
            Success = false;
        }

        public string? Error { get; set; }
        public bool Success { get; set; }
    }

    public class ApiResponse<T> : ApiResponse where T : class
    {
        public ApiResponse(string error): base(error) { }
        public ApiResponse(T data) : base(true)
        {
            Data = data;
        }

        public T? Data { get; set; }
    }
}
