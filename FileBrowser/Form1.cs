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
using System.Threading;
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
        public bool loading = false;

        public List<string> Files;
        public List<string> Folders;

        public MainForm()
        {
            InitializeComponent();

            new Task(() =>
            {
                server = new FileBrowserHelper();
                server.LoadServerListener();
            }).Start();

            new Task(() =>
                {
                    cb = new ClientCallback(this);
                    client = new FileBrowserHelper();
                    client.LoadClientListener(cb);
                    client.SendMessageToServer(js.Serialize(new Request() { RequestType = Models.RequestEnum.ListOfDrives }));
                }).Start();
        }

        public void UpdateProgress(int filePerc, int jobPerc)
        {
            jobProgress.Value = jobPerc;
            fileProgress.Value = filePerc;
        }

        public void UpdatePanes()
        {
            UpdatePane(left, ref leftPaneDirectory);
            UpdatePane(right, ref rightPaneDirectory);
        }

        private void UpdatePane(ListView box, ref string dir)
        {
            if (String.IsNullOrEmpty(dir))
            {
                box.Items.Clear();
                foreach (var d in drives)
                {
                    box.Items.Add(d.DriveLetter + " [" + d.DriveType + "]");
                }

                UpdateDirBoxes();
            }
            else
            {
                client.SendMessageToServer(js.Serialize(new Request() { RequestType = Models.RequestEnum.OpenFolder, Data = dir }));
                loading = true;
                while (loading) { Thread.Sleep(200); }

                box.Items.Clear();
                var files = this.Files;
                var dirs = this.Folders;

                box.Items.Add(" .. [ BACK ]");

                foreach (var d in dirs)
                {
                    box.Items.Add(Path.GetFileName(d) + " [ FOLDER ]");
                }

                foreach (var f in files)
                {
                    box.Items.Add(Path.GetFileName(f) + " [ file ]");
                }

                UpdateDirBoxes();
            }
        }

        private void HandleDoubleClick(ListView box, string s, ref string dir)
        {
            if (s.Contains(" [ BACK ]"))
            {
                var parent = Directory.GetParent(dir);
                if (parent == null)
                {
                    dir = "";
                }
                else
                {
                    dir = parent.FullName;
                }
                UpdatePane(box, ref dir);
            }
            else if (s.Contains(" [ FOLDER ]"))
            {
                s = s.Replace(" [ FOLDER ]", "");
                dir = Path.Combine(dir, s);
                UpdatePane(box, ref dir);
            }
            else if (!s.Contains(" [ file ]") && s[3] == ' ')
            {
                s = s.Substring(0, 3);
                dir = Path.Combine(dir, s);
                UpdatePane(box, ref dir);
            }
        }

        private void UpdateDirBoxes()
        {
            leftBox.Text = leftPaneDirectory;
            rightBox.Text = rightPaneDirectory;
        }

        private void left_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var ret = left.HitTest(e.Location);
            var item = ret.Item;
            HandleDoubleClick(left, item.Text, ref leftPaneDirectory);
        }

        private void right_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var ret = right.HitTest(e.Location);
            var item = ret.Item;
            HandleDoubleClick(right, item.Text, ref rightPaneDirectory);
        }

        private void copyLeftToRight_Click(object sender, EventArgs e)
        {
            List<string> sourceItems = new List<string>();
            foreach (ListViewItem i in left.SelectedItems)
            {
                if (i.Text.Contains(" [ FOLDER ]") || i.Text.Contains(" [ file ]"))
                {
                    sourceItems.Add(Path.Combine(leftPaneDirectory, i.Text.Replace(" [ FOLDER ]", "").Replace(" [ file ]", "")));
                }
            }

            if (sourceItems.Count == 0 || String.IsNullOrWhiteSpace(rightPaneDirectory))
            {
                return;
            }

            client.SendMessageToServer(js.Serialize(new Request() { RequestType = Models.RequestEnum.CopyData, Data = new CopyDataRequest() { Data = sourceItems, Destination = rightPaneDirectory } }));
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
            else if (r.RequestType == RequestEnum.OpenFolder)
            {
                FolderData data = ((JObject)r.Data).ToObject<FolderData>();
                form.Files = data.Files;
                form.Folders = data.Folders;
                form.loading = false;
            }
            else if (r.RequestType == RequestEnum.CopyData)
            {
                Debug.WriteLine("Client Recieved Copy Complete!");
                form.Invoke((MethodInvoker)delegate
                {
                    form.UpdatePanes();
                });
                form.loading = false;
            }
            else if (r.RequestType == RequestEnum.CopyUpdate)
            {
                CopyDataUpdate data = ((JObject)r.Data).ToObject<CopyDataUpdate>();
                Debug.WriteLine("Job Progress: " + (data.jobProgress * 100.0 / data.jobTotal * 1.0) + " - Copy Progress: " + data.Progress);
                form.Invoke((MethodInvoker)delegate
                {
                    form.UpdateProgress(data.Progress, Convert.ToInt32(data.jobProgress * 100.0 / data.jobTotal * 1.0));
                });
                
            }
        }
    }
}
