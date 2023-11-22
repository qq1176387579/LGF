/***************************************************
/// 作者:      liuhuan
/// 创建日期:  2022/7/3 1:46:50
/// 功能描述:  文件相关操作
****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using LGF;
using LGF.Log;

namespace LGF.Util
{
    public class FileUtils
    {

        //File.WriteAllText(GameSavePath, JsonMapper.ToJson(db)); 覆盖写入
        //  var dbstr = File.ReadAllText(GameSavePath);  全部写入  但是需要判断文件是否存在


        /// <summary>
        /// 获取目录名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetDirectoryName(string path)
        {
            // Path.GetDirectoryName返回的目录名是\\的
            var dir = Path.GetDirectoryName(path);
            return dir.Replace('\\', '/');
        }


        /// <summary>
        /// 文件夹创建
        /// </summary>
        /// <param name="path"></param>
        public static void DirectoryCreate(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        /// <summary>
        /// 文件夹删除
        /// </summary>
        /// <param name="path"></param>
        public static void DirectoryDelete(string path, bool recursive = false)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, recursive);
        }


        /// <summary>
        /// 文件夹删除在创建
        /// </summary>
        /// <param name="path"></param>
        public static void DirectoryDeleteOrCreate(string path)
        {
            //CreateDir(path);    //先检查路径是否存在
            DirectoryDelete(path, true);
            DirectoryCreate(path);
        }





        //private static void CreateDir(string filefullpath)
        //{
        //    if (File.Exists(filefullpath))
        //    {
                
        //    }
        //    else //判断路径中的文件夹是否存在
        //    {
        //        string dirpath = filefullpath.Substring(0, filefullpath.LastIndexOf('/'));
        //        string[] pathes = dirpath.Split('/');
        //        if (pathes.Length > 1)
        //        {
        //            string path = pathes[0];
        //            for (int i = 1; i < pathes.Length; i++)
        //            {
        //                path += "/" + pathes[i];
        //                if (!Directory.Exists(path))
        //                {
        //                    Directory.CreateDirectory(path);
        //                }
        //            }
        //        }
        //    }
        //}



        /// <summary>
        /// 文件拷贝
        /// </summary>
        /// <param name="path"></param>
        /// <param name="toPath"></param>
        public static void FileCopy(string path, string toPath)
        {
            File.Delete(toPath);      //删除指定文件
            File.Copy(path, toPath);
        }



        /// <summary>
        /// 文件拷贝
        /// </summary>
        /// <param name="path"></param>
        /// <param name="toPath"></param>
        public static void FileDelete(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
       


    }

}

