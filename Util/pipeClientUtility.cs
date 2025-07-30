using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateSystem.Util
{
    internal class pipeClientUtility
    {

        public static async void SendMessage(string message, string pipeName = "MyPipe")
        {
            //string pipeName = "MyPipe";
            await Task.Run(() => {

                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(
                   ".", pipeName, PipeDirection.Out))
                {
                    try
                    {
                        Console.WriteLine("尝试连接服务器...");
                        //pipeClient.WriteTimeout = 1000;
                        pipeClient.Connect(1000);
                        Console.WriteLine("已连接到服务器！");

                        using (StreamWriter writer = new StreamWriter(pipeClient))
                        {

                            //string message = "识别测试";
                            writer.WriteLine(message);
                            writer.Flush(); // 确保数据立即发送

                        }
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e.ToString());
                    }
                }

            });
           
        }
    }
}
