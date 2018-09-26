﻿#region Usings

using Clamper.Base.ProcessOutput.Abstract;

#endregion

namespace Clamper.Base.ProcessOutput.Concrete
{
    /// <summary>
    ///     A process output implementation that does not do anything
    /// </summary>
    internal class NonFunctioningProcessOutput : IProcessOutput
    {
        public void WriteInformation(string content)
        {
            /*Does nothing*/
        }

        public IProgressReporter Progress(int total, string initalMessage, string endMessage)
        {
            return new NonFunctioningProgressReporter();
        }

        public void WriteSuccess(string content)
        {
            /*Does nothing*/
        }

        public void WriteWarning(string content)
        {
            /*Does nothing*/
        }
    }
}