Shader "Mobile/DiffuseX"
{
    Properties
    {
        _CustomColor("Color",COLOR) = (0.5,0.5,0.5,1.0)
        _MainTex("Base (RGB)", 2D) = "white" {}
    }

        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 150
        CGPROGRAM
        #pragma surface surf Lambert noforwardadd

        sampler2D _MainTex;
        fixed4 _CustomColor;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _CustomColor;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
        Fallback "Mobile/VertexLit"
}