Shader "Custom/BlurShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
		    float4 _MainTex_TexelSize;

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
		        o.pos = mul(UNITY_MATRIX_MVP, input.pos);
		        o.uv = input.uv;
		        return o;
		    }

		    float4 box(sampler2D tex, float2 uv, float4 size) {
		    	float4 c = tex2D(tex, uv + float2(-size.x,size.y))+tex2D(tex, uv + float2(0,size.y))+tex2D(tex, uv + float2(size.x,size.y)) +
		    			   tex2D(tex, uv + float2(-size.x,0))+tex2D(tex, uv + float2(0,0))+tex2D(tex, uv + float2(size.x,0)) +
		    			   tex2D(tex, uv + float2(-size.x,-size.y))+tex2D(tex, uv + float2(0,-size.y))+tex2D(tex, uv + float2(size.x,-size.y));
		    	return c/9;
		    }

		    half4 frag(vertOutput output) : COLOR {
		    	float4 col = box(_MainTex,output.uv,_MainTex_TexelSize);
		    	return col;
		    }
		    ENDCG
		}
	}
}
