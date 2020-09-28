using CFLMedCab.Http.Bll;
using CFLMedCab.Http.Helper;
using CFLMedCab.Http.Model;
using CFLMedCab.Http.Model.Base;
using CFLMedCab.Http.Model.Common;
using CFLMedCab.Infrastructure;
using CFLMedCab.Infrastructure.ToolHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CFLMedCab.View.ShelfFast
{
    /// <summary>
    /// ShelfFastView.xaml 的交互逻辑
    /// </summary>
    public partial class ShelfFastView : UserControl
    {
        public delegate void EnterShelfFastDetailHandler(object sender, ShelfTaskFast e);
        public event EnterShelfFastDetailHandler EnterShelfFastDetailEvent;


        //显示加载数据的进度条
        public delegate void LoadingDataHandler(object sender, bool e);
        public event LoadingDataHandler LoadingDataEvent;

        public ShelfFastView()
        {
            InitializeComponent();
            DataContext = this;

            Timer iniTimer = new Timer(100);
            iniTimer.AutoReset = false;
            iniTimer.Enabled = true;
            iniTimer.Elapsed += new ElapsedEventHandler(onInitData);
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        private void onInitData(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                tbInputNumbers.Focus();
            }));
        }


        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterDetail_Click(object sender, RoutedEventArgs e)
        {
            //无法进入下一个页面，将输入框代码文字清空，并且重新设置焦点
            if(!HandleEnterDetail(out string str))
            {
                MessageBox.Show(str, "温馨提示", MessageBoxButton.OK);

                tbInputNumbers.Text = "";
                tbInputNumbers.Focus();
            }
        }

        /// <summary>
        /// 处理输入事件
        /// </summary>
        /// <returns></returns>
        private bool HandleEnterDetail(out string waring)
        {
            string inputStr = tbInputNumbers.Text;
            if (string.IsNullOrWhiteSpace(inputStr))
            {
                waring =  "任务单号不可以为空！";
                return false;
            }

            TaskOrder taskOrder;
            string name;
            try
            {
                taskOrder = JsonConvert.DeserializeObject<TaskOrder>(inputStr);
                name = taskOrder.name;
            }
            catch (Exception ex)
            {
                LogUtils.Error($"数据解析失败！{inputStr} ; 异常报错为：{ex.Message}");
                name = inputStr;
            }


            var shelfFastBillInst = ShelfFastBll.GetInstance();

            SourceBill sourceBill = new SourceBill();
            BaseData<ProcessTask> bdProcessTask = new BaseData<ProcessTask>();
            BaseData<AllotAcceptance> bdAllotAcceptance = new BaseData<AllotAcceptance>();

            if (name.StartsWith("GPT") || name.StartsWith("PT"))
            {
                LoadingDataEvent(this, true);
                bdProcessTask = shelfFastBillInst.GetProcessTask(name.ToUpper());
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(bdProcessTask, out bool isSuccess1);
                if (!isSuccess1)
                {
                    waring = "此加工任务单中商品已经领取完毕, 或没有登记在您名下，或者不存在！";
                    return false;
                }

                sourceBill.object_name = "ProcessTask";
                sourceBill.object_id = bdProcessTask.body.objects[0].id;
            }
            else if (name.StartsWith("DT"))
            {
                LoadingDataEvent(this, true);
                BaseData<DistributionTask> bdDistributionTask = shelfFastBillInst.GetDistributionTask (name.ToUpper());
                LoadingDataEvent(this, false);
                HttpHelper.GetInstance().ResultCheck(bdDistributionTask, out bool isSuccess1);
                if (!isSuccess1)
                {
                    waring = "此配送任务单已完成或被撤销, 或没有登记在您名下，或者不存在！";
                    return false;
                }

                LoadingDataEvent(this, true);
                bdAllotAcceptance = shelfFastBillInst.GetAllotAcceptanceByDistributionTaskId(bdDistributionTask.body.objects[0].id);
                LoadingDataEvent(this, false);

                HttpHelper.GetInstance().ResultCheck(bdAllotAcceptance, out isSuccess1);
                if (!isSuccess1)
                {
                    waring = "无法找到此配送任务单对应的调拨验收单！";
                    return false;
                }

                sourceBill.object_name = "DistributionTask";
                sourceBill.object_id = bdDistributionTask.body.objects[0].id;
            }
            else
            {
                waring = "输入的不是加工任务单或者调拨配送！";
                return false;
            }

            LoadingDataEvent(this, true);
            BaseData<ShelfTaskFast> baseDataShelfTaskFast = shelfFastBillInst.GetShelfTaskFast(sourceBill.object_name, sourceBill.object_id);
            LoadingDataEvent(this, false);
            HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskFast, out bool isSuccess);
            
            if (!isSuccess)
            {
                BaseData<ProcessDoneCommodity> bdProcessDoneCommodity = new BaseData<ProcessDoneCommodity>();
                BaseData<AcceptanceCommodity> bdAcceptanceCommodity = new BaseData<AcceptanceCommodity>();

                if (name.StartsWith("GPT") || name.StartsWith("PT"))
                {
                    bdProcessDoneCommodity = shelfFastBillInst.GetProcessDoneCommodity(bdProcessTask.body.objects[0]);
                    HttpHelper.GetInstance().ResultCheck(bdProcessDoneCommodity, out bool isSuccess1);

                    if(!isSuccess1)
                    {
                        waring = "无法获取加工任务单的商品明细！";
                        return false;
                    }
                }
                else
                {
                    bdAcceptanceCommodity = shelfFastBillInst.GetAcceptanceCommodity(bdAllotAcceptance.body.objects[0]);
                    HttpHelper.GetInstance().ResultCheck(bdAcceptanceCommodity, out bool isSuccess1);

                    if (!isSuccess1)
                    {
                        waring = "无法获取调拨验收单的商品明细！";
                        return false;
                    }
                }

                BasePostData<ShelfTaskFast> baseDataShelfTaskFastNew = shelfFastBillInst.CreateShelfTaskFask(new ShelfTaskFast
                {
                    @Operator = ApplicationState.GetUserInfo().id,
                    Status = "待上架",
                    SourceBill = sourceBill,
                });
                HttpHelper.GetInstance().ResultCheck(baseDataShelfTaskFastNew, out bool isSuccess2);

                if(!isSuccess2)
                {
                    waring = "创建便捷上架任务单不成功！";
                    return false;
                }

                List<ShelfTaskFastDetail> shelfTaskFastDetails = new List<ShelfTaskFastDetail>();

                if (name.StartsWith("GPT") || name.StartsWith("PT"))
                {
                    foreach(var item in bdProcessDoneCommodity.body.objects)
                    {
                        shelfTaskFastDetails.Add(new ShelfTaskFastDetail
                        {
                            CommodityCodeId = item.CommodityCodeId,
                            CommodityId = item.CommodityId,
                            Status = "待上架",
                            ShelfTaskFastId = baseDataShelfTaskFastNew.body[0].id,
                        }) ;
                    }
                }
                else
                {
                    foreach (var item in bdAcceptanceCommodity.body.objects)
                    {
                        shelfTaskFastDetails.Add(new ShelfTaskFastDetail
                        {
                            CommodityCodeId = item.CommodityCodeId,
                            CommodityId = item.CommodityId,
                            Status = "待上架",
                            ShelfTaskFastId = baseDataShelfTaskFastNew.body[0].id,
                        });
                    }
                }

                BasePostData<ShelfTaskFastDetail> basePostDataShelfTaskFastDetail = shelfFastBillInst.CreateShelfTaskFaskDetail(shelfTaskFastDetails);
                HttpHelper.GetInstance().ResultCheck(basePostDataShelfTaskFastDetail, out bool isSuccess3);

                if(!isSuccess3)
                {
                    waring = "创建便捷上架任务单详情失败！";
                    return false;
                }

                EnterShelfFastDetailEvent(this, baseDataShelfTaskFastNew.body[0]);
                waring = "";
                return true;
            }
            else
            {
                //如果领⽤单作废标识为【是】则弹窗提醒手术单作废，跳转回前⻚
                if ("已完成".Equals(baseDataShelfTaskFast.body.objects[0].Status) || "已撤销".Equals(baseDataShelfTaskFast.body.objects[0].Status))
                {
                    waring = "此任务单下的便捷上架已经完成或被撤销！";
                    return false;
                }
                else
                {
                    EnterShelfFastDetailEvent(this, baseDataShelfTaskFast.body.objects[0]);
                    waring = "";
                    return true;
                }
            }
        }


        /// <summary>
        /// 扫码查询事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                EnterDetail_Click(this, null);
            }
        }

    }
}
