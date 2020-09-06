using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
using System;
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
        public delegate void ExitDepartChooseBoardHandler(object sender, string e);
        public event ExitDepartChooseBoardHandler ExitDepartChooseBoardEvent;

        public delegate void EnterHomePageHandler(object sender, Department e);
        public event EnterHomePageHandler EnterHomePageEvent;

        List<Button> buttons = new List<Button>();

        protected User user;

        public DepartChooseBoard(BaseData<Department> bdDepartment)
        {
            InitializeComponent();

            HttpHelper.GetInstance().ResultCheck(bdDepartment, out bool isSuccess);
            if (isSuccess)
            {
                btnGrid.Visibility = Visibility.Visible;
                warningView.Visibility = Visibility.Hidden;

                for (int i = 0; i < bdDepartment.body.objects.Count; ++i)
                {
                    Button button = new Button()
                    {
                        Style = Application.Current.Resources["OpenDoorButton"] as Style,
                        Content = string.Format("{0}", bdDepartment.body.objects[i].name),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Tag = bdDepartment.body.objects[i],
                        Width = 180,
                    };

                    button.Click += onExitDepartChooseBoard;
                    btnGrid.Children.Add(button);

                    buttons.Add(button);
                }

                if(bdDepartment.body.objects.Count == 1)
                {
                    ColumnDefinition col1 = new ColumnDefinition();
                    btnGrid.ColumnDefinitions.Add(col1);

                    Grid.SetColumn(buttons[0], 0);

                }
                else if (bdDepartment.body.objects.Count == 2)
                {
                    ColumnDefinition col1 = new ColumnDefinition();
                    btnGrid.ColumnDefinitions.Add(col1);

                    ColumnDefinition col2 = new ColumnDefinition();
                    btnGrid.ColumnDefinitions.Add(col2);

                    Grid.SetColumn(buttons[0], 0);
                    Grid.SetColumn(buttons[1], 1);
                }
                else if (bdDepartment.body.objects.Count == 3)
                {
                    ColumnDefinition col1 = new ColumnDefinition();
                    btnGrid.ColumnDefinitions.Add(col1);

                    ColumnDefinition col2 = new ColumnDefinition();
                    btnGrid.ColumnDefinitions.Add(col2);

                    RowDefinition row1 = new RowDefinition();
                    btnGrid.RowDefinitions.Add(row1);

                    RowDefinition row2 = new RowDefinition();
                    btnGrid.RowDefinitions.Add(row2);

                    Grid.SetColumn(buttons[0], 0);
                    Grid.SetRow(buttons[0], 0);

                    Grid.SetColumn(buttons[1], 1);
                    Grid.SetRow(buttons[1], 0);

                    Grid.SetColumn(buttons[2], 0);
                    Grid.SetRow(buttons[2], 1);
                }
                else if (bdDepartment.body.objects.Count == 4)
                {
                    ColumnDefinition col1 = new ColumnDefinition();
                    btnGrid.ColumnDefinitions.Add(col1);

                    ColumnDefinition col2 = new ColumnDefinition();
                    btnGrid.ColumnDefinitions.Add(col2);

                    RowDefinition row1 = new RowDefinition();
                    btnGrid.RowDefinitions.Add(row1);

                    RowDefinition row2 = new RowDefinition();
                    btnGrid.RowDefinitions.Add(row2);

                    Grid.SetColumn(buttons[0], 0);
                    Grid.SetRow(buttons[0], 0);

                    Grid.SetColumn(buttons[1], 1);
                    Grid.SetRow(buttons[1], 0);

                    Grid.SetColumn(buttons[2], 0);
                    Grid.SetRow(buttons[2], 1);

                    Grid.SetColumn(buttons[3], 0);
                    Grid.SetRow(buttons[3], 1);
                }
                else 
                {
                    for(int i  = 0; i < 3; i++)
                    {
                        ColumnDefinition col = new ColumnDefinition();
                        btnGrid.ColumnDefinitions.Add(col);
                    }

                    for (int i = 0; i <= (bdDepartment.body.objects.Count-1) / 3; i++)
                    {
                        RowDefinition row = new RowDefinition();
                        btnGrid.RowDefinitions.Add(row);
                    }

                    for (int  i = 0; i< bdDepartment.body.objects.Count; i++)
                    {
                        int col = i % 3;
                        int row = i / 3;

                        Grid.SetColumn(buttons[i], col);
                        Grid.SetRow(buttons[i], row);
                    }
                }
            }
            else
            {
                btnGrid.Visibility = Visibility.Hidden;
                warningView.Visibility = Visibility.Visible;
                warningText.Text = "获取您的部门信息时发生错误,请联系工作人员！";
            }
            

        }

        private void onExitDepartChooseBoard(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            EnterHomePageEvent(this, (Department)btn.Tag);
        }


        private void onLogout(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ExitDepartChooseBoardEvent(this, "");
            });
        }

    }
}
