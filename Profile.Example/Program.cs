using SoftCube.Profile;
using System.Threading;

namespace Profile.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var transaction = Profiler.Start("A"))
            {
                Thread.Sleep(1000);
            }


            for (int i = 0; i < 10000; i++)
            {
                using var profile = Profiler.Start("A");
            }

            for (int i = 0; i < 10000; i++)
            {
                using var profile = Profiler.Start("B");
            }

            for (int i = 0; i < 10000; i++)
            {
                using var profile = Profiler.Start("C");
            }
        }
    }
}
