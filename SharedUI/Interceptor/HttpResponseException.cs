

namespace SharedUI.Interceptor
{
    [Serializable]
    internal class HttpResponseException : Exception
    {
        public HttpResponseException()
        {
        }
        public HttpResponseException(string message)
            : base(message)
        {
        }

        public HttpResponseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected HttpResponseException(SerializationInfo info, StreamingContext context)
#pragma warning disable SYSLIB0051 // Type or member is obsolete
            : base(info, context)
#pragma warning restore SYSLIB0051 // Type or member is obsolete
        {
        }
    }
}
