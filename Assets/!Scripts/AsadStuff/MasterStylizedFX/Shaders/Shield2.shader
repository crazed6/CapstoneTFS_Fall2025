// Made with Amplify Shader Editor v1.9.3.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/MultiPassDistortion"
{
	Properties
	{
		_FresnelScale("FresnelScale", Float) = 1
		_FresnelPower("FresnelPower", Float) = 1
		[HDR]_IntersectionColor("Intersection Color", Color) = (0.4338235,0.4377282,1,0)
		_FresnelScale2("FresnelScale2", Float) = 1
		_FresnelPower2("FresnelPower2", Float) = 1
		_FresnelColor2("FresnelColor2", Color) = (0,0,0,0)
		_BottomMask("BottomMask", Range( -1 , 1)) = 0
		_IntersectionDistance("IntersectionDistance", Float) = 0
		_IntersectionIntensity("IntersectionIntensity", Range( -1 , 1)) = 1

	}
	
	SubShader
	{
		LOD 0

		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		
		Pass
		{
			
			Name "First Pass"
			CGINCLUDE
			#pragma target 3.0
			ENDCG
			Blend SrcAlpha OneMinusSrcAlpha
			AlphaToMask Off
			Cull Off
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
				float4 ase_texcoord : TEXCOORD0;
			};

			uniform float4 _IntersectionColor;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform float _IntersectionDistance;
			uniform float _IntersectionIntensity;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord = screenPos;
				
				
				v.vertex.xyz +=  float3(0,0,0) ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				fixed4 finalColor;
				float4 IntersectionColor184 = _IntersectionColor;
				float4 screenPos = i.ase_texcoord;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth146 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
				float distanceDepth146 = abs( ( screenDepth146 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _IntersectionDistance ) );
				float SaturatedDepthFade186 = saturate( distanceDepth146 );
				float4 appendResult143 = (float4((IntersectionColor184).rgb , ( ( 1.0 - SaturatedDepthFade186 ) * _IntersectionIntensity )));
				
				
				finalColor = appendResult143;
				return finalColor;
			}
			ENDCG
		}

		
		Pass
		{
			Name "Second Pass"
			
			CGINCLUDE
			#pragma target 3.0
			ENDCG
			Blend SrcAlpha OneMinusSrcAlpha
			AlphaToMask Off
			Cull Back
			ColorMask RGBA
			ZWrite On
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
				float3 ase_normal : NORMAL;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float3 ase_normal : NORMAL;
			};

			uniform float4 _IntersectionColor;
			uniform float _FresnelScale;
			uniform float _FresnelPower;
			uniform float _BottomMask;
			uniform float4 _FresnelColor2;
			uniform float _FresnelScale2;
			uniform float _FresnelPower2;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (v.vertex).xyz, 1 )).xyz;
				o.ase_texcoord.xyz = ase_worldPos;
				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord1.xyz = ase_worldNormal;
				
				o.ase_normal = v.ase_normal;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.w = 0;
				o.ase_texcoord1.w = 0;
				
				v.vertex.xyz +=  float3(0,0,0) ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				fixed4 finalColor;
				float4 IntersectionColor184 = _IntersectionColor;
				float3 ase_worldPos = i.ase_texcoord.xyz;
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(ase_worldPos);
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = i.ase_texcoord1.xyz;
				float fresnelNdotV200 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode200 = ( 0.0 + _FresnelScale * pow( 1.0 - fresnelNdotV200, _FresnelPower ) );
				float smoothstepResult201 = smoothstep( _BottomMask , 1.0 , i.ase_normal.y);
				float clampResult203 = clamp( ( fresnelNode200 * smoothstepResult201 ) , 0.0 , 1.0 );
				float4 appendResult204 = (float4(IntersectionColor184.rgb , clampResult203));
				float fresnelNdotV215 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode215 = ( 0.0 + _FresnelScale2 * pow( 1.0 - fresnelNdotV215, _FresnelPower2 ) );
				float temp_output_219_0 = ( smoothstepResult201 * fresnelNode215 );
				float4 lerpResult218 = lerp( appendResult204 , ( _FresnelColor2 * temp_output_219_0 ) , temp_output_219_0);
				
				
				finalColor = lerpResult218;
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
Node;AmplifyShaderEditor.RangedFloatNode;197;1026.379,-38.1517;Inherit;False;Property;_BottomMask;BottomMask;6;0;Create;True;0;0;0;False;0;False;0;-0.57;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;196;1039.041,-220.1319;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;198;1108.355,-493.6443;Inherit;False;Property;_FresnelScale;FresnelScale;0;0;Create;True;0;0;0;False;0;False;1;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;199;1108.355,-360.0348;Inherit;False;Property;_FresnelPower;FresnelPower;1;0;Create;True;0;0;0;False;0;False;1;1.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;188;1586,-1560.752;Inherit;False;1348.532;537.3893;;11;143;148;207;206;147;149;186;146;174;184;172;First Pass only renders intersection;1,1,1,1;0;0
Node;AmplifyShaderEditor.FresnelNode;200;1371.456,-542.5376;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;172;1835.738,-1518.222;Float;False;Property;_IntersectionColor;Intersection Color;2;1;[HDR];Create;True;0;0;0;False;0;False;0.4338235,0.4377282,1,0;0.1705174,2,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;213;1151.664,196.1406;Inherit;False;Property;_FresnelScale2;FresnelScale2;3;0;Create;True;0;0;0;False;0;False;1;1.65;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;214;1151.664,329.75;Inherit;False;Property;_FresnelPower2;FresnelPower2;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;201;1391.971,-203.3276;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;202;1652.338,-399.8188;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;184;2175,-1508;Float;False;IntersectionColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;215;1377.74,239.6131;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;203;1854.549,-459.8796;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;205;1810.233,-693.2427;Inherit;False;184;IntersectionColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;216;1423.387,55.00781;Inherit;False;Property;_FresnelColor2;FresnelColor2;5;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.86616,1,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;219;1703.908,260.5761;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;204;2458.165,-786.4634;Inherit;True;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;217;1743.287,-16.20479;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;209;1770.938,-974.3387;Inherit;True;Property;_TextureSample0;Texture Sample 0;9;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;208;2223.658,-883.7568;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;174;2414.56,-1510.752;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DepthFade;146;1835,-1292;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;186;2244,-1300;Float;False;SaturatedDepthFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;149;2068,-1267;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;147;1636,-1252;Float;False;Property;_IntersectionDistance;IntersectionDistance;7;0;Create;True;0;0;0;False;0;False;0;4.45;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;206;2692.775,-1278.25;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;207;2278.187,-1161.145;Inherit;False;Property;_IntersectionIntensity;IntersectionIntensity;8;0;Create;True;0;0;0;False;0;False;1;0.17;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;148;2497.358,-1303.788;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;143;2751.11,-1423.664;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;218;2814.02,-597.0686;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;135;2983.332,-1419.903;Float;False;True;-1;2;ASEMaterialInspector;0;12;ASESampleShaders/MultiPassDistortion;003dfa9c16768d048b74f75c088119d8;True;First;0;0;First Pass;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;False;False;0;False;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;0;True;2;False;0;;0;0;Standard;0;0;2;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;138;3137.059,-564.6704;Float;False;False;-1;2;ASEMaterialInspector;0;12;ASETemplateShaders/DoublePassUnlit;003dfa9c16768d048b74f75c088119d8;True;Second;0;1;Second Pass;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;RenderType=Opaque=RenderType;False;False;0;False;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;True;2;False;0;;0;0;Standard;0;False;0
WireConnection;200;2;198;0
WireConnection;200;3;199;0
WireConnection;201;0;196;2
WireConnection;201;1;197;0
WireConnection;202;0;200;0
WireConnection;202;1;201;0
WireConnection;184;0;172;0
WireConnection;215;2;213;0
WireConnection;215;3;214;0
WireConnection;203;0;202;0
WireConnection;219;0;201;0
WireConnection;219;1;215;0
WireConnection;204;0;205;0
WireConnection;204;3;203;0
WireConnection;217;0;216;0
WireConnection;217;1;219;0
WireConnection;208;0;209;0
WireConnection;208;1;205;0
WireConnection;174;0;184;0
WireConnection;146;0;147;0
WireConnection;186;0;149;0
WireConnection;149;0;146;0
WireConnection;206;0;148;0
WireConnection;206;1;207;0
WireConnection;148;0;186;0
WireConnection;143;0;174;0
WireConnection;143;3;206;0
WireConnection;218;0;204;0
WireConnection;218;1;217;0
WireConnection;218;2;219;0
WireConnection;135;0;143;0
WireConnection;138;0;218;0
ASEEND*/
//CHKSM=31D2371B45D1B7D4F543050774316919E20FBF7C