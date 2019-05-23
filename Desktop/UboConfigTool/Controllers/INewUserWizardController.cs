using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UboConfigTool.Infrastructure.UserGuidBase;
using UboConfigTool.View;

namespace UboConfigTool.Controllers
{
    public delegate void WizardProcedureChanged( WizardProcedureType type );

    public interface INewUserWizardController
    {
        event WizardProcedureChanged WizardProcedureChangedEvent;

        DelegateCommand NextProcedureCommand
        {
            get;
        }

        DelegateCommand GoBackCommand
        {
            get;
        }

        DelegateCommand SkipProcessCommand
        {
            get;
        }

        WizardProcedureType CurrentProcedure
        {
            get;
        }

    }
}
