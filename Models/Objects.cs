using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

    public class DriveData
    {
        public string DriveLetter;
        public string DriveName;
        public long FreeSpace;
        public long UsedSpace;
        public string DriveType;
    }

    public class FolderData
    {
        public List<string> Files;
        public List<string> Folders;
    }

    public class CopyDataRequest
    {
        public List<string> Data;
        public string Destination;
    }

    public class CopyDataUpdate
    {
        public string Source;
        public string Destination;
        public int Progress;
        public long jobProgress;
        public long jobTotal;
    }

    public interface IClientCallback
    {
        void Handle(Response r);
    }
}
