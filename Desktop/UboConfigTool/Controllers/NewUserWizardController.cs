using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Regions;
using UboConfigTool.View;
using Microsoft.Practices.Prism.ViewModel;
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.UserGuidBase;
using Microsoft.Practices.Prism.Events;
using UboConfigTool.Infrastructure.Events;

namespace UboConfigTool.Controllers
{
    [Export( typeof( INewUserWizardController ) )]
    [PartCreationPolicy( CreationPolicy.NonShared )]
    public class NewUserWizardController : NotificationObject, INewUserWizardController
    {
        private static Uri _newUserWizard_CreateUBOGroup_Uri = new Uri( "NewUserWizard_CreateUBOGroup", UriKind.Relative );
        private static Uri _newUserWizard_AddUBO_Uri = new Uri( "NewUserWizard_AddUBO", UriKind.Relative );
        private static Uri _newUserWizard_AddRule_Uri = new Uri( "NewUserWizard_AddRules", UriKind.Relative );
        private static Uri _newUserWizard_AddUSBs_Uri = new Uri( "NewUserWizard_AddUSBs", UriKind.Relative );
        private static Uri _newUserWizard_OverAll_Uri = new Uri( "NewUserWizard_Overall", UriKind.Relative );

        private IRegionManager RegionManager;

        [ImportingConstructor]
        public NewUserWizardController( IRegionManager regionMgr )
        {
            if( regionMgr == null )
            {
                throw new ArgumentNullException( "RegionManager" );
            }
            _nextProcedureCommand = new DelegateCommand( NextProcedure, CanExecuteNextProcedure );
            _goBackCommand = new DelegateCommand( GoBack, CanGoBack );
            _currentProcedure = WizardProcedureType.AddUBOGroup;
            RegionManager = regionMgr;
            //this.RegionManager.RequestNavigate( RegionNames.NewUserWizardRegion, _newUserWizard_CreateUBOGroup_Uri );
        }

        private bool CanGoBack()
        {
            return CurrentProcedure != WizardProcedureType.AddUBOGroup;
        }

        private void GoBack()
        {
            switch( CurrentProcedure )
            {
                case WizardProcedureType.AddUBO:
                    this.RegionManager.RequestNavigate( RegionNames.NewUserWizardRegion, _newUserWizard_CreateUBOGroup_Uri );
                    CurrentProcedure = WizardProcedureType.AddUBOGroup;
                    break;
                case WizardProcedureType.AddRules:
                    this.RegionManager.RequestNavigate( RegionNames.NewUserWizardRegion, _newUserWizard_AddUBO_Uri );
                    CurrentProcedure = WizardProcedureType.AddUBO;
                    break;
                case WizardProcedureType.AddUSBs:
                    this.RegionManager.RequestNavigate( RegionNames.NewUserWizardRegion, _newUserWizard_AddRule_Uri );
                    CurrentProcedure = WizardProcedureType.AddRules;
                    break;
                case WizardProcedureType.Finish:
                    this.RegionManager.RequestNavigate( RegionNames.NewUserWizardRegion, _newUserWizard_AddUSBs_Uri );
                    CurrentProcedure = WizardProcedureType.AddUSBs;
                    break;
                default:
                    break;
            }

            if( WizardProcedureChangedEvent != null )
            {
                WizardProcedureChangedEvent( CurrentProcedure );
            }
             
            NextProcedureCommand.RaiseCanExecuteChanged();
            GoBackCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteNextProcedure()
        {
            return CurrentProcedure != WizardProcedureType.Finish;
        }

        private void NextProcedure()
        {
            var currentProcedureViewObj = this.RegionManager.Regions[RegionNames.NewUserWizardRegion].ActiveViews.First( obj => obj != null );

            if( currentProcedureViewObj is IUserGuidValidation && !( currentProcedureViewObj as IUserGuidValidation ).ValidateData() )
                return;

            switch( CurrentProcedure )
            {
                case WizardProcedureType.AddUBOGroup:
                    this.RegionManager.RequestNavigate( RegionNames.NewUserWizardRegion, _newUserWizard_AddUBO_Uri, NavigationCallBack );
                    CurrentProcedure = WizardProcedureType.AddUBO;
                    break;
                case WizardProcedureType.AddUBO:
                    this.RegionManager.RequestNavigate( RegionNames.NewUserWizardRegion, _newUserWizard_AddRule_Uri, NavigationCallBack );
                    CurrentProcedure = WizardProcedureType.AddRules;
                    break;
                case WizardProcedureType.AddRules:
                    this.RegionManager.RequestNavigate( RegionNames.NewUserWizardRegion, _newUserWizard_AddUSBs_Uri, NavigationCallBack );
                    CurrentProcedure = WizardProcedureType.AddUSBs;
                    break;
                case WizardProcedureType.AddUSBs:
                    this.RegionManager.RequestNavigate( RegionNames.NewUserWizardRegion, _newUserWizard_OverAll_Uri, NavigationCallBack );
                    CurrentProcedure = WizardProcedureType.Finish;
                    break;
                default:
                    break;
            }

            if( WizardProcedureChangedEvent != null )
            {
                WizardProcedureChangedEvent( CurrentProcedure );
            }

            NextProcedureCommand.RaiseCanExecuteChanged();
            GoBackCommand.RaiseCanExecuteChanged();
        }

        private void NavigationCallBack( NavigationResult obj )
        {

        }

        private DelegateCommand _nextProcedureCommand;
        public DelegateCommand NextProcedureCommand
        {
            get
            {
                return _nextProcedureCommand;
            }
        }

        private DelegateCommand _goBackCommand;
        public DelegateCommand GoBackCommand
        {
            get
            {
                return _goBackCommand;
            }
        }

        public DelegateCommand SkipProcessCommand
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private WizardProcedureType _currentProcedure;
        public WizardProcedureType CurrentProcedure
        {
            get
            {
                return _currentProcedure;
            }
            private set
            {
                this._currentProcedure = value;
                this.RaisePropertyChanged( () => this.CurrentProcedure );
            }
        }

        public event WizardProcedureChanged WizardProcedureChangedEvent;
    }
}
