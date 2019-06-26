using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace pdftool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("pdftool");

            if (args.Count() > 1 && args[0] == "--merge")
            {
                merge(args);
                return;
            }
            if (args.Count() > 1 && args[0] == "--split")
            {
                split(args);
                return;
            }

            help();
        }

        static void merge(string[] files)
        {
            Console.WriteLine("");
            Console.WriteLine("正在合并：");

            string dirname = "";
            string outputfile = "";
            List<string> filelist = new List<string>();
            string filenames = "";

            for (int i = 1; i < files.Count(); i++)
            {
                if (File.Exists(@files[i]))
                {
                    filelist.Add(files[i]);
                    Console.WriteLine(Path.GetFileName(@files[i]));
                }
            }

            filelist.Sort();

            for (int i = 0; i < filelist.Count(); i++)
            {
                if (dirname == "" && outputfile == "")
                {
                    dirname = Path.GetDirectoryName(@filelist[i]);
                    int j = 0;
                    outputfile = Path.GetFileNameWithoutExtension(@filelist[i])+ "合并(" + j.ToString() + ")";
                    while (File.Exists(@dirname + "/" + outputfile + ".pdf"))
                    {
                        j++;
                        outputfile = Path.GetFileNameWithoutExtension(@filelist[i]) + "合并(" + j.ToString() + ")";
                    }
                }
                filenames += " \"" + filelist[i] + "\" ";
            }

            Console.WriteLine("到：");
            Console.WriteLine(outputfile+".pdf");

            string str = "pdftk.exe "+filenames+" cat output \""+dirname+"/"+outputfile+".pdf";

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(str);
            p.StandardInput.WriteLine("exit");


            p.StandardInput.AutoFlush = true;
            //p.StandardInput.WriteLine("exit");
            //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
            //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令



            //获取cmd窗口的输出信息
            string output = p.StandardOutput.ReadToEnd();

            //StreamReader reader = p.StandardOutput;
            //string line=reader.ReadLine();
            //while (!reader.EndOfStream)
            //{
            //    str += line + "  ";
            //    line = reader.ReadLine();
            //}

            p.WaitForExit();//等待程序执行完退出进程
            p.Close();
            
            Console.WriteLine(output);

            for (int i = 0; i < filelist.Count(); i++)
            {
                File.Delete(@filelist[i]);
            }


        }

        static void split(string[] files)
        {
            Console.WriteLine("");
            Console.WriteLine("正在拆分：");

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            //向cmd窗口发送输入信息
            for (int i = 1; i < files.Count(); i++)
            {
                if (File.Exists(@files[i]))
                {
                    Console.WriteLine(Path.GetFileName(@files[i]));
                    string str = "pdftk.exe \"" + files[i] + "\" burst ";
                    //Console.WriteLine(str);
                    p.StandardInput.WriteLine("cd \"" + Path.GetDirectoryName(@files[i]) + "\"");
                    p.StandardInput.WriteLine("mkdir \"" + Path.GetFileNameWithoutExtension(@files[i]) + "\"");
                    p.StandardInput.WriteLine("cd \"" + Path.GetFileNameWithoutExtension(@files[i]) + "\"");
                    p.StandardInput.WriteLine(str);
                }
            }
            p.StandardInput.WriteLine("exit");

            p.StandardInput.AutoFlush = true;

            string output = p.StandardOutput.ReadToEnd();
            
            p.WaitForExit();
            p.Close();

            Console.WriteLine(output);
        }

        static void help()
        {
            Console.WriteLine("");
            Console.WriteLine("    pdftool: 合并/分割PDF文件");
            Console.WriteLine("");
            Console.WriteLine("    pdftool [--merge | --split] file0 file1 file2 file3...");
            Console.WriteLine("");
            Console.WriteLine("        --merge    合并");
            Console.WriteLine("        --split    拆分");
            Console.WriteLine("");
        }
    }
}
