using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC3Functions
{
    public class GeneralException : Exception
    {
        public GeneralException(string message)
            : base(message)
        {
        }
    }

    public class SectionNotFoundException : Exception
    {
        public SectionNotFoundException()
            : base("Section not found")
        {
        }
    }

    public class Class4SectionException : Exception
    {
        public Class4SectionException()
            : base("Section is class 4")
        {
        }
    }

    public class SectionNotYetHandledException : Exception
    {
        public SectionNotYetHandledException()
            : base("Section not yet handled")
        {
        }
    }

    public class InvalidSectionClassException : Exception
    {
        public InvalidSectionClassException()
            : base("Invalid section class")
        {
        }
    }
}
