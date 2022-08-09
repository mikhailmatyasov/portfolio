using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BSB.Microservices.NServiceBus
{
   public  class None : IAssemblySearchPattern
    {
        public string PatternToMatch { get; set; }

        public bool Match(Assembly assemblyName)
        {
            return false;
        }

        public bool Match(string assemblyName)
        {
            return false;
        }
    }
}
