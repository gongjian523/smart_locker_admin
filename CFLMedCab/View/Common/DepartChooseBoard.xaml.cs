using CFLMedCab.Http.Model;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CFLMedCab.View.Common
{
    /// <summary>
    /// OpenDoorBtnBoard.xaml 的交互逻辑
    /// </summary>
    public partial class DepartChooseBoard : UserControl
    {
        public delegate void EnterMainFrameHandler(object sender, User e);
        public event EnterMainFrameHandler EnterMainFrameEvent;

        List<Button> buttons = new List<Button>();

        protected User user;

        public DepartChooseBoard(User cuser)
        {
            InitializeComponent();

            user = cuser;

            for (int i = 0; i < user.DepartmentIds.Count; ++i)
            {
                ColumnDefinition col1 = new ColumnDefinition();

                btnGrid.ColumnDefinitions.Add(col1);
            }

            for (int i = 0; i < user.DepartmentIds.Count; ++i)
            {
                Button button = new Button()
                {
                    Style = Application.Current.Resources["OpenDoorButton"] as Style,
                    Content = string.Format("{0}", user.DepartmentIds[i]),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Tag = user.DepartmentIds[i],
                };

                button.Click += onOpenDoor;
                btnGrid.Children.Add(button);
                Grid.SetColumn(button, i);

                buttons.Add(button);
            }
        }

        private void onOpenDoor(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            user.DepartmentIdInUse = btn.Tag.ToString();
            EnterMainFrameEvent(this, user);
        }

    }
}
