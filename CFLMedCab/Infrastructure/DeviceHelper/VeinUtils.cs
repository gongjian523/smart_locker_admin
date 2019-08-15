using CFLMedCab.Infrastructure.ToolHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace CFLMedCab.Infrastructure.DeviceHelper
{
    public  class VeinUtils
    {
        public enum Match_Flg
        {
            M_1_1,      //1比1验证
            M_1_G,      //1比G识别
            M_1_N,      //1比N识别
        };

        //错误代码
        public const int FV_ERRCODE_SUCCESS = 0;         //成功、正常
        public const int FV_ERRCODE_WRONG_PARAMETER = -1;		//参数错误：数值范围，内存大小，字符数组与字符串等
        public const int FV_ERRCODE_MEMORY_ALLOC_FAIL = -2;		//内存分配失败：内存不足
        public const int FV_ERRCODE_FUNCTION_INVALID = -3;		//功能无效：功能尚未支持
        public const int FV_ERRCODE_DEVICE_NOT_EXIST = -4;		//设备不存在：获取本地usb列表空，本地列表无微盾设备，获取某usb通讯接口空，设备列表该名称空, 装载tty不存在，设备被拔插属性已改变
        public const int FV_ERRCODE_DEVICE_NOT_INITED = -5;		//设备未初始化：未用忽略
        public const int FV_ERRCODE_INVALID_ERR_CODE = -6;		//无效错误代码：未用忽略
        public const int FV_ERRCODE_UNKNOWN = -9;		//未知错误：数据编解码错误，类
        public const int FV_ERRCODE_MATCH_FAILED = -10;		//验证失败
        public const int FV_ERRCODE_WRONG_IMAGE_TYPE = -11;	//不支持的图片类型：未用忽略
        public const int FV_ERRCODE_NOT_PERMITTED = -12;		//没有权限：信息读取，usb打开
        public const int FV_ERRCODE_NO_VALID_IMG = -13;		//没有有效图片：未用忽略
        public const int FV_ERRCODE_BUFFER_NOT_ENOUGH = -14;		//缓冲区大小不足：未用忽略
        public const int FV_ERRCODE_USER_NOT_EXIST = -15;		//上传的用户不存在
        public const int FV_ERRCODE_EXISTING = -16;		//设备已存在
        public const int FV_ERRCODE_LISTFULL = -17;		//设备列表满
        public const int FV_ERRCODE_LOAD_LIB = -18;	//加载动态库失败
        public const int FV_ERRCODE_LIBUSBINIT = -19;		//库初始化失败
        public const int FV_ERRCODE_DEVOPEN = -20;		//设备打开失败
        public const int FV_ERRCODE_DEVICE_NOT_OPEN = -21;		//设备未打开
        public const int FV_ERRCODE_INVALID_FEATURE = -22;		//无效的指静脉模板
        public const int FV_ERRCODE_GATHER_FAILED = -23;		//采集失败：手指放置时间过短内部等待放置超时等

        public const int FV_SENDERR = -50;		//发送失败
        public const int FV_RECEIERR = -51;	//接收失败
        public const int FV_RECEITIMEOUT = -52;		//接收超时
        public const int FV_SENDTIMEOUT = -53;		//发送超时
        public const int FV_CMDDIFFE = -54;		//接收数据串包
        public const int FV_PACKDAMAGE = -55;		//接收数据损坏
        public const int FV_CHECKERR = -56;		//接收数据未通过校验

        public const int FV_IMAGE_TYPE_JPG = 0x06;	//图片格式-jpg
        public const int FV_IMAGE_TYPE_BMP = 0x07;	//图片格式-bmp
        public const int FV_DYNAMIC_FEATURE_CNT = 3;		//单指动态特征推荐个数
        public const int FV_DEVNAME_CNT = 10;		//允许连接最大设备数

        public const int FV_DEVNAME_LEN = 64;		//设备名称长度(字节)
        public const int FV_FEATURE_SIZE = 512;	//单个特征数据长度(字节)
        public const int FV_DEVPARAM_LEN = 512;	//设备参数长度(字节)
        public const int FV_DEVSERIAL_LEN = 32;	//设备序列号长度（字节）
        public const int FV_DEVUUID_LEN = 32;		//设备uuid长度（字节）
        public const int FV_TTYFILENAME_LEN = 32;		//串口tty文件名称长度（字节）
        public const int FV_SDKV_LEN = 32;		//SDK版本号长度（字节）
        public const int FV_SIGN_LEN = 36;		//设备签名长度（字节）
        public const int FV_IMAGE_BMP_SIZE_352240 = 85558;	//352*240分辨率8位BMP文件大小(字节)
        public const int FV_IMAGE_BMP_SIZE_320240 = 77878;  //320*240分辨率8位BMP文件大小(字节)

        public const int D_FINGER_DETECT_INTERVAL = 100;     //检测手指放置状态的间隔，单位ms
        public const int D_FINGER_DETECT_PERIOD = 30000;  //手指检测的总时间，单位ms

        public const int FEATURE_COLLECT_CNT = 3;	//单指特征采集次数


        [DllImport("BioVein.Win32.dll", EntryPoint = "FV_GetSdkVersion", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_GetSdkVersion(StringBuilder version);
        //public static extern int FV_GetSdkVersion(ref char version);

        [DllImport("BioVein.Win32.dll", EntryPoint = "FV_EnumDevice", CallingConvention = CallingConvention.Cdecl)]
        //public static extern int FV_EnumDevice(char[][] devList);
        public static extern int FV_EnumDevice(StringBuilder[] devList);

        //装载设备-串口
        [DllImport("BioVein.Win32.dll", EntryPoint = "FV_LoadingDevice", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_LoadingDevice(StringBuilder devName, StringBuilder ttyName, int baudRate, uint libIndex);

        //卸载设备-串口
        [DllImport("BioVein.Win32.dll", EntryPoint = "FV_RemoveDevice", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_RemoveDevice(StringBuilder devName);

        //设备初始化
        [DllImport("BioVein.Win32.dll", EntryPoint = "FV_InitDevice", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_InitDevice(StringBuilder devName);

        //打开设备
        [DllImport("BioVein.Win32.dll", EntryPoint = "FV_OpenDevice", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_OpenDevice(StringBuilder devName, int flg);

        //关闭设备
        [DllImport("BioVein.Win32.dll", EntryPoint = "FV_CloseDevice", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_CloseDevice(StringBuilder devName);

        //检测设备上手指放置的状态
        [DllImport("BioVein.Win32.dll", EntryPoint = "FV_FingerDetect", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_FingerDetect(StringBuilder devName, ref byte fingerStatus);

        //读取一个指静脉特征
        [DllImport("BioVein.Win32.dll", EntryPoint = "FV_GrabFeature", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_GrabFeature(StringBuilder devName, byte[] featureData, char flg);

        //注册过程判断是否为同一根手指，不可用于识别
        [DllImport("BioVein.Win32.dll", EntryPoint = "FV_IsSameFinger", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_IsSameFinger(byte[] matchFeature, byte[] regFeature, byte regCnt, char flg);

        //指静脉模板比对
        [DllImport("BioVein.Win32.dll", EntryPoint = "FV_MatchFeature", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_MatchFeature(byte[] matchFeature, byte[] regFeature, byte regCnt, char flg, uint securityLevel, ref uint diff, byte[] aiFeatureBuf, ref uint aiDataLen);

        //读取设备的签名信息
        [DllImport("BioVein.Win32.dll", EntryPoint = "FV_GetDevSign", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_GetDevSign(StringBuilder devName, byte[] devsign, out ushort devsignLen);

        //设置设备的签名信息
        [DllImport("BioVein.Win32.dll", EntryPoint = "FV_SetDevSign", CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_SetDevSign(StringBuilder devName, byte[] srvsign, ushort srvsignLen);


        StringBuilder[] devIdList = new StringBuilder[10];	//设备列表
        StringBuilder devName = new StringBuilder("CabS");          //选择被操作设备
        StringBuilder serialName = new StringBuilder("COM7:");
        //StringBuilder devName = new StringBuilder(ApplicationState.GetMCabName());
        //StringBuilder serialName = new StringBuilder(ApplicationState.GetMVeinCOM() + ":");

        int devNum;                          //枚举设备数1~10

        bool bExitDetect;

        //监测
        public delegate void FingerDetectedHandler(object sender, int e);
        public event FingerDetectedHandler FingerDetectedEvent;

        // 定义一个静态变量来保存类的实例
        private static VeinUtils singleton;

        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        //定义公有方法提供一个全局访问点。
        public static VeinUtils GetInstance()
        {
            //这里的lock其实使用的原理可以用一个词语来概括“互斥”这个概念也是操作系统的精髓
            //其实就是当一个进程进来访问的时候，其他进程便先挂起状态
            if (singleton == null)
            {
                lock (locker)
                {
                    // 如果类的实例不存在则创建，否则直接返回
                    if (singleton == null)
                    {
                        singleton = new VeinUtils();
                    }
                }
            }
            return singleton;
        }

        public string GetSdkVersion()
        {
            char[] chars = new char[512];
            StringBuilder str = new StringBuilder(512);

            int res = FV_GetSdkVersion(str);
            //int res = FV_GetSdkVersion(ref chars[0]);

            if (res == 0)
                return str.ToString();
            //return new string(chars);
            else
                return "";
        }

        //枚举设备
        public void EnumDevice()
        {
            //枚举设备，带回设备名称列表

            for (int i = 0; i < 10; i++)
                devIdList[i] = new StringBuilder();

            int ret = FV_EnumDevice(devIdList);
            if (FV_ERRCODE_SUCCESS == ret)
            {
                //累计设备个数
                devNum = 0;
                for (int i = 0; devIdList[i].Length > 0; i++)
                    devNum++;
                LogUtils.Debug("Enumeration device successful, the number of devices is " + devNum);
            }
            else
            {
                LogUtils.Debug("Enum device failed! ret=" + ret);
            }
        }

        //安装设备
        public int LoadingDevice()
        {
            //枚举设备，带回设备名称列表
            DateTime startTime = DateTime.Now;
            int ret = FV_LoadingDevice(devName, serialName, 9600, 2);
            DateTime endTime = DateTime.Now;
            LogUtils.Debug($"调用LoadingDeviceSdk耗时{endTime.Subtract(startTime).TotalMilliseconds}");
            if (FV_ERRCODE_SUCCESS != ret)
            {
                LogUtils.Debug("Loading device failed! ret=" + ret);
            }

            return ret;
        }


        //初始化并打开设备
        public int  OpenDevice()
        {
            int ret;

            //1.初始化设备
            DateTime startTime = DateTime.Now;
            ret = FV_InitDevice(devName);
            DateTime endTime = DateTime.Now;
            LogUtils.Debug($"调用LoadingDeviceSdk耗时{endTime.Subtract(startTime).TotalMilliseconds}");
         
            if (FV_ERRCODE_SUCCESS != ret)
            {
                LogUtils.Debug("CabS: init failed! ret = " + ret);
                return ret;
            }

            //2.打开某设备
            startTime = DateTime.Now;
            ret = FV_OpenDevice(devName, 0);
            endTime = DateTime.Now;
            LogUtils.Debug($"调用OpenDeviceSdk耗时{endTime.Subtract(startTime).TotalMilliseconds}");
            if (FV_ERRCODE_SUCCESS == ret)
                LogUtils.Debug("CabS: open successed! ret=" + ret);
            else
                LogUtils.Debug("CabS: open failed! ret=" + ret);

            return ret;
        }


        //初始化并打开设备
        public int CloseDevice()
        {
            int ret;

            //1.初始化设备
            DateTime startTime = DateTime.Now;
            ret = FV_CloseDevice(devName);
            DateTime endTime = DateTime.Now;
            LogUtils.Debug($"调用CloseDeviceSdk耗时{endTime.Subtract(startTime).TotalMilliseconds}");

            if (FV_ERRCODE_SUCCESS != ret)
            {
                LogUtils.Debug("CabS: close failed! ret = " + ret);
            }
            return ret;
        }

        // 设置是否退出监测的状态变量（true 退出）
        public void SetDetectFingerState(bool state)
        {
            bExitDetect = state;
            LogUtils.Debug("Set bExitDetect " + (state ? "true":"false"));
        }

        // 检测手指是否放置到指静脉设备上 
        public void DetectFinger(object obj)
		{
            LogUtils.Debug($"调用DetectFinger1开始");
            int wDetectCnt = 0;                         //循环检测次数
			int wErrCount = 0;                          //检测产生错误的次数
			byte u1FingerStatus = 0;           //手指状态

			int nRetVal;
			int nDetectTimes = D_FINGER_DETECT_PERIOD / D_FINGER_DETECT_INTERVAL;
			int nInterval = D_FINGER_DETECT_INTERVAL;
			int nStartCnt = nDetectTimes / 30;

			int flg = 3;
			bExitDetect = false;
            LogUtils.Debug("Set bExitDetect false");

            string stateE = flg != 0 ? "Place" : "Remove";  //0移开；3放置
			string state = flg != 0 ? "放置" : "移开";  //0移开；3放置

            DateTime startDT = DateTime.Now;

			//等待移开手指、
			int i = 0;
			while (true)
			{   
                if(bExitDetect)
                {
                    LogUtils.Debug($"调用DetectFinger1退出1");
                    return;
                }

                ////每5分钟重启
                //if(DateTime.Now.Subtract(startDT).TotalSeconds > 5 * 60)
                //{
                //    int actCount = 0;
                //    for(actCount = 0; actCount < 3; actCount++)
                //    {
                //        nRetVal = FV_CloseDevice(devName);
                         
                //        if(nRetVal != FV_ERRCODE_SUCCESS)
                //        {
                //            LogUtils.Debug("关闭指静脉设备失败！");
                //            continue;
                //        }

                //        Thread.Sleep(1000);

                //        nRetVal = FV_OpenDevice(devName,0);

                //        if (nRetVal == FV_ERRCODE_SUCCESS)
                //        {
                //            startDT = DateTime.Now;
                //            break;
                //        }
                //        else
                //        {
                //            LogUtils.Debug("打开指静脉设备失败！");
                //        }
                //    }

                //    //失败3次之后，就需要重新
                //    if(actCount == 3)
                //    {
                //        FingerDetectedEvent(this, -1);
                //        return;
                //    }
                //}

                //循环检测手指
                LogUtils.Debug($"调用FingerDetect1 开始");
                DateTime startTime = DateTime.Now;
                nRetVal = FV_FingerDetect(devName, ref u1FingerStatus);
                DateTime endTime = DateTime.Now;
                LogUtils.Debug($"调用FingerDetect1 Sdk耗时{endTime.Subtract(startTime).TotalMilliseconds}");
                if (FV_ERRCODE_SUCCESS != nRetVal)
                {//检测时产生错误。
					wErrCount++;

					Thread.Sleep(nInterval);

					if (3 < wErrCount)
					{
						LogUtils.Debug("Wait for" + stateE + "finger error! " + Thread.CurrentThread.ManagedThreadId.ToString());
						FingerDetectedEvent(this, -1);
                        LogUtils.Debug($"调用DetectFinger1结束： -1");
                        return;
					}
				}
				else
				{
					wErrCount = 0;
					if ((flg & 0xFF) != u1FingerStatus)
					{
						//手指还没有移开
						if (0 == (wDetectCnt % nStartCnt))
						{
							LogUtils.Debug("Please " + stateE + " finger " + i + " " + Thread.CurrentThread.ManagedThreadId.ToString());//客户可以根据自己的系统情况采用语音、图片、文字等方式进行提示
							i++;
						}
						Thread.Sleep(nInterval);
					}
					else
					{
						//手指已经移开
						LogUtils.Debug("Finger detected correct " + stateE + " " + Thread.CurrentThread.ManagedThreadId.ToString());
						FingerDetectedEvent(this, 0);
                        LogUtils.Debug($"调用DetectFinger1结束： 0");
                        return;
					}
				}
			}
        }

        //检测手指是否放置到指静脉设备上  
        //public void DetectFinger()
        //{
        //    int wDetectCnt = 0;                         //循环检测次数
        //    int wErrCount = 0;                          //检测产生错误的次数
        //    byte u1FingerStatus = 0;           //手指状态

        //    int nRetVal;
        //    int nDetectTimes = D_FINGER_DETECT_PERIOD / D_FINGER_DETECT_INTERVAL;
        //    int nInterval = D_FINGER_DETECT_INTERVAL;
        //    int nStartCnt = nDetectTimes / 30;

        //    int flg = 3;
        //    bExitDetect = false;

        //    string stateE = flg != 0 ? "Place" : "Remove";  //0移开；3放置
        //    string state = flg != 0 ? "放置" : "移开";  //0移开；3放置

        //    //等待移开手指、
        //    int i = 0;
        //    while (!bExitDetect)
        //    { //循环检测手指

        //        LogUtils.Debug($"调用FingerDetect2 开始");
        //        DateTime startTime = DateTime.Now;
        //        nRetVal = FV_FingerDetect(devName, ref u1FingerStatus);
        //        DateTime endTime = DateTime.Now;
        //        LogUtils.Debug($"调用FingerDetect2 Sdk耗时{endTime.Subtract(startTime).TotalMilliseconds}");
        //        if (FV_ERRCODE_SUCCESS != nRetVal)
        //        {//检测时产生错误。
        //            wErrCount++;

        //            Thread.Sleep(nInterval);

        //            if (3 < wErrCount)
        //            {
        //                LogUtils.Debug("Wait for" + stateE + "finger error! " + Thread.CurrentThread.ManagedThreadId.ToString());
        //                FingerDetectedEvent(this, -1);
        //                LogUtils.Debug($"调用DetectFinger结束 -1");
        //                return;
        //            }
        //        }
        //        else
        //        {
        //            wErrCount = 0;
        //            if ((flg & 0xFF) != u1FingerStatus)
        //            {
        //                //手指还没有移开
        //                if (0 == (wDetectCnt % nStartCnt))
        //                {
        //                    LogUtils.Debug("Please " + stateE + " finger " + i + " " + Thread.CurrentThread.ManagedThreadId.ToString());//客户可以根据自己的系统情况采用语音、图片、文字等方式进行提示
        //                    i++;
        //                }
        //                Thread.Sleep(nInterval);
        //            }
        //            else
        //            {
        //                //手指已经移开
        //                LogUtils.Debug("Finger detected correct " + stateE + " " + Thread.CurrentThread.ManagedThreadId.ToString());
        //                FingerDetectedEvent(this, 0);
        //                LogUtils.Debug($"调用DetectFinger结束 0");
        //                return;
        //            }
        //        }
        //    }
        //    LogUtils.Debug($"调用DetectFinger结束 退出");
        //}

        // 等待手指某状态（ 0移开；3放置 ） 
        public int WaitState(int flg, out string info)
        {
            int wDetectCnt = 0;                         //循环检测次数
            int wErrCount = 0;                          //检测产生错误的次数
            byte u1FingerStatus = 0;           //手指状态

            int nRetVal;
            int nDetectTimes = D_FINGER_DETECT_PERIOD / D_FINGER_DETECT_INTERVAL;
            int nInterval = D_FINGER_DETECT_INTERVAL;
            int nStartCnt = nDetectTimes / 30;

            info = "";

            string stateE = flg != 0 ? "Place" : "Remove";  //0移开；3放置
            string state = flg != 0 ? "放置" : "移开";  //0移开；3放置

            //等待移开手指、
            int i = 0;
            while (nDetectTimes > wDetectCnt++)
            { //循环检测手指

                LogUtils.Debug($"调用FingerDetect3 开始");
                DateTime startTime = DateTime.Now;
                nRetVal = FV_FingerDetect(devName, ref u1FingerStatus);
                DateTime endTime = DateTime.Now;
                LogUtils.Debug($"调用FingerDetect3 Sdk耗时{endTime.Subtract(startTime).TotalMilliseconds}");
                if (FV_ERRCODE_SUCCESS != nRetVal)
                {//检测时产生错误。
                    wErrCount++;

                    Thread.Sleep(nInterval);

                    if (3 < wErrCount)
                    {
                        LogUtils.Debug("Wait for" + stateE + "finger error!");
                        info = "等待" + state + "指静脉时发生错误!";
                        return -1;
                    }
                }
                else
                {
                    wErrCount = 0;
                    if ((flg & 0xFF) != u1FingerStatus)
                    {//手指还没有移开
                        if (0 == (wDetectCnt % nStartCnt))
                        {
                            LogUtils.Debug("Please " + stateE + " finger " + i);//客户可以根据自己的系统情况采用语音、图片、文字等方式进行提示
                            i++;
                        }
                        Thread.Sleep(nInterval);
                    }
                    else
                    {//手指已经移开
                        LogUtils.Debug("Finger detected correct " + stateE);
                        return 0;
                    }
                }
            }

            LogUtils.Debug("Wait for " + stateE + " finger to timeout!");
            info = "等待" + state + "指静脉超时!";
            return -1;
        }

        //用户注册，例每个用户只注册一个根手指
        public int RegisterProcess(int feature_getCnt, byte[] sampfeature, out byte[] regfeature, out string info)
        {
            regfeature = new byte[512];
            info = "";

            //采集指静脉模板
            LogUtils.Debug($"调用GrabFeature 开始");
            DateTime startTime = DateTime.Now;
            int ret = FV_GrabFeature(devName, regfeature, (char)0x00);
            DateTime endTime = DateTime.Now;
            LogUtils.Debug($"调用GrabFeature Sdk耗时{endTime.Subtract(startTime).TotalMilliseconds}");
            if (FV_ERRCODE_SUCCESS != ret)
            {
                LogUtils.Debug("无法采集到指静脉特征:" + ret);
                info = "无法采集到指静脉特征!";
                return ret;
            }

            //第一枚以后的特征要与它之前的特征进行简单验证，简单判断是否同一手指
            if (feature_getCnt > 0)
            {
                LogUtils.Debug($"调用GrabFeature 开始");
                startTime = DateTime.Now;
                ret = FV_IsSameFinger(regfeature, sampfeature, (byte)feature_getCnt, (char)0x03);
                endTime = DateTime.Now;
                LogUtils.Debug($"调用GrabFeature Sdk耗时{endTime.Subtract(startTime).TotalMilliseconds}");

                if (FV_ERRCODE_SUCCESS != ret)
                {
                    //采集的模板不属于同一根手指，结束采集
                    LogUtils.Debug("比对指静脉特征失败：" + ret);
                    info = "本次采集的手指和上一个不同！";
                    return ret;
                }
            }

            info = "成功采集到第" + (feature_getCnt+1) +"个指静脉特征！";
            return ret;
        }

        //获取特征与已注册特征比对
        public int GrabFeature (byte[] macthfeature, out string info)
        {
            int ret = FV_GrabFeature(devName, macthfeature, (char)0x00);
            if (FV_ERRCODE_SUCCESS != ret){
                LogUtils.Debug("采集指静脉特征失败：" + ret);
                info = "采集指静脉特征失败！";
            }
            else{
                LogUtils.Debug("采集指静脉特征成功.");
                info = "采集指静脉特征成功！";
            }
            return ret;
        }

        //调用比对接口注意事项
        public int Match(byte[] macthfeature, byte[] regfeature, int regfcnt, byte[] aifeature,
            ref uint diff, int matchflg, ref uint ailen)
        {
            //说明：
            //比对结果 match(单个待比对特征，某根手指的注册特征，注册特征个数，该手指的动态特征，存放比多结果差异度）；动态特征可能被更新
            //比对应以某根手指为单位，这样才能生产对应该根手指的动态特征


            //1.获取被比对特征 = 某根手指的注册特征 + 该手指的动态特征
            int accept_match_feature_cnt = regfcnt + FV_DYNAMIC_FEATURE_CNT;
            byte[] accept_match_feature = new byte[accept_match_feature_cnt* FV_FEATURE_SIZE];
            Array.Copy(regfeature, 0, accept_match_feature, 0, regfcnt * FV_FEATURE_SIZE);
            Array.Copy(aifeature, 0, accept_match_feature, regfcnt * FV_FEATURE_SIZE, FV_DYNAMIC_FEATURE_CNT * FV_FEATURE_SIZE);

            //2.确定（安全级别）参数
            int securityLevel = -1;
            if (matchflg == (int)Match_Flg.M_1_1)
                securityLevel = 6; //1:1：已经通过其它方式确定某一个特定用户，调用本接口确定是否为其本人，建议数字为6
            else
                securityLevel = 4; //1:N：N个用户中循环调用本接口比对查找匹配的用户时，建议数字为4

            //3.说明你开辟的动态特征缓冲区大小
            //uint ailen = FV_DYNAMIC_FEATURE_CNT * FV_FEATURE_SIZE;  //输入为动态特征缓冲区大小，输出为动态模板长度

            //4.调用比对接口
            int ret = FV_MatchFeature(macthfeature, accept_match_feature, (byte)accept_match_feature_cnt, (char)0x03, (uint)securityLevel, ref diff, aifeature, ref ailen);

            //5.分析比对返回值
            if (ret != FV_ERRCODE_SUCCESS)
            {
                return ret;
            }

            //6.判断返回的动态特征长度长度，是否需要保存动态特征
            if (ailen > 0)
            {
                //在比对成功的前提下
                //比对函数返回了有效的学习数据，数据已经保存在FV_MatchFeature接口调用的第7个参数m_FeatureDataAI缓冲区中。
                //利用智能学习功能，把比对通过的情况下，把学习生成的数据作为下一次比对数据的一部分可以大大的提高用户体验.
                //返回的ailen值为0的话，aifeature的内容不会被改变，也就是保留上次学习的数据。

                //!!!!
                //更新fid的动态特征，更新可以逐渐提升比对精度及速度。
                //这里aifeature传参若为某个用户的内存，比对成功下直接被修改，不用在手动更新
                //如果aifeature传参不是某个用户的内存，需要拷贝自己更新

                LogUtils.Debug("Dynamic features have been updated.\r\n");
            }

            return ret;
        }

        //读取设备的签名信息
        public int GetDevSign(byte[] devsign, out ushort devsignLen)
        {
            DateTime startTime = DateTime.Now;
            int ret = FV_GetDevSign(devName, devsign, out devsignLen);
            DateTime endTime  = DateTime.Now;
            LogUtils.Debug($"调用GetDevSign Sdk耗时{endTime.Subtract(startTime).TotalMilliseconds}");

            return ret;
        }

        //设置设备的签名信息
        public int SetDevSign(byte[] srvsign, ushort srvsignLen)
        {
            DateTime startTime = DateTime.Now;
            int ret = FV_SetDevSign(devName, srvsign, srvsignLen);
            DateTime endTime = DateTime.Now;
            LogUtils.Debug($"调用SetDevSign Sdk耗时{endTime.Subtract(startTime).TotalMilliseconds}");
            return ret;
        }

    }
}
