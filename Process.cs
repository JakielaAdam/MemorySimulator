using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2_CS452
{
   
    class Process
    {
        public int pid;
        public int vmSize;
        public VirtualMemory vm;
        public PageTable pt;

        public Process(int _pid, int _vmSize, PhysicalMemory pm)
        {
            pid = _pid;
            vmSize = _vmSize;

            vm = new VirtualMemory(pid, vmSize);
            pt = new PageTable(vmSize, pid, vm, pm, this);
        }

        public void addReference(int frameId, int step)
        {
            vm.setReference(frameId);
            
            //if the new reference is not in the page table, add it
            if (!pt.inPageTable(frameId))
            {
                Console.WriteLine("CALLED.");
                pt.addPage(frameId, step);
            }
            else
            {
                //TODO: if it is, signal the reference
                Console.WriteLine("REF FOUND.");
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
    }
}
