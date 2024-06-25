// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FX_EnvFog_Trees"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_T2D_BC("T2D_BC", 2D) = "white" {}
		[HDR][Header(Far Fog Settings)]_Fog_Distance_Color("Fog_Distance_Color", Color) = (1,1,1,1)
		[HDR][Header(Height Fog Settings)]_Fog_Height_Color("Fog_Height_Color", Color) = (1,1,1,1)
		_Grid_Color("Grid_Color", Color) = (0,0,0,0)
		_Fog_Dist_FadeOffset("Fog_Dist_FadeOffset", Float) = 2
		_Fog_Dist_FadeLength("Fog_Dist_FadeLength", Float) = 5
		_Fog_Height_Offset("Fog_Height_Offset", Float) = 0
		_Fog_Height_MinY("Fog_Height_MinY", Float) = 0
		_Fog_Height_MaxY("Fog_Height_MaxY", Float) = 0
		_Fog_Height_Distance("Fog_Height_Distance", Range( 0 , 1)) = 0.7
		_Fog_Height_Power("Fog_Height_Power", Range( 0 , 5)) = 1.5
		_Grid_Scale("Grid_Scale", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float eyeDepth;
			float3 worldPos;
		};

		uniform float4 _Grid_Color;
		uniform sampler2D _T2D_BC;
		uniform float _Grid_Scale;
		uniform float4 _Fog_Distance_Color;
		uniform float _Fog_Dist_FadeLength;
		uniform float _Fog_Dist_FadeOffset;
		uniform float4 _Fog_Height_Color;
		uniform float _Fog_Height_Distance;
		uniform float _Fog_Height_MaxY;
		uniform float _Fog_Height_MinY;
		uniform float _Fog_Height_Power;
		uniform float _Fog_Height_Offset;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 color48 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
			float2 temp_cast_0 = (_Grid_Scale).xx;
			float2 uv_TexCoord44 = i.uv_texcoord * temp_cast_0;
			float4 tex2DNode42 = tex2D( _T2D_BC, uv_TexCoord44 );
			float4 lerpResult49 = lerp( _Grid_Color , color48 , tex2DNode42);
			float4 Var_Albedo11 = lerpResult49;
			float4 Var_FogDist_Color9 = _Fog_Distance_Color;
			float cameraDepthFade18 = (( i.eyeDepth -_ProjectionParams.y - _Fog_Dist_FadeOffset ) / _Fog_Dist_FadeLength);
			float Var_FogFar_Depth32 = saturate( cameraDepthFade18 );
			float4 lerpResult37 = lerp( ( Var_Albedo11 * Var_FogDist_Color9 ) , Var_FogDist_Color9 , Var_FogFar_Depth32);
			float4 Var_FogHeight_Color8 = _Fog_Height_Color;
			float cameraDepthFade13 = (( i.eyeDepth -_ProjectionParams.y - ( _Fog_Height_Distance * _Fog_Dist_FadeOffset ) ) / ( _Fog_Height_Distance * _Fog_Dist_FadeLength ));
			float Var_FogFar_HeightWeighted31 = saturate( cameraDepthFade13 );
			float3 ase_worldPos = i.worldPos;
			float Var_World_Y27 = ase_worldPos.y;
			float Var_FogHeight_Amount29 = saturate( ( Var_FogFar_HeightWeighted31 * ((0.0 + (Var_World_Y27 - _Fog_Height_MaxY) * (1.0 - 0.0) / (_Fog_Height_MinY - _Fog_Height_MaxY))*_Fog_Height_Power + _Fog_Height_Offset) ) );
			float4 lerpResult40 = lerp( lerpResult37 , Var_FogHeight_Color8 , Var_FogHeight_Amount29);
			o.Emission = lerpResult40.rgb;
			o.Alpha = 1;
			float Var_Alpha52 = tex2DNode42.a;
			clip( Var_Alpha52 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19202
Node;AmplifyShaderEditor.CommentaryNode;1;-3152.592,-76.5687;Inherit;False;1868.123;410.6414;;12;30;29;28;27;26;25;24;23;22;21;20;19;Height Fog Amount;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;2;-3153.712,-470.2694;Inherit;False;1221.269;289.1048;Height based contrarily to dist;6;31;17;16;13;6;4;Height Fog Scale;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;3;-3159.42,-874.2744;Inherit;False;1219.36;273.3727;FAR FROM CAMERA 0-1;5;32;18;10;7;5;Distance Fog;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-2763.443,-420.2694;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-3107.236,-706.7245;Inherit;False;Property;_Fog_Dist_FadeOffset;Fog_Dist_FadeOffset;5;0;Create;True;1;Fade Away when getting to far away;0;0;False;0;False;2;14.7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;6;-2362.259,-406.6032;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;7;-2496.862,-817.7476;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-3109.42,-803.6011;Inherit;False;Property;_Fog_Dist_FadeLength;Fog_Dist_FadeLength;6;0;Create;True;0;0;0;False;0;False;5;27.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;12;-3950.051,-543.6453;Inherit;False;Property;_Fog_Distance_Color;Fog_Distance_Color;2;2;[HDR];[Header];Create;True;1;Far Fog Settings;0;0;False;0;False;1,1,1,1;0.7011021,0.7011021,0.7011021,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CameraDepthFade;13;-2619.417,-410.5357;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;15;-3932.729,-356.8441;Inherit;False;Property;_Fog_Height_Color;Fog_Height_Color;3;2;[HDR];[Header];Create;True;1;Height Fog Settings;0;0;False;0;False;1,1,1,1;0.6514058,0.6514058,0.6514058,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-2770.155,-316.1647;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-3103.712,-378.6726;Inherit;False;Property;_Fog_Height_Distance;Fog_Height_Distance;10;0;Create;True;0;0;0;False;0;False;0.7;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;18;-2773.42,-823.6011;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;19;-2539.339,45.61774;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;20;-1685.12,-5.84211;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-2290.809,218.0726;Inherit;False;Property;_Fog_Height_Offset;Fog_Height_Offset;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-2358.741,126.4403;Inherit;False;Property;_Fog_Height_Power;Fog_Height_Power;11;0;Create;True;0;0;0;False;0;False;1.5;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;23;-2106.702,-26.56863;Inherit;False;31;Var_FogFar_HeightWeighted;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-2765.462,198.1328;Inherit;False;Property;_Fog_Height_MinY;Fog_Height_MinY;8;0;Create;True;0;0;0;False;0;False;0;-10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-2775.063,116.9143;Inherit;False;Property;_Fog_Height_MaxY;Fog_Height_MaxY;9;0;Create;True;0;0;0;False;0;False;0;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;26;-2026.37,52.42906;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;27;-2898.061,37.86273;Inherit;False;Var_World_Y;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1820.492,-8.996319;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;29;-1555.46,-10.41371;Inherit;False;Var_FogHeight_Amount;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;30;-3102.592,-2.038794;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-2229.443,-412.4076;Inherit;False;Var_FogFar_HeightWeighted;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;41;-1104.972,-51.06876;Inherit;False;1001.914;361.0467;note: since we do height after distance this gives priority to height based tint;9;40;39;38;37;36;35;34;46;51;Distance And Height Mode;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;37;-731.4742,-1.068697;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;38;-583.861,111.4419;Inherit;False;8;Var_FogHeight_Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;39;-589.6755,193.9784;Inherit;False;29;Var_FogHeight_Amount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;9;-3699.108,-545.8984;Inherit;False;Var_FogDist_Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-3674.019,-360.2182;Inherit;False;Var_FogHeight_Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;11;-3707.693,-654.5994;Inherit;False;Var_Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-3877.302,-123.5725;Inherit;False;Property;_FogDist_Ammount;FogDist_Ammount;12;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;33;-3603.231,-121.9187;Inherit;False;Var_FogDist_Amount;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;32;-2304.988,-820.9396;Inherit;False;Var_FogFar_Depth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;-1019.025,181.3523;Inherit;False;32;Var_FogFar_Depth;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;42;-4343.331,-651.5032;Inherit;True;Property;_T2D_BC;T2D_BC;1;0;Create;True;0;0;0;False;0;False;-1;018becf40e7ee8a4c84343927f91ee23;400c0ba8b4c11c343a582e6f17b44fd9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;44;-4660.286,-627.212;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;45;-4949.286,-602.212;Inherit;False;Property;_Grid_Scale;Grid_Scale;13;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-885.8716,-0.6026611;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;35;-1094.971,114.5511;Inherit;False;9;Var_FogDist_Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;34;-1091.5,2.654728;Inherit;False;11;Var_Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;40;-286.061,0.295197;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;49;-3994.963,-812.879;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;48;-4304.836,-829.4611;Inherit;False;Constant;_Grid_Color_B;Grid_Color_B;13;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;47;-4303.803,-1007.718;Inherit;False;Property;_Grid_Color;Grid_Color;4;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.764151,0.764151,0.764151,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;50;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;FX_EnvFog_Trees;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;52;-3999.964,-6.252899;Inherit;False;Var_Alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;-357.5343,372.8145;Inherit;False;52;Var_Alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-277.0078,204.8159;Inherit;False;Constant;_Float0;Float 0;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
WireConnection;4;0;17;0
WireConnection;4;1;10;0
WireConnection;6;0;13;0
WireConnection;7;0;18;0
WireConnection;13;0;4;0
WireConnection;13;1;16;0
WireConnection;16;0;17;0
WireConnection;16;1;5;0
WireConnection;18;0;10;0
WireConnection;18;1;5;0
WireConnection;19;0;27;0
WireConnection;19;1;25;0
WireConnection;19;2;24;0
WireConnection;20;0;28;0
WireConnection;26;0;19;0
WireConnection;26;1;22;0
WireConnection;26;2;21;0
WireConnection;27;0;30;2
WireConnection;28;0;23;0
WireConnection;28;1;26;0
WireConnection;29;0;20;0
WireConnection;31;0;6;0
WireConnection;37;0;46;0
WireConnection;37;1;35;0
WireConnection;37;2;36;0
WireConnection;9;0;12;0
WireConnection;8;0;15;0
WireConnection;11;0;49;0
WireConnection;33;0;43;0
WireConnection;32;0;7;0
WireConnection;42;1;44;0
WireConnection;44;0;45;0
WireConnection;46;0;34;0
WireConnection;46;1;35;0
WireConnection;40;0;37;0
WireConnection;40;1;38;0
WireConnection;40;2;39;0
WireConnection;49;0;47;0
WireConnection;49;1;48;0
WireConnection;49;2;42;0
WireConnection;50;2;40;0
WireConnection;50;10;53;0
WireConnection;52;0;42;4
ASEEND*/
//CHKSM=D88C71D54D4B88E6600827772515C4F755D0527A