using System;

namespace GemTracker.Shared.Extensions
{
    public static class ExceptionExt
    {
        public static string GetFullMessage(this Exception ex)
            => ex.InnerException == null
                 ? ex.Message
                 : ex.Message + ": " + ex.InnerException.GetFullMessage();
    }
}