using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Internal;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Get_Process_Manager;


namespace Explot
{
    [DataContract]
    public partial class Form1 : Form
    {
        const int WM_DEVICECHANGE = 0x219;
        const int DBT_DEVICEARRIVAL = 0x8000;
        const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private ListView activeList;
        private ListView activeList2;
        
        [DataMemberAttribute]
        public string currentActivePuth { get; set; }
        [DataMemberAttribute]
        public string currentActivePuth2;
        public Form1()
        {
            InitializeComponent();

            if (IsAdmin())
            {
                button2.Enabled = false;
            }
            else
            {
                SetButtonShield(button2, true);
            }

        }

        

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            
            if (keyData == Keys.Escape)
            {
                if (listViewDrive2.Enabled != true)
                {
                    currentActivePuth = currentActivePuth + "\\..";
                    FillList(currentActivePuth);
                }
                else
                {
                    currentActivePuth2 = currentActivePuth2 + "\\..";
                    FillList2(currentActivePuth2);
                }


            }

            if (keyData == Keys.Enter)
            {
                if (activeList.Enabled)
                {
                    if (Path.GetExtension(Path.Combine(currentActivePuth, activeList.SelectedItems[0].Text)) == "")
                    {
                        if (listViewDrive2.Enabled != true)
                        {
                            currentActivePuth = Path.Combine(
                                currentActivePuth, activeList.SelectedItems[0].Text);
                            FillList(currentActivePuth);
                        }
                        else
                        {
                            currentActivePuth2 = Path.Combine(
                                currentActivePuth2, activeList2.SelectedItems[0].Text);
                            FillList2(currentActivePuth2);
                        }
                    }
                    else
                    {
                        Process.Start(Path.Combine(currentActivePuth, activeList.SelectedItems[0].Text));
                    }
                }
                else
                {
                    if (Path.GetExtension(Path.Combine(currentActivePuth2, activeList2.SelectedItems[0].Text)) == "")
                    {
                        if (listViewDrive2.Enabled != true)
                        {
                            currentActivePuth = Path.Combine(
                                currentActivePuth, activeList.SelectedItems[0].Text);
                            FillList(currentActivePuth);
                        }
                        else
                        {
                            currentActivePuth2 = Path.Combine(
                                currentActivePuth2, activeList2.SelectedItems[0].Text);
                            FillList2(currentActivePuth2);
                        }
                    }
                    else
                    {
                        Process.Start(Path.Combine(currentActivePuth2, activeList2.SelectedItems[0].Text));
                    }
                }
                
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void WndProc(ref Message m)
        {

            if (m.Msg == WM_DEVICECHANGE && (int)m.WParam == DBT_DEVICEARRIVAL || (int)m.WParam == DBT_DEVICEREMOVECOMPLETE)
            {
                FillDriveButtons2();
                FillDriveButtons();
               
            }
            base.WndProc(ref m);
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(HandleRef hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        public static void SetButtonShield(Button btn, bool showShield)
        {
            // BCM_SETSHIELD = 0x0000160C

            btn.FlatStyle = FlatStyle.System;
            SendMessage(new HandleRef(btn, btn.Handle), 0x160C, IntPtr.Zero, showShield ? new IntPtr(1) : IntPtr.Zero);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            listViewDrive2.Enabled = false;
            FillDriveButtons2();
            FillDriveButtons();
            activeList = listViewDrive;
            activeList2 = listViewDrive2;
            FillList(Directory.GetLogicalDrives()[0]);
            FillList2(Directory.GetLogicalDrives()[0]);
            activeList.MouseDoubleClick += ActiveList_ItemCheck;
            activeList2.MouseDoubleClick += ActiveList_ItemCheck2;
            currentActivePuth = Directory.GetLogicalDrives()[0];
            currentActivePuth2 = Directory.GetLogicalDrives()[0];

        }

        private void ActiveList_ItemCheck(object sender, MouseEventArgs e)
        {
            if (Path.GetExtension(Path.Combine(currentActivePuth, activeList.SelectedItems[0].Text)) == "")
            {
                if (activeList.SelectedItems.Count == 1)
                {
                    if (activeList.Items[0].Selected)
                    {

                        currentActivePuth = currentActivePuth + "\\..";
                        FillList(currentActivePuth);

                    }
                    else
                    {
                        currentActivePuth = Path.Combine(
                            currentActivePuth, activeList.SelectedItems[0].Text);
                        FillList(currentActivePuth);
                    }
                }
            }
            else
            {
                Process.Start(Path.Combine(currentActivePuth, activeList.SelectedItems[0].Text));
            }

        }
        
        private void ActiveList_ItemCheck2(object sender, MouseEventArgs e)
        {
            if (Path.GetExtension(Path.Combine(currentActivePuth2, activeList2.SelectedItems[0].Text)) == "")
            {
                if (activeList2.SelectedItems.Count == 1)
                {
                    if (activeList2.Items[0].Selected)
                    {

                        currentActivePuth2 = currentActivePuth2 + "\\..";
                        FillList2(currentActivePuth2);

                    }
                    else
                    {
                        currentActivePuth2 = Path.Combine(
                            currentActivePuth2, activeList2.SelectedItems[0].Text);
                        FillList2(currentActivePuth2);
                    }
                }
            }
            else
            {
                Process.Start(Path.Combine(currentActivePuth2, activeList2.SelectedItems[0].Text));
            }

        }

        private void FillDriveButtons()
        {

            toolStrip1.Items.Clear();
            foreach (var drive in Directory.GetLogicalDrives())
            {

                ToolStripButton btn = new ToolStripButton
                {
                    Image = IconHelper.ExtractAssociatedIcon(drive).ToBitmap(),
                    Text = drive.Substring(0, 1),
                    Tag = drive,
                    DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
                };
                toolStrip1.Items.Add(btn);
                btn.Click += (s, a) => FillList(currentActivePuth = (s as ToolStripButton).Tag as string);

            }

        }

        private void FillDriveButtons2()
        {

            toolStrip1.Items.Clear();
            foreach (var drive in Directory.GetLogicalDrives())
            {

                ToolStripButton btn = new ToolStripButton
                {
                    Image = IconHelper.ExtractAssociatedIcon(drive).ToBitmap(),
                    Text = drive.Substring(0, 1),
                    Tag = drive,
                    DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
                };
                toolStrip1.Items.Add(btn);
                btn.Click += (s, a) => FillList2(currentActivePuth2 = (s as ToolStripButton).Tag as string);

            }

        }

        private void FillList(string path)
        {

            DirectoryInfo dir = new DirectoryInfo(path);
            activeList.Items.Clear();
            activeList.Items.Add("...", "back");
            imageListSmall.Images.Clear();
           


            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                imageListSmall.Images.Add(di.Name, IconHelper.ExtractAssociatedIcon(Path.GetFullPath(di.FullName)));
                ListViewItem list = new ListViewItem();
               
                list.Text = di.Name;
                list.ImageKey = di.Name;
                list.SubItems.Add(di.CreationTime.ToShortDateString());
                list.SubItems.Add("");
                list.SubItems.Add(di.Attributes.ToString());
                activeList.Items.Add(list);

            }

            foreach (FileInfo fi in dir.GetFiles())
            {
                imageListSmall.Images.Add(fi.Name, Icon.ExtractAssociatedIcon(fi.FullName));
                ListViewItem list = new ListViewItem();

                list.Text = fi.Name;
                list.ImageKey = fi.Name;
                long size = fi.Length;
                list.SubItems.Add(fi.CreationTime.ToShortDateString());
                list.SubItems.Add(Size(size));
                list.SubItems.Add(fi.Attributes.ToString());
                

                activeList.Items.Add(list);

            }

        }
        private string Size(long size)
        {
            if (size < 1000)
                return size + " B";
            else if (size < 1000000)
                return size / 1000 + " MB";
            else
                return size / 1000000 + " MB";
        }

        private void FillList2(string path)
        {
            activeList2.Items.Clear();
            DirectoryInfo dir = new DirectoryInfo(path);
            imageListSmall2.Images.Clear();
            activeList2.Items.Add("...", "back");

            ImageList images = new ImageList();
            

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                imageListSmall2.Images.Add(di.Name, IconHelper.ExtractAssociatedIcon(Path.GetFullPath(di.FullName)));
                ListViewItem list = new ListViewItem();

                list.Text = di.Name;
                list.ImageKey = di.Name;
                list.SubItems.Add(di.CreationTime.ToShortDateString());
                list.SubItems.Add("");
                list.SubItems.Add(di.Attributes.ToString());
                activeList2.Items.Add(list);
            }

            foreach (FileInfo fi in dir.GetFiles())
            {
                imageListSmall2.Images.Add(fi.Name, Icon.ExtractAssociatedIcon(fi.FullName));
                ListViewItem list = new ListViewItem();

                list.Text = fi.Name;
                list.ImageKey = fi.Name;
                long size = fi.Length;
                list.SubItems.Add(fi.CreationTime.ToShortDateString());
                list.SubItems.Add(Size(size));
                list.SubItems.Add(fi.Attributes.ToString());
                
                activeList2.Items.Add(list);

            }
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            if (listViewDrive.Enabled)
            {
                listViewDrive2.Enabled = true;
                FillDriveButtons2();
                listViewDrive.Enabled = false;

            }

            else
            {
                listViewDrive.Enabled = true;
                FillDriveButtons();
                listViewDrive2.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //RestartAsAdmin();
            this.Close();
            RunAsAdmin("explot.exe", "restart");
        }
        public bool IsAdmin()
        {

            System.Security.Principal.WindowsIdentity id = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal p = new System.Security.Principal.WindowsPrincipal(id);

            if (p.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator) == true)
            {
                return true;
            } 
            else
            {
                return false;
            }

            
        }

        public static void RunAsAdmin(string aFileName, string anArguments)
        {
            System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo();
            processInfo.FileName = aFileName;
            processInfo.Arguments = anArguments;
            processInfo.UseShellExecute = true;
            processInfo.Verb = "runas";

            System.Diagnostics.Process.Start(processInfo);
        }

        private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

            FormBase form_process = new FormBase();
            form_process.ShowDialog();

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }

   
}
