using System;
using System.Linq;
using Clamper.Base.Generating;

namespace ClamperCLI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var fileName = "clamper.config.json";
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
   ____ _                                            ____ _     ___ 
  / ___| | __ _ _ __ ___  _ __   ___ _ __           / ___| |   |_ _|
 | |   | |/ _` | '_ ` _ \| '_ \ / _ \ '__|  _____  | |   | |    | | 
 | |___| | (_| | | | | | | |_) |  __/ |    |_____| | |___| |___ | | 
  \____|_|\__,_|_| |_| |_| .__/ \___|_|             \____|_____|___|
                         |_|                                        

                      -- Data Access Layer Generator --

https://github.com/divicent/Clamper                      
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