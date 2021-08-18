using System;

namespace MDVM.Model
{
    class MDVMScatterplotDataLayer : MDVMDataLayer
    {
        public override Type GetViewType()
        {
            return typeof(MDVMScatterplotView);
        }
    }
}
