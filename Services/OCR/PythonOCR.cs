using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TamakenService.Log;

namespace TamakenService.Services.OCR
{
    public class PythonOCR
    {
        private string pythonPath;
        private string imagePath;
        private string oCRProgramPath;
        private string tesseractPath;
        private NlogService nlogService;
        public PythonOCR(string _pythonPath, NlogService _nlogService)
        {
            pythonPath = _pythonPath;   
            nlogService = _nlogService;
        }
        public string ImagePath { get; set; }
        public string OCRProgram { get; set; }
        public string TesseractPath { get; set; }
        public string RunOCR()
        {
            if (string.IsNullOrWhiteSpace(imagePath)) 
            {
                throw new Exception("imagePath required");
            }
            if (string.IsNullOrWhiteSpace(oCRProgramPath))
            {
                throw new Exception("oCRProgramPath required");
            }
            string output="";
            string error="";
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = pythonPath,
                Arguments = oCRProgramPath+" "+ imagePath+" \\temp\\temp.png "+ tesseractPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using (Process process = Process.Start(psi))
            {
                try
                {
                    // 等待外部應用程式結束
                    process.WaitForExit();

                    // 讀取標準輸出和標準錯誤
                    output = process.StandardOutput.ReadToEnd();
                    error = process.StandardError.ReadToEnd();

                    // 顯示結果
                    Console.WriteLine("Python 腳本回傳: " + output);
                    Console.WriteLine("Python 腳本錯誤: " + error);
                }
                catch (Exception e)
                {
                    Console.WriteLine("錯誤: " + e);
                }
            }
            return output;
        }
    }
}
