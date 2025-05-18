// Made with Amplify Shader Editor v1.9.3.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Shield"
{
	Properties
	{
		_DepthFadeDistance("Depth Fade Distance", Float) = 0
		[HDR]_ShieldColor("ShieldColor", Color) = (0,0,0,0)
		_IntersectionColor("Intersection Color", Color) = (0.4338235,0.4377282,1,0)

	}
	
	SubShader
	{
		LOD 0

		Tags { "RenderType"="Opaque" }
		
		Pass
		{
			
			Name "First"
			CGINCLUDE
			#pragma target 3.0
			ENDCG
			Blend Off
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
			uniform float _DepthFadeDistance;
			uniform float4 _ShieldColor;

			
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
				float4 IntersectionColor43 = _IntersectionColor;
				float4 screenPos = i.ase_texcoord;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth42 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
				float distanceDepth42 = abs( ( screenDepth42 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFadeDistance ) );
				float SaturatedDepthFade44 = saturate( distanceDepth42 );
				float temp_output_40_0 = ( 1.0 - SaturatedDepthFade44 );
				float4 appendResult41 = (float4((IntersectionColor43).rgb , temp_output_40_0));
				float3 temp_output_33_0 = (_ShieldColor).rgb;
				float screenDepth12 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
				float distanceDepth12 = saturate( abs( ( screenDepth12 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( 1.55 ) ) );
				float4 appendResult29 = (float4(temp_output_33_0 , saturate( ( 1.0 - distanceDepth12 ) )));
				
				
				finalColor = ( appendResult41 + appendResult29 );
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
			Blend Off
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
				
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
			};

			uniform float4 _ShieldColor;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;

			
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
				float3 temp_output_33_0 = (_ShieldColor).rgb;
				float4 screenPos = i.ase_texcoord;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth12 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
				float distanceDepth12 = saturate( abs( ( screenDepth12 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( 1.55 ) ) );
				float4 appendResult29 = (float4(temp_output_33_0 , saturate( ( 1.0 - distanceDepth12 ) )));
				
				
				finalColor = appendResult29;
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
Node;AmplifyShaderEditor.DepthFade;12;-292.4389,-291.595;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1.55;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;8;-15.76069,-558.4717;Inherit;False;Property;_ShieldColor;ShieldColor;3;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0,0.9872486,1.498039,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;13;-24.56904,-250.5168;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;36;-367.094,-1151.099;Inherit;False;1268.871;416.1864;;9;45;44;43;42;41;40;39;38;37;First Pass only renders intersection;1,1,1,1;0;0
Node;AmplifyShaderEditor.ComponentMaskNode;33;372.5134,-491.5869;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;34;273.2421,-317.8337;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-317.094,-842.3466;Float;False;Property;_DepthFadeDistance;Depth Fade Distance;2;0;Create;True;0;0;0;False;0;False;0;1.96;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;42;-118.094,-882.3466;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;7;-376.8853,314.6642;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-368.3554,489.5808;Inherit;False;Property;_BottomMask;BottomMask;4;0;Create;True;0;0;0;False;0;False;0;-0.55;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-552.4595,50.57055;Inherit;False;Property;_FresnelScale;FresnelScale;0;0;Create;True;0;0;0;False;0;False;1;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-552.4594,184.1801;Inherit;False;Property;_FresnelPower;FresnelPower;1;0;Create;True;0;0;0;False;0;False;1;1.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;45;114.906,-857.3466;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;1;-289.3585,1.676987;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;10;-146.8755,355.3786;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;37;-13.09399,-1098.347;Float;False;Property;_IntersectionColor;Intersection Color;5;0;Create;True;0;0;0;False;0;False;0.4338235,0.4377282,1,0;0.08495052,0.9963852,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;290.906,-890.3466;Float;False;SaturatedDepthFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-8.476595,144.3961;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;43;226.906,-1098.347;Float;False;IntersectionColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;40;547.147,-914.3196;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;39;461.4661,-1101.099;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;32;632.5414,112.8057;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;41;706.4769,-1026.276;Inherit;True;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;48;1025.133,-1038.741;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;49;902.3505,-1230.87;Inherit;False;43;IntersectionColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;29;696.0154,-432.941;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ClampOpNode;9;193.7344,84.33521;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;50;737.2809,-766.9014;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;52;1343.766,-787.1434;Float;False;False;-1;2;ASEMaterialInspector;0;12;New Amplify Shader;003dfa9c16768d048b74f75c088119d8;True;Second;0;1;Second;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;RenderType=Opaque=RenderType;False;False;0;False;True;0;1;False;;0;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;True;2;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;51;1255.974,-1160.046;Float;False;True;-1;2;ASEMaterialInspector;0;12;Shield;003dfa9c16768d048b74f75c088119d8;True;First;0;0;First;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;RenderType=Opaque=RenderType;False;False;0;False;True;0;1;False;;0;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;0;True;2;False;0;;0;0;Standard;0;0;2;True;True;False;;False;0
WireConnection;13;0;12;0
WireConnection;33;0;8;0
WireConnection;34;0;13;0
WireConnection;42;0;38;0
WireConnection;45;0;42;0
WireConnection;1;2;2;0
WireConnection;1;3;3;0
WireConnection;10;0;7;2
WireConnection;10;1;11;0
WireConnection;44;0;45;0
WireConnection;6;0;1;0
WireConnection;6;1;10;0
WireConnection;43;0;37;0
WireConnection;40;0;44;0
WireConnection;39;0;43;0
WireConnection;32;0;33;0
WireConnection;32;3;9;0
WireConnection;41;0;39;0
WireConnection;41;3;40;0
WireConnection;48;0;41;0
WireConnection;48;1;29;0
WireConnection;29;0;33;0
WireConnection;29;3;34;0
WireConnection;9;0;6;0
WireConnection;50;0;40;0
WireConnection;50;1;9;0
WireConnection;52;0;29;0
WireConnection;51;0;48;0
ASEEND*/
//CHKSM=5F8481172F408ED8866AC0BB2A4673BBB3C11548