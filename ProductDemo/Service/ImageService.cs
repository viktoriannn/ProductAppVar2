
using System.IO;
using System.Windows.Media.Imaging;

namespace ProductDemo.Service
{
    public class ImageService
    {
        private string Folder = Path.Combine(AppContext.BaseDirectory, "Res");

        public BitmapImage LoadImage(string? path)
        {
            string file = Path.Combine(AppContext.BaseDirectory, path ?? "");

            if (File.Exists(file))
                return Load(new Uri(file));

            try
            {
                string name = string.IsNullOrEmpty(path)
                    ? "picture.png"
                    : Path.GetFileName(path);

                return Load(new Uri("pack://application:,,,/Res/" + name));
            }
            catch
            {
                return Load(new Uri("pack://application:,,,/Res/picture.png"));
            }
        }

        private BitmapImage Load(Uri uri)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = uri;
            image.EndInit();
            image.Freeze();

            return image;
        }

        public bool CheckSize(string path)
        {
            BitmapImage image = Load(new Uri(path));
            return image.PixelWidth <= 300 && image.PixelHeight <= 200;
        }

        public string SaveImage(string path)
        {
            Directory.CreateDirectory(Folder);

            string name = Guid.NewGuid() + Path.GetExtension(path);
            File.Copy(path, Path.Combine(Folder, name));

            return "Res/" + name;
        }

        public void DeleteImage(string? path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            string file = Path.Combine(AppContext.BaseDirectory, path);

            if (File.Exists(file))
                File.Delete(file);
        }
    }
}
