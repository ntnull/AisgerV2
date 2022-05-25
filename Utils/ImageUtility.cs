using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Aisger.Utils
{
    public static class ImageUtility
    {
        public static SaveImageResult SaveImage(string path, int maxSize, string allowedExtensions,
            HttpPostedFileBase image, HttpServerUtilityBase server)
        {
            return SaveImage(path, maxSize, allowedExtensions, image, server, new Guid());
        }

        public static void WriteFileFromStream(Stream stream, string toFile)
        {
            using (FileStream fileToSave = new FileStream(toFile, FileMode.Create))
            {
                stream.CopyTo(fileToSave);
            }
        }
        public static SaveImageResult SaveImage(string path, int maxSize, string allowedExtensions, HttpPostedFileBase image,
            HttpServerUtilityBase server, Guid guid)
        {
            var result = new SaveImageResult { Success = false };

            if (image == null || image.ContentLength == 0)
            {
                result.Errors.Add("There was problem with sending image.");
                return result;
            }

            // Check image size
            if (image.ContentLength > maxSize)
                result.Errors.Add("Image is too big.");

            // Check image extension
            var extension = Path.GetExtension(image.FileName).Substring(1).ToLower();
            if (!allowedExtensions.Contains(extension))
                result.Errors.Add(string.Format("'{0}' format is not allowed.", extension));

            // If there are no errors save image
            if (!result.Errors.Any())
            {
                // Generate unique name for safety reasons
                var newName = guid.ToString("N") + "." + extension;
                var serverPath = server.MapPath("~" + path + newName);
                image.SaveAs(serverPath);

                result.Success = true;
                result.FileName = newName;
            }

            return result;
        }
    }

    public class SaveImageResult
    {
        public bool Success { get; set; }
        public String FileName { get; set; }
        public List<string> Errors { get; set; }

        public SaveImageResult()
        {
            Errors = new List<string>();
        }
    }
}