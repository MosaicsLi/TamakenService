from PIL import Image
import pytesseract
import cv2
import argparse

def denoise_image(input_path, output_path, strength=10):
    # 读取图像
    image = cv2.imread(input_path)

    # 将图像转换为灰度图
    gray_image = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

    # 使用去噪函数
    denoised_image = cv2.fastNlMeansDenoising(gray_image, None, h=strength)

    # 将结果保存到文件
    cv2.imwrite(output_path, denoised_image)

if __name__ == "__main__":
    # 建立參數解析器
    parser = argparse.ArgumentParser(description="Denoise and OCR image.")
    parser.add_argument("input_image_path", help="Input image file path")
    parser.add_argument("output_image_path", help="Output denoised image file path")
    parser.add_argument("Tesseract_path", help="Tesseract file path")
    args = parser.parse_args()

    # 调用去噪函数
    denoise_image(args.input_image_path, args.output_image_path)

    # 設定 Tesseract 的路徑
    pytesseract.tesseract_cmd = args.Tesseract_path

    # 開啟去噪後的圖片
    img = Image.open(args.output_image_path)

    # 進行 OCR 辨識
    text = pytesseract.image_to_string(img, lang='eng')
    print(text.encode('utf-8').decode('utf-8'))

    #等待使用者輸入
    input("Press Enter to exit...")