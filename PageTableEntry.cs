using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2_CS452
{
    class PageTableEntry
    {
        private bool validBit = false;
        private bool residentBit = false;
        private int frameNumber = 0;
        private int secondaryAddress = 0;
        private int lastModified = 0;
        private int ownerPID = 0;

        public PageTableEntry(int frameId, bool valid, bool resident, int secondary, int pid)
        {
            frameNumber = frameId;
            validBit = valid;
            residentBit = resident;
            secondaryAddress = secondary;
            ownerPID = pid;
        }

        public int getFrame()
        {
            return frameNumber;
        }

        public int getSecondary()
        {
            return secondaryAddress;
        }

        public int getLastModified()
        {
            return lastModified;
        }

        public int getOwnerPID()
        {
            return ownerPID;
        }

        public bool isValid()
        {
            return validBit;
        }

        public bool isResident()
        {
            return residentBit;
        }

        public void setFrame(int frame)
        {
            frameNumber = frame;
        }

        public void setSecondary(int secondary)
        {
            secondaryAddress = secondary;
        }

        public void setValid(bool valid)
        {
            validBit = valid;
        }

        public void setResident(bool resident)
        {
            residentBit = resident;
        }

        public void setModified(int modified)
        {
            lastModified = modified;
        }

        public bool isEmpty()
        {
            bool empty = false;
            if (frameNumber == -1)
                empty = true;
            return empty;
            
        }

    }
}
