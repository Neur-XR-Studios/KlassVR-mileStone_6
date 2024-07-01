Shader "Unlit/Pano360Shader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,0.5)
    }
    SubShader 
    {
        Tags { "RenderType" = "Opaque" }
        Cull Front // This ensures the sphere is rendered correctly from inside
        CGPROGRAM
        #pragma surface surf SimpleLambert // Surface shader with Lambert lighting model
        half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten)
        {
            half4 c;
            c.rgb = s.Albedo;
            return c;
        }
      
        sampler2D _MainTex;
        struct Input
        {
            float2 uv_MainTex;
            float4 myColor : COLOR;
        };
 
        fixed3 _Color;
        void surf (Input IN, inout SurfaceOutput o)
        {
            // Mirror the image horizontally to correct for texture coordinates
            IN.uv_MainTex.x = 1 - IN.uv_MainTex.x;
            // Sample the texture and apply the color
            fixed3 result = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = result.rgb; // Set surface color
            o.Alpha = 1; // Set alpha value
        }
        ENDCG
    }
    Fallback "Diffuse" // Fallback shader if this one fails to compile
}
