using Models;
using RemoteFileBrowser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileBrowser
{
    public partial class Form1 : Form
    {
        FileBrowserHelper h;
        public Form1()
        {
            InitializeComponent();
            h = new FileBrowserHelper();
            h.LoadListener();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            JsonSerializer js = new JsonSerializer();
            h.SendMessage(js.Serialize(new Request() { RequestType = Models.RequestEnum.ListOfDrives }));
        }
    }
}
