using JumpKick.HttpLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp2.test1
{
    public class Class2
    {
        //原文：https://archive.codeplex.com/?p=httplib
        //      https://github.com/j6mes/httplib/



        //get
        public void test1()
        {
            Http.Get("https://www.cnblogs.com/xuliangxing/p/8004403.html").OnSuccess(result =>
            {
                Console.Write(result);
            }).Go();
        }


        //get
        public void test2()
        {
            Http.Get("https://www.cnblogs.com/xuliangxing/p/8004403.html").OnSuccess(result =>
            {
                Console.Write(result);
            }).OnFail(webexception =>
            {
                Console.Write(webexception.Message);
            }).Go();
        }


        //post raw
        public void test3()
        {
            //方式1
            //string strUrlPara = "{ \"uri\": \"rtsp://admin:kj20091228@149.129.67.70:5554/Streaming/Channels/102?transportmode=unicast\"}";

            //方式2
            string strUrlPara = JsonConvert.SerializeObject(new
            {
                uri = "rtsp://admin:kj20091228@149.129.67.70:5554/Streaming/Channels/102?transportmode=unicast"
            });

            Http.Post("http://149.129.67.70:8080/start").Body(strUrlPara).OnSuccess(result =>
            {
                Console.Write(result);
            }).OnFail(webexception =>
            {
                Console.Write(webexception.Message);
            }).Go();
        }


        //post form
        public void test4()
        {
            Http.Post("http://183.66.231.18:8084/Login/ValidateLogin").Form(new
            {
                name = "test1",
                pwd = "test12",
                remember = false
            }).OnSuccess(result =>
            {
                Console.Write(result);
            }).OnFail(webexception =>
            {
                Console.Write(webexception.Message);
            }).Go();
        }


        //post cookie
        public void test5()
        {
            IDictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Cookie", "ASP.NET_SessionId=jwgaqf0xzbsuc2ojb1dojwzg");

            Http.Post("http://183.66.231.18:8084/SensorManagement/BindPosition").Form(new
            {
                bid = "15"
            }).Headers(header).OnSuccess(result =>
            {
                Console.Write(result);
            }).OnFail(webexception =>
            {
                Console.Write(webexception.Message);
            }).Go();
        }


        //upload file
        public void test6()
        {
            //测试外网 这个地址没有成功，可能是提交file的时候，还一起提交了其它参数
            //http://183.66.231.18:8084/Area/BridgeThreeModel/Save
            //http://183.66.231.18:8084/Document/Upload

            IDictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Cookie", "ASP.NET_SessionId=jwgaqf0xzbsuc2ojb1dojwzg");

            //上传文件的时候 附带上传参数 未成功

            //IDictionary<string, string> form = new Dictionary<string, string>();
            //form.Add("ID", "0");
            //form.Add("BridgeID", "15");

            //var obj = new
            //{
            //    ID = 0,
            //    BridgeID = 15
            //};
            //string form = JsonConvert.SerializeObject(obj);

            //var obj = new
            //{
            //    fileName = "44444"
            //};
            //string form = JsonConvert.SerializeObject(obj);
            ////var form = obj;

            //Stream stream = new MemoryStream();
            //byte[] bs = System.Text.Encoding.UTF8.GetBytes(form);
            //stream.Write(bs, 0, bs.Length);
            //stream.Flush();
            //stream.Close();

            Http.Post("http://localhost:47285/jquery.form/Handler1.ashx?Action=formUpload").Headers(header).Upload(files: new[] {
                new NamedFileStream("file", "photo.jpg", "application/octet-stream", File.OpenRead(@"1.png"))
            }).OnSuccess(result =>
            {
                Console.Write(result);
            }).OnFail(webexception =>
            {
                Console.Write(webexception.Message);
            }).Go();
        }


        //upload file 显示进度 感觉没有用
        public void test7()
        {
            IDictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Cookie", "ASP.NET_SessionId=jwgaqf0xzbsuc2ojb1dojwzg");

            Http.Post("http://183.66.231.18:8084/Document/Upload").Headers(header).Upload(files: new[] {
                new NamedFileStream("file", "1.png", "application/octet-stream", File.OpenRead(@"1.png"))
            }, onProgressChanged: (bytesSent, totalBytes) =>
            {
                var num = ((double)bytesSent / totalBytes.Value) * 100;
                num = Math.Round(num, 0);
                Console.WriteLine($"Uploading: {num}%");
            }).OnSuccess(result =>
            {
                Console.Write(result);
            }).OnFail(webexception =>
            {
                Console.Write(webexception.Message);
            }).Go();
        }


        //down file 显示下载进度
        public void test8()
        {
            Http.Get("http://localhost:47285/jquery.form/梁场数据.zip").DownloadTo(@"1111.zip", onProgressChanged: (bytesCopied, totalBytes) =>
            {
                if (totalBytes.HasValue)
                {
                    var num = ((double)bytesCopied / totalBytes.Value) * 100;
                    num = Math.Round(num, 0);
                    Console.WriteLine($"Downloaded: {num}%");
                }
                Console.WriteLine("Downloaded: " + bytesCopied.ToString() + " bytes");
            }, onSuccess: (headers) =>
             {
                 Console.WriteLine("Download Complete");
             }).Go();
        }








        public string HttpPostRaw(string url, string data)
        {
            string value = "";
            HttpWebRequest reqest = (HttpWebRequest)WebRequest.Create(url);
            reqest.Method = "POST";
            reqest.ContentType = "application/json";

            Stream stream = reqest.GetRequestStream();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(data);
            stream.Write(bs, 0, bs.Length);
            stream.Flush();
            stream.Close();

            HttpWebResponse response = (HttpWebResponse)reqest.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            value = sr.ReadToEnd();
            response.Close();
            return value;
        }

        public void kk2()
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            string strUrlPara = "{ \"uri\": \"rtsp://admin:kj20091228@149.129.67.70:5554/Streaming/Channels/102?transportmode=unicast\"}";
            byte[] data = new ASCIIEncoding().GetBytes(strUrlPara);
            byte[] responseArray = wc.UploadData("http://149.129.67.70:8080/start", data);
            var response = Encoding.UTF8.GetString(responseArray);
            Console.WriteLine(response);
        }


    }


}
