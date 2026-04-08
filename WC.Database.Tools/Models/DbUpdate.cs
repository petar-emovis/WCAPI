using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WC.Database.Tools.Models
{
    internal class DbUpdate
    {
        public string Name { get; private set; }
        public string FilePath { get; private set; }
        public int Version { get; private set; }

        public DbUpdate(FileInfo fileInfo)
        {
            string version = fileInfo.Name.Split('_').First();
            Version = Int32.Parse(version);
            Name = fileInfo.Name;
            FilePath = fileInfo.FullName;
        }
        public string GetContent()
        {
            return File.ReadAllText(FilePath);
        }
    }
}
