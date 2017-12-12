// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/MirrorShaderRenderTexture"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ReflectionY("ReflectionY", Range(0,1)) = 0.5
	}
	SubShader
	{
		ZWrite On Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }

		LOD 100

		Pass {
			Name "MirrorEffect"
		    CGPROGRAM

		    #pragma vertex vert             
		    #pragma fragment frag
		    #include "UnityCG.cginc"

		    sampler2D  _Mirror;
		    sampler2D _GrabTexture;
		    fixed4 _Color;
		    float _ReflectionY;

		    struct vertInput {
		        float4 pos : POSITION;
		    };  

		    struct vertOutput {
		        float4 pos : SV_POSITION;
		        float4 uvgrab : TEXCOORD1;
		    };

		    vertOutput vert(vertInput input) {
		    	float time = _Time[1];
		        vertOutput o;
		        o.pos = UnityObjectToClipPos(input.pos);
		        o.uvgrab = ComputeGrabScreenPos(o.pos);
		        return o;
		    }


		    half4 frag(vertOutput output) : COLOR {
		    	float2 uv = output.uvgrab;
		    	uv = float2(uv.x,_ReflectionY-uv.y);
		    	float4 col = tex2D(_Mirror, uv.xy);
		    	if (col[3]>0.5) {
		    		return fixed4(col.rgb,output.uvgrab.y*1.6-0.2);
		    	} else {
		    		return fixed4(0,0,0,0);
		    	}
				
		    }
		    ENDCG
		}
	}
}
