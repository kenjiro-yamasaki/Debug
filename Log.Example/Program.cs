using System;

namespace SoftCube.Log
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
