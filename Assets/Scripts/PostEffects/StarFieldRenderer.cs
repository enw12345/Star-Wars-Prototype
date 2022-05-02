using System;
using UnityEngine;

namespace PostEffects
{
    [ExecuteInEditMode]
    public class StarFieldRenderer : MonoBehaviour
    {
        [SerializeField] private Material StarFieldMaterial;
        [Range(0, 0.1f)] [SerializeField] private float StarFieldDensity;
        [SerializeField] private float StarFieldBrightness;
        [SerializeField] private Vector3 StarFieldCellSize;

        private int _starFieldDensityProperty;
        private int _starFieldBrightnessProperty;
        private int _starFieldCellSizeProperty;
        
        private void Awake()
        {
            _starFieldDensityProperty = Shader.PropertyToID("_StarFieldDensity");
            _starFieldBrightnessProperty = Shader.PropertyToID("_StarFieldBrightness");
            _starFieldCellSizeProperty = Shader.PropertyToID("_StarFieldCellSize");
            
            Shader.SetGlobalFloat(_starFieldDensityProperty, StarFieldDensity);
            Shader.SetGlobalFloat(_starFieldBrightnessProperty, StarFieldBrightness);
            Shader.SetGlobalVector(_starFieldCellSizeProperty, StarFieldCellSize);
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            TestShaderProperties();
            Graphics.Blit(src, dest, StarFieldMaterial);
        }

        private void TestShaderProperties()
        {
            _starFieldDensityProperty = Shader.PropertyToID("_StarFieldDensity");
            _starFieldBrightnessProperty = Shader.PropertyToID("_StarFieldBrightness");
            _starFieldCellSizeProperty = Shader.PropertyToID("_StarFieldCellSize");
            
            Shader.SetGlobalFloat(_starFieldDensityProperty, StarFieldDensity);
            Shader.SetGlobalFloat(_starFieldBrightnessProperty, StarFieldBrightness);
            Shader.SetGlobalVector(_starFieldCellSizeProperty, StarFieldCellSize);
        }
        
    }
}