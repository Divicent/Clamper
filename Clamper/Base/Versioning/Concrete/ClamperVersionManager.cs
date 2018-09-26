using Clamper.Base.Exceptions;
using Clamper.Base.Files.Abstract;
using Clamper.Base.Versioning.Abstract;

namespace Clamper.Base.Versioning.Concrete
{
    public class ClamperVersionManager : IVersionManager
    {
        private readonly IFileSystem _fileSystem;

        public ClamperVersionManager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string GetCurrentVersion()
        {
            var assembliLocation = _fileSystem.GetCurrentAssemblyLocation();
            var versionFileLocation = _fileSystem.CombinePaths(assembliLocation, ".version");

            if (!_fileSystem.Exists(versionFileLocation))
                throw new ClamperException($".version file not found in the location {assembliLocation}");

            return _fileSystem.ReadText(versionFileLocation);
        }
    }
}