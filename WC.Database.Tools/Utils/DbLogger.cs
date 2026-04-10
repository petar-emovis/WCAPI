using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WC.Database.Tools.Utils
{
    public class DbLogger(string text)
    {
        public string? Text { get; set; } = text;

        public void Info(string? t)
        { 
            Text += Environment.NewLine + t; 
        }

    }
}
