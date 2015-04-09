using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2_CS452
{
    class VirtualFrame
    {
        int index;
        Boolean referenced = false;

        public VirtualFrame(int _index)
        {
            index = _index;
        }

        public void setReferenced()
        {
            referenced = true;
        }

        public Boolean isReferenced()
        {
            return referenced;
        }


    }
}
