#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Clamper.Base.Exceptions;
using Clamper.Base.Files.Abstract;
using Clamper.Base.ProcessOutput.Abstract;
using Clamper.Models.Abstract;

#endregion

namespace Clamper.Base.Writing
{
    internal static class DalWriter
    {
        /// <summary>
        ///     Write files to the disk
        /// </summary>
        /// <param name="fls">Files to write</param>
        /// <param name="basePath">Base path </param>
        /// <param name="output">A process output</param>
        /// <param name="fileSystem"></param>
        public static void Write(IEnumerable<IContentFile> fls, string basePath, IProcessOutput output,
            IFileSystem fileSystem)
        {
            try
            {
                var createdDirectories = new HashSet<string>();
                var files = fls.ToList();
                var progress =
                    output.Progress(files.Count, "Writing Files", $"Done writing {files.Count} files.");

                foreach (var contentFile in files)
                {
                    var file = fileSystem.CombinePaths(basePath, contentFile.Path);
                    var directory = fileSystem.GetDirectoryOfAFile(file);

                    if (!createdDirectories.Contains(directory) && directory != null &&
                        !fileSystem.DirectoryExists(directory))
                    {
                        fileSystem.CreateDirectory(directory);
                        createdDirectories.Add(directory);
                    }

                    fileSystem.WriteText(contentFile.Content, file);
                    progress.Tick(contentFile.Path);
                }
            }
            catch (Exception e)
            {
                throw new ClamperException("Unable to write files to the disc", e);
            }
        }
    }
}