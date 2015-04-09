using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2_CS452
{
    class PhysicalFrame
    {
        PageTableEntry pte = null;

        //fills the physical memory frame with a page
        public void fillFrame(PageTableEntry _pte)
        {
            pte = _pte;
        }

        public bool isEmpty()
        {
            if (pte == null)
                return true;
            else
                return false;
        }

        //returns the last time the page in the frame was referenced
        public int getStepLastReferenced()
        {
            if (pte == null)
                return 0;
            else
                return pte.getLastModified();
        }

        public PageTableEntry getEntry()
        {
            return pte;
        }

    }
}
