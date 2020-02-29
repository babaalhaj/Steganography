using System.Drawing;
using System.Drawing.Imaging;

namespace GigHub.Services
{
    public class ImageProcessor
    {

        public static string GetImageExtension(Bitmap bitmap)
        {
            if(bitmap.RawFormat.Equals(ImageFormat.Jpeg))
                return ".jpg";
            if(bitmap.RawFormat.Equals(ImageFormat.Png))
                return ".png";
            if(bitmap.RawFormat.Equals(ImageFormat.Bmp))
                return ".bmp";
            if(bitmap.RawFormat.Equals(ImageFormat.Gif))
                return ".gif";
            return bitmap.RawFormat.Equals(ImageFormat.Icon) ? ".icon" : ".tiff";
        }
    }
}
