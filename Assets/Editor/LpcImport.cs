#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LPCAutoImporter : EditorWindow
{
    private const string ROOT_PATH = "Assets/NPC_GEN/LPC_Assets";
    private const string SO_SAVE_PATH = "Assets/NPC_GEN/LPC_EquipmentData";
    
    // Set Pixels Per Unit to 16 to match your 16x16 dungeon tiles!
    private const int PIXELS_PER_UNIT = 16; 
    private const int CELL_SIZE = 64;

    [MenuItem("Tools/LPC/1-Click Auto Slice & Generate Equipment (Walk)")]
    public static void AutoSliceAndGenerate()
    {
        if (!Directory.Exists(ROOT_PATH))
        {
            Debug.LogError($"Directory {ROOT_PATH} not found!");
            return;
        }

        if (!Directory.Exists(SO_SAVE_PATH))
        {
            Directory.CreateDirectory(SO_SAVE_PATH);
            AssetDatabase.Refresh();
        }

        string[] filePaths = Directory.GetFiles(ROOT_PATH, "*.png", SearchOption.AllDirectories);
        List<string> walkPaths = new List<string>();

        foreach (string path in filePaths)
        {
            string clean = path.Replace("\\", "/");
            if (clean.ToLower().Contains("walk"))
            {
                walkPaths.Add(clean);
            }
        }

        try
        {
            for (int i = 0; i < walkPaths.Count; i++)
            {
                string unityPath = walkPaths[i];
                EditorUtility.DisplayProgressBar("Slicing & Generating LPC Sprites", Path.GetFileName(unityPath), (float)i / walkPaths.Count);

                // 1. Force PPU, Point Filter, and 64x64 Grid Slices
                SliceTextureGrid(unityPath);

                // 2. Load sliced sprites
                Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(unityPath);
                List<Sprite> sprites = new List<Sprite>();

                foreach (var asset in subAssets)
                {
                    if (asset is Sprite sprite)
                    {
                        sprites.Add(sprite);
                    }
                }

                if (sprites.Count == 0) continue;

                // 3. Create / Update ScriptableObject
                EquipSlot slot = DetermineSlot(unityPath);
                string itemName = GenerateItemName(unityPath);
                string soPath = $"{SO_SAVE_PATH}/{itemName}.asset";

                EquipmentItem item = AssetDatabase.LoadAssetAtPath<EquipmentItem>(soPath);
                if (item == null)
                {
                    item = ScriptableObject.CreateInstance<EquipmentItem>();
                    AssetDatabase.CreateAsset(item, soPath);
                }

                item.itemName = itemName;
                item.slot = slot;
                item.sprites = sprites.ToArray();

                EditorUtility.SetDirty(item);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"<color=green>SUCCESS: Re-sliced with PPU={PIXELS_PER_UNIT} and updated equipment items!</color>");
    }

    private static void SliceTextureGrid(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;

        importer.isReadable = true;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        
        // This is what controls the scale in world space:
        importer.spritePixelsPerUnit = PIXELS_PER_UNIT;

        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (texture == null)
        {
            importer.SaveAndReimport();
            texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        int texWidth = texture.width;
        int texHeight = texture.height;
        int cols = texWidth / CELL_SIZE;
        int rows = texHeight / CELL_SIZE;

        List<SpriteMetaData> metas = new List<SpriteMetaData>();

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                SpriteMetaData meta = new SpriteMetaData
                {
                    rect = new Rect(c * CELL_SIZE, texHeight - (r + 1) * CELL_SIZE, CELL_SIZE, CELL_SIZE),
                    name = $"{Path.GetFileNameWithoutExtension(path)}_{r * cols + c}",
                    alignment = (int)SpriteAlignment.BottomCenter,
                    pivot = new Vector2(0.5f, 0.0f)
                };
                metas.Add(meta);
            }
        }

        importer.spritesheet = metas.ToArray();
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
    }

    private static EquipSlot DetermineSlot(string path)
    {
        string lower = path.ToLower();
        if (lower.Contains("/bodies/")) return EquipSlot.Body;
        if (lower.Contains("/heads/")) return EquipSlot.Head;
        if (lower.Contains("/hair/")) return EquipSlot.Hair;
        if (lower.Contains("/helmets/")) return EquipSlot.Helmet;
        if (lower.Contains("/armor/")) return EquipSlot.Armor;
        if (lower.Contains("/legs/")) return EquipSlot.Pants;
        if (lower.Contains("/feet/")) return EquipSlot.Pants;
        if (lower.Contains("/weapons/")) return EquipSlot.Weapon;
        return EquipSlot.Body;
    }

    private static string GenerateItemName(string path)
    {
        string relative = path.Replace(ROOT_PATH + "/", "").Replace(".png", "");
        return relative.Replace("/", "_");
    }
}
#endif