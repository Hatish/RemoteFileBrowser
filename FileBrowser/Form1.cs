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
                left.Items.Clear();
                var files = Directory.GetFiles(leftPaneDirectory);
                var dirs = Directory.GetDirectories(leftPaneDirectory);

                foreach (var d in dirs)
                {
                    left.Items.Add(Path.GetFileNameWithoutExtension(d) + " [ FOLDER ]");
                }

                foreach (var f in files)
                {
                    left.Items.Add(Path.GetFileName(f) + " [ file ]");
                }
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

            foreach (var i in left.Items)
            {
                
            }
        }

        private void left_DoubleClick(object sender, EventArgs e)
        {
            Debug.WriteLine("Double Click! - " + left.SelectedItem);

            var s = left.SelectedItem.ToString();
            if (s.Contains(" [ FOLDER ]"))
            {
                s = s.Replace(" [ FOLDER ]", "");
                leftPaneDirectory = Path.Combine(leftPaneDirectory, s);
                UpdatePanes();
            }
            else if (!s.Contains(" [ file ]") && s[3] == ' ')
            {
                s = s.Substring(0, 3);
                leftPaneDirectory = Path.Combine(leftPaneDirectory, s);
                UpdatePanes();
            }
        }

        private void left_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void right_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void right_DoubleClick(object sender, EventArgs e)
        {

        }

        private void left_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            Graphics g = e.Graphics;
            if (e.Index != -1)
            {
                var s = left.Items[e.Index].ToString();
                
                if (s.Contains(" [ FOLDER ]"))
                {
                    g.FillRectangle(new SolidBrush(Color.Silver), e.Bounds);
                }
                else if (s.Contains(" [ file ]"))
                {
                    g.FillRectangle(new SolidBrush(Color.AliceBlue), e.Bounds);
                }
                // Print text
            }
            e.DrawFocusRectangle();
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
