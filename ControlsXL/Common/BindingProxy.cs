using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ControlsXL.Common
{
    /// <summary>
    /// 
    /// </summary>
    ///     <common:BindingProxy x:Key="BorderThicknessProxy" Data="{DynamicResource {ComponentResourceKey xl:Styles, BorderThickness}}"/>


    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof(Data), typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
    }
    
}
