Shader "Custom/Dissolve"
{
    Properties
    {
		_Color("Color", Color) = (1,1,1,1)
		
		_MainTex("Albedo (RGB)", 2D) = "white" {}

		[Header(Dissolve Properties)]
		_DissolveTex("Dissolve Texture", 2D) = "white" {}
		[HDR]
		_EdgeColor("Edge Color", Color) = (1,1,1,1)
		_EdgeSize("Edge Size",Float) = 0.05
		_DissolveAmount("Dissolve Amount",Range(0,1)) = 0.5
		[Space(25)]
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _DissolveTex;
        struct Input
        {
            float2 uv_MainTex;
        };
		half _DissolveAmount,_EdgeSize;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color,_EdgeColor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {

			half dissolve = tex2D(_DissolveTex, IN.uv_MainTex).r;
			clip(dissolve - _DissolveAmount);

            fixed4 c = _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
			o.Emission = _EdgeColor * step(dissolve - _DissolveAmount, _EdgeSize);
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
