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
using System.Windows.Threading;
using CFLMedCab.View;
using CFLMedCab.View.Inventory;
using MahApps.Metro.Controls;
using CFLMedCab.Infrastructure.DeviceHelper;
using System.IO.Ports;
using System.Timers;
using System.Media;
using CFLMedCab.View.Login;
using CFLMedCab.Model;
using GDotnet.Reader.Api.Protocol.Gx;
using GDotnet.Reader.Api.DAL;
using CFLMedCab.View.ReplenishmentOrder;
using CFLMedCab.View.Return;
using CFLMedCab.DAL;
using SqlSugar;
using CFLMedCab.BLL;
using CFLMedCab.Infrastructure;
using System.Speech.Synthesis;
using CFLMedCab.View.Fetch;
using System.Collections;
using CFLMedCab.DTO.Replenish;
using CFLMedCab.DTO.Picking;
using CFLMedCab.Test;
using CFLMedCab.DTO.Goodss;

namespace CFLMedCab
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private DispatcherTimer ShowTimer;
        private VeinHelper vein;


        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            ShowTime();
            ShowTimer = new DispatcherTimer();
            ShowTimer.Tick += new EventHandler(ShowCurTimer);//起个Timer一直获取当前时间
            ShowTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            ShowTimer.Start();

            vein = new VeinHelper("COM9", 9600);
            vein.DataReceived += new SerialDataReceivedEventHandler(onReceivedDataVein);
            Console.WriteLine("onStart");
            vein.ChekVein();


			//App.Current.Dispatcher.Invoke((Action)(() =>
			//{
			//    PopFrame.Visibility = Visibility.Hidden;
			//    MaskView.Visibility = Visibility.Hidden;

			//        LoginBkView.Visibility = Visibility.Hidden;
			//}));




			//bool isGetSuccess;

			//Hashtable cur =  RfidHelper.GetEpcData(out isGetSuccess);
			//ApplicationState.SetValue((int)ApplicationKey.CurGoods, cur);//读取机柜内当前的商品编码

			var testData = new ReplenishBll().GetReplenishSubOrderDto(new APO.BasePageDataApo {
				PageIndex = 1,
				PageSize = 2
			});


            //Test();

            ConsoleManager.Show();
        }


        private void onLoginInfoHidenEvent(object sender, LoginStatus e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Hidden;
                MaskView.Visibility = Visibility.Hidden;

                if (e.LoginState == 1)
                    LoginBkView.Visibility = Visibility.Hidden;
            }));

            if (e.LoginState == 0)
            {
                Console.WriteLine("onLoginInfoHidenEvent");
                vein.Close();
                vein.ChekVein();
            }
        }


        private void onReceivedDataVein(object sender, SerialDataReceivedEventArgs e)
        {
            int id = vein.GetVeinId();

            if (id >= 0)
            {
                vein.Close();

                LoginStatus sta = new LoginStatus();

                //验证失败
                if (id == 0)
                {
                    sta.LoginState = 0;
                    sta.LoginString = "登录失败";
                    sta.LoginString2 = "请再次进行验证";
                }
                else
                {
                    //UserBll userBll = new UserBll();
                    //User user = userBll.GetUserByVeinId(id);
                    UserDal userDal = new UserDal();
                    User user = userDal.GetUserByVeinId(id);

                    //本地数据库中没有查询到此次指静脉信息
                    if (user == null)
                    {
                        sta.LoginState = 0;
                        sta.LoginString = "登录失败";
                        sta.LoginString2 = "请再次进行验证";
                    }
                    //验证成功
                    else
                    {
                        sta.LoginState = 1;
                        sta.LoginString = "登录成功";
                        sta.LoginString2 = "欢迎" + user.name + "登录";

                        ApplicationState.SetValue((int)ApplicationKey.CurUser, user);
                    }
                }

                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    LoginInfo loginInfo = new LoginInfo(sta);

                    PopFrame.Visibility = Visibility.Visible;
                    MaskView.Visibility = Visibility.Visible;

                    loginInfo.LoginInfoHidenEvent += new LoginInfo.LoginInfoHidenHandler(onLoginInfoHidenEvent);

                    PopFrame.Navigate(loginInfo);

                }));
            }
            else
            {
                Console.WriteLine("onReceivedDataVein");
                vein.Close();
                vein.ChekVein();

            }

        }

        private void ShowCurTimer(object sender, EventArgs e)
        {
            ShowTime();
        }

        //ShowTime方法
        private void ShowTime()
        {
            //获得年月日
            this.tbDateText.Text = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
            //获得时分秒
            // this.tbTimeText.Text = DateTime.Now.ToString("HH:mm:ss");
        }
        #region 领用
        #region 一般领用
        /// <summary>
        /// 一般领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterGerFetch(object sender, RoutedEventArgs e)
        {
            GerFetchState gerFetchState = new GerFetchState(1);
            FullFrame.Navigate(gerFetchState);
            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData("COM2", out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterGerFectchLockerEvent);
        }

        /// <summary>
        /// 一般领用关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterGerFectchLockerEvent(object sender, bool isClose)
        {
            System.Diagnostics.Debug.WriteLine("返回开锁状态{0}", isClose);

            if (!isClose)
                return;
            bool isGetSuccess;
            Hashtable ht = RfidHelper.GetEpcData(out isGetSuccess);

            ApplicationState.SetValue((int)ApplicationKey.CurGoods, ht);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                GerFetchView gerFetchView = new GerFetchView();
                gerFetchView.EnterPopCloseEvent += new GerFetchView.EnterPopCloseHandler(onEnterPopClose);
                gerFetchView.EnterGerFetch += new GerFetchView.EnterFetchOpenHandler(onEnterGerFetch);
                FullFrame.Navigate(gerFetchView);
            }));
        }
        #endregion

        #region 手术领用

        #region 无手术单领用
        /// <summary>
        /// 进入手术无单领用-开门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterSurgeryNoNumOpen(object sender, RoutedEventArgs e)
        {
            GerFetchState gerFetchState = new GerFetchState(1);
            ContentFrame.Navigate(gerFetchState);
            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData("COM2", out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterSurgeryNoNumLockerEvent);

        }

        /// <summary>
        /// 手术无单领用关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterSurgeryNoNumLockerEvent(object sender, bool isClose)
        {
            System.Diagnostics.Debug.WriteLine("返回开锁状态{0}", isClose);

            if (!isClose)
                return;
            Hashtable ht = RfidHelper.GetEpcData(out bool isGetSuccess);

            ApplicationState.SetValue((int)ApplicationKey.CurGoods, ht);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {

                SurgeryNoNumClose surgeryNoNumClose = new SurgeryNoNumClose();
                surgeryNoNumClose.EnterPopCloseEvent += new SurgeryNoNumClose.EnterPopCloseHandler(onEnterPopClose);
                surgeryNoNumClose.EnterSurgeryNoNumOpenEvent += new SurgeryNoNumClose.EnterSurgeryNoNumOpenHandler(onEnterGerFetch);
                FullFrame.Navigate(surgeryNoNumClose);
            }));
        }
        #endregion

        #region 有手术单领用
        /// <summary>
        /// 手术领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterSurgery(object sender, RoutedEventArgs e)
        {
            SurgeryQuery surgeryQuery = new SurgeryQuery();
            surgeryQuery.EnterSurgeryDetailEvent += new SurgeryQuery.EnterSurgeryDetailHandler(onEnterSurgeryDetail);//有手术单号进入手术领用单详情
            surgeryQuery.EnterSurgeryNoNumOpenEvent += new SurgeryQuery.EnterSurgeryNoNumOpenHandler(onEnterSurgeryNoNumOpen);//无手术单号直接开柜领用
            ContentFrame.Navigate(surgeryQuery);
        }
        /// <summary>
        /// 手术领用详情页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fetchOrder"></param>
        private void onEnterSurgeryDetail(object sender, FetchOrder fetchOrder)
        {
            SurgeryOrderDetail surgeryOrderDetail = new SurgeryOrderDetail(fetchOrder);
            surgeryOrderDetail.EnterSurgeryNumOpenEvent += new SurgeryOrderDetail.EnterSurgeryNumOpenHandler(EnterSurgeryNumOpenEvent);
            surgeryOrderDetail.EnterSurgeryConsumablesDetailEvent += new SurgeryOrderDetail.EnterSurgeryConsumablesDetailHandler(EnterSurgeryConsumablesDetailEvent);
            ContentFrame.Navigate(surgeryOrderDetail);
        }

        /// <summary>
        /// 手术耗材详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fetchOrder"></param>
        public void EnterSurgeryConsumablesDetailEvent(object sender, FetchOrder fetchOrder)
        {
            ConsumablesDetails consumablesDetails = new ConsumablesDetails(fetchOrder);
            consumablesDetails.EnterSurgeryDetailEvent += new ConsumablesDetails.EnterSurgeryDetailHandler(onEnterSurgeryDetail);
            ContentFrame.Navigate(consumablesDetails);
        }

        /// <summary>
        /// 进入手术有单领用-开门状态 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterSurgeryNumOpenEvent(object sender, FetchOrder fetchOrder)
        {
            NaviView.Visibility = Visibility.Hidden;

            SurgeryNumOpen surgeryNumOpen = new SurgeryNumOpen(fetchOrder);
            FullFrame.Navigate(surgeryNumOpen);

            MaskView.Visibility = Visibility.Visible;
            PopFrame.Visibility = Visibility.Visible;
            OpenCabinet openCabinet = new OpenCabinet();
            openCabinet.HidePopOpenEvent += new OpenCabinet.HidePopOpenHandler(onHidePopOpen);
            PopFrame.Navigate(openCabinet);

            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData("COM2", out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterSurgeryNumLockerEvent);

        }

        /// <summary>
        /// 手术有单领用关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterSurgeryNumLockerEvent(object sender, bool isClose)
        {
            System.Diagnostics.Debug.WriteLine("返回开锁状态{0}", isClose);

            if (!isClose)
                return;
            Hashtable ht = RfidHelper.GetEpcData(out bool isGetSuccess);

            ApplicationState.SetValue((int)ApplicationKey.CurGoods, ht);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                SurgeryNumClose surgeryNumClose = new SurgeryNumClose(new FetchOrder());
                surgeryNumClose.EnterPopCloseEvent += new SurgeryNumClose.EnterPopCloseHandler(onEnterPopClose);
                surgeryNumClose.EnterSurgeryNumOpenEvent += new SurgeryNumClose.EnterSurgeryNumOpenHandler(EnterSurgeryNumOpenEvent);
                FullFrame.Navigate(surgeryNumClose);
            }));
        }
        #endregion
        #endregion

        #region 领用退回
        /// <summary>
        /// 领用退回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnFetch(object sender, RoutedEventArgs e)
        {
            GerFetchState gerFetchState = new GerFetchState(2);
            FullFrame.Navigate(gerFetchState);
            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData("COM2", out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReturnFetchLockerEvent);
        }

        /// <summary>
        /// 领用退回关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnFetchLockerEvent(object sender, bool isClose)
        {
            System.Diagnostics.Debug.WriteLine("返回开锁状态{0}", isClose);

            if (!isClose)
                return;
            bool isGetSuccess;
            Hashtable ht = RfidHelper.GetEpcData(out isGetSuccess);

            //ApplicationState.SetValue((int)ApplicationKey.CurGoods, ht);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReturnFetchView returnFetchView = new ReturnFetchView();
                returnFetchView.EnterPopCloseEvent += new ReturnFetchView.EnterPopCloseHandler(onEnterPopClose);
                returnFetchView.EnterReturnFetch += new ReturnFetchView.EnterReturnFetchHandler(onEnterReturnFetch);
                FullFrame.Navigate(returnFetchView);
            }));
        }
        #endregion
        #endregion

        #region Replenishment
        /// <summary>
        /// 进入上架单列表页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReplenishment(object sender, RoutedEventArgs e)
        {
            Replenishment replenishment = new Replenishment();
            replenishment.EnterReplenishmentDetailEvent += new Replenishment.EnterReplenishmentDetailHandler(onEnterReplenishmentDetail);
            replenishment.EnterReplenishmentDetailOpenEvent += new Replenishment.EnterReplenishmentDetailOpenHandler(onEnterReplenishmentDetailOpen);

            ContentFrame.Navigate(replenishment);

        }

        /// <summary>
        /// 进入上架单详情页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReplenishmentDetail(object sender, ReplenishSubOrderDto e)
        {
            ReplenishmentDetail replenishmentDetail = new ReplenishmentDetail(e);
            replenishmentDetail.EnterReplenishmentDetailOpenEvent += new ReplenishmentDetail.EnterReplenishmentDetailOpenHandler(onEnterReplenishmentDetailOpen);
            replenishmentDetail.EnterReplenishmentEvent += new ReplenishmentDetail.EnterReplenishmentHandler(onEnterReplenishment);

            ContentFrame.Navigate(replenishmentDetail);
        }

        /// <summary>
        /// 进入上架单详情页-开门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReplenishmentDetailOpen(object sender, ReplenishSubOrderDto e)
        {
            NaviView.Visibility = Visibility.Hidden;

            ReplenishmentDetailOpen replenishmentDetailOpen = new ReplenishmentDetailOpen(e);
            FullFrame.Navigate(replenishmentDetailOpen);

            MaskView.Visibility = Visibility.Visible;
            PopFrame.Visibility = Visibility.Visible;
            OpenCabinet openCabinet = new OpenCabinet();
            openCabinet.HidePopOpenEvent += new OpenCabinet.HidePopOpenHandler(onHidePopOpen);
            PopFrame.Navigate(openCabinet);


            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData("COM2", out bool isGetSuccess);
            delegateGetMsg.userData = e;
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReplenishmentCloseEvent);
        }


        /// <summary>
        /// 进入上架单详情页-关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReplenishmentCloseEvent(object sender, bool isClose)
        {
            LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
            System.Diagnostics.Debug.WriteLine("返回开锁状态{0}", isClose);

            if (!isClose)
                return;

            bool isGetSuccess;
            Hashtable ht = RfidHelper.GetEpcData(out isGetSuccess);
            ReplenishSubOrderDto replenishSubOrderDto = (ReplenishSubOrderDto)delegateGetMsg.userData;
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReplenishmentClose replenishmentClose = new ReplenishmentClose(replenishSubOrderDto, ht);
                replenishmentClose.EnterReplenishmentDetailOpenEvent += new ReplenishmentClose.EnterReplenishmentDetailOpenHandler(onEnterReplenishmentDetailOpen);
                replenishmentClose.EnterPopCloseEvent += new ReplenishmentClose.EnterPopCloseHandler(onEnterPopClose);

                FullFrame.Navigate(replenishmentClose);
            }));
        }
        #endregion

        #region  ReturnGoods
        /// <summary>
        /// 进入退货出库页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnGoods(object sender, RoutedEventArgs e)
        {
            ReturnGoods returnGoods = new ReturnGoods();
            returnGoods.EnterReturnGoodsDetailEvent += new ReturnGoods.EnterReturnGoodsDetailHandler(onEnterReturnGoodsDetail);
            returnGoods.EnterReturnGoodsDetailOpenEvent += new ReturnGoods.EnterReturnGoodsDetailOpenHandler(onEnterReturnGoodsDetailOpen);

            ContentFrame.Navigate(returnGoods);
        }

        /// <summary>
        /// 进入退货出库详情页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnGoodsDetail(object sender, PickingSubOrderDto e)
        {
            ReturnGoodsDetail returnGoodsDetail = new ReturnGoodsDetail(e);
            returnGoodsDetail.EnterReturnGoodsDetailOpenEvent += new ReturnGoodsDetail.EnterReturnGoodsDetailOpenHandler(onEnterReturnGoodsDetailOpen);
            returnGoodsDetail.EnterReturnGoodsEvent += new ReturnGoodsDetail.EnterReturnGoodsHandler(onEnterReturnGoods);

            ContentFrame.Navigate(returnGoodsDetail);
        }


        /// <summary>
        /// 进入退货出库详情页面-开门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnGoodsDetailOpen(object sender, PickingSubOrderDto e)
        {
            NaviView.Visibility = Visibility.Hidden;

            ReturnGoodsDetailOpen returnGoodsDetailOpen = new ReturnGoodsDetailOpen(e);
            FullFrame.Navigate(returnGoodsDetailOpen);

            MaskView.Visibility = Visibility.Visible;
            PopFrame.Visibility = Visibility.Visible;
            OpenCabinet openCabinet = new OpenCabinet();
            openCabinet.HidePopOpenEvent += new OpenCabinet.HidePopOpenHandler(onHidePopOpen);
            PopFrame.Navigate(openCabinet);

            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData("COM2", out bool isGetSuccess);
               delegateGetMsg.userData = e;
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(onEnterReturnGoodsCloseEvent);
        }


        /// <summary>
        /// 进入拣货单详情页-关门状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterReturnGoodsCloseEvent(object sender, bool isClose)
        {
            LockHelper.DelegateGetMsg delegateGetMsg = (LockHelper.DelegateGetMsg)sender;
            System.Diagnostics.Debug.WriteLine("返回开锁状态{0}", isClose);

            if (!isClose)
                return;

            bool isGetSuccess;
            Hashtable ht = RfidHelper.GetEpcData(out isGetSuccess);

            //ApplicationState.SetValue((int)ApplicationKey.CurGoods, ht);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ReturnGoodsClose returnGoodsClose = new ReturnGoodsClose((PickingSubOrderDto)delegateGetMsg.userData, ht);
                returnGoodsClose.EnterReturnGoodsDetailOpenEvent += new ReturnGoodsClose.EnterReturnGoodsDetailOpenHandler(onEnterReturnGoodsDetailOpen);
                returnGoodsClose.EnterPopCloseEvent += new ReturnGoodsClose.EnterPopCloseHandler(onEnterPopClose);

                FullFrame.Navigate(returnGoodsClose);
            }));
        }
        #endregion

        #region Inventory
        /// <summary>
        /// 库存盘点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterInvtory(object sender, RoutedEventArgs e)
        {
            HomePageView.Visibility = Visibility.Hidden;

            BtnEnterInventory.IsChecked = true;

            Inventory inventory = new Inventory();
            inventory.EnterPopInventoryEvent += new Inventory.EnterPopInventoryHandler(onEnterPopInventory);
            inventory.EnterPopInventoryPlanEvent += new Inventory.EnterPopInventoryPlanHandler(onEnterPopInventoryPlan);
            inventory.EnterInventoryDetailEvent += new Inventory.EnterInventoryDetailHandler(onEnterInventoryDetail);

            ContentFrame.Navigate(inventory);

        }

        /// <summary>
        /// 弹出库存盘点正在进行中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterPopInventory(object sender, System.EventArgs e)
        {
            InventoryOngoing inventoryOngoing = new InventoryOngoing();
            inventoryOngoing.HidePopInventoryEvent += new InventoryOngoing.HidePopInventoryHandler(onHidePopInventory);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Visible;
                MaskView.Visibility = Visibility.Visible;

                PopFrame.Navigate(inventoryOngoing);
            }));
        }

        /// <summary>
        /// 关闭库存盘点正在进行中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onHidePopInventory(object sender, System.EventArgs e)
        {
            //bool isGetSuccess;
            //Hashtable ht = RfidHelper.GetEpcData(out isGetSuccess);

            //Hashtable ht = new Hashtable();
            //HashSet<string> hs1 = new HashSet<string> { "E20000176012027919504D98", "E20000176012025319504D67", "E20000176012025619504D70", "E20000176012028119504DA5", "E20000176012023919504D48" };
            //ht.Add("COM1", hs1);
            //HashSet<string> hs4 = new HashSet<string> { "E20000176012028219504DAD", "E20000176012026619504D8D", "E20000176012026319404F98", "E20000176012028019504DA0", "E20000176012026519504D85" };
            //ht.Add("COM4", hs4);

            //ApplicationState.SetValue((int)ApplicationKey.CurGoods, ht);

            //InventoryBll inventoryBll = new InventoryBll();
            //GoodsBll goodsBll = new GoodsBll();
            //List<GoodsDto> list = goodsBll.GetInvetoryGoodsDto(ht);
            //int id = inventoryBll.NewInventory(InventoryType.Manual);
            //inventoryBll.InsertInventoryDetails(list, id);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Hidden;
                MaskView.Visibility = Visibility.Hidden;
            }));            
        }

        /// <summary>
        /// 关闭库存盘点正在进行中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterInventoryDetail(object sender, InventoryDetailPara e)
        {
            InventoryDetail inventoryDetail = new InventoryDetail(e);
            inventoryDetail.EnterAddProductEvent += new InventoryDetail.EnterAddProductHandler(onEnterPopAddProduct);
            inventoryDetail.EnterInventoryEvent += new InventoryDetail.EnterInventoryHandler(onEnterInvtory);

            ContentFrame.Navigate(inventoryDetail);

        }


        /// <summary>
        /// 弹出盘点计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterPopInventoryPlan(object sender, System.EventArgs e)
        {
            InventoryPlanDetail inventoryPlanDetail = new InventoryPlanDetail();
            inventoryPlanDetail.HidePopInventoryPlanEvent += new InventoryPlanDetail.HidePopInventoryPlanHandler(onHidePopInventoryPlan);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Visible;
                MaskView.Visibility = Visibility.Visible;

                PopFrame.Navigate(inventoryPlanDetail);
            }));
        }

        /// <summary>
        /// 关闭盘点计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onHidePopInventoryPlan(object sender, System.EventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Hidden;
                MaskView.Visibility = Visibility.Hidden;
            }));
        }


        /// <summary>
        /// 进入添加单品码弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterPopAddProduct(object sender, InventoryDetailPara e)
        {
            AddProduct addProduct = new AddProduct(e);
            addProduct.HidePopAddProductEvent += new AddProduct.HidePopAddProductHandler (onHidePopAddProduct);
            addProduct.EnterInventoryDetailEvent += new AddProduct.EnterInventoryDetailHandler(onEnterInventoryDetail);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Visible;
                MaskView.Visibility = Visibility.Visible;

                PopFrame.Navigate(addProduct);
            }));
        }


        /// <summary>
        /// 关闭添加单品码弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onHidePopAddProduct(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Hidden;
                MaskView.Visibility = Visibility.Hidden;
            }));
        }

 

        /// <summary>
        /// 库存查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterStock(object sender, RoutedEventArgs e)
        {
            HomePageView.Visibility = Visibility.Hidden;
            BtnEnterStock.IsChecked = true;

            Stock stock = new Stock();
            stock.EnterStockDetailedEvent += new Stock.EnterStockDetailedHandler(onEnterStockDetailedEvent);
            ContentFrame.Navigate(stock);
        }

        private void onEnterStockDetailedEvent(object sender, GoodDto goodDto)
        {
            StockDetailed stockDetailed = new StockDetailed(goodDto);
            stockDetailed.EnterStockEvent += new StockDetailed.EnterStockHandler(colseStockDetailedEvent);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Visible;
                MaskView.Visibility = Visibility.Visible;

                PopFrame.Navigate(stockDetailed);
            }));
        }

        private void colseStockDetailedEvent(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Hidden;
                MaskView.Visibility = Visibility.Hidden;
            }));
        }
        #endregion


        /// <summary>
        /// 弹出关门提示框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEnterPopClose(object sender, RoutedEventArgs e)
        {
            CloseCabinet closeCabinet = new CloseCabinet();
            closeCabinet.HidePopCloseEvent += new CloseCabinet.HidePopCloseHandler(onHidePopClose);

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Visible;
                MaskView.Visibility = Visibility.Visible;

                PopFrame.Navigate(closeCabinet);
            }));

        }

        /// <summary>
        /// 关门提示框关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onHidePopClose(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Hidden;
                MaskView.Visibility = Visibility.Hidden;

                LoginBkView.Visibility = Visibility.Visible;

                vein.Close();
                vein.ChekVein();
            }));
        }


        /// <summary>
        /// 开门提示框关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onHidePopOpen(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                PopFrame.Visibility = Visibility.Hidden;
                MaskView.Visibility = Visibility.Hidden;
            }));
        }


        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
           // this.Topmost = true;
            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight; 
        }


        #region test
        private void Test()
        {
            TestGoods test = new TestGoods();
            test.AddGoodTest();
 
            User user = new User
            {
                id = 1111,
                name = "Nathan",
                vein_id = 12323,
            };
            ApplicationState.SetValue((int)ApplicationKey.CurUser, user);

        }
 

        private void TestLocker(object sender, ElapsedEventArgs e)
        {
            Console.ReadKey();
            LockHelper.DelegateGetMsg delegateGetMsg = LockHelper.GetLockerData("COM2", out bool isGetSuccess);
            delegateGetMsg.DelegateGetMsgEvent += new LockHelper.DelegateGetMsg.DelegateGetMsgHandler(TestLockerEvent);
        }

        private void TestLockerEvent(object sender, bool isClose)
        {
            Console.WriteLine("返回开锁状态{0}", isClose);
            System.Diagnostics.Debug.WriteLine("返回开锁状态{0}", isClose);
        }
        #endregion

    }
}
