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
using CFLMedCab.Model.Enum;
using System;
using System.Collections;
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
        public delegate void EnterPopCloseHandler(object sender, RoutedEventArgs e);
        public event EnterPopCloseHandler EnterPopCloseEvent;

        private SurgeryOrderDto surgeryOrderDto;
        private Hashtable after;
        private List<GoodsDto> goodsChageOrderdtls;
        private GoodsBll goodsBll = new GoodsBll();
        private FetchOrderBll fetchOrderBll = new FetchOrderBll();
        public SurgeryNumClose(SurgeryOrderDto model, Hashtable hashtable)
        {
            InitializeComponent();
            operatorName.Content = ApplicationState.GetValue<User>((int)ApplicationKey.CurUser).name;
            time.Content = DateTime.Now; Hashtable before = ApplicationState.GetValue<Hashtable>((int)ApplicationKey.CurGoods);
            surgeryNum.Content = model.code;
            after = hashtable;
            surgeryOrderDto = model;
            List<GoodsDto> goodDtos = goodsBll.GetInvetoryGoodsDto(hashtable);//盘点当前柜子里的商品
            goodsChageOrderdtls = goodsBll.GetCompareGoodsDto(before, hashtable);//库存变化信息
            goodsChageOrderdtls = fetchOrderBll.GetSurgeryGoodsOperateDto(goodsChageOrderdtls, model.code, out int currentOperateNum, out int storageOperateExNum, out int notStorageOperateExNum);
            List<SurgeryOrderdtlDto> surgeryOrderdtlDtos = fetchOrderBll.GetSurgeryOrderdtlOperateDto(new SurgeryOrderApo { GoodsDtos = goodDtos, SurgeryOrderCode= model.code, OperateGoodsDtos= goodsChageOrderdtls }, out int notFetchGoodsNum).Data;
            listView.DataContext = surgeryOrderdtlDtos;
            listView1.DataContext = goodsChageOrderdtls;
            inNum.Content = currentOperateNum;//领用数
            abnormalInNum.Content = storageOperateExNum;//异常入库
            abnormalOutNum.Content = notStorageOperateExNum;//异常出库
            waitNum.Content = notFetchGoodsNum;//待领用数

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
            if (fetchOrderBll.InsertFetchAndGoodsChangeInfo(goodsChageOrderdtls, RequisitionType.有单手术领用, surgeryOrderDto.code))
                ApplicationState.SetValue((int)ApplicationKey.CurGoods, after);
            EnterPopCloseEvent(this, null);
        }
    }
}
