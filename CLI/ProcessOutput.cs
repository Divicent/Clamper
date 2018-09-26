﻿using System;
using Clamper.Base.ProcessOutput.Abstract;
using ClamperCLI.Progress;

namespace ClamperCLI
{
    public class ProcessOutput : IProcessOutput
    {
        public bool NoInfo { private get; set; }
        public bool Silent { private get; set; }

        public void WriteInformation(string content)
        {
            if (Silent || NoInfo) return;
            Console.WriteLine("-> " + content);
        }

        public IProgressReporter Progress(int total, string initialMessage, string endMessage)
        {
            return new ProgressReporter(total, initialMessage, endMessage);
        }

        public void WriteSuccess(string content)
        {
            if (Silent) return;
            Console.Write("-> ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(content);
            Console.ResetColor();
        }

        public void WriteWarning(string content)
        {
            if (Silent) return;
            Console.Write("-> ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(content);
            Console.ResetColor();
        }
    }
}