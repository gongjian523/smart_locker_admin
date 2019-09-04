using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CFLMedCab.View.Common
{
    /// <summary>
    /// OpenDoorBtnBoard.xaml 的交互逻辑
    /// </summary>
    public partial class OpenDoorBtnBoard : UserControl
    {
        public delegate void OpenDoorHandler(object sender, string e);
        public event OpenDoorHandler OpenDoorEvent;

        List<Button> buttons = new List<Button>();

        public OpenDoorBtnBoard()
        {
            InitializeComponent();

            List<Locations> list = ApplicationState.GetLocations();

            for (int i = 0; i < list.Count; ++i)
            {
                ColumnDefinition col1 = new ColumnDefinition();

                btnGrid.ColumnDefinitions.Add(col1);
            }

            for (int i = 0; i < list.Count; ++i)
            {
                Button button = new Button()
                {
                    Style = Application.Current.Resources["OpenDoorButton"] as Style,
                    Content = string.Format("{0}", list[i].Code),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Tag = list[i].Code,
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
            btn.IsEnabled = false;

            string code = btn.Tag.ToString();
            OpenDoorEvent(this, btn.Tag.ToString());
        }

        public void SetButtonEnable(bool state, string code)
        {
            Button btn = buttons.Where(item =>(string)item.Tag == code).First();
            if(btn != null)
            {
                btn.IsEnabled = state;
            }
        }
    }
}
