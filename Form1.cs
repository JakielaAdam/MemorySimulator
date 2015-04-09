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

        LinkedList<string> fileLines = new LinkedList<string>();
        LinkedListNode<string> node;

        List<Process> processList;
        List<DataGridView> dgvList;

        Boolean started = false;

        PhysicalMemory pm;

        int stepCount = 0;
        int lineCount = 0;
       
        public Form1()
        {
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

        void runNext()
        {
            referenceLabel.Text = node.Value + " (Line: " + stepCount + "/" + lineCount + ")";

            reference(node.Value);
            
            
            if (node.Next != null)
            {
                node = node.Next;
                stepCount++;
            }
            else
            {
                //TODO: end program
                referenceLabel.Text = "Complete";
                this.nextButton.Enabled = false;
            }
        }

    

        //main logic ran here
        void reference(string curRef)
        {
            //parse process and reference binary
            int pid = Convert.ToInt32(curRef.Substring(1, 1));
            string bin = curRef.Substring(4, 6);
            Console.WriteLine(bin);
            int index = Convert.ToInt32(bin, 2);

            //add reference
            //the process class will handle computation
            processList[pid].addReference(index, stepCount);

            //update the page tables
            foreach (Process p in processList)
            {
                p.getPageTable().update();
            }

            //when the process is done with the reference,
            //update the GUI
            updateGui(pid, processList[pid]);

        }


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
            //TODO
        }

        void updateGui(int _pid, Process p)
        {
            DataGridView dgv = this.dgvList[_pid - 1];
            List<VirtualFrame> vfl = p.getFrameList();
            List<PageTableEntry> ptel = p.getPageTable().getEntryList();
            List<PhysicalFrame> pfl = this.pm.getFrameList();

            //virtual memory
            for (int x = 0; x < LOGICAL_ADDR_SPACE; x++)
            {
                VirtualFrame vf = vfl[x];
                if (vf.isReferenced())
                {
                    dgv.Rows[x].Cells[1].Value = "Yes";
                    dgv.Rows[x].Cells[1].Style.BackColor = Color.Gray;
                }
            }

            this.pageDataGridView.Rows.Clear();

            //page table
            int y = 0;
            foreach (PageTableEntry pte in ptel)
            {
                string frame = pte.getFrame().ToString();
                string valid = "Y";
                string resident = "Y";
                string secondary = pte.getSecondary().ToString();

                if (pte.getFrame() == -1)
                    frame = "";

                if (pte.getSecondary() == -1)
                    secondary = "";

                if(!pte.isValid())
                    valid = "N";

                if (y < 16)
                {
                    if (!pte.isResident())
                        resident = "N";
                }
                else
                {
                    resident = "-";
                }


                this.pageDataGridView.Rows.Add(frame, valid, resident, secondary);
                y++;
            }
            
            this.pageGroupBox.Text = "Page Table (P" + _pid + ")";

            //physical memory
            for (int x = 0; x < PHY_MEM_SIZE; x++)
            {
                if (!pfl[x].isEmpty())
                {
                    PageTableEntry pteInMem = pfl[x].getEntry();
                    int framePID = pteInMem.getOwnerPID();
                    int pageTableFrame = pteInMem.getFrame();
                    this.phyDataGridView.Rows[x].Cells[1].Value = "Frame: " + pageTableFrame;
                    this.phyDataGridView.Rows[x].Cells[1].Style.BackColor = getColorForPID(framePID);
                }
            }
        }


        void initDataGridViews()
        {
            //add all the data drid views to the list
            this.dgvList = new List<DataGridView>();
            this.dgvList.Add(this.dataGridView1);
            this.dgvList.Add(this.dataGridView2);
            this.dgvList.Add(this.dataGridView3);
            this.dgvList.Add(this.dataGridView4);
            this.dgvList.Add(this.dataGridView5);

            for (int x = 0; x < LOGICAL_ADDR_SPACE; x++)
            {
                foreach (DataGridView dgv in this.dgvList)
                {
                    dgv.Rows.Add(x.ToString(), "No");
                }


                if (x < 16)
                    this.pageDataGridView.Rows.Add("", "Y", "N", "");
                else if (x > 16 && x < 21)
                    this.pageDataGridView.Rows.Add("", "N", "-", "");
            }

            for (int x = 0; x < PHY_MEM_SIZE; x++)
            {
                this.phyDataGridView.Rows.Add(x.ToString(), "No");
            }
        }

        private Color getColorForPID(int pid)
        {
            switch (pid - 1) {
                case 0:
                    return Color.LightCoral;
                    break;
                case 1:
                    return Color.DarkOrange;
                    break;
                case 2:
                    return Color.YellowGreen;
                    break;
                case 3:
                    return Color.LightSeaGreen;
                    break;
                case 4:
                    return Color.Gold;
                    break;  
            }
            return Color.Gray;

        }
    }   
}
