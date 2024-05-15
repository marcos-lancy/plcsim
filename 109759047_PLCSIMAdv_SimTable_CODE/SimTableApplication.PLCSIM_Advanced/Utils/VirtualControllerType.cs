using System.ComponentModel;

namespace SimTableApplication.PLCSIM_Advanced.Utils
{
    public enum VirtualControllerType
    {        
        [Description("S7-1500")]
        S71500 = 1500,
        [Description("ET200SP-PLC")]
        ET200SP = 132572,
    }
}
