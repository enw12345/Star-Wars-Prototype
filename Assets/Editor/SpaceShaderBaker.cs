using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class SpaceShaderBaker : EditorWindow
{
    private Material ImageMaterial;
    private string FilePath = "Assets";
    private Vector2Int Resolution;

    private bool hasMaterial;
    private bool hasResolution;
    // private bool hasFilePath;

    [MenuItem("Window/Create Space SkyBox Texture")]
    static void OpenWindow()
    {
        SpaceShaderBaker window = EditorWindow.GetWindow<SpaceShaderBaker>();
        window.Show();

        window.CheckInput();
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox("Set the material you want to bake as well as the size " +
                                "and location of the texture you want to bake to, then press the \"Bake\" button. " +
                                "Use a 6 sided cubemap asset.", MessageType.None);

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            ImageMaterial = (Material)EditorGUILayout.ObjectField("Material", ImageMaterial, typeof(Material), false);
            Resolution = EditorGUILayout.Vector2IntField("Image Resolution", Resolution);
            // FilePath = FileField(FilePath);

            if (check.changed)
            {
                CheckInput();
            }
        }

        GUI.enabled = hasMaterial && hasResolution;
        if (GUILayout.Button("Bake"))
        {
            BakeHorizontalCubemap();
        }
        
        GUI.enabled = true;

        if (!hasMaterial)
        {
            EditorGUILayout.HelpBox("You're still missing a material to bake.", MessageType.Warning);
        }
        if (!hasResolution)
        {
            EditorGUILayout.HelpBox("Please set a size bigger than zero.", MessageType.Warning);
        }
    }

    private void BakeHorizontalCubemap()
    {
        //Create a temporary render texture
        RenderTexture renderTexture = RenderTexture.GetTemporary(Resolution.x, Resolution.y);
        //Blit our material to the render texture in order to grab the output of the shader
        Graphics.Blit(null, renderTexture, ImageMaterial);

        //Create a texture2D to save our render texture to
        
        //Set our render texture as the active render texture
        RenderTexture.active = renderTexture;
        renderTexture.wrapMode = TextureWrapMode.Repeat;

        //Read the texture from six different places the height of the texture is 1/3 the size of the map,
        //the width of every image is 1/4 the size of the map
        
        var width = (float)1 / 3 * Resolution.x;
        var height = (float)1 / 4 * Resolution.y;
        
        var size = new Vector2(width, height);

        var posX = new Vector2(width * 2, height);
        var negX = new Vector2(0, height);
        var posY = new Vector2(width, 0);
        var negY = new Vector2(width, height * 2);
        var posZ = new Vector2(width, height);
        var negZ = new Vector2(width, height * 3);
        
        var rectPosX = new Rect(posX, size);
        var rectNegX = new Rect(negX, size);
        var rectPosY = new Rect(posY, size);
        var rectNegY = new Rect(negY, size);
        var rectPosZ = new Rect(posZ, size);
        var rectNegZ = new Rect(negZ, size);

        var newResolutionX = Mathf.FloorToInt(size.x);
        var newResolutionY = Mathf.FloorToInt(size.y);

        Texture2D textureXPos = new Texture2D(newResolutionX, newResolutionY);
        Texture2D textureXNeg = new Texture2D(newResolutionX, newResolutionY);
        Texture2D textureYPos = new Texture2D(newResolutionX, newResolutionY);
        Texture2D textureYNeg = new Texture2D(newResolutionX, newResolutionY);
        Texture2D textureZPos = new Texture2D(newResolutionX, newResolutionY);
        Texture2D textureZNeg = new Texture2D(newResolutionX, newResolutionY);

        textureXPos.ReadPixels(rectPosX,  0, 0);
        textureXNeg.ReadPixels(rectNegX,  0, 0);
        textureYPos.ReadPixels(rectPosY,  0, 0);
        textureYNeg.ReadPixels(rectNegY,  0, 0);
        textureZPos.ReadPixels(rectPosZ,  0, 0);
        textureZNeg.ReadPixels(rectNegZ,  0, 0);

        textureXPos.name = "+X";
        textureXNeg.name = "-X";
        textureYPos.name = "+Y";
        textureYNeg.name = "-Y";
        textureZPos.name = "+Z";
        textureZNeg.name = "-Z";
        
        var textures = new[]
            {textureXPos, textureXNeg, textureYPos, textureYNeg, textureZPos, textureZNeg};

        //Save the image as a png and save it in the asset folder
        for (int i = 0; i < textures.Length; i++)
        {
            byte[] png = textures[i].EncodeToPNG();
            File.WriteAllBytes(FilePath + $"/{textures[i].name}.png", png);
            AssetDatabase.Refresh();
            DestroyImmediate(textures[i]);
        }
        
        //Clean Up
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(renderTexture);
    }

    private void CheckInput()
    {
        //check which values are entered already
        hasMaterial = ImageMaterial != null;
        hasResolution = Resolution.x > 0 && Resolution.y > 0;
    }
}
