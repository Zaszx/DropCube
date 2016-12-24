Shader "Drop/Cube" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Normal ("Normal", Vector) = (0, 1, 0, 0)


	}
	SubShader 
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200

		
		// Extra pass to write to depth buffer
		Pass
		{
			ZWrite On
			ColorMask 0
		}
		

		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off
		ZWrite Off
		ZTest LEqual

		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		fixed4 _Color;
		uniform float4 _Normal;

		struct Input 
		{
			float2 uv_MainTex;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = _Color.rgb;
			o.Alpha = 1.0;

			// Borders
			if(IN.uv_MainTex.x < 0.05 || IN.uv_MainTex.x > 0.95 || IN.uv_MainTex.y < 0.05 || IN.uv_MainTex.y > 0.95)
			{
				o.Albedo = float3(0,0,0);
			}
		}
		ENDCG
	} 
	FallBack "Legacy Shaders/Diffuse"
}
