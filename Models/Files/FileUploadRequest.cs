using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Files
{
    public class FileUploadRequest
    {
        public string FileName { get; set; }
        public Stream FileContent { get; set; }
        public string ContentType { get; set; }
    }
}
