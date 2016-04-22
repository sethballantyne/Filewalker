using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filewalker
{
    public class FilePath
    {
        string filename;
        string path;

        public FilePath(string filename, string path)
        {
            this.filename = filename;
            this.path = path;
        }

        public string Filename
        {
            get { return filename; }
        }

        public string Path
        {
            get { return path; }
        }

        public string AbsolutePath
        {
            get { return path + "\\" + filename; }
        }
    }
}
