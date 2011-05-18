using System.IO;
using Encog.Util.File;
using Directory = System.IO.Directory;

namespace Encog.Util
{
    public class TempDir
    {
        private readonly FileInfo _tempdir;

        public TempDir()
        {
            _tempdir = FileUtil.CombinePath(new FileInfo(Path.GetTempPath()), "encog-ut");
            Directory.CreateDirectory(_tempdir.ToString());
        }

        public FileInfo CreateFile(string filename)
        {
            return FileUtil.CombinePath(_tempdir, filename);
        }

        public override string ToString()
        {
            return _tempdir.ToString();
        }
    }
}