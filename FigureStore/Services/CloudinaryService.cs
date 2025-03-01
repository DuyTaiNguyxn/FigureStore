using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace FigureStore.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(string cloudName, string apiKey, string apiSecret)
        {
            // Khởi tạo tài khoản Cloudinary với 3 thông tin quan trọng
            Account account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        // Hàm upload ảnh
        public string UploadImage(string filePath)
        {
            // Tạo thông số upload
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(filePath)
            };

            // Thực hiện upload
            var uploadResult = _cloudinary.Upload(uploadParams);

            // Trả về đường dẫn (URL) của ảnh đã upload
            return uploadResult.SecureUrl.ToString();
        }

        // Phương thức xóa ảnh từ Cloudinary dựa trên URL
        public string DeleteImageByUrl(string imageUrl)
        {
            var segments = imageUrl.Split('/');
            var fileName = segments.Last(); // "your_public_id.jpg"
            var publicId = fileName.Split('.').First(); // "your_public_id"

            var deletionParams = new DeletionParams(publicId);
            var result = _cloudinary.Destroy(deletionParams);
            return result.Result; // "ok" nếu thành công
        }
    }
}
