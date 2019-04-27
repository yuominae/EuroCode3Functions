using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC3Functions
{
    enum SectionTypes
    {
        I,
        RHS,
        CHS,
        BOX,
        INOWEB,
        BOXNOWEB,
        ANGLE,
        CHANNEL,
        USERDEFINED,
        OTHER
    }

    enum ShearAxis
    {
        Y,
        Z
    }

    enum BendingAxis
    {
        YY,
        ZZ
    }
}
