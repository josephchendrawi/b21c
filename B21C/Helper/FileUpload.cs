using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace B21C.Helper
{
    public class FileHelper
    {
        public static string SaveFile(HttpPostedFileBase FileModel, string postfix)
        {
            string uploadedPath = "";
            if (FileModel.ContentLength > 0)
            {
                var fileName = Path.GetFileName(FileModel.FileName);
                fileName = Path.GetFileNameWithoutExtension(fileName) + "_" + postfix + Path.GetExtension(fileName);
                fileName = fileName.Replace(" ", "-");
                //save to local
                uploadedPath = fileName;
                var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Upload"), fileName);
                FileModel.SaveAs(path);
            }
            else
            {
                throw new Exception("Invalid File.");
            }

            return uploadedPath;
        }

        public static string SaveImage(Image Image, string Filename, string postfix)
        {
            string uploadedPath = "";
            if (Image != null)
            {
                var fileName = Path.GetFileName(Filename);
                fileName = Path.GetFileNameWithoutExtension(fileName) + "_" + postfix + Path.GetExtension(fileName);
                fileName = fileName.Replace(" ", "-");
                //save to local
                uploadedPath = fileName;
                var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Upload"), fileName);
                Image.Save(path);
            }
            else
            {
                throw new Exception("Invalid Image.");
            }

            return uploadedPath;
        }

    }
}