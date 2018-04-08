using System;

namespace DContainer.TestData.Decorators
{
    public class Logger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
