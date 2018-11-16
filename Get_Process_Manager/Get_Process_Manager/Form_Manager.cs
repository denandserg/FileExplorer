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


namespace Get_Process_Manager
{   

    public partial class FormBase : Form
    {
        
        PerformanceCounter cpuCounter = new PerformanceCounter();
        private ListViewColumnSorter lvwColumnSorter;
        public string pr_kill;
        public FormBase()
        {
            InitializeComponent();
            lvwColumnSorter = new ListViewColumnSorter();
            this.listView1.ListViewItemSorter = lvwColumnSorter;
        }

       

        private void button_Get_Process_Click(object sender, EventArgs e)
        {
            Start_fill_list();
        }

        private void Start_fill_list()
        {
            Process[] allProcess = System.Diagnostics.Process.GetProcesses();
            PerformanceCounter theCPUCounter = new PerformanceCounter("Process", "% Processor Time",
                            Process.GetCurrentProcess().ProcessName);
            PerformanceCounter myCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use", null);
            listView1.Items.Clear();
            ImageList images = new ImageList();

            foreach (var process in allProcess)
            {
                imageList1.Images.Add(process.ProcessName, IconHelper.ExtractAssociatedIcon(Path.GetFullPath(process.ProcessName)));
                ListViewItem list = new ListViewItem();
                list.Text = process.ProcessName;
                list.ImageKey = process.ProcessName;
                list.SubItems.Add(process.Id.ToString());
                list.SubItems.Add(theCPUCounter.NextValue() + "%");
                long ram = process.PrivateMemorySize64;
                list.SubItems.Add((Size(ram)));
                list.SubItems.Add(Convert.ToInt32(myCounter.NextValue()).ToString()+" %");
                listView1.Items.Add(list);

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

        private void FormBase_Load(object sender, EventArgs e)
        {
            ColumnHeader columnheader;// Used for creating column headers.
            ListViewItem listviewitem;// Used for creating listview items.
           

        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
              
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            
            this.listView1.Sort();
        }

        private void buttonKill_Click(object sender, EventArgs e)
        {
            foreach (Process process in Process.GetProcessesByName(pr_kill))
            {
                process.Kill();
                process.WaitForExit();
            }

            Start_fill_list();


        }

        public void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
           pr_kill = e.Item.Text;
            
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }
        
        
        private string CPU_total()
        {
            
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";
            string rezult = String.Format("{0:00} %", cpuCounter.NextValue());
            return rezult;
        }

        public string getAvailableRAM()
        {
           
               PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes", true);
               string rezult = Convert.ToInt32(ramCounter.NextValue()).ToString() + " Mb";
            return rezult;

        }


        


        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = CPU_total();
            label2.Text = getAvailableRAM();
        }



    }
}

