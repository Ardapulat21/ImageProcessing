using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Services
{
    public class Console
    {
        public static void WriteLine(string text, ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Red:
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case ConsoleColor.Yellow:
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case ConsoleColor.Green:
                    System.Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case ConsoleColor.Cyan:
                    System.Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
            }
            System.Console.WriteLine(text);

        }
    }
}
