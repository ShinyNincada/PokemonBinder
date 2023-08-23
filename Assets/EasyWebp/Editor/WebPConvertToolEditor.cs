using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class WebPConvertToolEditor : EditorWindow
{
    private static WebPConvertToolEditor window;

    private static string tempConvertDir;

    private static string inputVideoPath;
    private static string outputVideoPath;

    private int framerate = 30;
    private bool lossless = true;

    private bool userOriginalSize = true;
    private int sWidth = 1280;
    private int sHeight = 720;

    [MenuItem("Window/WebP Converter")]
    static void Init()
    {
        window = (WebPConvertToolEditor)EditorWindow.GetWindow(typeof(WebPConvertToolEditor));
        window.titleContent = new GUIContent("WebP Converter");
        window.Show();

        outputVideoPath = Application.streamingAssetsPath;

        tempConvertDir = Application.streamingAssetsPath + "/TempConvert";
    }


    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        inputVideoPath = EditorGUILayout.TextField("Input Video Path :", inputVideoPath);
        if (GUILayout.Button("Load Video"))
        {
            string path = EditorUtility.OpenFilePanel("Select the video file", "", "mov");
            if (path.Length != 0)
            {
                inputVideoPath = path;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        outputVideoPath = EditorGUILayout.TextField("Output Video Path :", outputVideoPath);
        if (GUILayout.Button("Save Path"))
        {
            string path = EditorUtility.OpenFolderPanel("Save Path", "", "");
            if (path.Length != 0)
            {
                outputVideoPath = path;
            }
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("framerate");
        framerate = EditorGUILayout.IntSlider(framerate, 1, 120);
        EditorGUILayout.EndHorizontal();

        lossless = EditorGUILayout.Toggle("lossless", lossless);

        userOriginalSize = EditorGUILayout.Toggle("User Original Size", userOriginalSize);
        if (!userOriginalSize)
        {
            sWidth = EditorGUILayout.IntField("Width", sWidth);
            sHeight = EditorGUILayout.IntField("Height", sHeight);
        }

        if (GUILayout.Button("Convert"))
        {
            ConverterVideo(inputVideoPath, outputVideoPath);
        }
    }

    void ConverterVideo(string inputVideoPath, string outputVideoPath)
    {
        if (!Directory.Exists(tempConvertDir))
        {
            Directory.CreateDirectory(tempConvertDir);
        }

        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputVideoPath);

        inputVideoPath = "\"" + inputVideoPath + "\"";
        outputVideoPath = "\"" + outputVideoPath + "\"";

        Process p = new Process();
        p.StartInfo.FileName = Application.streamingAssetsPath + "/ConvertTools/ffmpeg.dll";

        //ffmpeg -i Element-16_c4dsky_.mov -f image2 tt/image-%3d.png
        string args = "-i " + inputVideoPath + " -f image2 " + tempConvertDir + "/image-%3d.png";

        p.StartInfo.Arguments = args;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.CreateNoWindow = true;
        //p.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
        p.ErrorDataReceived += new DataReceivedEventHandler(Output);
        p.Start();
        p.BeginErrorReadLine();
        p.WaitForExit();
        p.Close();
        p.Dispose();

        fileNameWithoutExtension = "\"" + fileNameWithoutExtension + "\"";

        ConverterEmageToWebP(outputVideoPath, fileNameWithoutExtension);
    }

    void ConverterEmageToWebP(string inputVideoPath, string saveName)
    {
        //建立外部调用进程
        Process p = new Process();
        p.StartInfo.FileName = Application.streamingAssetsPath + "/ConvertTools/ffmpeg.dll";

        string args;
        //ffmpeg -f image2 -framerate 30 -i tt/image-%3d.png -loop 0 -y loading.webp
        //string args = "-f image2 -framerate " + framerate + " -i " + tempConvertDir + "/image-%3d.png -loop 0  -lossless " + (lossless == true ? 0 : 1) + (userOriginalSize == true ? " -s " + sWidth + "x" + sHeight : "") + " -y " + inputVideoPath + "/" + saveName + ".webp";
        if (!userOriginalSize)
            args = "-f image2 -framerate " + framerate + " -i " + tempConvertDir + "/image-%3d.png -loop 0  -lossless " + (lossless == true ? 0 : 1) + (" -s " + sWidth + "x" + sHeight) + " -y " + inputVideoPath + "/" + saveName + ".webp";
        else
            args = "-f image2 -framerate " + framerate + " -i " + tempConvertDir + "/image-%3d.png -loop 0  -lossless " + (lossless == true ? 0 : 1) + " -y " + inputVideoPath + "/" + saveName + ".webp";
   
        p.StartInfo.Arguments = args;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.CreateNoWindow = true;
        //p.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
        p.ErrorDataReceived += new DataReceivedEventHandler(Output);
        p.Start();
        p.BeginErrorReadLine();
        p.WaitForExit();
        p.Close();
        p.Dispose();

        if (Directory.Exists(tempConvertDir))
        {
            DirectoryInfo di = new DirectoryInfo(tempConvertDir);
            di.Delete(true);
        }
    }


    private void Output(object sendProcess, DataReceivedEventArgs output)
    {
        if (!string.IsNullOrEmpty(output.Data))
        {
            UnityEngine.Debug.Log(output.Data);
        }
    }

}