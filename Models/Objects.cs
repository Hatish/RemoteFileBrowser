using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Request
    {
        public RequestEnum RequestType;
        public int id;
        public object Data;
    }

    public class Response
    {
        public RequestEnum RequestType;
        public int id;
        public object Data;
    }


    public class ListOfDrives
    {
        public List<Drive> Drives;
    }

    public class Drive
    {
        public string DriveLetter;
        public string DriveName;
        public long FreeSpace;
        public long UsedSpace;
        public string DriveType;
    }
}
