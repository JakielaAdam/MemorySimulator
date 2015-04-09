using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2_CS452
{
    class PhysicalMemory
    {
        public List<PhysicalFrame> pfList;
        public int size;

        public PhysicalMemory(int _size)
        {
            size = _size;
            pfList = new List<PhysicalFrame>();
            
            //instantiate the frame list with empty frames
            for (int x = 0; x < size; x++)
            {
                pfList.Add(new PhysicalFrame(x));
            }
        }

        public PhysicalFrame getFrame(int index)
        {
            return pfList[index];
        }

        //TODO: should return the frame number
        public void addPage(PageTableEntry pte, int step)
        {
            bool pageAdded = false;
            int longestDeltaTime = 0;
            int longestDeltaTimeIndex = 0;
            int index = 0;

            //see if there is an empty slot
            foreach (PhysicalFrame pf in pfList)
            {
                int newDelta = step - pf.getStepLastModified();
                if (newDelta > longestDeltaTime)
                {
                    longestDeltaTime = newDelta;
                    longestDeltaTimeIndex = index;
                }

                if (pf.isEmpty())
                {
                    pf.fillFrame(pte, step);
                    pageAdded = true;
                    break;
                }

                index++;
            }

            if (pageAdded)
                return;

            pfList[longestDeltaTimeIndex].fillFrame(pte, step);

        }

        public void removePage(PageTableEntry pte)
        {

        }

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

    }
}
 