using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Input;
using UboConfigTool.Controllers;
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Models;
using UboConfigTool.Infrastructure.UserGuidBase;
using UboConfigTool.View;

namespace UboConfigTool.ViewModel
{
    public enum WizardType
    {
        NewUser = 0,
        NewUBOGroup
    }
    
    [Export]
    public class NewUserWizardViewModel : PopupViewModelBase
    {
        private INewUserWizardController _wizardController;

        public NewUserWizardViewModel( INewUserWizardController wizardController, WizardType type = WizardType.NewUser )
        {
            if( wizardController == null )
            {
                throw new ArgumentNullException( "NewUserWizardController" );
            }

            Title = ( type == WizardType.NewUser ? "新手向导" : "新建群组" );

            this._wizardController = wizardController;

            this._wizardController.WizardProcedureChangedEvent += _wizardController_WizardProcedureChangedEvent;
        }

        void _wizardController_WizardProcedureChangedEvent( WizardProcedureType type )
        {
            switch( type )
            {
                case WizardProcedureType.AddUBOGroup:
                    CurrentCompletedPercentage = 0;
                    break;
                case WizardProcedureType.AddUBO:
                    CurrentCompletedPercentage = 20;
                    break;
                case WizardProcedureType.AddRules:
                    CurrentCompletedPercentage = 40;
                    break;
                case WizardProcedureType.AddUSBs:
                    CurrentCompletedPercentage = 60;
                    break;
                case WizardProcedureType.Finish:
                    CurrentCompletedPercentage = 80;
                    break;
                default:
                    break;
            }

            IsLastStep = ( type == WizardProcedureType.Finish );
        }

        public ICommand NextProcedureCommand
        {
            get
            {
                return _wizardController.NextProcedureCommand;
            }
        }

        public ICommand GobackCommand
        {
            get
            {
                return _wizardController.GoBackCommand;
            }
        }

        public ICommand SkipProcessCommand
        {
            get
            {
                return _wizardController.SkipProcessCommand;
            }
        }

        private double _currentCompletedPercentage = 0;
        public double CurrentCompletedPercentage
        {
            get
            {
                return _currentCompletedPercentage;
            }
            set
            {
                _currentCompletedPercentage = value;
                this.RaisePropertyChanged( () => CurrentCompletedPercentage );
            }
        }

        private bool _isLastStep = false;
        public bool IsLastStep
        {
            get
            {
                return _isLastStep;
            }
            set
            {
                _isLastStep = value;
                this.RaisePropertyChanged( () => IsLastStep );
            }
        }

        public WizardProcedureType CurrentProcedure
        {
            get
            {
                return _wizardController.CurrentProcedure;
            }
        }

        private DTOUBOGroup _wizardDataObj = new DTOUBOGroup();
        public DTOUBOGroup WizardDataObject
        {
            get
            {
                return _wizardDataObj;
            }
        }

        public string Title
        {
            get;
            private set;
        }

        public override bool Validate()
        {
            return true;
        }
    }
}
