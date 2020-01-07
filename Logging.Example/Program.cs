using System;

namespace SoftCube.Logging
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Logger.Trace("Hello World!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
