// Made with Amplify Shader Editor v1.9.3.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "EnergyExpSphere"
{
	Properties
	{
		[Header(Main)][NoScaleOffset]_Main("Main", 2D) = "white" {}
		_Tiling("Tiling", Vector) = (1,1,0,0)
		_Offset("Offset", Vector) = (0,0,0,0)
		_Scroll("Scroll", Vector) = (1,0,0,0)
		[Header(LimitUV)]_LimitUVRange("LimitUVRange", Vector) = (0,1,0,1)
		[Toggle]_LimitUV("LimitUV", Float) = 1
		[Header(StretchUV)]_StretchUVDes("StretchUVDes", Vector) = (0,0,0,0)
		_StretchMultiplier("StretchMultiplier", Vector) = (0,0,0,0)
		[Toggle]_Stretch("Stretch", Float) = 0
		[Header(Mask)][NoScaleOffset]_NoiseMask("NoiseMask", 2D) = "white" {}
		[Toggle]_Mask("Mask", Float) = 0
		_MaskScroll("MaskScroll", Vector) = (0,0,0,0)
		_MaskTiling("MaskTiling", Vector) = (0,0,0,0)
		_MaskOffset("MaskOffset", Vector) = (0,0,0,0)
		_Feather("Feather", Range( 0 , 1)) = 0
		[Header(StaticMask)]_StaticMask("StaticMask", 2D) = "white" {}
		_SmoothStep("SmoothStep", Vector) = (0,1,0,0)
		_FresnelScale("FresnelScale", Float) = 1
		_BackColor("BackColor", Color) = (0,0,0,0)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}
	
	SubShader
	{
		LOD 0

		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		
		Pass
		{
			
			Name "First"
			CGINCLUDE
			#pragma target 3.0
			ENDCG
			Blend SrcAlpha OneMinusSrcAlpha
			AlphaToMask Off
			Cull Front
			ColorMask RGBA
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
				
			};

			uniform float4 _BackColor;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				
				
				v.vertex.xyz +=  float3(0,0,0) ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				fixed4 finalColor;
				
				
				finalColor = _BackColor;
				return finalColor;
			}
			ENDCG
		}

		
		Pass
		{
			Name "Second"
			
			CGINCLUDE
			#pragma target 3.0
			ENDCG
			Blend SrcAlpha OneMinusSrcAlpha
			AlphaToMask Off
			Cull Back
			ColorMask RGBA
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#define ASE_NEEDS_FRAG_COLOR


			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float3 ase_normal : NORMAL;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
			};

			uniform sampler2D _Main;
			uniform float _LimitUV;
			uniform float2 _Tiling;
			uniform float2 _Scroll;
			uniform float2 _Offset;
			uniform float _Stretch;
			uniform float2 _StretchMultiplier;
			uniform float2 _StretchUVDes;
			uniform float4 _LimitUVRange;
			uniform float _Mask;
			uniform float _Feather;
			uniform sampler2D _NoiseMask;
			uniform float2 _MaskTiling;
			uniform float2 _MaskOffset;
			uniform float2 _MaskScroll;
			uniform float2 _SmoothStep;
			uniform sampler2D _StaticMask;
			uniform float4 _StaticMask_ST;
			uniform sampler2D _TextureSample0;
			uniform float4 _TextureSample0_ST;
			uniform float _FresnelScale;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (v.vertex).xyz, 1 )).xyz;
				o.ase_texcoord2.xyz = ase_worldPos;
				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord3.xyz = ase_worldNormal;
				
				o.ase_texcoord = v.ase_texcoord1;
				o.ase_texcoord1 = v.ase_texcoord;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.w = 0;
				
				v.vertex.xyz +=  float3(0,0,0) ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				fixed4 finalColor;
				float2 texCoord120 = i.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float3 texCoord76 = i.ase_texcoord1.xyz;
				texCoord76.xy = i.ase_texcoord1.xyz.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_cast_0 = (0.0).xx;
				float2 texCoord102 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float4 texCoord119 = i.ase_texcoord1;
				texCoord119.xy = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult106 = (float2(( _StretchMultiplier * ( _StretchUVDes - texCoord102 ) * texCoord119.w )));
				float2 texCoord74 = i.ase_texcoord1.xy * _Tiling + ( ( texCoord76.z * _Scroll ) + _Offset + (( _Stretch )?( appendResult106 ):( temp_cast_0 )) );
				float4 appendResult84 = (float4(_LimitUVRange.x , _LimitUVRange.z , 0.0 , 0.0));
				float4 appendResult85 = (float4(_LimitUVRange.y , _LimitUVRange.w , 0.0 , 0.0));
				float2 clampResult80 = clamp( texCoord74 , appendResult84.xy , appendResult85.xy );
				float4 tex2DNode73 = tex2D( _Main, (( _LimitUV )?( clampResult80 ):( texCoord74 )) );
				float4 texCoord137 = i.ase_texcoord;
				texCoord137.xy = i.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float lerpResult172 = lerp( ( 0.0 - _Feather ) , ( 1.0 + _Feather ) , texCoord137.z);
				float4 texCoord157 = i.ase_texcoord;
				texCoord157.xy = i.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord156 = i.ase_texcoord1.xy * _MaskTiling + ( _MaskOffset + ( texCoord157.w * _MaskScroll ) );
				float smoothstepResult155 = smoothstep( ( lerpResult172 - _Feather ) , ( lerpResult172 + _Feather ) , tex2D( _NoiseMask, texCoord156 ).r);
				float2 uv_StaticMask = i.ase_texcoord1.xy * _StaticMask_ST.xy + _StaticMask_ST.zw;
				float smoothstepResult165 = smoothstep( _SmoothStep.x , _SmoothStep.y , tex2D( _StaticMask, uv_StaticMask ).r);
				float clampResult194 = clamp( ( ( i.ase_color.a * tex2DNode73.a ) * (( _Mask )?( smoothstepResult155 ):( 1.0 )) * smoothstepResult165 ) , 0.0 , 1.0 );
				float4 appendResult145 = (float4(( ( 1.0 + texCoord120.x ) * tex2DNode73 * i.ase_color ).rgb , clampResult194));
				float2 uv_TextureSample0 = i.ase_texcoord1.xy * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
				float3 ase_worldPos = i.ase_texcoord2.xyz;
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(ase_worldPos);
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = i.ase_texcoord3.xyz;
				float2 texCoord205 = i.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float fresnelNdotV179 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode179 = ( 0.0 + _FresnelScale * pow( 1.0 - fresnelNdotV179, texCoord205.y ) );
				float4 myVarName199 = ( i.ase_color.a * tex2D( _TextureSample0, uv_TextureSample0 ) * fresnelNode179 );
				float4 temp_output_206_0 = ( appendResult145.w > 0.02 ? appendResult145 : myVarName199 );
				
				
				finalColor = temp_output_206_0;
				return finalColor;
			}
			ENDCG
		}
		
	}
	// CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19302
