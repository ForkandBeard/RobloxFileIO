using System;

namespace RobloxFileIO
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ArgumentProcessor.ProcessArguments(args);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }
    }
}
