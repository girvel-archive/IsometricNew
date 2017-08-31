using System;

namespace Isometric.Core.Common
{
    public static class TimeSpanHelper
    {
        public static TimeSpan Multiple(this TimeSpan t, float k)
            => TimeSpan.FromTicks((long) (t.Ticks * k));
    }
}