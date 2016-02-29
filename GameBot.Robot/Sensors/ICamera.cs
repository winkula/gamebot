﻿using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Robot.Sensors
{
    public interface ICamera
    {
        Image Capture();
    }
}
