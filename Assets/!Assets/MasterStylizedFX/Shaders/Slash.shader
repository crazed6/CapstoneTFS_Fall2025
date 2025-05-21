// Made with Amplify Shader Editor v1.9.3.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Slash"
{
	Properties
	{
		[NoScaleOffset]_MainTexture("MainTexture", 2D) = "white" {}
		_MainTiling("MainTiling", Vector) = (0,0,0,0)
		_MainOffset("MainOffset", Vector) = (-0.07,0,0,0)
		_MainScroll("MainScroll", Vector) = (0,0,0,0)
		[Toggle]_LoopMain("LoopMain", Float) = 0
		[NoScaleOffset]_Mask("Mask", 2D) = "white" {}
		_MaskScale("MaskScale", Vector) = (1,1,0,0)
		_MaskOffset("MaskOffset", Range( -1 , 1)) = 0
		_MaskScroll("MaskScroll", Vector) = (0,0,0,0)
		_EdgeSharpness("EdgeSharpness", Range( 0 , 1)) = 0
		_StaticMask("StaticMask", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 uv_texcoord;
			float4 uv2_texcoord2;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _MainTexture;
		uniform float _LoopMain;
		uniform float2 _MainTiling;
		uniform float2 _MainOffset;
		uniform float2 _MainScroll;
		uniform float _EdgeSharpness;
		uniform sampler2D _Mask;
		uniform float2 _MaskScale;
		uniform float2 _MaskScroll;
		uniform float _MaskOffset;
		uniform sampler2D _StaticMask;
		uniform float4 _StaticMask_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uvs_TexCoord2 = i.uv_texcoord;
			uvs_TexCoord2.xy = i.uv_texcoord.xy * _MainTiling + _MainOffset;
			float2 appendResult77 = (float2(pow( uvs_TexCoord2.xy.x , ( 1.0 + i.uv2_texcoord2.xyz.z ) ) , uvs_TexCoord2.xy.y));
			float temp_output_9_0 = ( 1.0 - i.uv_texcoord.z );
			float2 appendResult55 = (float2(( _MainScroll.x * temp_output_9_0 ) , ( temp_output_9_0 * _MainScroll.y )));
			float2 temp_output_6_0 = ( appendResult77 + appendResult55 );
			float2 clampResult8 = clamp( temp_output_6_0 , float2( -99,0 ) , float2( 1,1 ) );
			float4 tex2DNode1 = tex2D( _MainTexture, (( _LoopMain )?( temp_output_6_0 ):( clampResult8 )) );
			o.Emission = ( tex2DNode1 * i.vertexColor * ( i.uv2_texcoord2.y + 1.0 ) ).rgb;
			float2 appendResult43 = (float2(0.0 , _MaskOffset));
			float2 uvs_TexCoord31 = i.uv_texcoord;
			uvs_TexCoord31.xy = i.uv_texcoord.xy * _MaskScale + ( ( _MaskScroll * ( 1.0 - i.uv2_texcoord2.xy.x ) ) + appendResult43 );
			float4 appendResult29 = (float4(uvs_TexCoord31.xy.x , uvs_TexCoord31.xy.y , 0.0 , 0.0));
			float4 tex2DNode10 = tex2D( _Mask, appendResult29.xy );
			float smoothstepResult12 = smoothstep( i.uv_texcoord.w , ( i.uv_texcoord.w + _EdgeSharpness ) , tex2DNode10.r);
			float2 uv_StaticMask = i.uv_texcoord * _StaticMask_ST.xy + _StaticMask_ST.zw;
			o.Alpha = ( tex2DNode1.a * smoothstepResult12 * i.vertexColor.a * tex2D( _StaticMask, uv_StaticMask ).r );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 customPack1 : TEXCOORD1;
				float4 customPack2 : TEXCOORD2;
				float3 worldPos : TEXCOORD3;
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xyzw = customInputData.uv_texcoord;
				o.customPack1.xyzw = v.texcoord;
				o.customPack2.xyzw = customInputData.uv2_texcoord2;
				o.customPack2.xyzw = v.texcoord1;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xyzw;
				surfIN.uv2_texcoord2 = IN.customPack2.xyzw;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.vertexColor = IN.color;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	// CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19302
Node;AmplifyShaderEditor.TextureCoordinatesNode;41;-1503.131,30.57038;Inherit;True;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;44;-898.2465,526.056;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;62;-1129.084,-540.9309;Inherit;False;Property;_MainTiling;MainTiling;1;0;Create;True;0;0;0;False;0;False;0,0;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;63;-1140.584,-357.0073;Inherit;False;Property;_MainOffset;MainOffset;2;0;Create;True;0;0;0;False;0;False;-0.07,0;-0.81,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;80;-726.8883,-946.8317;Inherit;False;Constant;_Float2;Float 2;12;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;78;-786.1381,-858.2084;Inherit;True;1;-1;3;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;9;-1193.788,17.13062;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;58;-1222.901,-140.4532;Inherit;False;Property;_MainScroll;MainScroll;3;0;Create;True;0;0;0;False;0;False;0,0;1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.OneMinusNode;46;-613.5474,565.0714;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-774.9366,751.4554;Inherit;False;Property;_MaskOffset;MaskOffset;7;0;Create;True;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;64;-593.561,399.1113;Inherit;False;Property;_MaskScroll;MaskScroll;8;0;Create;True;0;0;0;False;0;False;0,0;1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-643.1315,-422.6316;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;79;-504.4402,-815.1527;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-925.664,188.0646;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-956.6622,-106.3203;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-351.886,475.1554;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;43;-323.2667,649.3185;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;75;-390.0539,-581.8015;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;55;-723.2347,29.27092;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;26;-297.0805,893.0111;Inherit;False;Property;_MaskScale;MaskScale;6;0;Create;True;0;0;0;False;0;False;1,1;0.7,2.17;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;66;-84.75073,599.7093;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;77;-208.9484,-565.2256;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-95.58041,-386.0784;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;31;139.7293,690.3251;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;29;653.7019,530.7637;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;18;376.2626,1475.97;Inherit;False;Property;_EdgeSharpness;EdgeSharpness;9;0;Create;True;0;0;0;False;0;False;0;0.428;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;42;393.8576,1108.973;Inherit;True;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;8;158.697,-253.9434;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;-99,0;False;2;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;802.9433,1386.165;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;61;377.2645,-382.0606;Inherit;False;Property;_LoopMain;LoopMain;4;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;10;869.5454,486.8579;Inherit;True;Property;_Mask;Mask;5;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;4e09b5b2b10fa1d4e9d91646a223ccc1;4e09b5b2b10fa1d4e9d91646a223ccc1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;68;1220.828,-375.4452;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;69;1254.842,-198.2165;Inherit;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;12;1503.023,896.5301;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;954.0311,-69.44924;Inherit;True;Property;_MainTexture;MainTexture;0;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;743e979d144c0924583c2cd743af24e4;743e979d144c0924583c2cd743af24e4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;49;1209.573,223.4704;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;70;1487.566,-262.6633;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;72;1767.225,489.6884;Inherit;True;Property;_StaticMask;StaticMask;10;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;1522.267,-61.86047;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;40;1185.719,723.7791;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;37;862.613,845.4276;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;540.226,930.5369;Inherit;False;Constant;_Float1;Float 1;8;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;2115.739,271.6643;Inherit;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2480.288,14.49028;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Slash;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;41;3
WireConnection;46;0;44;1
WireConnection;2;0;62;0
WireConnection;2;1;63;0
WireConnection;79;0;80;0
WireConnection;79;1;78;3
WireConnection;59;0;9;0
WireConnection;59;1;58;2
WireConnection;57;0;58;1
WireConnection;57;1;9;0
WireConnection;65;0;64;0
WireConnection;65;1;46;0
WireConnection;43;1;51;0
WireConnection;75;0;2;1
WireConnection;75;1;79;0
WireConnection;55;0;57;0
WireConnection;55;1;59;0
WireConnection;66;0;65;0
WireConnection;66;1;43;0
WireConnection;77;0;75;0
WireConnection;77;1;2;2
WireConnection;6;0;77;0
WireConnection;6;1;55;0
WireConnection;31;0;26;0
WireConnection;31;1;66;0
WireConnection;29;0;31;1
WireConnection;29;1;31;2
WireConnection;8;0;6;0
WireConnection;19;0;42;4
WireConnection;19;1;18;0
WireConnection;61;0;8;0
WireConnection;61;1;6;0
WireConnection;10;1;29;0
WireConnection;12;0;10;1
WireConnection;12;1;42;4
WireConnection;12;2;19;0
WireConnection;1;1;61;0
WireConnection;70;0;68;2
WireConnection;70;1;69;0
WireConnection;47;0;1;0
WireConnection;47;1;49;0
WireConnection;47;2;70;0
WireConnection;40;0;10;1
WireConnection;40;1;37;0
WireConnection;40;2;37;0
WireConnection;37;0;39;0
WireConnection;37;1;31;1
WireConnection;11;0;1;4
WireConnection;11;1;12;0
WireConnection;11;2;49;4
WireConnection;11;3;72;1
WireConnection;0;2;47;0
WireConnection;0;9;11;0
ASEEND*/
//CHKSM=0C430BE8F3B9FC4B785C41BD49F94D22399705CC