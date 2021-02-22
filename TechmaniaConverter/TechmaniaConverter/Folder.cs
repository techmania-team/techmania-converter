using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechmaniaConverter
{
    // A custom class to allow traversing folders using intuitive method calls.
    class Folder
    {
        private string folder;  // May be null
        public Folder(string folder)
        {
            this.folder = Path.TrimEndingDirectorySeparator(folder);
        }

        public override string ToString()
        {
            return folder;
        }

        public Folder GoUp()
        {
            return new Folder(Path.GetDirectoryName(folder));
        }

        public Folder Open(string subfolder)
        {
            if (folder == null) return new Folder(null);
            return new Folder(Path.Combine(folder, subfolder));
        }

        public string OpenFile(string filename)
        {
            if (folder == null || filename == null) return null;
            return Path.Combine(folder, filename);
        }
    }
}
