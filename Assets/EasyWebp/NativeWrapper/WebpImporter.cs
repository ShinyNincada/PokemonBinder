using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using WebP;

public class WebpImporter : MonoBehaviour
{
    public static async void LoadWebpTexture2D(string path, Action<Texture2D> callback)
    {
        byte[] bytes = await LoadAsyncBytes(path);
        LoadWebpTexture2D(bytes, callback);
    }

    public static void LoadWebpTexture2D(byte[] bytes, Action<Texture2D> callback)
    {
        WebP.Error lError;
        var texture = Texture2DExt.CreateTexture2DFromWebP(bytes, lMipmaps: false, lLinear: true, lError: out lError);
        if (callback != null)
        {
            callback(texture);
        }
    }


    public static async void PlayWebpAnimation(string path, Action<Texture2D> callback)
    {
        byte[] bytes = await LoadAsyncBytes(path);

        PlayWebpAnimation(bytes, callback);
    }

    public static async void PlayWebpAnimation(byte[] bytes, Action<Texture2D> callback)
    {
        List<(Texture2D, int)> lst = Texture2DExt.LoadTexture2DFromWebP(bytes);
        int prevTimestamp = 0;
        for (int i = 0; i < lst.Count; ++i)
        {
            (Texture2D texture, int timestamp) = lst[i];
            if (texture == null)
                return;

            if (callback != null)
            {
                callback(texture);
            }
            int delay = timestamp - prevTimestamp;
            prevTimestamp = timestamp;
            if (delay < 0)
            {
                delay = 0;
            }
            await Task.Delay(delay);
            if (i == lst.Count - 1)
            {
                i = -1;
            }
        }
    }

    public static async void LoadWebp(string path, Action<Texture2D> callback)
    {
        byte[] bytes = await LoadAsyncBytes(path);
        if (!isAnimationWebp(bytes))
        {
            LoadWebpTexture2D(bytes, callback);
        }
        else
        {
            PlayWebpAnimation(bytes, callback);
        }
    }

    public static IEnumerator PlayWebpAnimationWebGL(string loadPath, Action<Texture2D> callback)
    {
        var webRequest = UnityWebRequest.Get(loadPath);
        webRequest.timeout = 30;
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.ConnectionError)
        {
            byte[] bytes = webRequest.downloadHandler.data;
            List<(Texture2D, int)> lst = Texture2DExt.LoadTexture2DFromWebP(bytes);

            int prevTimestamp = 0;
            for (int i = 0; i < lst.Count; ++i)
            {
                (Texture2D texture, int timestamp) = lst[i];

                if (callback != null)
                {
                    callback(texture);
                }

                int delay = timestamp - prevTimestamp;
                prevTimestamp = timestamp;
                if (delay < 0)
                {
                    delay = 0;
                }
                yield return new WaitForSeconds(delay / 1000.0f);
                if (i == lst.Count - 1)
                {
                    i = -1;
                }
            }
        }
    }


    static async Task<byte[]> LoadAsyncBytes(string url)
    {
        var getRequest = UnityWebRequest.Get(url);
        await getRequest.SendWebRequest();
        return getRequest.downloadHandler.data; ;
    }


    #region isAnimationWebp
    /// <summary>
    /// Use byte[] data to determine whether the webp file is a dynamic image or a static image
    /// 通过byte[]数据判断webp文件是动态图片还是静态图片
    /// </summary>
    /// <param name="webpData"></param>
    /// <returns></returns>
    public static bool isAnimationWebp(byte[] webpData)
    {
        byte[] anmf = new byte[] { 65, 78, 77, 70 };
        List<byte> sourceList = webpData.ToList();
        int index = 0;
        return isContainBytes(sourceList, anmf, ref index);
    }

    private static bool isContainBytes(List<byte> sourceList, byte[] search, ref int index)
    {
        index = sourceList.IndexOf(search[0], index);
        if (index < 0)
            return false;
        if (search.Length == 1)
            return true;
        bool isContain = false;
        for (int i = 1; i < search.Length; i++)
        {
            if (sourceList[index + i] != search[i])
            {
                isContain = false;
                break;
            }
            else
                isContain = true;
        }
        if (!isContain)
        {
            index += 1;
            return isContainBytes(sourceList, search, ref index);
        }
        return true;
    }
    #endregion
}


public static class ExtensionMethods
{
    public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
    {
        var tcs = new TaskCompletionSource<object>();
        asyncOp.completed += obj => { tcs.SetResult(null); };
        return ((Task)tcs.Task).GetAwaiter();
    }
}
