using System;

namespace Common.Tests
{
    public static class Port
    {
        private static readonly Random _r = new Random();

        public static int Next() => _r.Next(12345, 65535);
    }
}
