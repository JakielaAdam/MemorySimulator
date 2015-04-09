using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2_CS452
{
    class PageTable
    {
        public List<PageTableEntry> entryList;
        public int pid;
        private VirtualMemory vm;
        private PhysicalMemory pm;
        private Process owningProcess;

        public PageTable(int size, int _pid, VirtualMemory _vm, PhysicalMemory _pm, Process p)
        {
            pid = _pid;
            entryList = new List<PageTableEntry>();
            vm = _vm;
            pm = _pm;
            owningProcess = p;

            for (int x = 0; x < 64; x++)
            {
                if(x < 16)
                    entryList.Add(new PageTableEntry(-1, true, false, -1, pid));
                else if(x > 16 && x < 64)
                    entryList.Add(new PageTableEntry(-1, false, false, -1, pid));
            }
        }

        public List<PageTableEntry> getEntryList()
        {
            return entryList;
        }

        public bool inPageTable(int frameId)
        {
            bool inTable = false;
            foreach (PageTableEntry pte in entryList)
            {
                if (frameId == pte.getFrame())
                {
                    inTable = true;
                    break;
                }
            }
            return inTable;
        }

        public void addPage(int frameId, int step)
        {
            bool added = false;

            for (int x = 0; x < 16; x++)
            {
                //an empty valid frame was found in page table
                if (entryList[x].isEmpty())
                {
                    entryList[x].setFrame(frameId);
                    added = true;
                    pm.addPage(entryList[x], owningProcess.getPid());
                    break;
                }
            }

            if (added)
                return;

            //a valid page wasnt found, make room for the frame in valid page
            int leasedUsed = 0;
            int leasedUsedIndex = 0;
            int index = 0;
            foreach (PageTableEntry pte in entryList)
            {
                //find the page which was lease recently used
                int timeDelta = step - pte.getLastModified();
                if (timeDelta > leasedUsed)
                {
                    leasedUsed = timeDelta;
                    leasedUsedIndex = index;
                }
                index++;
            }

            //save a copy of the frame being removed in next open space
            //TODO: if the frame being removed was in memory, do proper computations
            for (int x = 16; x < 64; x++)
            {
                if (entryList[x].isEmpty())
                {
                    //copy page info to new frame
                    PageTableEntry pte = entryList[x];
                    pte.setFrame(entryList[index].getFrame());
                    pte.setModified(step);
                    pte.setResident(false);
                    pte.setValid(false);
                    break;
                }
            }

            //overwrite the old valid page
            PageTableEntry npte = entryList[index];
            npte.setFrame(frameId);
            npte.setModified(step);
            pm.addPage(npte, owningProcess.getPid());

        }

        public void update()
        {
            //TODO
            foreach (PageTableEntry pte in entryList)
            {
                int phyFrameId = pm.inFrame(pte);
                bool acctuallyInMem = (phyFrameId != -1);
                if (pte.isResident() != acctuallyInMem)
                {
                    pte.setResident(!pte.isResident());
                    pte.setResident(false);
                }

                if (acctuallyInMem)
                {
                    pte.setSecondary(phyFrameId);
                    pte.setResident(true);
                }

            }
        }
    }
}
