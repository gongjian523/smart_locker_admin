using CFLMedCab.APO.Surgery;
using CFLMedCab.BLL;
using CFLMedCab.DAL;
using CFLMedCab.DTO.Fetch;
using CFLMedCab.DTO.Goodss;
using CFLMedCab.DTO.Surgery;
using CFLMedCab.Infrastructure;
using CFLMedCab.Model;
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
    /// ConsumablesDetails.xaml 的交互逻辑
    /// </summary>
    public partial class ConsumablesDetails : UserControl
    {
        public delegate void EnterSurgeryDetailHandler(object sender, SurgeryOrderDto e);
        public event EnterSurgeryDetailHandler EnterSurgeryDetailEvent;
       
        private SurgeryOrderDto surgeryOrderDto;
        private GoodsBll goodsBll = new GoodsBll();
        private FetchOrderBll fetchOrderBll = new FetchOrderBll();
        public ConsumablesDetails(SurgeryOrderDto model)
        {
            InitializeComponent();
            surgeryOrderDto = model;
            surgeryNum.Content = model.code;
            time.Content = model.surgery_time;
            Hashtable before = ApplicationState.GetValue<Hashtable>((int)ApplicationKey.CurGoods);
            List<GoodsDto> goodsDtos = goodsBll.GetInvetoryGoodsDto(before);
            listView.DataContext = fetchOrderBll.GetSurgeryOrderdtlDto(new SurgeryOrderApo { SurgeryOrderCode = surgeryOrderDto.code, GoodsDtos = goodsDtos }).Data;
        }

        /// <summary>
        /// 返回领用页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Return(object sender, RoutedEventArgs e)
        {
            EnterSurgeryDetailEvent(this, surgeryOrderDto);

        }
    }
}
