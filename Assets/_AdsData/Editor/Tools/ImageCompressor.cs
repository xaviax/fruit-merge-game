using UnityEngine;
using UnityEditor;

public class ImageCompressor : Editor
{
    [MenuItem("Finz/Texture Compression/Multiple of Four #&p")]
    public static void Convert()
    {
        Texture2D tex = Selection.activeObject as Texture2D;
        if (tex)
        {
            string texPath = AssetDatabase.GetAssetPath(tex);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texPath);
            importer.crunchedCompression = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.isReadable = true;
            importer.SaveAndReimport();

            int width = tex.width;
            int height = tex.height;

            while (width % 4 != 0)
            {
                width++;
            }

            while (height % 4 != 0)
            {
                height++;
            }

            Texture2D newTex = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    newTex.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }

            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    Color col = tex.GetPixel(x, y);
                    newTex.SetPixel(x, y, col);
                }
            }

            newTex.Apply();

            string path = AssetDatabase.GetAssetPath(tex);
            string absolutePath = Application.dataPath + "/" + path.Replace("Assets/", "");
            System.IO.File.WriteAllBytes(absolutePath, newTex.EncodeToPNG());

            importer.crunchedCompression = true;
            importer.compressionQuality = 80;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.isReadable = false;
            importer.SaveAndReimport();

            AssetDatabase.Refresh();
        }
        else
        {
            Debug.Log("Must have a image selected");
        }
    }


	[MenuItem("Finz/Texture Compression/POT #&o")]
	public static void ConvertPot()
	{
		Texture2D tex = Selection.activeObject as Texture2D;
		if (tex)
		{
			string texPath = AssetDatabase.GetAssetPath(tex);
			TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texPath);
			importer.crunchedCompression = false;
			importer.textureCompression = TextureImporterCompression.Uncompressed;
			importer.isReadable = true;
			importer.SaveAndReimport();

			int width = tex.width;
			int height = tex.height;

			// for making it into multiple of four

			/*
			while (width % 2 != 0)
			{
				width++;
			}

			while (height % 2 != 0)
			{
				height++;
			}

			*/

			// for making it into POT Texture

			width = Mathf.NextPowerOfTwo (width);

			height = Mathf.NextPowerOfTwo (height);

			//

			Texture2D newTex = new Texture2D(width, height);

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					newTex.SetPixel(x, y, new Color(0, 0, 0, 0));
				}
			}

			for (int x = 0; x < tex.width; x++)
			{
				for (int y = 0; y < tex.height; y++)
				{
					Color col = tex.GetPixel(x, y);
					newTex.SetPixel(x, y, col);
				}
			}

			newTex.Apply();

			string path = AssetDatabase.GetAssetPath(tex);
			string absolutePath = Application.dataPath + "/" + path.Replace("Assets/", "");
			System.IO.File.WriteAllBytes(absolutePath, newTex.EncodeToPNG());

			importer.crunchedCompression = true;
			importer.compressionQuality = 80;
			importer.textureCompression = TextureImporterCompression.Compressed;
			importer.isReadable = false;
			importer.SaveAndReimport();

			AssetDatabase.Refresh();
		}
		else
		{
			Debug.Log("Must have a image selected");
		}
	}


	[MenuItem("Finz/Texture Compression/POT_512 #&2")]
	public static void ConvertPot512()
	{
		Texture2D tex = Selection.activeObject as Texture2D;
		if (tex)
		{
			string texPath = AssetDatabase.GetAssetPath(tex);
			TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texPath);
			importer.crunchedCompression = false;
			importer.textureCompression = TextureImporterCompression.Uncompressed;
			importer.isReadable = true;
			importer.SaveAndReimport();

			int width = tex.width;
			int height = tex.height;

			// for making it into multiple of four

			/*
			while (width % 2 != 0)
			{
				width++;
			}

			while (height % 2 != 0)
			{
				height++;
			}

			*/

			// for making it into POT Texture

			width = 512;

			height = 512;

			//

			Texture2D newTex = new Texture2D(width, height);

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					newTex.SetPixel(x, y, new Color(0, 0, 0, 0));
				}
			}

			for (int x = 0; x < tex.width; x++)
			{
				for (int y = 0; y < tex.height; y++)
				{
					Color col = tex.GetPixel(x, y);
					newTex.SetPixel(x, y, col);
				}
			}

			newTex.Apply();

			string path = AssetDatabase.GetAssetPath(tex);
			string absolutePath = Application.dataPath + "/" + path.Replace("Assets/", "");
			System.IO.File.WriteAllBytes(absolutePath, newTex.EncodeToPNG());

			importer.crunchedCompression = true;
			importer.compressionQuality = 80;
			importer.textureCompression = TextureImporterCompression.Compressed;
			importer.isReadable = false;
			importer.SaveAndReimport();

			AssetDatabase.Refresh();
		}
		else
		{
			Debug.Log("Must have a image selected");
		}
	}

	[MenuItem("Finz/Texture Compression/POT_1024 #&2")]
	public static void ConvertPot1024()
	{
		Texture2D tex = Selection.activeObject as Texture2D;
		if (tex)
		{
			string texPath = AssetDatabase.GetAssetPath(tex);
			TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texPath);
			importer.crunchedCompression = false;
			importer.textureCompression = TextureImporterCompression.Uncompressed;
			importer.isReadable = true;
			importer.SaveAndReimport();

			int width = tex.width;
			int height = tex.height;

			// for making it into multiple of four

			/*
			while (width % 2 != 0)
			{
				width++;
			}

			while (height % 2 != 0)
			{
				height++;
			}

			*/

			// for making it into POT Texture

			width = 1024;

			height = 1024;

			//

			Texture2D newTex = new Texture2D(width, height);

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					newTex.SetPixel(x, y, new Color(0, 0, 0, 0));
				}
			}

			for (int x = 0; x < tex.width; x++)
			{
				for (int y = 0; y < tex.height; y++)
				{
					Color col = tex.GetPixel(x, y);
					newTex.SetPixel(x, y, col);
				}
			}

			newTex.Apply();

			string path = AssetDatabase.GetAssetPath(tex);
			string absolutePath = Application.dataPath + "/" + path.Replace("Assets/", "");
			System.IO.File.WriteAllBytes(absolutePath, newTex.EncodeToPNG());

			importer.crunchedCompression = true;
			importer.compressionQuality = 80;
			importer.textureCompression = TextureImporterCompression.Compressed;
			importer.isReadable = false;
			importer.SaveAndReimport();

			AssetDatabase.Refresh();
		}
		else
		{
			Debug.Log("Must have a image selected");
		}
	}

	[MenuItem("Finz/Texture Compression/POT_2048 #&2")]
	public static void ConvertPot2048()
	{
		Texture2D tex = Selection.activeObject as Texture2D;
		if (tex)
		{
			string texPath = AssetDatabase.GetAssetPath(tex);
			TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texPath);
			importer.crunchedCompression = false;
			importer.textureCompression = TextureImporterCompression.Uncompressed;
			importer.isReadable = true;
			importer.SaveAndReimport();

			int width = tex.width;
			int height = tex.height;

			// for making it into multiple of four

			/*
			while (width % 2 != 0)
			{
				width++;
			}

			while (height % 2 != 0)
			{
				height++;
			}

			*/

			// for making it into POT Texture

			width = 2048;

			height = 2048;

			//

			Texture2D newTex = new Texture2D(width, height);

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					newTex.SetPixel(x, y, new Color(0, 0, 0, 0));
				}
			}

			for (int x = 0; x < tex.width; x++)
			{
				for (int y = 0; y < tex.height; y++)
				{
					Color col = tex.GetPixel(x, y);
					newTex.SetPixel(x, y, col);
				}
			}

			newTex.Apply();

			string path = AssetDatabase.GetAssetPath(tex);
			string absolutePath = Application.dataPath + "/" + path.Replace("Assets/", "");
			System.IO.File.WriteAllBytes(absolutePath, newTex.EncodeToPNG());

			importer.crunchedCompression = true;
			importer.compressionQuality = 80;
			importer.textureCompression = TextureImporterCompression.Compressed;
			importer.isReadable = false;
			importer.SaveAndReimport();

			AssetDatabase.Refresh();
		}
		else
		{
			Debug.Log("Must have a image selected");
		}
	}


	//These are the supported keys (can also be combined together):
	//
	//% – CTRL on Windows / CMD on OSX
	//
	//# – Shift
	//
	//& – Alt
}