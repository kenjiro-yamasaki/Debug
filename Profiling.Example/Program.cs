using SoftCube.Logging;
using System.Threading;

namespace SoftCube.Profiling
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Trace("A");
            using (var transaction = Profiler.Start("A"))
            {
                Thread.Sleep(1000);
            }

            Logger.Trace("B");
            using (var transaction = Profiler.Start("B"))
            {
                Thread.Sleep(3000);
            }

            Logger.Trace("C");
            using (var transaction = Profiler.Start("C"))
            {
                Thread.Sleep(2000);
            }
        }
    }
}