Node;AmplifyShaderEditor.CommentaryNode;107;-97.24704,1076.068;Inherit;False;981.2;834.1053;StretchUV;9;119;106;114;118;103;102;115;123;124;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;115;-77.15573,1593.25;Inherit;False;Property;_StretchUVDes;StretchUVDes;6;1;[Header];Create;True;1;StretchUV;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;102;-73.93272,1408.671;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;103;175.7717,1477.859;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;118;207.5538,1247.947;Inherit;False;Property;_StretchMultiplier;StretchMultiplier;7;0;Create;True;0;0;0;False;0;False;0,0;0,0.54;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;119;115.9227,1689.141;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;87;164.4255,-288.6086;Inherit;False;1122.08;1253.271;Scroll;7;74;78;82;81;77;76;75;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;419.421,1426.666;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;160;1302.843,700.6349;Inherit;False;781.2883;525.4138;NoiseScroll;7;156;158;159;157;161;162;163;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;75;221.351,191.4847;Inherit;False;Property;_Scroll;Scroll;3;0;Create;True;0;0;0;False;0;False;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;76;223.2823,-11.87827;Inherit;False;0;-1;3;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;106;565.1396,1403.484;Inherit;False;FLOAT2;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;124;469.7593,1201.906;Inherit;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;86;1321.36,-797.9218;Inherit;False;1078.309;819.3665;UVlimitation;5;80;85;84;83;109;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;146;2160.628,535.6385;Inherit;False;1444.062;1209.11;Mask;11;170;169;137;125;147;155;148;171;172;173;174;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;531.4758,50.08174;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;81;213.7936,341.0704;Inherit;False;Property;_Offset;Offset;2;1;[Header];Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ToggleSwitchNode;123;641.1641,1216.096;Inherit;False;Property;_Stretch;Stretch;8;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;157;1383.586,750.6349;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;159;1372.48,1010.377;Inherit;False;Property;_MaskScroll;MaskScroll;11;0;Create;True;0;0;0;False;0;False;0,0;0,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;82;787.2185,100.1137;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;78;772.4404,-184.6765;Inherit;False;Property;_Tiling;Tiling;1;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector4Node;83;1367.36,-657.3679;Inherit;False;Property;_LimitUVRange;LimitUVRange;4;1;[Header];Create;True;1;LimitUV;0;0;False;0;False;0,1,0,1;0,3,0,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;1650.095,937.4907;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;162;1644.921,1093.576;Inherit;False;Property;_MaskOffset;MaskOffset;13;0;Create;True;0;0;0;False;0;False;0,0;1.51,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;173;2365.489,1175.01;Inherit;False;Constant;_Float3;Float 3;19;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;174;2405.195,1359.424;Inherit;False;Constant;_Float5;Float 5;19;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;170;2207.666,1316.503;Inherit;False;Property;_Feather;Feather;14;0;Create;True;0;0;0;False;0;False;0;0.278;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;84;1594.783,-747.9218;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;85;1595.883,-532.4218;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;74;1020.081,-44.5886;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;161;1608.19,748.5315;Inherit;False;Property;_MaskTiling;MaskTiling;12;0;Create;True;0;0;0;False;0;False;0,0;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;163;1904.504,1022.324;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;137;2261.345,1507.165;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;171;2530.214,1162.544;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;169;2557.224,1287.075;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;80;1889.626,-389.0704;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;156;1842.131,810.5052;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;172;2698.201,1299.677;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;98;3090.025,-536.2324;Inherit;False;601.9905;998.6679;VertexColor;6;121;95;97;96;122;120;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;167;2765.758,1850.221;Inherit;False;715.4617;522.7068;StaticMask;3;166;165;164;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ToggleSwitchNode;109;2179.447,-325.5556;Inherit;False;Property;_LimitUV;LimitUV;5;0;Create;True;0;0;0;False;0;False;1;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;175;2984.314,1256.771;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;176;2970.993,1420.424;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;125;2827.966,834.0557;Inherit;True;Property;_NoiseMask;NoiseMask;9;2;[Header];[NoScaleOffset];Create;True;1;Mask;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;73;2583.71,-121.8407;Inherit;True;Property;_Main;Main;0;2;[Header];[NoScaleOffset];Create;True;1;Main;0;0;False;0;False;-1;None;b28cb2b096d0ce743802a614e722bc3d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;96;3114.111,-285.9428;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;148;2824.794,646.5159;Inherit;False;Constant;_Float2;Float 2;12;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;164;2815.758,1900.221;Inherit;True;Property;_StaticMask;StaticMask;15;1;[Header];Create;True;1;StaticMask;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;166;2867.849,2130.773;Inherit;False;Property;_SmoothStep;SmoothStep;16;0;Create;True;0;0;0;False;0;False;0,1;0,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SmoothstepOpNode;155;3270.459,987.067;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;185;3190.522,-1373.765;Inherit;False;1263.672;858.125;Comment;7;199;210;179;205;181;215;216;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;120;3127.76,-404.0327;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;122;3136.393,-476.821;Inherit;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;165;3214.41,1984.56;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;147;3171.941,733.5972;Inherit;False;Property;_Mask;Mask;10;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;3408.337,212.0015;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;181;3227.23,-1111.672;Inherit;False;Property;_FresnelScale;FresnelScale;17;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;205;3209.444,-1020.033;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;121;3355.341,-456.593;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;3595.287,453.4724;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;179;3444.263,-1128.889;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;210;3469.235,-1313.158;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;216;3376.689,-849.959;Inherit;True;Property;_TextureSample0;Texture Sample 0;19;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;3460.323,-196.6636;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;194;3768.789,305.6582;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;215;3931.917,-1274.812;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;145;3957.888,76.4185;Inherit;True;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;199;4251.059,-1249.301;Inherit;False;myVarName;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;207;4232.617,58.95951;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;200;4128.698,-351.4824;Inherit;False;199;myVarName;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;209;4274.917,-1524.628;Inherit;False;Property;_BackColor;BackColor;18;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.05098038,0.3715754,0.8,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;208;4186.282,368.8572;Inherit;False;Constant;_Color0;Color 0;19;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;192;4812.337,-57.42535;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Compare;206;4464.412,119.1469;Inherit;False;2;4;0;FLOAT;0;False;1;FLOAT;0.02;False;2;FLOAT4;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;142;4676.583,-1250.358;Float;False;True;-1;2;ASEMaterialInspector;0;12;EnergyExpSphere;003dfa9c16768d048b74f75c088119d8;True;First;0;0;First;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;False;False;0;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;1;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;0;True;2;False;0;;0;0;Standard;0;0;2;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;143;5132.728,234.7474;Float;False;False;-1;2;ASEMaterialInspector;0;12;New Amplify Shader;003dfa9c16768d048b74f75c088119d8;True;Second;0;1;Second;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;RenderType=Opaque=RenderType;False;False;0;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;0;True;2;False;0;;0;0;Standard;0;False;0
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
WireConnection;158;0;157;4
WireConnection;158;1;159;0
WireConnection;84;0;83;1
WireConnection;84;1;83;3
WireConnection;85;0;83;2
WireConnection;85;1;83;4
WireConnection;74;0;78;0
WireConnection;74;1;82;0
WireConnection;163;0;162;0
WireConnection;163;1;158;0
WireConnection;171;0;173;0
WireConnection;171;1;170;0
WireConnection;169;0;174;0
WireConnection;169;1;170;0
WireConnection;80;0;74;0
WireConnection;80;1;84;0
WireConnection;80;2;85;0
WireConnection;156;0;161;0
WireConnection;156;1;163;0
WireConnection;172;0;171;0
WireConnection;172;1;169;0
WireConnection;172;2;137;3
WireConnection;109;0;74;0
WireConnection;109;1;80;0
WireConnection;175;0;172;0
WireConnection;175;1;170;0
WireConnection;176;0;172;0
WireConnection;176;1;170;0
WireConnection;125;1;156;0
WireConnection;73;1;109;0
WireConnection;155;0;125;1
WireConnection;155;1;175;0
WireConnection;155;2;176;0
WireConnection;165;0;164;1
WireConnection;165;1;166;1
WireConnection;165;2;166;2
WireConnection;147;0;148;0
WireConnection;147;1;155;0
WireConnection;97;0;96;4
WireConnection;97;1;73;4
WireConnection;121;0;122;0
WireConnection;121;1;120;1
WireConnection;126;0;97;0
WireConnection;126;1;147;0
WireConnection;126;2;165;0
WireConnection;179;2;181;0
WireConnection;179;3;205;2
WireConnection;95;0;121;0
WireConnection;95;1;73;0
WireConnection;95;2;96;0
WireConnection;194;0;126;0
WireConnection;215;0;210;4
WireConnection;215;1;216;0
WireConnection;215;2;179;0
WireConnection;145;0;95;0
WireConnection;145;3;194;0
WireConnection;199;0;215;0
WireConnection;207;0;145;0
WireConnection;192;0;200;0
WireConnection;192;1;206;0
WireConnection;206;0;207;3
WireConnection;206;2;145;0
WireConnection;206;3;200;0
WireConnection;142;0;209;0
WireConnection;143;0;206;0
ASEEND*/
//CHKSM=576618540C85003E2C2510D1296D7E3E200B2819