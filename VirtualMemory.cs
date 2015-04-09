using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2_CS452
{
    class VirtualMemory
    {
        public int pid;
        public int size;
        public List<VirtualFrame> vfList;

        public VirtualMemory(int _pid, int _size)
        {
            pid = _pid;
            size = _size;
            vfList = new List<VirtualFrame>();

            for (int x = 0; x < size; x++)
            {
                vfList.Add(new VirtualFrame(x));
            }
        }

        public VirtualFrame getFrame(int index)
        {
            return vfList[index];
        }

        public void setReference(int index)
        {
            vfList[index].setReferenced();

        }

    }
}
