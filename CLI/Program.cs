using System;
using System.Linq;
using Clamper.Base.Generating;

namespace ClamperCLI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var fileName = "ClamperSettings.json";
            var output = new ProcessOutput();
            if (args.Length > 0)
            {
                if (args.Contains("-ni")) output.NoInfo = true;

                if (args.Contains("-s")) output.Silent = true;

                if (args.Contains("-f"))
                {
                    var index = args.ToList().IndexOf("-f");
                    if (args.Length > index + 1)
                    {
                        var fn = args[index + 1];
                        if (!string.IsNullOrWhiteSpace(fn)) fileName = fn;
                    }
                }
            }

            var path = $@"./{fileName}";
            
            Console.WriteLine(@"
   ____                  _                       ____   _       ___ 
  / ___|   ___   _ __   (_)   ___               / ___| | |     |_ _|
 | |  _   / _ \ | '_ \  | |  / _ \    _____    | |     | |      | | 
 | |_| | |  __/ | | | | | | |  __/   |_____|   | |___  | |___   | | 
  \____|  \___| |_| |_| |_|  \___|              \____| |_____| |___|

                      -- Data Access Layer Generator --

https://rusith.github.io/Clamper                      
                                                               ");

            var result = args.Contains("-s") ? Clamper.Base.Clamper.Generate(path) :
                Clamper.Base.Clamper.Generate(path, output);

            if (!result.Success)
            {
                Console.Write(":> ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("Done!");
            }

            if (args.Contains("-y")) 
                return;
            try
            {
                Console.ReadKey();
            }
            catch { /* ignored */ }
        }
    }
}