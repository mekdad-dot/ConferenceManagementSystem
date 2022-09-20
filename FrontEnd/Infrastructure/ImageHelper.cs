using NuGet.Protocol;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;


namespace FrontEnd.Infrastructure
{
    public class ImageHelper
    {
        public static async Task UploadedFile(IFormFile ProfileImage, string uniqueFileName)
        {
            if (ProfileImage != null)
            {

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images");

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var image = Image.Load(ProfileImage.OpenReadStream()))
                {
                    string newSize = await ResizeImage(image, 800, 800);

                    string[] aSize = newSize.Split(',');

                    image.Mutate(h => h.Resize(Convert.ToInt32(aSize[1]), Convert.ToInt32(aSize[0])));
                    
                    image.Save(filePath);

                }
            }
        }

        public static async Task<string> ResizeImage(Image img,int MaxWidth,int MaxHeight)
        {
            if(img.Width > MaxWidth || img.Height > MaxHeight)
            {
                double widthRatio = (double) img.Width / (double) MaxWidth;   
                double heightRatio = (double) img.Height / (double)MaxHeight;

                double ratio = Math.Max(widthRatio, heightRatio);

                int newWidth = (int)(img.Width / ratio);
                int newHeight = (int)(img.Height / ratio);

                return newHeight.ToString() + "," + newWidth.ToString();
            }
            else
            {
                return img.Height.ToString() + "," + img.Width.ToString();
            }
        }

    }
}
