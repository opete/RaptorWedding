// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Brightness" {

	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_BrightnessAmount("Brightness Amount", Range(0.0, 10.0)) = 1.0
		_SaturationAmount("Saturation Amount", Range(0.0, 2.0)) = 1.0
		_ContrastAmount("Contrast Amount", Range(0.0, 100.0)) = 1.0
	    _CutOff("Cut off", float) = 0.1
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			// declare corresponding CGPROGRAM variables
			uniform sampler2D _MainTex;
			fixed _BrightnessAmount;
			fixed _SaturationAmount;
			fixed _ContrastAmount;
			uniform float _CutOff;

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
			};
			float4 _MainTex_ST;

			float3 BrightnessSaturationContrast(float3 color, float brightness, float saturation, float contrast)
			{
				// adjust these values to adjust R, G, B colors separately
				float avgLumR = 0.5;
				float avgLumG = 0.5;
				float avgLumB = 0.5;

				// luminance coefficient for getting luminance from the image
				float3 luminanceCoeff = float3(0.2125, 0.7154, 0.0721);

				// Brightness calculation
				float3 avgLum = float3(avgLumR, avgLumG, avgLumB);
				float3 brightnessColor = color * brightness;
				float intensityf = dot(brightnessColor, luminanceCoeff);
				float3 intensity = float3(intensityf, intensityf, intensityf);

				// Saturation calculation
				float3 saturationColor = lerp(intensity, brightnessColor, saturation);

				// Contrast calculation
				float3 contrastColor = lerp(avgLum, saturationColor, contrast);

				return contrastColor;
			}

			v2f vert_img(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f_img i) : COLOR
			{
				// Get the colors from the RenderTexture and the UVs from the v2f_img struct
				fixed4 renderTex = tex2D(_MainTex, i.uv);

				// Apply the Brightness, Saturation, and Contrast calculations
				renderTex.rgb = BrightnessSaturationContrast(renderTex.rgb,
															_BrightnessAmount,
															_SaturationAmount,
															_ContrastAmount);
				if (renderTex.a < _CutOff) discard;

				return renderTex;
			}

		ENDCG
		}
	}

}