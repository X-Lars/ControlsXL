using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlsXL.Interfaces
{
    public interface INextItemsAdorner
    {
        string Next { get; }
        string Previous { get; }
        double ActualHeight { get; }
        double ActualWidth { get; }
    }
}
