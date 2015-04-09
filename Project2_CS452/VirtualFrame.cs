using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2_CS452
{
    class VirtualFrame
    {
        Boolean referenced = false;
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
