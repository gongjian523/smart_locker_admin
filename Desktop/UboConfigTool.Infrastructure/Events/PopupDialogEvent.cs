using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UboConfigTool.Infrastructure.Events
{
    public class PopupDialogBeginEvent : CompositePresentationEvent<bool>
    {
    }

    public class PopupDialogEndEvent : CompositePresentationEvent<bool>
    {
    }
}
