using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Project2_CS452
{
    public partial class Form1 : Form
    {
        public const int LOGICAL_ADDR_SPACE = 64;
        public const int PHY_MEM_SIZE = 16;
        public const int FRAME_SIZE = 1;
        public const int PROCESS_NUM = 5;

        int stepCount = 0;
        int lineCount = 0;

        LinkedList<string> fileLines = new LinkedList<string>();
        LinkedListNode<string> node;
        List<Process> processList;
        List<DataGridView> dgvList;         //stores virtual mem GUI tables
        List<DataGridView> pageDVGList;     //stores page table GUI tables

        PhysicalMemory pm;                  //Physical mem object for the system

        bool started = false;
        bool runToEnd = false;

        public Form1()
        {
            //initialize the UI elements
            InitializeComponent();

            initDataGridViews();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            nextButton.Click += new EventHandler(this.NextBtn_Click);
            completeButton.Click += new EventHandler(this.ContinueBtn_Click);

            nextButton.Text = "Start";

            //create the physical memory
            pm = new PhysicalMemory(PHY_MEM_SIZE);
            
            //create the 5 processes
            processList = new List<Process>();
            for (int x = 0; x < (PROCESS_NUM + 1); x++)
            {
                processList.Add(new Process(x, LOGICAL_ADDR_SPACE, pm));
            }

            //read in the lines from the file
            string[] lines = System.IO.File.ReadAllLines(@"input2a.data");
            foreach (string line in lines)
            {
                Console.WriteLine(line);
                fileLines.AddLast(line);
                lineCount++;
            }

            node = fileLines.First;

        }

        //iterates through the input file
        void runNext()
        {
            if (!runToEnd)
            {
                referenceLabel.Text = node.Value + " (Line: " + stepCount + "/" + lineCount + ")";
            }
            
            //run the main logic for the line
            reference(node.Value);

            if (node.Next != null)
            {
                node = node.Next;
                stepCount++;
            }
            else
            {
                stepCount++;
                referenceLabel.Text = "Complete";
                this.nextButton.Enabled = false;
            }
        }



        //main starts logic ran here
        void reference(string curRef)
        {
            //parse process and reference binary
            int pid = Convert.ToInt32(curRef.Substring(1, 1));
            string bin = curRef.Substring(4, 6);
            Console.WriteLine(bin);
            int index = Convert.ToInt32(bin, 2);

            //add reference
            //the process class will handle computation
            processList[pid].reference(index, stepCount);

            //update the page tables
            foreach (Process p in processList)
            {
                p.getPageTable().update();
            }

            //when the process is done with the reference,
            //update the GUI
            if (!runToEnd || (stepCount == (lineCount - 1)))
            {
                updateGui(pid);
            }
        }

        void updateGui(int _pid)
        {
            this.pageFaultLabel.Text = "Page faults: " + pm.getFaultCounter()
            + " (P1: " + processList[1].getPageFaultCounter() + " |"
            + " P2: " + processList[2].getPageFaultCounter() + " |"
            + " P3: " + processList[3].getPageFaultCounter() + " |"
            + " P4: " + processList[4].getPageFaultCounter() + " |"
            + " P5: " + processList[5].getPageFaultCounter() + ")";

            //update the VM data grid views
            for (int z = 0; z < PROCESS_NUM; z++)
            {
                DataGridView dgv = this.dgvList[z];
                for (int x = 0; x < LOGICAL_ADDR_SPACE; x++)
                {
                    List<VirtualFrame> vfl = processList[z + 1].getFrameList();
                    VirtualFrame vf = vfl[x];
                    if (vf.isReferenced())
                    {
                        dgv.Rows[x].Cells[1].Value = "Yes";
                        dgv.Rows[x].Cells[1].Style.BackColor = Color.Gray;
                    }
                }
            }

            //update each page table data grid view
            int y = 0;
            foreach (DataGridView dgv in this.pageDVGList)
            {
                List<PageTableEntry> ptel = this.processList[y + 1].getPageTable().getEntryList();
                int ptIndex = 0;
                foreach (PageTableEntry pte in ptel)
                {
                    string frame = pte.getFrame().ToString();
                    string valid = "Y";
                    string resident = "Y";

                    if (pte.getFrame() == -1)
                    {
                        frame = "";
                        valid = "";
                        resident = "";
                    }
                    else
                    {
                        if (!pte.isValid())
                            valid = "N";

                        if (!pte.isResident())
                            resident = "N";

                        dgv[0, ptIndex].Value = frame;
                        dgv[1, ptIndex].Value = valid;
                        dgv[2, ptIndex].Value = resident;
                    }
                    ptIndex++;
                }
                y++;
            }

            //update physical memory data grid view
            List<PhysicalFrame> pfl = this.pm.getFrameList();
            for (int x = 0; x < PHY_MEM_SIZE; x++)
            {
                if (!pfl[x].isEmpty())
                {
                    PageTableEntry pteInMem = pfl[x].getEntry();
                    int framePID = pteInMem.getOwnerPID();
                    int pageTableFrame = pteInMem.getFrame();
                    this.phyDataGridView.Rows[x].Cells[1].Value = pteInMem.getLastModified().ToString();
                    this.phyDataGridView.Rows[x].Cells[1].Style.BackColor = getColorForPID(framePID);
                }
            }
        }


        void initDataGridViews()
        {
            //add all VM data grid views to the list
            this.dgvList = new List<DataGridView>();
            this.dgvList.Add(this.dataGridView1);
            this.dgvList.Add(this.dataGridView2);
            this.dgvList.Add(this.dataGridView3);
            this.dgvList.Add(this.dataGridView4);
            this.dgvList.Add(this.dataGridView5);

            //add all page table data grid views to the list
            this.pageDVGList = new List<DataGridView>();
            this.pageDVGList.Add(this.page1DataGridView);
            this.pageDVGList.Add(this.page2dataGridView);
            this.pageDVGList.Add(this.page3DataGridView);
            this.pageDVGList.Add(this.page4DataGridView);
            this.pageDVGList.Add(this.page5DataGridView);

            //instantiate the table cells
            for (int x = 0; x < LOGICAL_ADDR_SPACE; x++)
            {
                foreach (DataGridView dgv in this.dgvList)
                {
                    dgv.Rows.Add(x.ToString(), "No");
                }


                foreach (DataGridView dgv in this.pageDVGList)
                {
                    dgv.Rows.Add("", "", "", "");
                }

            }

            for (int x = 0; x < PHY_MEM_SIZE; x++)
            {
                this.phyDataGridView.Rows.Add(x.ToString(), "No");
            }
        }

        //returns the color associated with a process
        private Color getColorForPID(int pid)
        {
            switch (pid - 1)
            {
                case 0:
                    return Color.LightCoral;
                case 1:
                    return Color.DarkOrange;
                case 2:
                    return Color.YellowGreen;
                case 3:
                    return Color.LightSeaGreen;
                case 4:
                    return Color.Gold;
            }
            return Color.Gray;
        }

        //============GUI event handlers============
        void NextBtn_Click(Object sender,
                           EventArgs e)
        {
            if (started == false)
            {
                nextButton.Text = "Next";
                started = true;
            }

            runNext();
        }

        void ContinueBtn_Click(Object sender,
                           EventArgs e)
        {
            runToEnd = true;
            while (stepCount != lineCount)
            {
                Console.WriteLine(stepCount);
                runNext();

            }
        }
    }
}
