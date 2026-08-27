using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using System.Linq;


public class ResourcesTextureOptimization : MonoBehaviour {


	#if UNITY_EDITOR

	public Texture2D[] tex;
	string path;

	[ContextMenu("Select Textures")]
	public void SelectAllTextures()
	{
	//	tempTex = (Texture2D[])Resources.FindObjectsOfTypeAll(typeof(Texture2D)).Cast<Texture2D>().ToArray(); // load all textures
	
		// Folder that contain Texture (that need to be compressed) should be in Resources Folder & name as "myTextures"
		 tex = 	Resources.LoadAll("myTextures", typeof(Texture2D)).Cast<Texture2D>().ToArray();  //correct one

		//ConvertAll ();




	}

	[ContextMenu("Convert Textures")]
	public void ConvertAllTextures()
	{
		//if (tex)
		for(int i = 0; i < tex.Length; i++)
		{
			if (tex [i] == null)
				continue;
			
			string texPath = AssetDatabase.GetAssetPath(tex[i]);
			TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texPath);
			importer.crunchedCompression = false;
			importer.textureCompression = TextureImporterCompression.Uncompressed;
			importer.isReadable = true;
			importer.SaveAndReimport();

			int width = tex[i].width;
			int height = tex[i].height;

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

			for (int x = 0; x < tex[i].width; x++)
			{
				for (int y = 0; y < tex[i].height; y++)
				{
					Color col = tex[i].GetPixel(x, y);
					newTex.SetPixel(x, y, col);
				}
			}

			newTex.Apply();

			string path = AssetDatabase.GetAssetPath(tex[i]);
			string absolutePath = Application.dataPath + "/" + path.Replace("Assets/", "");
			System.IO.File.WriteAllBytes(absolutePath, newTex.EncodeToPNG());

			//importer.crunchedCompression = true;
			//importer.compressionQuality = 80;
			//importer.textureCompression = TextureImporterCompression.Compressed;
			importer.isReadable = false;
			importer.SaveAndReimport();

			AssetDatabase.Refresh();
		}


	}

	[ContextMenu("Dual work")]
	public void DualWork()
	{
		SelectAllTextures ();
		ConvertAllTextures ();
	}
    /*
	[ContextMenuItem("Randomize Name", "Randomize")]
	public string Name;

	private void Randomize()
	{
		Name = "Some Random Name";
		SelectAllTextures ();
	}
	*/



    [ContextMenu("Convert POT")]
    public void ConvertPot()
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

            width = Mathf.NextPowerOfTwo(width);

            height = Mathf.NextPowerOfTwo(height);

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

           // importer.crunchedCompression = true;
           // importer.compressionQuality = 80;
          //  importer.textureCompression = TextureImporterCompression.Compressed;
            importer.isReadable = false;
            importer.SaveAndReimport();

            AssetDatabase.Refresh();
        }
        else
        {
            Debug.Log("Must have a image selected");
        }
    }


    [ContextMenu("POT 512")]
    public void ConvertPot512()
    {
        for (int i = 0; i < tex.Length; i++)
        {
            if (tex[i] == null)
                continue;

            string texPath = AssetDatabase.GetAssetPath(tex[i]);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texPath);
            importer.crunchedCompression = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.isReadable = true;
            importer.SaveAndReimport();

            int width = tex[i].width;
            int height = tex[i].height;

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

            Texture2D newTex = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    newTex.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }

            for (int x = 0; x < tex[i].width; x++)
            {
                for (int y = 0; y < tex[i].height; y++)
                {
                    Color col = tex[i].GetPixel(x, y);
                    newTex.SetPixel(x, y, col);
                }
            }

            newTex.Apply();

            string path = AssetDatabase.GetAssetPath(tex[i]);
            string absolutePath = Application.dataPath + "/" + path.Replace("Assets/", "");
            System.IO.File.WriteAllBytes(absolutePath, newTex.EncodeToPNG());

            //importer.crunchedCompression = true;
            //importer.compressionQuality = 80;
            //importer.textureCompression = TextureImporterCompression.Compressed;
            importer.isReadable = false;
            importer.SaveAndReimport();

            AssetDatabase.Refresh();
        }
    }

    [ContextMenu("POT 1024")]
    public void ConvertPot1024()
    {
        for (int i = 0; i < tex.Length; i++)
        {
            if (tex[i] == null)
                continue;

            string texPath = AssetDatabase.GetAssetPath(tex[i]);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texPath);
            importer.crunchedCompression = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.isReadable = true;
            importer.SaveAndReimport();

            int width = tex[i].width;
            int height = tex[i].height;

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

            Texture2D newTex = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    newTex.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }

            for (int x = 0; x < tex[i].width; x++)
            {
                for (int y = 0; y < tex[i].height; y++)
                {
                    Color col = tex[i].GetPixel(x, y);
                    newTex.SetPixel(x, y, col);
                }
            }

            newTex.Apply();

            string path = AssetDatabase.GetAssetPath(tex[i]);
            string absolutePath = Application.dataPath + "/" + path.Replace("Assets/", "");
            System.IO.File.WriteAllBytes(absolutePath, newTex.EncodeToPNG());

            //importer.crunchedCompression = true;
            //importer.compressionQuality = 80;
            //importer.textureCompression = TextureImporterCompression.Compressed;
            importer.isReadable = false;
            importer.SaveAndReimport();

            AssetDatabase.Refresh();
        }
    }

    [ContextMenu("POT 2048")]
    public void ConvertPot2048()
    {
        for (int i = 0; i < tex.Length; i++)
        {
            if (tex[i] == null)
                continue;

            string texPath = AssetDatabase.GetAssetPath(tex[i]);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(texPath);
            importer.crunchedCompression = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.isReadable = true;
            importer.SaveAndReimport();

            int width = tex[i].width;
            int height = tex[i].height;

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

            Texture2D newTex = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    newTex.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }

            for (int x = 0; x < tex[i].width; x++)
            {
                for (int y = 0; y < tex[i].height; y++)
                {
                    Color col = tex[i].GetPixel(x, y);
                    newTex.SetPixel(x, y, col);
                }
            }

            newTex.Apply();

            string path = AssetDatabase.GetAssetPath(tex[i]);
            string absolutePath = Application.dataPath + "/" + path.Replace("Assets/", "");
            System.IO.File.WriteAllBytes(absolutePath, newTex.EncodeToPNG());

            //importer.crunchedCompression = true;
            //importer.compressionQuality = 80;
            //importer.textureCompression = TextureImporterCompression.Compressed;
            importer.isReadable = false;
            importer.SaveAndReimport();

            AssetDatabase.Refresh();
        }
    }

#endif

}
