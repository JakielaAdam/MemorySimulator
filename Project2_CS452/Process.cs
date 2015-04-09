using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2_CS452
{
   
    class Process
    {
        private int pid;
        private int vmSize;
        private VirtualMemory vm;
        private PageTable pt;
        private int pageFaultCounter;

        public Process(int _pid, int _vmSize, PhysicalMemory pm)
        {
            pid = _pid;
            vmSize = _vmSize;
            pageFaultCounter = 0;

            vm = new VirtualMemory(pid, vmSize);
            pt = new PageTable(vmSize, pid, vm, pm, this);
        }

        public void reference(int frameId, int step)
        {
            vm.setReference(frameId);
            
            //if the new reference is not in the page table, add it
            if (!pt.inPageTable(frameId))
            {
                pt.addPage(frameId, step);
            }
            else
            {
                //if it is, reference it
                pt.reference(frameId, step);
            }
        }

        public List<VirtualFrame> getFrameList()
        {
            return vm.vfList;
        }

        public PageTable getPageTable()
        {
            return pt;
        }

        public int getPid()
        {
            return pid;
        }


        public int getPageFaultCounter()
        {
            return pageFaultCounter;
        }

        public void incrementPageFaultCounter()
        {
            pageFaultCounter++;
        }
    }
}
