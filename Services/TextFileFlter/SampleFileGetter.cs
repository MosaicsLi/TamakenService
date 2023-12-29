using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TamakenService.Services.TextFileFlter
{
    public class SampleFileGetter
    {
        private string rootPath;
        private List<string> directoryList;
        private HashSet<string> fileList;
        public SampleFileGetter(string _rootPath)
        {
            rootPath = _rootPath;
            SetDirectoryList();
            SetFileList();
        }
        public HashSet<string> GetFileList
        {
            get { return fileList; }
        }
        private void SetDirectoryList()
        {
            directoryList = new List<string>();
            try
            {
                //Standard_FinalReport
                string[] secondPath = Directory.GetDirectories(rootPath);
                foreach (string folder in secondPath)
                {
                    //這裡抓日期
                    string[] thirdPath = Directory.GetDirectories(folder);
                    foreach (string thirdFolder in thirdPath)
                    {
                        if (thirdFolder.StartsWith($"{folder}/Standard") && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {//在這裡抓檔案
                            directoryList.Add(thirdFolder);
                        }
                        if (thirdFolder.StartsWith($"{folder}\\Standard") && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {//在這裡抓檔案
                            directoryList.Add(thirdFolder);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"查詢filePath: {rootPath} 發生錯誤:{ex.Message}");
            }
            Console.WriteLine($"已讀取根目錄: {rootPath} 總共有: {directoryList.Count} 個資料夾");
        }
        private void SetFileList()
        {
            fileList = new HashSet<string>();
            foreach (var folder in directoryList)
            {
                Console.WriteLine($"正在讀取資料夾: {folder}");
                string[] txtFiles = Directory.GetFiles(folder, "*.txt");
                foreach (string file in txtFiles)
                {
                    if (file.Contains("Sample_Map"))
                    {
                        Console.WriteLine($"已排除Sample_Map:{file}");
                        continue;
                    }
                    if (file.Contains("SNP_Map"))
                    {
                        Console.WriteLine($"已排除SNP_Map:{file}");
                        continue;
                    }
                    fileList.Add(file);
                }
            }
            Console.WriteLine($"總共有: {fileList.Count} 個Sample資料");

        }
    }
}
