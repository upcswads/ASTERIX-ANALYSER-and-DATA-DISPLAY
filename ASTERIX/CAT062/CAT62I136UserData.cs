﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsterixDisplayAnalyser
{
    class CAT62I136UserData
    {

        public static void DecodeCAT62I136(byte[] Data)
        {
            // Increase data buffer index so it ready for the next data item.
            CAT62.CurrentDataBufferOctalIndex = CAT62.CurrentDataBufferOctalIndex + 2;
        }
    }
}
