#region Usings

using System;
using System.IO;
using System.Linq;
using Clamper.Base.Configuration.Abstract;
using Clamper.Base.Configuration.Concrete;
using Clamper.Base.Exceptions;
using Clamper.Base.Files.Abstract;
using Clamper.Base.Files.Concrete;
using Clamper.Base.Generating;
using Clamper.Base.ObstacleManaging;
using Clamper.Base.ProcessOutput.Abstract;
using Clamper.Base.ProcessOutput.Concrete;
using Clamper.Base.ProjectFileManaging;
using Clamper.Base.Reading.Concrete;
using Clamper.Base.Writing;
using Clamper.Models.Concrete;
using Clamper.Tools;
using Newtonsoft.Json;

#endregion

namespace Clamper.Base
{
    /// <summary>
    ///     The base Clamper class that only provides a static method to generate
    /// </summary>
    public static class Clamper
    {
        /// <summary>
        ///     Generates the Data Access Layer using given configuration file
        /// </summary>
        /// <param name="pathToConfigurationJsonFile">the full readable path to the configuration JSON file.</param>
        /// <param name="output">Just implement your own one and pass it here</param>
        /// <returns>the result of the generation process</returns>
        public static ClamperGenerationResult Generate(string pathToConfigurationJsonFile, IProcessOutput output)
        {
            return GenerateInternal(pathToConfigurationJsonFile, output);
        }

        /// <summary>
        ///     Generates the Data Access Layer using given configuration file.
        /// </summary>
        /// <param name="pathToConfigurationJsonFile">the full readable path to the configuration JSON file.</param>
        /// <returns>the result of the generation process</returns>
        public static ClamperGenerationResult Generate(string pathToConfigurationJsonFile)
        {
            return GenerateInternal(pathToConfigurationJsonFile, new NonFunctioningProcessOutput());
        }


        private static ClamperGenerationResult GenerateInternal(string pathToConfigurationJsonFile, IProcessOutput output)
        {
            var result = new ClamperGenerationResult();

            IConfiguration config = null;
            IFileSystem fileSystem = new ClamperFileSystem();
            try
            {
                if (!fileSystem.Exists(pathToConfigurationJsonFile))
                    throw new ClamperException(
                        $"The configuration file ({pathToConfigurationJsonFile}) could not be found.\nPlease make sure that the ClamperSettings.json file exists in the location.");

                output.WriteInformation("Reading Configuration File");
                try
                {
                    config = JsonConvert.DeserializeObject<ClamperConfiguration>(
                        fileSystem.ReadText(pathToConfigurationJsonFile));

                    config.Setup();
                }
                catch (ClamperException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    throw new ClamperException(
                        $"Unable to read the configuration file ({exception.Message}), The configuration file must be a valid JSON file.");
                }

                output.WriteInformation("Validating Configuration");
                config.Validate();
            }
            catch (Exception exception)
            {
                result.Error = ClamperExceptionMessageFormatter.FormatException(exception,
                    "En error occurred while trying to initialize generation process (probably a configuration error)");
            }

            if (!string.IsNullOrEmpty(result.Error) || config == null)
            {
                result.Success = false;
                return result;
            }

            try
            {
                if (!Path.IsPathRooted(config.ProjectPath))
                {
                    /* Getting the full path to the project folder
                     if the specified path is relative to the configuration file */
                    if (config.ProjectPath == "." || config.ProjectPath == "./")
                        config.ProjectPath = "";

                    config.ProjectPath =
                        Path.GetFullPath(
                            $"{new FileInfo(pathToConfigurationJsonFile).Directory.FullName}{config.ProjectPath}");
                }

                output.WriteInformation("Reading Database Schema");
                var schemaReader = DatabaseSchemaReaderFactory.GetReader(config.DBMS);
                var schema = schemaReader.Read(config);

                var contentFiles = DalGenerator.Generate(schema, config, output).ToList();

                ObstacleManager.Clear(config.ProjectPath, output, config, fileSystem);

                DalWriter.Write(contentFiles, config.ProjectPath, output, fileSystem);

                output.WriteInformation("Processing project files");
                if (!string.IsNullOrWhiteSpace(config.ProjectFile))
                    CSharpProjectItemManager.Process(
                        fileSystem.CombinePaths(config.ProjectPath, config.ProjectFile), output, config, 
                        contentFiles.Where(i => !i.External).Select(c => c.Path));
            }
            catch (Exception e)
            {
                result.Error = ClamperExceptionMessageFormatter.FormatException(e.InnerException, e.Message);
                result.Success = false;
                return result;
            }

            return result;
        }
    }
}