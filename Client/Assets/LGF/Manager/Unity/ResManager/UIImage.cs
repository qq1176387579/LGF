using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if ADDRESSABLE_ASSETS
namespace LGF
{
    [RequireComponent(typeof(Image))]
    public class UIImage : MonoAssetLoader<Sprite>
    {
        /// <summary>
        /// Image组件
        /// </summary>
        private Image mImage;

        /// <summary>
        /// Image
        /// </summary>
        public Image Image {
            get {
                if (null == mImage) {
                    mImage = GetComponent<Image>();
                }
                return mImage;
            }
        }

        protected override void OnLoadComplete(Sprite res)
        {
            Image.sprite = res;
        }

        /// <summary>
        /// 设置贴图
        /// </summary>
        /// <param name="imageName"></param>
        public void SetImage(string imageName)
        {
            LoadAsset(imageName);
        }

    }
}
#endif