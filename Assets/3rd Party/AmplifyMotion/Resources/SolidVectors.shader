// Amplify Motion - Full-scene Motion Blur for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

Shader "Hidden/Amplify Motion/SolidVectors" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.25
	}
	SubShader {
		Blend Off Cull Off Fog { Mode off }
		ZTest LEqual ZWrite On

		CGINCLUDE
			#pragma fragmentoption ARB_precision_hint_fastest
			#if SHADER_API_D3D9 || SHADER_API_D3D11_9X
				#pragma target 3.0
			#endif
			#include "Shared.cginc"

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 pos_prev : TEXCOORD0;
				float4 pos_curr : TEXCOORD1;
				float4 motion : TEXCOORD2;
				float4 screen_pos : TEXCOORD3;
				float2 uv : TEXCOORD4;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Cutoff;

			v2f vert_base( appdata_t v, bool mobile )
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT( v2f, o );

				float4 pos = o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
				float4 pos_prev = mul( _EFLOW_MATRIX_PREV_MVP, v.vertex );
				float4 pos_curr = o.pos;

			#if UNITY_UV_STARTS_AT_TOP
				pos_curr.y = -pos_curr.y;
				pos_prev.y = -pos_prev.y;
				if ( _ProjectionParams.x > 0 )
					pos.y = -pos.y;
			#endif

				if ( mobile )
					o.motion = SolidMotionVector( pos_prev, pos_curr, _EFLOW_OBJECT_ID );					
				else
				{
					o.pos_prev = pos_prev;
					o.pos_curr = pos_curr;
				}

				o.screen_pos = ComputeScreenPos( pos );
				o.uv = TRANSFORM_TEX( v.texcoord, _MainTex );
				return o;
			}

			inline half4 frag_opaque( v2f i, const bool mobile )
			{
				DepthTest( i.screen_pos );
				if ( mobile )
					return i.motion;
				else
					return SolidMotionVector( i.pos_prev, i.pos_curr, _EFLOW_OBJECT_ID );
			}

			inline half4 frag_cutout( v2f i, const bool mobile )
			{
				DepthTest( i.screen_pos );

				clip( tex2D( _MainTex, i.uv ).a - _Cutoff );

				if ( mobile )
					return i.motion;
				else
					return SolidMotionVector( i.pos_prev, i.pos_curr, _EFLOW_OBJECT_ID );
			}
		ENDCG
		
		Pass {
			Name "MOB_OPAQUE"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				v2f vert( appdata_t v ) { return vert_base( v, true ); }
				half4 frag( v2f v ) : SV_Target { return frag_opaque( v, true ); }
			ENDCG
		}
		Pass {
			Name "MOB_CUTOUT"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				v2f vert( appdata_t v ) { return vert_base( v, true ); }
				half4 frag( v2f v ) : SV_Target { return frag_cutout( v, true ); }
			ENDCG
		}
		Pass {
			Name "STD_OPAQUE"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				v2f vert( appdata_t v ) { return vert_base( v, false ); }
				half4 frag( v2f v ) : SV_Target { return frag_opaque( v, false ); }
			ENDCG
		}
		Pass {
			Name "STD_CUTOUT"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				v2f vert( appdata_t v ) { return vert_base( v, false ); }
				half4 frag( v2f v ) : SV_Target { return frag_cutout( v, false ); }
			ENDCG
		}
	}

	FallBack Off
}
