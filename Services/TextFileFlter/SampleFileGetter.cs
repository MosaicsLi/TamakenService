using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TamakenService.Log;

namespace TamakenService.Services.TextFileFlter
{
    public class SampleFileGetter
    {
        private string rootPath;
        private List<string> directoryList;
        private HashSet<string> fileList;
        private NlogService nlogService;
        public SampleFileGetter(NlogService _nlogService)
        {
            nlogService = _nlogService;
        }
        public SampleFileGetter(string _rootPath, NlogService _nlogService)
        {
            nlogService = _nlogService;
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
                nlogService.WriteLine($"查詢filePath: {rootPath} 發生錯誤:{ex.Message}");
            }
            nlogService.WriteLine($"已讀取根目錄: {rootPath} 總共有: {directoryList.Count} 個資料夾");
        }
        private void SetFileList()
        {
            fileList = new HashSet<string>();
            foreach (var folder in directoryList)
            {
                nlogService.WriteLine($"正在讀取資料夾: {folder}");
                string[] txtFiles = Directory.GetFiles(folder, "*.txt");
                foreach (string file in txtFiles)
                {
                    if (file.Contains("Sample_Map"))
                    {
                        nlogService.WriteLine($"已排除Sample_Map:{file}");
                        continue;
                    }
                    if (file.Contains("SNP_Map"))
                    {
                        nlogService.WriteLine($"已排除SNP_Map:{file}");
                        continue;
                    }
                    fileList.Add(file);
                }
            }
            nlogService.WriteLine($"總共有: {fileList.Count} 個Sample資料");

        }
        public void GetFileFromCsv(string filePath)
        {
            // 讀取原始 CSV 檔案
            fileList = new HashSet<string>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var CsvData = line.Split(",");
                    if (CsvData[1].Equals("N"))
                    {
                        fileList.Add(CsvData[0]);
                        nlogService.WriteLine($"資料已讀取: {CsvData[0]}");
                    }
                }
            }
            nlogService.WriteLine($"此次資料已讀取: {fileList.Count} 個Sample資料");
        }
        public void UpdateCsvFile(string filePath, HashSet<string> batch)
        {
            // 讀取原始 CSV 檔案
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var CsvData = line.Split(",");
                    if (batch.Contains(CsvData[0]))
                    {
                        line = line.Replace(",N", ",Y");
                        nlogService.WriteLine($"已標記為讀取完畢: {CsvData[0]}");
                    }
                    lines.Add(line);
                }
            }

            // 寫入更新後的 CSV 檔案
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (string line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }
    }
}
