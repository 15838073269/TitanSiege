
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class EmojiBuilder : EditorWindow
{

    private string m_inputPath = "Assets/Art/UIRes/Emoji/Input/";
    private string m_outputPath = "Assets/Art/UIRes/Emoji/Output/";
    

    private readonly Vector2[] AtlasSize = new Vector2[]{
        new Vector2(32,32),
        new Vector2(64,64),
        new Vector2(128,128),
        new Vector2(256,256),
        new Vector2(512,512),
        new Vector2(1024,1024),
        new Vector2(2048,2048)
    };

    struct EmojiInfo
    {
        public string key;
        public string x;
        public string y;
        public string size;
    }

    /// <summary>
    /// 单个表情图片的尺寸
    /// </summary>
    private int m_emojiSize = 128;


    [MenuItem("EmojiText/Emoji Build Editor")]
    public static void OpenEmojiBuildEditor()
    {
        var win = GetWindow<EmojiBuilder>();
        win.Show();
    }

    private void OnGUI()
    {
        
        m_inputPath = EditorGUILayout.TextField("表情散图存放路径", m_inputPath);
        m_outputPath = EditorGUILayout.TextField("表情图集生成路径", m_outputPath);
        GUILayout.Label("注意：每个表情图片尺寸需要统一，\n图片命名规范：[纯字母.png] 或 [纯字母_数字.png]，例：a.png, b_0.png，b_1.png");
        m_emojiSize = EditorGUILayout.IntField("单个表情图尺寸", m_emojiSize);
        if(GUILayout.Button("生成图集"))
        {
            GenerateEmojiAtlas();
        }
    }



    /// <summary>
    /// 生成表情图集
    /// </summary>
    public void GenerateEmojiAtlas()
    {
        Dictionary<string, int> sourceDic = new Dictionary<string, int>();
        string[] files = Directory.GetFiles(Application.dataPath.Replace("Assets", "") + m_inputPath, "*.png");
        for (int i = 0; i < files.Length; i++)
        {
            string filename = Path.GetFileName(files[i]);
            filename = filename.Replace(Path.GetExtension(filename), "");
            string id = filename.Split('_')[0];
            Debug.Log("id: " + id);
            if (sourceDic.ContainsKey(id))
            {
                sourceDic[id]++;
            }
            else
            {
                sourceDic.Add(id, 1);
            }
        }

        if (!Directory.Exists(m_outputPath))
        {
            Directory.CreateDirectory(m_outputPath);
        }

        Dictionary<string, EmojiInfo> emojiDic = new Dictionary<string, EmojiInfo>();

        int totalFrames = 0;
        foreach (int value in sourceDic.Values)
        {
            totalFrames += value;
        }
        Vector2 texSize = ComputeAtlasSize(totalFrames);
        Texture2D newTex = new Texture2D((int)texSize.x, (int)texSize.y, TextureFormat.ARGB32, false);
        Texture2D dataTex = new Texture2D((int)texSize.x / m_emojiSize, (int)texSize.y / m_emojiSize, TextureFormat.ARGB32, false);
        int x = 0;
        int y = 0;
        int keyindex = 0;
        foreach (string key in sourceDic.Keys)
        {
            for (int index = 0; index < sourceDic[key]; index++)
            {
                string path =  m_inputPath + key;
                if (sourceDic[key] == 1)
                {
                    path += ".png";
                }
                else
                {
                    path += "_" + index.ToString() + ".png";
                }
                Texture2D asset = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (null == asset) continue;

                Color[] colors = asset.GetPixels(0);

                for (int i = 0; i < m_emojiSize; i++)
                {
                    for (int j = 0; j < m_emojiSize; j++)
                    {
                        newTex.SetPixel(x + i, y + j, colors[i + j * m_emojiSize]);
                    }
                }

                // 帧数，转为二进制
                string t = System.Convert.ToString(sourceDic[key] - 1, 2);

                float r = 0, g = 0, b = 0;
                if (t.Length >= 3)
                {
                    r = t[2] == '1' ? 0.5f : 0;
                    g = t[1] == '1' ? 0.5f : 0;
                    b = t[0] == '1' ? 0.5f : 0;
                }
                else if (t.Length >= 2)
                {
                    r = t[1] == '1' ? 0.5f : 0;
                    g = t[0] == '1' ? 0.5f : 0;
                }
                else
                {
                    r = t[0] == '1' ? 0.5f : 0;
                }

                dataTex.SetPixel(x / m_emojiSize, y / m_emojiSize, new Color(r, g, b, 1));

                if (!emojiDic.ContainsKey(key))
                {
                    EmojiInfo info;

                    info.key = "[" + keyindex + "]";
                    info.x = (x * 1.0f / texSize.x).ToString();
                    info.y = (y * 1.0f / texSize.y).ToString();
                    info.size = (m_emojiSize * 1.0f / texSize.x).ToString();

                    emojiDic.Add(key, info);
                    keyindex++;
                }

                x += m_emojiSize;
                if (x >= texSize.x)
                {
                    x = 0;
                    y += m_emojiSize;
                }
            }
        }

        byte[] bytes1 = newTex.EncodeToPNG();
        string outputfile1 = m_outputPath + "emoji_tex.png";
        File.WriteAllBytes(outputfile1, bytes1);

        byte[] bytes2 = dataTex.EncodeToPNG();
        string outputfile2 = m_outputPath + "emoji_data.png";
        File.WriteAllBytes(outputfile2, bytes2);

        using (StreamWriter sw = new StreamWriter(m_outputPath + "emoji.txt", false))
        {
            sw.WriteLine("Name\tKey\tFrames\tX\tY\tSize");
            foreach (string key in emojiDic.Keys)
            {
                sw.WriteLine("{" + key + "}\t" + emojiDic[key].key + "\t" + sourceDic[key] + "\t" + emojiDic[key].x + "\t" + emojiDic[key].y + "\t" + emojiDic[key].size);
            }
            sw.Close();
        }

        File.Copy(m_outputPath + "emoji.txt", "Assets/Resources/emoji.txt", true);

        AssetDatabase.Refresh();
        FormatTexture();

        EditorUtility.DisplayDialog("生成成功", "生成表情图集成功!", "确定");
    }

    private Vector2 ComputeAtlasSize(int count)
    {
        long total = count * m_emojiSize * m_emojiSize;
        for (int i = 0; i < AtlasSize.Length; i++)
        {
            if (total <= AtlasSize[i].x * AtlasSize[i].y)
            {
                return AtlasSize[i];
            }
        }
        return Vector2.zero;
    }

    private void FormatTexture()
    {
        TextureImporter emojiTex = AssetImporter.GetAtPath(m_outputPath + "emoji_tex.png") as TextureImporter;
        emojiTex.filterMode = FilterMode.Point;
        emojiTex.mipmapEnabled = false;
        emojiTex.sRGBTexture = true;
        emojiTex.alphaSource = TextureImporterAlphaSource.FromInput;
        emojiTex.textureCompression = TextureImporterCompression.Uncompressed;
        emojiTex.SaveAndReimport();

        TextureImporter emojiData = AssetImporter.GetAtPath(m_outputPath + "emoji_data.png") as TextureImporter;
        emojiData.filterMode = FilterMode.Point;
        emojiData.mipmapEnabled = false;
        emojiData.sRGBTexture = false;
        emojiData.alphaSource = TextureImporterAlphaSource.None;
        emojiData.textureCompression = TextureImporterCompression.Uncompressed;
        emojiData.SaveAndReimport();
    }
}
