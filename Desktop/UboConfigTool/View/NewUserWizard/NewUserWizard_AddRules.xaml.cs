using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Models;
using UboConfigTool.Infrastructure.UserGuidBase;
using UboConfigTool.ViewModel;

namespace UboConfigTool.View
{
    public class RuleSpecification
    {
        public string Description
        {
            get;
            set;
        }

        public string Pattern
        {
            get;
            set;
        }

        public bool IsSelected
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interaction logic for NewUserWizard_AddRules.xaml
    /// </summary>
    [Export( "NewUserWizard_AddRules" )]
    public partial class NewUserWizard_AddRules : UserControl, IUserGuidValidation, INavigationAware
    {
        [Import]
        public IRegionManager RegionManager;

        public NewUserWizard_AddRules()
        {
            InitializeComponent();

            InitRules();

            this.DataContext = this;
        }

        public bool ValidateData()
        {
            return true;
        }

        private ObservableCollection<RuleSpecification> _whiteRules = new ObservableCollection<RuleSpecification>();
        public ObservableCollection<RuleSpecification> WhiteRules
        {
            get
            {
                return _whiteRules;
            }
        }

        private ObservableCollection<RuleSpecification> _blackRules = new ObservableCollection<RuleSpecification>();
        public ObservableCollection<RuleSpecification> BlackRules
        {
            get
            {
                return _blackRules;
            }
        }

        public bool IsNavigationTarget( NavigationContext navigationContext )
        {
            return true;
        }

        public void OnNavigatedFrom( NavigationContext navigationContext )
        {
            DTOUBOGroup wizardDataObj = ( RegionManager.Regions[RegionNames.SecondaryRegion].Context as NewUserWizardViewModel ).WizardDataObject;
            
            if( wizardDataObj.rules == null )
                wizardDataObj.rules = new List<DTORule>();

            wizardDataObj.rules.Clear();
            foreach(RuleSpecification rule in _whiteRules)
            {
                if( !rule.IsSelected )
                    continue;

                wizardDataObj.rules.Add( new DTORule()
                    {
                        rule_type = 2,
                        pattern = rule.Pattern
                    } );
            }

            foreach(RuleSpecification rule in _blackRules)
            {
                if( !rule.IsSelected )
                    continue;

                wizardDataObj.rules.Add( new DTORule()
                {
                    rule_type = 1,
                    pattern = rule.Pattern
                } );
            }
        }

        public void OnNavigatedTo( NavigationContext navigationContext )
        {
            DTOUBOGroup wizardDataObj = ( RegionManager.Regions[RegionNames.SecondaryRegion].Context as NewUserWizardViewModel ).WizardDataObject;
            InitRules();
            if( wizardDataObj.rules != null )
            {
                foreach( DTORule rule in wizardDataObj.rules )
                {
                    RuleSpecification ruleSpec =  _whiteRules.FirstOrDefault( item => item.Pattern == rule.pattern );
                    if( ruleSpec != null )
                        ruleSpec.IsSelected = true;
                    else
                    {
                        ruleSpec = _blackRules.FirstOrDefault( item => item.Pattern == rule.pattern );
                        if( ruleSpec != null )
                            ruleSpec.IsSelected = true;
                    }
                }
            }
        }

        private void InitRules()
        {
            _whiteRules.Clear();
            _whiteRules.Add( new RuleSpecification()
            {
                Description = ".DOC文件",
                Pattern = ".doc"
            } );

            _whiteRules.Add( new RuleSpecification()
            {
                Description = ".TXT文件",
                Pattern = ".txt"
            } );

            _whiteRules.Add( new RuleSpecification()
            {
                Description = ".XLS文件",
                Pattern = ".xls"
            } );

            _whiteRules.Add( new RuleSpecification()
            {
                Description = ".PPT文件",
                Pattern = ".ppt"
            } );

            _blackRules.Clear();
            _blackRules.Add( new RuleSpecification()
            {
                Description = ".EXE文件",
                Pattern = ".exe"
            } );

            _blackRules.Add( new RuleSpecification()
            {
                Description = ".MP3文件",
                Pattern = ".mp3"
            } );

            _blackRules.Add( new RuleSpecification()
            {
                Description = ".AVI文件",
                Pattern = ".avi"
            } );

            _blackRules.Add( new RuleSpecification()
            {
                Description = ".SWF文件",
                Pattern = ".swf"
            } );
        }
    }
}
