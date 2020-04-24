using DataModel.FileDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DataModel.CustomDataManager;
using System.Collections;

namespace DataModel
{
    public static class DataCenter
    {
        /// <summary>
        /// 文件夹类字典key值是路径
        /// </summary>
        private static Dictionary<string, CustomDirectoryInfo> dirDic = new Dictionary<string, CustomDirectoryInfo>();
        /// <summary>
        /// 文件类字典key值是路径
        /// </summary>
        private static Dictionary<string, CustomFileInfo> fileDic = new Dictionary<string, CustomFileInfo>();
        /// <summary>
        /// 全景图列表
        /// </summary>
        private static List<PanoInfo> panoList = new List<PanoInfo>();
        /// <summary>
        /// 产品列表
        /// </summary>
        private static List<ProductInfo> productList = new List<ProductInfo>();
        /// <summary>
        /// 场景列表
        /// </summary>
        private static List<SceneInfo> sceneList = new List<SceneInfo>();

        /// <summary>
        /// 报错代码，以及路径
        /// </summary>
        private static string errorPath = string.Empty;

        public static void initAll(string dirPath)
        {
            dirDic.Clear();
            fileDic.Clear();
            panoList.Clear();
            productList.Clear();
            sceneList.Clear();

            errorPath = string.Empty;

            RecursiveDirs(dirPath);
            FindScene(dirPath);
        }
        /// <summary>
        /// 递归查找所有文件夹以及文件，填满字典
        /// </summary>
        /// <param name="dirPath">查找的目录</param>
        private static void RecursiveDirs(string dirPath)
        {
            errorPath = "init_" + dirPath;
            string[] files = Directory.GetFiles(dirPath);
            for (int i = 0; i < files.Length; i++)
            {
                CustomFileInfo item = new CustomFileInfo(files[i], Guid.NewGuid().ToString());
                if (fileDic.ContainsKey(files[i]))
                {
                    fileDic[files[i]] = item;
                }
                else
                {
                    fileDic.Add(files[i], item);
                }
            }

            string[] dirs = Directory.GetDirectories(dirPath);
            for (int i = 0; i < dirs.Length; i++)
            {
                CustomDirectoryInfo item = new CustomDirectoryInfo(dirs[i], Guid.NewGuid().ToString());
                if (dirDic.ContainsKey(dirs[i]))
                {
                    dirDic[dirs[i]] = item;
                }
                else
                {
                    dirDic.Add(dirs[i], item);
                }
                RecursiveDirs(dirs[i]);
            }
        }
        /// <summary>
        /// 填写场景列表
        /// </summary>
        /// <param name="allSceneDir">场景目录</param>
        private static void FindScene(string allSceneDir)
        {
            //设置报错路径
            errorPath = "FindScene" + allSceneDir;

            string[] scenesDir = Directory.GetDirectories(allSceneDir);
            for (int i = 0; i < scenesDir.Length; i++)
            {
                CustomDirectoryInfo dirItem;// = dirDic[scenesDir[i]];
                if (!dirDic.TryGetValue(scenesDir[i], out dirItem))
                {
                    errorPath = scenesDir[i];
                    throw new Exception("未检查到相关key值");
                }
                //设置报错路径
                errorPath = dirItem.FullPath;

                //去除中间的多余的横线造成的空字数据
                string[] sceneAtt = dirItem.everyPathArr[dirItem.everyPathArr.Length - 1].Split('-');
                List<string> sceneAttList = new List<string>(sceneAtt);
                for (int j = sceneAttList.Count - 1; j >= 0; j--)
                {
                    if (string.IsNullOrEmpty(sceneAttList[j]))
                    {
                        sceneAttList.RemoveAt(j);
                    }
                }
                //获取数据
                sceneAtt = sceneAttList.ToArray();
                //设置数据
                SceneInfo scene = new SceneInfo();
                scene.Name = sceneAtt[0];
                scene.sceneType.Add("厨房");

                //添加标签
                SceneFlagStruct flag = new SceneFlagStruct();
                flag.flagTypeName = sceneAtt[1];
                for (int j = 2; j < sceneAtt.Length; j++)
                {
                    flag.flagValue += sceneAtt[j];
                }
                scene.flag.Add(flag);

                //文件名列表,查找缩略图
                string[] filesPath = Directory.GetFiles(dirItem.FullPath);
                for (int j = 0; j < filesPath.Length; j++)
                {
                    CustomFileInfo fileInfo;
                    if (fileDic.TryGetValue(filesPath[j], out fileInfo))
                    {
                        if (fileInfo.ExName == ".jpg" || fileInfo.ExName == ".png" || fileInfo.ExName == ".jpge")
                        {
                            scene.ThumbnailPath = fileInfo.FullPath;
                            break;
                        }
                    }
                }

                FindProduct(dirItem, scene);

                errorPath = scene.Name + "找不到全景图";
                scene.DefaultPanoName = scene.includePanoNames[0];

                sceneList.Add(scene);
            }
        }
        /// <summary>
        /// 填写产品列表
        /// </summary>
        /// <param name="sceneDir">场景目录</param>
        /// <param name="scene">归属场景</param>
        private static void FindProduct(CustomDirectoryInfo sceneDir, SceneInfo scene)
        {
            errorPath = "FindProduct_" + sceneDir.FullPath;
            string[] productsDir = Directory.GetDirectories(sceneDir.FullPath);

            for (int i = 0; i < productsDir.Length; i++)
            {
                CustomDirectoryInfo dirItem;// = dirDic[productsDir[i]];
                if (!dirDic.TryGetValue(productsDir[i], out dirItem))
                {
                    errorPath = productsDir[i];
                    throw new Exception("未检查到相关key值");
                }
                errorPath = dirItem.FullPath;
                //去除中间的多余的横线造成的空字数据
                string[] productAtt = dirItem.everyPathArr[dirItem.everyPathArr.Length - 1].Split('-');
                List<string> productAttList = new List<string>(productAtt);
                for (int j = productAttList.Count - 1; j >= 0; j--)
                {
                    if (string.IsNullOrEmpty(productAttList[j]))
                    {
                        productAttList.RemoveAt(j);
                    }
                }
                productAtt = productAttList.ToArray();

                ProductInfo product = new ProductInfo();
                product.Name = productAtt[0];
                product.productType = "厨房";
                //添加标签
                ProductFlagStruct productFlag = new ProductFlagStruct();
                productFlag.flagTypeName = productAtt[1];
                productFlag.flagValue = productAtt[2];
                product.flag.Add(productFlag);

                //文件名列表,查找缩略图,以及产品介绍路径
                string[] filesPath = Directory.GetFiles(dirItem.FullPath);
                for (int j = 0; j < filesPath.Length; j++)
                {
                    CustomFileInfo fileInfo;
                    if (fileDic.TryGetValue(filesPath[j], out fileInfo))
                    {
                        if (fileInfo.ExName == ".jpg" || fileInfo.ExName == ".png" || fileInfo.ExName == ".jpge")
                        {
                            if (fileInfo.FileName.IndexOf("thumb") != -1)
                                product.ThumbnailPath = fileInfo.FullPath;
                            else
                            {
                                FindPano(fileInfo, product, scene);
                            }
                            //break;
                        }
                        else if (fileInfo.ExName == ".txt")
                        {
                            product.ProductContentPath = fileInfo.FullPath;
                        }
                    }
                }

                product.relatedSceneNames.Add(scene.Name);
                scene.includeProductNames.Add(product.Name);

                productList.Add(product);
            }
        }
        /// <summary>
        /// 查找全景图
        /// </summary>
        /// <param name="panoFile">全景图文件</param>
        /// <param name="product">归属产品</param>
        /// <param name="scene">归属场景</param>
        private static void FindPano(CustomFileInfo panoFile, ProductInfo product, SceneInfo scene)
        {
            string errorCode = errorPath;
            errorPath = "FindPano_" + panoFile.FullPath;

            PanoInfo pano = new PanoInfo();
            pano.Name = scene.Name + "_" + product.Name;
            if (panoFile.FileName.IndexOf("scene") != -1)
            {
                pano.PanoType = "全景图";
            }
            else
                pano.PanoType = "平面图";
            pano.PanoTexturePath = panoFile.FullPath;

            product.relatedPanoNames.Add(pano.Name);
            scene.includePanoNames.Add(pano.Name);

            panoList.Add(pano);
            errorPath = errorCode;
        }

        public static List<PanoInfo> GetPanoList()
        {
            return panoList;
        }
        public static List<ProductInfo> GetProductList()
        {
            return productList;
        }
        public static List<SceneInfo> GetSceneList()
        {
            return sceneList;
        }

        public static string GetErrorPath()
        {
            return errorPath;
        }
    }
}
