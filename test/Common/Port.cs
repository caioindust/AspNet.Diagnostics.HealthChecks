using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Tests
{
    public static class Port
    {
        static readonly Random _r = new Random();

        public static int Next() => _r.Next(12345, 65535);
    }
}
