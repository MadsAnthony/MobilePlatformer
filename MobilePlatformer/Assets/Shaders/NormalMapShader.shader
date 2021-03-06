﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/NormalMapShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NormalMap ("_NormalMap", 2D) = "white" {}
		_BumpMap ("_BumpMap", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Blend One One
		//ZWrite On Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "LightMode"="ForwardBase" }

		//LOD 100

		Pass {
		    CGPROGRAM

		    #pragma vertex vert             
		    #pragma fragment frag
		    #include "UnityCG.cginc"

		    sampler2D _MainTex;
		    sampler2D _NormalMap;
		    sampler2D _BumpMap;
		    fixed4 _Color;
		    float4 _MainTex_TexelSize;
		    fixed4 _LightColor0;

		    struct vertInput {
		        float4 pos : POSITION;
		        float2 uv : TEXCOORD0;
		    };

		    struct vertOutput {
		        float4 pos : SV_POSITION;
		        float2 uv : TEXCOORD0;
		        float3 lightDir : NORMAL;
		    };

		    float4 _MainTex_ST;

		    vertOutput vert(vertInput input) {
		        vertOutput o;
		        o.pos = UnityObjectToClipPos(input.pos);
		        o.uv = TRANSFORM_TEX(input.uv,_MainTex);

		        float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
		        o.lightDir = float3(lightDirection.x,-lightDirection.y,-lightDirection.z);

		        return o;
		    }

		    half4 frag(vertOutput output) : COLOR {
		   		float4 col = tex2D(_MainTex, output.uv);
		    	float4 normalV = tex2D(_NormalMap, output.uv);
		    	float4 bump = tex2D(_BumpMap, output.uv);


		    	float3 diffuseReflection = max(0.0, dot(normalV, output.lightDir));
		    	return col+float4(diffuseReflection, 1.0);
		    	//return col+float4(output.lightDir.x,output.lightDir.y,output.lightDir.z,0);
		    }
		    ENDCG
		}
	}
}
