Shader "Voxel/Procedural Voxel"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Smoothness ("Smoothness", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        CGPROGRAM
        #pragma surface ConfigureSurface Standard fullforwardshadows
        #pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
        #pragma editor_sync_compilation
        #pragma target 4.5

        float4 _BaseColor;
        float _Smoothness;
        
        #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
            StructuredBuffer<float4x4> _Matrices;
        #endif
        void ConfigureProcedural()
        {
            #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                _BaseColor = _Matrices[unity_InstanceID]._m30_m31_m32_m33;
                unity_ObjectToWorld = _Matrices[unity_InstanceID];
                unity_ObjectToWorld._m30_m31_m32_m33 = float4(0, 0, 0, 1);
            #endif
        }


        struct Input
        {
            float3 worldPos;
        };


        void ConfigureSurface(Input input, inout SurfaceOutputStandard surface)
        {
            surface.Albedo = _BaseColor.rgb;
            surface.Smoothness = _Smoothness;
        }
        
        ENDCG
    }
    FallBack "Diffuse"
}
