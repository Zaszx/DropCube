Shader "Drop/Cube" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Normal ("Normal", Vector) = (0, 1, 0, 0)
		_SideColor ("Side Color", Color) = (1,1,1,1)
		
		// x: highlighted
		// y: selected
		_Argument ("Argument", Vector) = (0, 0, 0, 0)

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
		fixed4 _SideColor;
		uniform float4 _Normal;
		uniform float4 _Argument;

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

			//o.Emission = o.Albedo * _Argument.x * (sin(_Time.w) * 0.2 + 0.2) + o.Albedo * _Argument.y;

			// Borders
			if(IN.uv_MainTex.x < 0.05 || IN.uv_MainTex.x > 0.95 || IN.uv_MainTex.y < 0.05 || IN.uv_MainTex.y > 0.95)
			{
				o.Albedo = fixed3(96.0 / 255.0, 124.0 / 225.0, 120.0 / 255.0) * 0.6;
			}

			if (o.Albedo.x > 0.95 && o.Albedo.y > 0.95 && o.Albedo.z > 0.95)
			{
				o.Emission = o.Albedo;
			}
			else
			{
				o.Emission = float3(0, 0, 0);
				o.Albedo = o.Albedo * 0.8;
			}
		}
		ENDCG
	} 
	FallBack "Legacy Shaders/Diffuse"
}
