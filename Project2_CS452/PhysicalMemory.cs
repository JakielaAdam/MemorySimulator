using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2_CS452
{
    class PhysicalMemory
    {
        private List<PhysicalFrame> pfList;
        private int pageFaultCounter;

        public PhysicalMemory(int size)
        {
            pageFaultCounter = 0;
            pfList = new List<PhysicalFrame>();
            
            //instantiate the frame list with empty frames
            for (int x = 0; x < size; x++)
            {
                pfList.Add(new PhysicalFrame());
            }
        }

        public int getFaultCounter()
        {
            return this.pageFaultCounter;
        }

        public void addPage(PageTableEntry pte, int step)
        {
            bool pageAdded = false;
            int longestDeltaTime = 0;
            int longestDeltaTimeIndex = 0;
            int index = 0;

            pte.setModified(step);

            //see if there is an empty slot (LRU algorithm)
            foreach (PhysicalFrame pf in pfList)
            {
                int newDelta = step - pf.getStepLastReferenced();
                if (newDelta > longestDeltaTime)
                {
                    longestDeltaTime = newDelta;
                    longestDeltaTimeIndex = index;
                }

                if (pf.isEmpty())
                {
                    pageFaultCounter++;
                    pte.getPageTable().getOwningProcess().incrementPageFaultCounter();
                    pf.fillFrame(pte);
                    pageAdded = true;
                    break;
                }
                index++;
            }

            if (pageAdded)
                return;

            pageFaultCounter++;
            pte.getPageTable().getOwningProcess().incrementPageFaultCounter();
            pfList[longestDeltaTimeIndex].fillFrame(pte);

        }

        //returns the location of a page table entry in the physical mem
        public int inFrame(PageTableEntry pte)
        {
            int retVal = 0;
            foreach (PhysicalFrame pf in pfList)
            {
                if (pf.getEntry() == pte)
                    return retVal;
                retVal++;
            }
            return -1;
        }

        public List<PhysicalFrame> getFrameList()
        {
            return pfList;
        }

        public PhysicalFrame getFrame(int index)
        {
            return pfList[index];
        }
    }
}
 