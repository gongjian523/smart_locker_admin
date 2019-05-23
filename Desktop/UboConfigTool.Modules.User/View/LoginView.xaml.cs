using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UboConfigTool.Infrastructure;
using UboConfigTool.Infrastructure.Behaviors;
using UboConfigTool.Infrastructure.Events;
using UboConfigTool.Infrastructure.Interfaces;
using UboConfigTool.Modules.User.ViewModel;

namespace UboConfigTool.Modules.User.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    [Export]
    public partial class LoginView : UserControl
    {
        [Import]
        public IEventAggregator EventAggregator;

        [Import]
        public IUBODataService _uboRepository;

        public LoginView()
        {
            InitializeComponent();
            this.Loaded += LoginView_Loaded;
        }

        void LoginView_Loaded( object sender, RoutedEventArgs e )
        {
            this.EventAggregator.GetEvent<UserLoginEvent>().Subscribe( this.UserLoggedIn, ThreadOption.UIThread );
        }

        private void UserLoggedIn( bool bSuc )
        {
            if( !bSuc )
                return;

            Storyboard outAnimation =   this.FindResource( "AnimationOut" ) as Storyboard;
            if( outAnimation != null )
            {
                outAnimation.Begin();
            }

            this.EventAggregator.GetEvent<LoadAllDataEvent>().Publish(true);
        }

        [Import]
        public UserLoginViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }

        private void userPsw_PasswordChanged( object sender, RoutedEventArgs e )
        {
            PasswordBox passwordtext = (PasswordBox)sender;
            ( passwordtext.DataContext as UserLoginViewModel ).Password = passwordtext.Password;
            SetPasswordBoxSelection( passwordtext, passwordtext.Password.Length + 1, passwordtext.Password.Length + 1 );
        }

        private static void SetPasswordBoxSelection( PasswordBox passwordBox, int start, int length )
        {
            var select = passwordBox.GetType().GetMethod( "Select",
                            BindingFlags.Instance | BindingFlags.NonPublic );
            select.Invoke( passwordBox, new object[] { start, length } );
        }
    }
}
