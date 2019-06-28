using CFLMedCab.APO.Surgery;
using CFLMedCab.BLL;
using CFLMedCab.DTO;
using CFLMedCab.DTO.Fetch;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Stock;
using CFLMedCab.DTO.Surgery;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.DeviceHelper;
using CFLMedCab.Infrastructure.ToolHelper;
using CFLMedCab.Model;
using CFLMedCab.Model.Constant;
using CFLMedCab.Model.Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CFLMedCab.View.Fetch
{
    /// <summary>
    /// SurgeryNumClose.xaml 的交互逻辑
    /// </summary>
    public partial class SurgeryNumClose : UserControl
    {
        public delegate void EnterSurgeryNumOpenHandler(object sender, SurgeryOrderDto surgeryOrderDto);
        public event EnterSurgeryNumOpenHandler EnterSurgeryNumOpenEvent;
        //跳出关闭弹出框
        public delegate void EnterPopCloseHandler(object sender, bool e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        private Timer endTimer;

        private SurgeryOrderDto surgeryOrderDto;
        private Hashtable after;
        //领用商品信息
        private List<GoodsDto> goodsChageOrderdtls;
        //当前柜子里面的商品
        private List<GoodsDto> goodDtos;
        //手术领用详细单变化信息
        private List<SurgeryOrderdtlDto> surgeryOrderdtlDtos;
        private GoodsBll goodsBll = new GoodsBll();
        private FetchOrderBll fetchOrderBll = new FetchOrderBll();

        public SurgeryNumClose(SurgeryOrderDto model, Hashtable hashtable)
        {
            InitializeComponent();
            operatorName.Content = ApplicationState.GetValue<User>((int)ApplicationKey.CurUser).name;
            time.Content = DateTime.Now.ToString("yyyy年MM月dd日");
            surgeryOrderDto = model;
            surgeryNum.Content = model.code;

            Hashtable before = ApplicationState.GetValue<Hashtable>((int)ApplicationKey.CurGoods);
            after = hashtable;
            //盘点当前柜子里的商品
            goodDtos = goodsBll.GetInvetoryGoodsDto(hashtable);
            //获取库存变化信息
            goodsChageOrderdtls = goodsBll.GetCompareGoodsDto(before, hashtable);
            //组装变化信息的异常情况
            goodsChageOrderdtls = fetchOrderBll.GetSurgeryGoodsOperateDto(goodsChageOrderdtls, model.code, out int currentOperateNum, out int storageOperateExNum, out int notStorageOperateExNum);
            //手术领用详细单变化信息
            surgeryOrderdtlDtos = fetchOrderBll.GetSurgeryOrderdtlOperateDto(new SurgeryOrderApo { GoodsDtos = goodDtos, SurgeryOrderCode = model.code, OperateGoodsDtos = goodsChageOrderdtls }, out int notFetchGoodsNum).Data;

            listView.DataContext = surgeryOrderdtlDtos.Where(it => it.not_fetch_num > 0);//手术领用详细单变化信息绑定
            listView1.DataContext = goodsChageOrderdtls;
            inNum.Content = currentOperateNum;//领用数
            abnormalInNum.Content = storageOperateExNum;//异常入库
            abnormalOutNum.Content = notStorageOperateExNum;//异常出库
            waitNum.Content = notFetchGoodsNum;//待领用数

            endTimer = new Timer(Contant.ClosePageEndTimer);
            endTimer.AutoReset = false;
            endTimer.Enabled = true;
            endTimer.Elapsed += new ElapsedEventHandler(onEndTimerExpired);
        }

        /// <summary>
        /// 不结束本次领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        public void onNoEndOperation(object sender, RoutedEventArgs e)
        {
            EnterSurgeryNumOpenEvent(this, surgeryOrderDto);
        }

        /// <summary>
        /// 结束本次领用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEndOperation(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            EndOperation(btn.Name == "YesAndExitBtn" ? true : false);
        }

        /// <summary>
        /// 结束定时器超时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEndTimerExpired(object sender, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() => {
                EndOperation(true);
            }));
        }

        private void EndOperation(bool bExit)
        {
            fetchOrderBll.UpdateSurgeryOrderdtl(new SurgeryOrderApo { SurgeryOrderCode = surgeryOrderDto.code, GoodsDtos = goodDtos, OperateGoodsDtos = goodsChageOrderdtls }, surgeryOrderdtlDtos);
            ApplicationState.SetValue((int)ApplicationKey.CurGoods, after);
            EnterPopCloseEvent(this, bExit);
        }
    }
}
