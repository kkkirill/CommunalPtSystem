using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CommunalPtSystem
{
    public partial class App : Application
    {
        static readonly string DatabaseName = "sqlite.db";
        static readonly string FolderPath = Environment.CurrentDirectory;
        public static readonly string DatabasePath = System.IO.Path.Combine(FolderPath, DatabaseName);
        public static readonly int DatabaseVersion = 3;
    }
}
