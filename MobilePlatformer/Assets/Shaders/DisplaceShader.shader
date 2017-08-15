// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/DisplaceShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DisplaceTex ("Texture", 2D) = "white" {}
		_Magnitude("Magnitude", Range(0,0.1)) = 1
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		//ZWrite On Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha
		//Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }

		//LOD 100

		Pass {
		    CGPROGRAM

		    #pragma vertex vert             
		    #pragma fragment frag
		    #include "UnityCG.cginc"

		    sampler2D _MainTex;
		    sampler2D _DisplaceTex;
		    float _Magnitude;

		    struct vertInput {
		        float4 pos : POSITION;
		        float2 uv : TEXCOORD0;
		    };

		    struct vertOutput {
		        float4 pos : SV_POSITION;
		        float2 uv : TEXCOORD0;
		    };

		    vertOutput vert(vertInput input) {
		        vertOutput o;
		        o.pos = UnityObjectToClipPos(input.pos);
		        o.uv = input.uv;
		        return o;
		    }

		    half4 frag(vertOutput output) : COLOR {
		    	float2 disp = tex2D(_DisplaceTex, output.uv +float2 (_Time.x, -_Time.x)).xy;
		    	disp = ((disp*2)-1)*_Magnitude;

		    	float4 col = tex2D(_MainTex, output.uv + disp);
		    	return col;
		    }
		    ENDCG
		}
	}
}
