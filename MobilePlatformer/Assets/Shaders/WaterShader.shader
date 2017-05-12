Shader "Custom/WaterShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		ZWrite On Lighting Off Cull Off Fog { Mode Off } Blend One Zero
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }

		GrabPass { "_GrabTexture" }

		LOD 100

		Pass {
			Name "WaterEffect"
		    CGPROGRAM

		    #pragma vertex vert             
		    #pragma fragment frag
		    #include "UnityCG.cginc"

		    sampler2D _GrabTexture;
		    fixed4 _Color;

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
		        o.pos = mul(UNITY_MATRIX_MVP, input.pos);
		        o.uvgrab = ComputeGrabScreenPos(o.pos);
		        if (o.pos.y>-0.4) {
		        	o.pos.y -= sin(o.pos.x*10+time*5)*0.01+0.01;
		        	o.uvgrab.y -= sin(o.pos.x*10+time*5)*0.005;
		        }
		        return o;
		    }


		    half4 frag(vertOutput output) : COLOR {
		    	//fixed alpha_current = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(output.uvgrab)).a;
		    	//fixed alpha_up = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(output.uvgrab+fixed4(0,-0.005,0,0))).a;
		   		//if (ceil(alpha_current) != 0 && ceil(alpha_up)==0) {
		   		//	return fixed4(0,0,0,1);
		   		//}
		    	fixed4 col = _Color+_Color*tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(output.uvgrab));
				return col;
		    }
		    ENDCG
		}

		GrabPass { "_GrabTexture2" }

		Pass {
			Name "Outline"
		    CGPROGRAM

		    #pragma vertex vert             
		    #pragma fragment frag
		    #include "UnityCG.cginc"

		    sampler2D _GrabTexture2;
		    fixed4 _Color;

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
		        o.pos = mul(UNITY_MATRIX_MVP, input.pos);
		        o.uvgrab = ComputeGrabScreenPos(o.pos);
//		        if (o.pos.y>-0.4) {
//		        	o.pos.y -= sin(o.pos.x*10+time*5)*0.01+0.01;
//		        	o.uvgrab.y -= sin(o.pos.x*10+time*5)*0.005;
//		        }
		        return o;
		    }


		    half4 frag(vertOutput output) : COLOR {
		    	fixed alpha_current = tex2Dproj( _GrabTexture2, UNITY_PROJ_COORD(output.uvgrab)).a;
		    	fixed alpha_up = tex2Dproj( _GrabTexture2, UNITY_PROJ_COORD(output.uvgrab+fixed4(0,-0.005,0,0))).a;
		   		if (ceil(alpha_current) != 0 && ceil(alpha_up)==0) {
		   			return 2*_Color;
		   		}
		    	fixed4 col = tex2Dproj( _GrabTexture2, UNITY_PROJ_COORD(output.uvgrab));
				return col;
		    }
		    ENDCG
		}
	}
}
