using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NicoV4.Common
{
    public static class WpfUtil
    {
        public static bool IsDesignMode()
        {
            // Check for design mode. 
            return (bool)DesignerProperties
                .IsInDesignModeProperty
                .GetMetadata(typeof(DependencyObject))
                .DefaultValue;
        }
    }
}
