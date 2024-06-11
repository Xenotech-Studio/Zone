Shader "PortalsFX2/Blended"
{
	Properties
	{
		[HDR]_TintColor("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_AlphaPow("Alpha Pow", Float) = 1
		_AlphaMul("Alpha Mul", Float) = 1
		_InvFade("Soft Fade", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 color : COLOR0;
				UNITY_VERTEX_OUTPUT_STEREO
#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD2;
#endif
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _TintColor;
			float _AlphaPow;
			float _AlphaMul;
			float _InvFade;
			UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

			v2f vert (appdata v)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//UNITY_TRANSFER_FOG(pixelInput, pixelInput.vertex);
				o.color = v.color;
#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos(o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
#endif
				UNITY_TRANSFER_FOG(o, v.vertex);
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half fade = 1;

#ifdef SOFTPARTICLES_ON
			float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
			float partZ = i.projPos.z;
			fade = saturate(_InvFade * (sceneZ - partZ));
			i.color.a *= fade;
#endif
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _TintColor * i.color;
				col.a = saturate(pow(col.a, _AlphaPow) * _AlphaMul * fade);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				
				return col;
			}
			ENDCG
		}
	}
}
