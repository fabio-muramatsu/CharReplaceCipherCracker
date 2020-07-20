using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CipherCrack
{
    class Program
    {
        static void Main(string[] args)
        {
            var cracker = new Cracker();
            cracker.Crack();
        }


    }
}
