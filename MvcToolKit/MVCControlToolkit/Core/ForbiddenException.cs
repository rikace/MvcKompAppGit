using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCControlsToolkit.Core
{
    public class ForbiddenException: Exception
    {
        public ForbiddenException(string errorMessage):base(errorMessage)
        {
        }
    }
}
