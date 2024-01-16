using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TamakenService.Log;
using TamakenService.Models.CubicAPI;
using TamakenService.Models.TextFileFilter;
using TamakenService.Services.TextFileFlter;

namespace TamakenService.Services.CubicAPI
{
    public class TextFileFilterServuce
    {
        private TextFileFilterInPutModel _pathModel;
        private NlogService _logger;
        private ExportFile ExportFile;
        private List<string> SNPIndexList;
        private Hashtable SNPMathFeatureHashtable;
        private Hashtable SNPHashtable;
        private HashSet<string> fileList;
        private int totalfile;
        private int batchSize=20;
        private SampleFileGetter folderList;

        public TextFileFilterServuce(TextFileFilterInPutModel inPutModel)
        {
            _logger = new NlogService($"log/{DateTime.Now.ToString("yyyyMMdd")}.log");
            _pathModel = inPutModel;
        }


        private void SetCriteria()
        {
            var filterCriteria = new FilterCriteria(_pathModel.SNPPath, _logger);//設定讀取Sample資料的條件物件物件
            SNPIndexList = filterCriteria.GetSNPIndexList();//取得欲分析的基因名稱
            SNPHashtable = filterCriteria.GetSNPHashtable();//取得顯示基因名稱與sample基因名稱對應Hashtable
            SNPMathFeatureHashtable = filterCriteria.GetSNPMathFeatureHashtable();//取得數值化參數與sample基因名稱對應hashtabll
            ExportFile = new ExportFile(SNPIndexList, SNPHashtable, _logger);//新建輸出檔案物件
        }
        public void SetSamplePathList()
        {

            var SampleListFileExists = File.Exists(_pathModel.SamplePathList);
            if (!SampleListFileExists)
            {
                _logger.WriteLine($"路徑 {_pathModel.SamplePathList} 不存在，將新建Sample年份資料", true);
                var folderListAll = new SampleFileGetter(_pathModel.SamplePath, _logger);
                HashSet<string> fileListAll = folderListAll.GetFileList;
                ExportFile.SaveFilePath(_pathModel.SamplePathList, fileListAll);
                _logger.WriteLine($"已將 {fileListAll.Count} 筆資料寫入 路徑： {_pathModel.SamplePathList}", true);
            }
            folderList = new SampleFileGetter(_logger);
            folderList.GetFileFromCsv(_pathModel.SamplePathList);
            fileList = folderList.GetFileList;
            totalfile = fileList.Count;
            _logger.WriteLine($"Sample年份資料中尚有 {totalfile} 筆資料未掃描", true);
        }
        public void Run()
        {

            try
            {
                SetCriteria();
                SetSamplePathList();
                for (int i = 0; i < totalfile; i += batchSize)
                {
                    HashSet<string> batch = new HashSet<string>(fileList.Take(batchSize));

                    _logger.WriteLine($"目前份數:{i} 剩餘: {fileList.Count}", true);

                    _logger.WriteLine($"開始工作", true);
                    ThreadWorker threadWorker = new ThreadWorker(batch, SNPMathFeatureHashtable, _logger);//使用ThreadWorker避免巢狀
                    HashSet<ExportSampleData> sampleData = threadWorker.Run();
                    _logger.WriteLine($"工作結束", true);

                    _logger.WriteLine($"開始輸出基因:{i} 剩餘: {fileList.Count}", true);
                    ExportFile.SaveSetToCSV(sampleData, _pathModel.OutPutPath);
                    _logger.WriteLine($"開始輸出數據:{i} 剩餘: {fileList.Count}", true);
                    ExportFile.SaveSetToMathCSV(sampleData, _pathModel.OutPutPath.Replace(".csv", "Math.csv"));

                    _logger.WriteLine($"已輸出當前批次資料", true);
                    folderList.UpdateCsvFile(_pathModel.SamplePathList, batch);
                    _logger.WriteLine($"已更新{_pathModel.SamplePathList}", true);
                    fileList.ExceptWith(batch);// 從原始 fileList 中移除已處理的部分

                    _logger.WriteLine($"開始清除sampleData : {sampleData.Count} 筆", true);
                    sampleData.Clear();
                    _logger.WriteLine($"已移除sampleData", true);
                    batch.Clear();
                }
                _logger.WriteLine($"掃描完畢", true);
            }
            catch (Exception ex)
            {
                _logger.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }
}
