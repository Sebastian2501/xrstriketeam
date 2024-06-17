// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "XRS/XRS_Skybox_BkGd"
{
	Properties
	{
		[HDR][Header(Far Fog Settings)]_Fog_Distance_Color("Fog_Distance_Color", Color) = (1,1,1,1)
		[HDR][Header(Height Fog Settings)]_Fog_Height_Color("Fog_Height_Color", Color) = (1,1,1,1)
		_Fog_Height_Offset("Fog_Height_Offset", Float) = 0
		_Fog_Height_MinY("Fog_Height_MinY", Float) = 0
		_Fog_Height_MaxY("Fog_Height_MaxY", Float) = 0
		_Fog_Height_Power("Fog_Height_Power", Range( 0 , 5)) = 1.5

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

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
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#define ASE_NEEDS_FRAG_WORLD_POSITION


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float4 _Fog_Distance_Color;
			uniform float4 _Fog_Height_Color;
			uniform float _Fog_Height_MaxY;
			uniform float _Fog_Height_MinY;
			uniform float _Fog_Height_Power;
			uniform float _Fog_Height_Offset;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float4 Var_FogDist_Color14 = _Fog_Distance_Color;
				float4 Var_FogHeight_Color16 = _Fog_Height_Color;
				float Var_World_Y8 = WorldPosition.y;
				float Var_FogHeight_Amount12 = saturate( ((0.0 + (Var_World_Y8 - _Fog_Height_MaxY) * (1.0 - 0.0) / (_Fog_Height_MinY - _Fog_Height_MaxY))*_Fog_Height_Power + _Fog_Height_Offset) );
				float3 temp_cast_0 = (Var_FogHeight_Amount12).xxx;
				float3 temp_cast_1 = (Var_FogHeight_Amount12).xxx;
				float3 gammaToLinear20 = GammaToLinearSpace( temp_cast_1 );
				float4 lerpResult9 = lerp( Var_FogDist_Color14 , Var_FogHeight_Color16 , float4( gammaToLinear20 , 0.0 ));
				
				
				finalColor = lerpResult9;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19202
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;100;5;XRS/XRS_Skybox_BkGd;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;0;1;False;;0;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.CommentaryNode;1;-3643.808,376.9439;Inherit;False;1910.69;790.1998;;14;18;17;16;15;14;13;12;8;7;6;5;4;3;2;Height Fog Amount;1,1,1,1;0;0
Node;AmplifyShaderEditor.TFHCRemapNode;2;-3018.139,499.1304;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-2769.609,671.5847;Inherit;False;Property;_Fog_Height_Offset;Fog_Height_Offset;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-2837.542,579.9528;Inherit;False;Property;_Fog_Height_Power;Fog_Height_Power;5;0;Create;True;0;0;0;False;0;False;1.5;0.25;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-3244.262,651.645;Inherit;False;Property;_Fog_Height_MinY;Fog_Height_MinY;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-3253.863,570.4267;Inherit;False;Property;_Fog_Height_MaxY;Fog_Height_MaxY;4;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;7;-2505.173,505.9417;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-3372.861,499.3754;Inherit;False;Var_World_Y;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;9;-261.7523,-0.2696664;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;10;-619.502,0.1572447;Inherit;False;14;Var_FogDist_Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;11;-607.2991,76.1758;Inherit;False;16;Var_FogHeight_Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-2009.265,505.0991;Inherit;False;Var_FogHeight_Amount;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;13;-3614.751,771.8553;Inherit;False;Property;_Fog_Distance_Color;Fog_Distance_Color;0;2;[HDR];[Header];Create;True;1;Far Fog Settings;0;0;False;0;False;1,1,1,1;0.1563674,1.616936,2.297397,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;15;-3613.394,965.75;Inherit;False;Property;_Fog_Height_Color;Fog_Height_Color;1;2;[HDR];[Header];Create;True;1;Height Fog Settings;0;0;False;0;False;1,1,1,1;0.1563674,1.616936,2.297397,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;17;-2208.928,505.6707;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;18;-3581.392,451.474;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GammaToLinearNode;20;-611.677,189.1669;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;14;-3363.815,769.6022;Inherit;False;Var_FogDist_Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;-3371.815,965.0504;Inherit;False;Var_FogHeight_Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;19;-857.3652,189.4514;Inherit;False;12;Var_FogHeight_Amount;1;0;OBJECT;;False;1;FLOAT;0
WireConnection;0;0;9;0
WireConnection;2;0;8;0
WireConnection;2;1;6;0
WireConnection;2;2;5;0
WireConnection;7;0;2;0
WireConnection;7;1;4;0
WireConnection;7;2;3;0
WireConnection;8;0;18;2
WireConnection;9;0;10;0
WireConnection;9;1;11;0
WireConnection;9;2;20;0
WireConnection;12;0;17;0
WireConnection;17;0;7;0
WireConnection;20;0;19;0
WireConnection;14;0;13;0
WireConnection;16;0;15;0
ASEEND*/
//CHKSM=446F3630960633669711B6029971A392B87469E0