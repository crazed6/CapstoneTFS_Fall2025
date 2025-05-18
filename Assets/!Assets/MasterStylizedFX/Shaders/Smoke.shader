// Made with Amplify Shader Editor v1.9.3.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Smoke"
{
	Properties
	{
		_ToonRamp("Toon Ramp", 2D) = "white" {}
		[Header(Main)][NoScaleOffset]_Main("Main", 2D) = "white" {}
		_Tiling("Tiling", Vector) = (1,1,0,0)
		_Offset("Offset", Vector) = (0,0,0,0)
		_Scroll("Scroll", Vector) = (1,0,0,0)
		[Header(LimitUV)]_LimitUVRange("LimitUVRange", Vector) = (0,1,0,1)
		[Toggle]_LimitUV("LimitUV", Float) = 0
		[Header(StretchUV)]_StretchUVDes("StretchUVDes", Vector) = (0,0,0,0)
		_StretchMultiplier("StretchMultiplier", Vector) = (0,0,0,0)
		[Toggle]_Stretch("Stretch", Float) = 0
		[Header(Mask)][NoScaleOffset]_NoiseMask("NoiseMask", 2D) = "white" {}
		[Toggle]_Mask("Mask", Float) = 1
		_MaskScroll("MaskScroll", Vector) = (0,0,0,0)
		_MaskTiling("MaskTiling", Vector) = (0,0,0,0)
		_MaskOffset("MaskOffset", Vector) = (0,0,0,0)
		_Feather("Feather", Range( 0 , 1)) = 0
		[Header(StaticMask)]_StaticMask("StaticMask", 2D) = "white" {}
		_SmoothStep("SmoothStep", Vector) = (0,1,0,0)
		[HDR]_FireColor("FireColor", Color) = (0,0,0,0)
		_FireTexture("FireTexture", 2D) = "white" {}
		_Feather1("Feather", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 uv2_texcoord2;
			float4 uv_texcoord;
			float4 vertexColor : COLOR;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float4 _FireColor;
		uniform float _Feather1;
		uniform sampler2D _FireTexture;
		uniform float4 _FireTexture_ST;
		uniform sampler2D _Main;
		uniform float _LimitUV;
		uniform float2 _Tiling;
		uniform float2 _Scroll;
		uniform float2 _Offset;
		uniform float _Stretch;
		uniform float2 _StretchMultiplier;
		uniform float2 _StretchUVDes;
		uniform float4 _LimitUVRange;
		uniform sampler2D _ToonRamp;
		uniform float _Mask;
		uniform float _Feather;
		uniform sampler2D _NoiseMask;
		uniform float2 _MaskTiling;
		uniform float2 _MaskOffset;
		uniform float2 _MaskScroll;
		uniform float2 _SmoothStep;
		uniform sampler2D _StaticMask;
		uniform float4 _StaticMask_ST;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float2 temp_cast_9 = (0.0).xx;
			float2 appendResult106 = (float2(( _StretchMultiplier * ( _StretchUVDes - i.uv_texcoord.xy ) * i.uv_texcoord.w )));
			float2 uvs_TexCoord74 = i.uv_texcoord;
			uvs_TexCoord74.xy = i.uv_texcoord.xy * _Tiling + ( ( i.uv_texcoord.xyz.z * _Scroll ) + _Offset + (( _Stretch )?( appendResult106 ):( temp_cast_9 )) );
			float4 appendResult84 = (float4(_LimitUVRange.x , _LimitUVRange.z , 0.0 , 0.0));
			float4 appendResult85 = (float4(_LimitUVRange.y , _LimitUVRange.w , 0.0 , 0.0));
			float2 clampResult80 = clamp( uvs_TexCoord74.xy , appendResult84.xy , appendResult85.xy );
			float4 tex2DNode73 = tex2D( _Main, (( _LimitUV )?( clampResult80 ):( uvs_TexCoord74.xy )) );
			float lerpResult172 = lerp( ( 0.0 - _Feather ) , ( 1.0 + _Feather ) , i.uv2_texcoord2.z);
			float2 uvs_TexCoord156 = i.uv_texcoord;
			uvs_TexCoord156.xy = i.uv_texcoord.xy * _MaskTiling + ( _MaskOffset + ( i.uv2_texcoord2.w * _MaskScroll ) );
			float smoothstepResult155 = smoothstep( ( lerpResult172 - _Feather ) , ( lerpResult172 + _Feather ) , tex2D( _NoiseMask, uvs_TexCoord156.xy ).r);
			float2 uv_StaticMask = i.uv_texcoord * _StaticMask_ST.xy + _StaticMask_ST.zw;
			float smoothstepResult165 = smoothstep( _SmoothStep.x , _SmoothStep.y , tex2D( _StaticMask, uv_StaticMask ).r);
			float temp_output_126_0 = ( ( i.vertexColor.a * tex2DNode73.a ) * (( _Mask )?( smoothstepResult155 ):( 1.0 )) * smoothstepResult165 );
			c.rgb = 0;
			c.a = temp_output_126_0;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
			float lerpResult228 = lerp( ( 0.0 - _Feather1 ) , ( 1.0 + _Feather1 ) , i.uv2_texcoord2.xy.y);
			float4 temp_cast_0 = (( lerpResult228 - _Feather1 )).xxxx;
			float4 temp_cast_1 = (( lerpResult228 + _Feather1 )).xxxx;
			float2 uv_FireTexture = i.uv_texcoord * _FireTexture_ST.xy + _FireTexture_ST.zw;
			float4 smoothstepResult224 = smoothstep( temp_cast_0 , temp_cast_1 , tex2D( _FireTexture, uv_FireTexture ));
			float2 temp_cast_2 = (0.0).xx;
			float2 appendResult106 = (float2(( _StretchMultiplier * ( _StretchUVDes - i.uv_texcoord.xy ) * i.uv_texcoord.w )));
			float2 uvs_TexCoord74 = i.uv_texcoord;
			uvs_TexCoord74.xy = i.uv_texcoord.xy * _Tiling + ( ( i.uv_texcoord.xyz.z * _Scroll ) + _Offset + (( _Stretch )?( appendResult106 ):( temp_cast_2 )) );
			float4 appendResult84 = (float4(_LimitUVRange.x , _LimitUVRange.z , 0.0 , 0.0));
			float4 appendResult85 = (float4(_LimitUVRange.y , _LimitUVRange.w , 0.0 , 0.0));
			float2 clampResult80 = clamp( uvs_TexCoord74.xy , appendResult84.xy , appendResult85.xy );
			float4 tex2DNode73 = tex2D( _Main, (( _LimitUV )?( clampResult80 ):( uvs_TexCoord74.xy )) );
			float4 Diffuse195 = ( i.vertexColor * tex2DNode73 * ( 1.0 + i.uv2_texcoord2.xy.x ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult180 = dot( ase_worldNormal , ase_worldlightDir );
			float2 temp_cast_5 = (saturate( (dotResult180*0.5 + 0.5) )).xx;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 Surface197 = ( ( Diffuse195 * tex2D( _ToonRamp, temp_cast_5 ) ) * ( ase_lightColor * float4( ( float3(0,0,0) + 1 ) , 0.0 ) ) );
			float lerpResult172 = lerp( ( 0.0 - _Feather ) , ( 1.0 + _Feather ) , i.uv2_texcoord2.z);
			float2 uvs_TexCoord156 = i.uv_texcoord;
			uvs_TexCoord156.xy = i.uv_texcoord.xy * _MaskTiling + ( _MaskOffset + ( i.uv2_texcoord2.w * _MaskScroll ) );
			float smoothstepResult155 = smoothstep( ( lerpResult172 - _Feather ) , ( lerpResult172 + _Feather ) , tex2D( _NoiseMask, uvs_TexCoord156.xy ).r);
			float2 uv_StaticMask = i.uv_texcoord * _StaticMask_ST.xy + _StaticMask_ST.zw;
			float smoothstepResult165 = smoothstep( _SmoothStep.x , _SmoothStep.y , tex2D( _StaticMask, uv_StaticMask ).r);
			float temp_output_126_0 = ( ( i.vertexColor.a * tex2DNode73.a ) * (( _Mask )?( smoothstepResult155 ):( 1.0 )) * smoothstepResult165 );
			float4 appendResult145 = (float4(( ( _FireColor * smoothstepResult224 * Surface197 ) + Surface197 ).rgb , temp_output_126_0));
			o.Emission = appendResult145.xyz;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows 

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
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
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
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xyzw = customInputData.uv2_texcoord2;
				o.customPack1.xyzw = v.texcoord1;
				o.customPack2.xyzw = customInputData.uv_texcoord;
				o.customPack2.xyzw = v.texcoord;
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
				surfIN.uv2_texcoord2 = IN.customPack1.xyzw;
				surfIN.uv_texcoord = IN.customPack2.xyzw;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.vertexColor = IN.color;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
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
Node;AmplifyShaderEditor.CommentaryNode;107;-97.24704,1076.068;Inherit;False;981.2;834.1053;StretchUV;9;119;106;114;118;103;102;115;123;124;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;115;-77.15573,1593.25;Inherit;False;Property;_StretchUVDes;StretchUVDes;8;1;[Header];Create;True;1;StretchUV;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;102;-73.93272,1408.671;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;103;175.7717,1477.859;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;118;207.5538,1247.947;Inherit;False;Property;_StretchMultiplier;StretchMultiplier;9;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;119;115.9227,1689.141;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;87;164.4255,-288.6086;Inherit;False;1122.08;1253.271;Scroll;7;74;78;82;81;77;76;75;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;419.421,1426.666;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;75;221.351,191.4847;Inherit;False;Property;_Scroll;Scroll;5;0;Create;True;0;0;0;False;0;False;1,0;1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;76;223.2823,-11.87827;Inherit;False;0;-1;3;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;106;565.1396,1403.484;Inherit;False;FLOAT2;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;124;469.7593,1201.906;Inherit;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;86;1321.36,-797.9218;Inherit;False;1078.309;819.3665;UVlimitation;5;80;85;84;83;109;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;531.4758,50.08174;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;81;213.7936,341.0704;Inherit;False;Property;_Offset;Offset;4;1;[Header];Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ToggleSwitchNode;123;641.1641,1216.096;Inherit;False;Property;_Stretch;Stretch;10;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;82;787.2185,100.1137;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;78;772.4404,-184.6765;Inherit;False;Property;_Tiling;Tiling;3;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector4Node;83;1367.36,-657.3679;Inherit;False;Property;_LimitUVRange;LimitUVRange;6;1;[Header];Create;True;1;LimitUV;0;0;False;0;False;0,1,0,1;0,1,0,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;84;1594.783,-747.9218;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;85;1595.883,-532.4218;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;74;1020.081,-44.5886;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;98;2929.765,-423.1228;Inherit;False;712.2244;719.6301;VertexColor;7;195;95;97;121;96;122;120;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;177;1279.031,-1362.072;Inherit;False;559.9969;396.921;Comment;3;194;193;180;N . L;1,1,1,1;0;0
Node;AmplifyShaderEditor.ClampOpNode;80;1889.626,-389.0704;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;178;2151.931,-1509.771;Inherit;False;723.599;290;Also know as Lambert Wrap or Half Lambert;3;185;183;181;Diffuse Wrap;1,1,1,1;0;0
Node;AmplifyShaderEditor.ToggleSwitchNode;109;2179.447,-325.5556;Inherit;False;Property;_LimitUV;LimitUV;7;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;120;2967.5,-290.9236;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;122;2976.133,-363.7119;Inherit;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;193;1365.721,-1125.193;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;194;1352.87,-1303.758;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;73;2583.71,-121.8407;Inherit;True;Property;_Main;Main;2;2;[Header];[NoScaleOffset];Create;True;1;Main;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;96;2956.925,-172.8337;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;121;3237.433,-342.9119;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;180;1679.031,-1250.072;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;181;2201.931,-1334.771;Float;False;Constant;_WrapperValue;Wrapper Value;0;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;160;1302.843,700.6349;Inherit;False;781.2883;525.4138;NoiseScroll;7;156;158;159;157;161;162;163;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;179;2643.409,-1129.198;Inherit;False;812;304;Comment;5;190;187;186;184;182;Attenuation and Ambient;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;3218.178,-102.1768;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;183;2467.333,-1459.771;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;231;4186.667,-1349.797;Inherit;False;1505.066;977.1329;SurfaceFireErosion;14;220;225;226;227;229;228;223;222;219;224;206;207;232;233;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;146;2160.628,535.6385;Inherit;False;1444.062;1209.11;Mask;13;170;169;137;125;147;155;148;171;172;173;174;175;176;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;157;1383.586,750.6349;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;159;1372.48,1010.377;Inherit;False;Property;_MaskScroll;MaskScroll;13;0;Create;True;0;0;0;False;0;False;0,0;1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.IndirectDiffuseLighting;184;2899.41,-1001.197;Inherit;False;Tangent;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;185;2700.531,-1452.972;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;195;3401.36,-134.8984;Inherit;False;Diffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;182;2670.809,-941.1971;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;1650.095,937.4907;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;162;1644.921,1093.576;Inherit;False;Property;_MaskOffset;MaskOffset;15;0;Create;True;0;0;0;False;0;False;0,0;0.5,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;174;2405.195,1359.424;Inherit;False;Constant;_Float5;Float 5;19;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;170;2210.485,1244.625;Inherit;False;Property;_Feather;Feather;16;0;Create;True;0;0;0;False;0;False;0;0.155;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;173;2278.105,1079.171;Inherit;False;Constant;_Float3;Float 3;19;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;220;4431.377,-617.4821;Inherit;False;Constant;_Float6;Float 5;19;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;226;4304.287,-897.7349;Inherit;False;Constant;_Float7;Float 3;19;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;186;3139.41,-969.1971;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightColorNode;187;2755.41,-1081.198;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;188;3071.031,-1474.072;Inherit;True;Property;_ToonRamp;Toon Ramp;1;0;Create;True;0;0;0;False;0;False;-1;None;c755faa3a4b9cd34694d54c650952df5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;196;3212.541,-1664.712;Inherit;False;195;Diffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;225;4236.667,-732.281;Inherit;False;Property;_Feather1;Feather;23;0;Create;True;0;0;0;False;0;False;0;0.155;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;161;1608.19,748.5315;Inherit;False;Property;_MaskTiling;MaskTiling;14;0;Create;True;0;0;0;False;0;False;0,0;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;163;1904.504,1022.324;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;137;2261.345,1507.165;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;171;2552.765,1124.49;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;169;2607.963,1285.665;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;227;4578.947,-852.4158;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;229;4634.145,-691.241;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;190;3299.41,-1081.198;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;191;3445.03,-1514.072;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;233;4241.063,-522.6058;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;156;1842.131,810.5052;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;172;2764.444,1233.436;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;228;4790.626,-743.47;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;192;3780.631,-1240.872;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;167;2765.758,1850.221;Inherit;False;715.4617;522.7068;StaticMask;3;166;165;164;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;125;2604.117,826.5101;Inherit;True;Property;_NoiseMask;NoiseMask;11;2;[Header];[NoScaleOffset];Create;True;1;Mask;0;0;False;0;False;-1;None;cfd22da0c4bc7af4bb5f6497b36ed0c2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;175;2984.314,1256.771;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;176;2970.993,1420.424;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;223;4997.175,-556.4821;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;222;5002.697,-798.1351;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;197;3968.692,-1228.28;Inherit;False;Surface;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;219;4690.869,-1099.623;Inherit;True;Property;_FireTexture;FireTexture;22;0;Create;True;0;0;0;False;0;False;-1;None;4e3951c538fc8a647a4a10a99b480987;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;148;2824.794,646.5159;Inherit;False;Constant;_Float2;Float 2;12;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;164;2815.758,1900.221;Inherit;True;Property;_StaticMask;StaticMask;17;1;[Header];Create;True;1;StaticMask;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;166;2867.849,2130.773;Inherit;False;Property;_SmoothStep;SmoothStep;18;0;Create;True;0;0;0;False;0;False;0,1;0,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SmoothstepOpNode;155;3270.459,987.067;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;224;5172.944,-944.6721;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;232;5256.142,-689.6547;Inherit;True;197;Surface;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;206;5145.22,-1299.797;Inherit;False;Property;_FireColor;FireColor;21;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;5.992157,1.262782,0.09411748,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;3214.735,62.38358;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;165;3214.41,1984.56;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;147;3171.941,733.5972;Inherit;False;Property;_Mask;Mask;12;0;Create;True;0;0;0;False;0;False;1;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;207;5513.733,-816.4454;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;198;5412.94,-236.3937;Inherit;False;197;Surface;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;210;4213.159,-1917.09;Inherit;False;618.6001;377.4633;FresnelMask;5;204;205;203;209;208;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;3595.287,453.4724;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;201;5669.538,-314.5926;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;199;4031.509,478.4039;Inherit;False;Constant;_Color0;Color 0;18;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;204;4263.159,-1748.953;Inherit;False;Property;_FresnelScale;FresnelScale;20;0;Create;True;0;0;0;False;0;False;0;4.65;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;205;4264.253,-1657.068;Inherit;False;Property;_FresnelPower;FresnelPower;19;0;Create;True;0;0;0;False;0;False;0;2.77;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;203;4458.961,-1867.09;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;209;4546.975,-1652.626;Inherit;False;Constant;_Float4;Float 4;22;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;208;4685.759,-1781.378;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;145;5917.56,-83.80117;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;200;6152.74,-87.66552;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Smoke;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;1;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;Custom;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;103;0;115;0
WireConnection;103;1;102;0
WireConnection;114;0;118;0
WireConnection;114;1;103;0
WireConnection;114;2;119;4
WireConnection;106;0;114;0
WireConnection;77;0;76;3
WireConnection;77;1;75;0
WireConnection;123;0;124;0
WireConnection;123;1;106;0
WireConnection;82;0;77;0
WireConnection;82;1;81;0
WireConnection;82;2;123;0
WireConnection;84;0;83;1
WireConnection;84;1;83;3
WireConnection;85;0;83;2
WireConnection;85;1;83;4
WireConnection;74;0;78;0
WireConnection;74;1;82;0
WireConnection;80;0;74;0
WireConnection;80;1;84;0
WireConnection;80;2;85;0
WireConnection;109;0;74;0
WireConnection;109;1;80;0
WireConnection;73;1;109;0
WireConnection;121;0;122;0
WireConnection;121;1;120;1
WireConnection;180;0;194;0
WireConnection;180;1;193;0
WireConnection;95;0;96;0
WireConnection;95;1;73;0
WireConnection;95;2;121;0
WireConnection;183;0;180;0
WireConnection;183;1;181;0
WireConnection;183;2;181;0
WireConnection;185;0;183;0
WireConnection;195;0;95;0
WireConnection;158;0;157;4
WireConnection;158;1;159;0
WireConnection;186;0;184;0
WireConnection;186;1;182;0
WireConnection;188;1;185;0
WireConnection;163;0;162;0
WireConnection;163;1;158;0
WireConnection;171;0;173;0
WireConnection;171;1;170;0
WireConnection;169;0;174;0
WireConnection;169;1;170;0
WireConnection;227;0;226;0
WireConnection;227;1;225;0
WireConnection;229;0;220;0
WireConnection;229;1;225;0
WireConnection;190;0;187;0
WireConnection;190;1;186;0
WireConnection;191;0;196;0
WireConnection;191;1;188;0
WireConnection;156;0;161;0
WireConnection;156;1;163;0
WireConnection;172;0;171;0
WireConnection;172;1;169;0
WireConnection;172;2;137;3
WireConnection;228;0;227;0
WireConnection;228;1;229;0
WireConnection;228;2;233;2
WireConnection;192;0;191;0
WireConnection;192;1;190;0
WireConnection;125;1;156;0
WireConnection;175;0;172;0
WireConnection;175;1;170;0
WireConnection;176;0;172;0
WireConnection;176;1;170;0
WireConnection;223;0;228;0
WireConnection;223;1;225;0
WireConnection;222;0;228;0
WireConnection;222;1;225;0
WireConnection;197;0;192;0
WireConnection;155;0;125;1
WireConnection;155;1;175;0
WireConnection;155;2;176;0
WireConnection;224;0;219;0
WireConnection;224;1;222;0
WireConnection;224;2;223;0
WireConnection;97;0;96;4
WireConnection;97;1;73;4
WireConnection;165;0;164;1
WireConnection;165;1;166;1
WireConnection;165;2;166;2
WireConnection;147;0;148;0
WireConnection;147;1;155;0
WireConnection;207;0;206;0
WireConnection;207;1;224;0
WireConnection;207;2;232;0
WireConnection;126;0;97;0
WireConnection;126;1;147;0
WireConnection;126;2;165;0
WireConnection;201;0;207;0
WireConnection;201;1;198;0
WireConnection;203;2;204;0
WireConnection;203;3;205;0
WireConnection;208;0;209;0
WireConnection;208;1;203;0
WireConnection;145;0;201;0
WireConnection;145;3;126;0
WireConnection;200;2;145;0
WireConnection;200;9;126;0
ASEEND*/
//CHKSM=1861862AFE9354AA0F0994AE29DFD10E62EBB24D