using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2_CS452
{
    class PhysicalFrame
    {
        int location;
        int stepLastModified;
        int pid;
        string data;
        bool empty = true;
        PageTableEntry pte = null;

        public PhysicalFrame(int _location)
        {
            location = _location;
            stepLastModified = 0;
        }

        public void fillFrame(PageTableEntry _pte, int _stepLastModified)
        {
            empty = false;
            pte = _pte;
            stepLastModified = _stepLastModified;
        }


        public bool isEmpty()
        {
            return empty;
        }

        public int getStepLastModified()
        {
            return stepLastModified;
        }

        public PageTableEntry getEntry()
        {
            return pte;
        }

    }
}
