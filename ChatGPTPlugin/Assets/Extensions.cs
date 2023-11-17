using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;

namespace GPTPlugin
{
    public static class ObjectExtensions
    {
        public static HttpContent ToHttpContent(this object value)
        {
            if (value is byte[])
            {
                return new ByteArrayContent((byte[])value);
            }

            return new StringContent(value?.ToString() ?? "");
        }
    }
}
