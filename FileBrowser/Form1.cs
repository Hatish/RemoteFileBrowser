using Models;
using Newtonsoft.Json.Linq;
using RemoteFileBrowser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileBrowser
{
    public partial class MainForm : Form
    {
        FileBrowserHelper server;
        FileBrowserHelper client;
        ClientCallback cb;
        JsonSerializer js = new JsonSerializer();
        public List<DriveData> drives;

        public string leftPaneDirectory = "";
        public string rightPaneDirectory = "";

        public MainForm()
        {
            InitializeComponent();
            cb = new ClientCallback(this);
            server = new FileBrowserHelper();
            server.LoadServerListener();
            client = new FileBrowserHelper();
            client.LoadClientListener(cb);
            client.SendMessageToServer(js.Serialize(new Request() { RequestType = Models.RequestEnum.ListOfDrives }));
        }

        public void UpdatePanes()
        {
            if (String.IsNullOrEmpty(leftPaneDirectory))
            {
                left.Items.Clear();
                foreach (var d in drives)
                {
                    left.Items.Add(d.DriveLetter + " [" + d.DriveType + "]");
                }
            }
            else
            {

            }

            if (String.IsNullOrEmpty(rightPaneDirectory))
            {
                right.Items.Clear();
                foreach (var d in drives)
                {
                    right.Items.Add(d.DriveLetter + " [" + d.DriveType + "]");
                }
            }
            else
            {

            }
        }
    }

    public class ClientCallback : IClientCallback
    {
        JsonSerializer js = new JsonSerializer();
        MainForm form;

        public ClientCallback(MainForm f)
        {
            form = f;
        }
        public void Handle(Response r)
        {
            if (r.RequestType == RequestEnum.ListOfDrives)
            {
                form.drives = JArray.FromObject(r.Data).ToObject<List<DriveData>>();

                form.Invoke((MethodInvoker)delegate
                {
                    form.UpdatePanes();
                });

                
            }
        }
    }
}
