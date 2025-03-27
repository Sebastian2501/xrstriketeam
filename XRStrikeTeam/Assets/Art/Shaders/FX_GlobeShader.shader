// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "OneAccenture/FX/FX_GlobeShader"
{
	Properties
	{
		_T2D_BC("T2D_BC", 2D) = "white" {}
		_T3D_HDR("T3D_HDR", CUBE) = "white" {}
		_T2D_N("T2D_N", 2D) = "bump" {}
		_Color_Shore("Color_Shore", Color) = (0,0,0,0)
		_Color_Ocean("Color_Ocean", Color) = (0,0,0,0)
		_Refl_MIP("Refl_MIP", Float) = 4
		_Water_Scale_01("Water_Scale_01", Float) = 1
		_Water_Scale_02("Water_Scale_02", Float) = 1
		_Water_Scale_03("Water_Scale_03", Float) = 1
		_TimeScale("Time Scale", Float) = 1
		_Refl_Strength("Refl_Strength", Range( 0 , 1)) = 0.09782609
		_Emission("Emission", Float) = 0
		[HDR][Header(Atmosphere)]_Color_Atmo("Color_Atmo", Color) = (0,0,0,0)
		_Atmo_Bias("Atmo_Bias", Float) = 0
		_Atmo_Scale("Atmo_Scale", Float) = 1
		_Atmo_Power("Atmo_Power", Float) = 5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityStandardBRDF.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.5
		#define ASE_VERSION 19801
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform sampler2D _T2D_BC;
		uniform float4 _T2D_BC_ST;
		uniform samplerCUBE _T3D_HDR;
		uniform sampler2D _T2D_N;
		uniform float _TimeScale;
		uniform float _Water_Scale_01;
		uniform float _Refl_MIP;
		uniform float _Water_Scale_02;
		uniform float _Water_Scale_03;
		uniform float _Refl_Strength;
		uniform float4 _Color_Shore;
		uniform float4 _Color_Ocean;
		uniform float4 _Color_Atmo;
		uniform float _Atmo_Bias;
		uniform float _Atmo_Scale;
		uniform float _Atmo_Power;
		uniform float _Emission;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_T2D_BC = i.uv_texcoord * _T2D_BC_ST.xy + _T2D_BC_ST.zw;
			float4 tex2DNode202 = tex2D( _T2D_BC, uv_T2D_BC );
			float3 ase_positionWS = i.worldPos;
			float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - ase_positionWS );
			float3 ase_viewDirSafeWS = Unity_SafeNormalize( ase_viewVectorWS );
			float3 appendResult210 = (float3(ase_viewDirSafeWS.x , ( ase_viewDirSafeWS.y * -1.0 ) , ase_viewDirSafeWS.z));
			float mulTime236 = _Time.y * _TimeScale;
			float2 appendResult169 = (float2(( _Water_Scale_01 * 2.0 ) , _Water_Scale_01));
			float2 uv_TexCoord159 = i.uv_texcoord * appendResult169;
			float2 panner158 = ( mulTime236 * float2( 0,0.1 ) + uv_TexCoord159);
			float2 appendResult172 = (float2(( _Water_Scale_02 * 2.0 ) , _Water_Scale_02));
			float2 uv_TexCoord166 = i.uv_texcoord * appendResult172;
			float2 panner160 = ( mulTime236 * float2( 0.1,0 ) + uv_TexCoord166);
			float2 appendResult175 = (float2(( _Water_Scale_03 * 2.0 ) , _Water_Scale_03));
			float2 uv_TexCoord167 = i.uv_texcoord * appendResult175;
			float2 panner165 = ( mulTime236 * float2( -0.05,0 ) + uv_TexCoord167);
			float3 temp_output_161_0 = BlendNormals( UnpackNormal( tex2Dlod( _T2D_N, float4( panner158, 0, _Refl_MIP) ) ) , BlendNormals( UnpackNormal( tex2Dlod( _T2D_N, float4( panner160, 0, _Refl_MIP) ) ) , UnpackNormal( tex2Dlod( _T2D_N, float4( panner165, 0, _Refl_MIP) ) ) ) );
			float3 lerpResult215 = lerp( ( temp_output_161_0 * 0 ) , temp_output_161_0 , _Refl_Strength);
			float Var_SeaMask150 = tex2DNode202.a;
			float4 lerpResult182 = lerp( _Color_Shore , _Color_Ocean , Var_SeaMask150);
			float4 Var_WaterOut154 = ( ( texCUBElod( _T3D_HDR, float4( ( appendResult210 + lerpResult215 ), 6.5) ) * 0.2 ) + lerpResult182 );
			float4 lerpResult151 = lerp( float4( tex2DNode202.rgb , 0.0 ) , Var_WaterOut154 , Var_SeaMask150);
			float3 ase_viewDirWS = normalize( ase_viewVectorWS );
			float3 ase_normalWS = i.worldNormal;
			float fresnelNdotV231 = dot( ase_normalWS, ase_viewDirWS );
			float fresnelNode231 = ( _Atmo_Bias + _Atmo_Scale * pow( 1.0 - fresnelNdotV231, _Atmo_Power ) );
			float4 lerpResult230 = lerp( lerpResult151 , ( _Color_Atmo + lerpResult151 ) , saturate( fresnelNode231 ));
			o.Albedo = lerpResult230.rgb;
			o.Emission = ( lerpResult151 * _Emission ).rgb;
			float4 break186 = Var_WaterOut154;
			o.Smoothness = saturate( ( ( break186.r + break186.g ) * Var_SeaMask150 ) );
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.5
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
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
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
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
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
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.CommentaryNode;221;-5032.92,650.4744;Inherit;False;3747.999;1228.275;Water and Reflection;38;160;165;166;172;173;175;174;157;164;158;159;169;170;171;163;168;161;184;154;180;181;83;179;177;176;167;182;206;156;183;216;209;210;215;219;236;237;239;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;174;-4982.92,1413.749;Inherit;False;Property;_Water_Scale_02;Water_Scale_02;7;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;177;-4982.92,1621.749;Inherit;False;Property;_Water_Scale_03;Water_Scale_03;8;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;173;-4790.92,1333.748;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;171;-4982.92,1205.748;Inherit;False;Property;_Water_Scale_01;Water_Scale_01;6;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;176;-4790.92,1541.749;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;172;-4550.92,1333.748;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;175;-4550.92,1541.749;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;170;-4790.92,1125.747;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;237;-4430.455,1708.91;Inherit;False;Property;_TimeScale;Time Scale;9;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;166;-4358.92,1317.748;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;2,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;169;-4550.92,1125.747;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;167;-4358.92,1525.749;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;2,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;236;-4227.151,1708.91;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;160;-3958.924,1301.748;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;165;-3958.924,1525.749;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.05,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;159;-4358.92,1109.747;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;2,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;163;-3926.924,933.7473;Inherit;False;Property;_Refl_MIP;Refl_MIP;5;0;Create;True;0;0;0;False;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;157;-3734.924,1285.748;Inherit;True;Property;_T_Gen_WaterSmooth_N1;T_Gen_WaterSmooth_N;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Instance;156;MipLevel;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;4;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;164;-3734.924,1493.749;Inherit;True;Property;_T_Gen_WaterSmooth_N2;T_Gen_WaterSmooth_N;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Instance;156;MipLevel;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;4;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.PannerNode;158;-3958.924,1109.747;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;156;-3734.924,1077.747;Inherit;True;Property;_T2D_N;T2D_N;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;MipLevel;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;4;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.BlendNormalsNode;168;-3391.924,1380.748;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;206;-3154.085,740.4744;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BlendNormalsNode;161;-3334.924,1077.747;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;209;-2927.085,700.4744;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;216;-3191.085,1189.475;Inherit;False;Property;_Refl_Strength;Refl_Strength;10;0;Create;True;0;0;0;False;0;False;0.09782609;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;219;-3059.084,1027.575;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;202;-663.2038,-424.1247;Inherit;True;Property;_T2D_BC;T2D_BC;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.DynamicAppendNode;210;-2747.085,763.4744;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;215;-2850.485,1052.074;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;240;-2546.29,894.6363;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;150;-332.5345,-294.7445;Inherit;False;Var_SeaMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;244;-2784.361,948.3661;Inherit;False;Constant;_Float0;Float 0;16;0;Create;True;0;0;0;False;0;False;6.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;181;-2454.924,1349.749;Inherit;False;Property;_Color_Shore;Color_Shore;3;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;183;-2230.922,1157.747;Inherit;False;Constant;_Float2;Float 0;24;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;83;-2453.725,1522.976;Inherit;False;Property;_Color_Ocean;Color_Ocean;4;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode;179;-2457.322,1698.601;Inherit;False;150;Var_SeaMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;239;-2419.29,797.6363;Inherit;True;Property;_T3D_HDR;T3D_HDR;1;0;Create;True;0;0;0;False;0;False;-1;03d81cfad4bfcb5478ba502af3616d17;03d81cfad4bfcb5478ba502af3616d17;True;0;False;white;Auto;False;Object;-1;MipLevel;Cube;8;0;SAMPLERCUBE;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;184;-2038.923,1029.747;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;182;-2054.923,1445.749;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;180;-1734.923,1029.747;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;154;-1526.923,1029.747;Inherit;False;Var_WaterOut;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;155;-736,-192;Inherit;False;154;Var_WaterOut;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;234;132.3125,383.118;Inherit;False;Property;_Atmo_Scale;Atmo_Scale;14;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;235;131.0126,453.9185;Inherit;False;Property;_Atmo_Power;Atmo_Power;15;0;Create;True;0;0;0;False;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;233;161.7126,313.4184;Inherit;False;Property;_Atmo_Bias;Atmo_Bias;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;186;-448,-32;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;149;-438.4303,154.5314;Inherit;False;150;Var_SeaMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;151;96,-224;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;231;365.6335,311.7514;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;228;-69.20916,-465.5673;Inherit;False;Property;_Color_Atmo;Color_Atmo;12;2;[HDR];[Header];Create;True;1;Atmosphere;0;0;False;0;False;0,0,0,0;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleAddOpNode;188;-208,-32;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;229;372.504,-468.8805;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;232;663.5257,313.3747;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;187;-64,128;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;227;73.72788,43.70614;Inherit;False;Property;_Emission;Emission;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;42;128,128;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;226;256.3814,18.14219;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;230;966.2247,-218.9106;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;238;1424.12,-30.16726;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;0;Standard;OneAccenture/FX/FX_GlobeShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;173;0;174;0
WireConnection;176;0;177;0
WireConnection;172;0;173;0
WireConnection;172;1;174;0
WireConnection;175;0;176;0
WireConnection;175;1;177;0
WireConnection;170;0;171;0
WireConnection;166;0;172;0
WireConnection;169;0;170;0
WireConnection;169;1;171;0
WireConnection;167;0;175;0
WireConnection;236;0;237;0
WireConnection;160;0;166;0
WireConnection;160;1;236;0
WireConnection;165;0;167;0
WireConnection;165;1;236;0
WireConnection;159;0;169;0
WireConnection;157;1;160;0
WireConnection;157;2;163;0
WireConnection;164;1;165;0
WireConnection;164;2;163;0
WireConnection;158;0;159;0
WireConnection;158;1;236;0
WireConnection;156;1;158;0
WireConnection;156;2;163;0
WireConnection;168;0;157;0
WireConnection;168;1;164;0
WireConnection;161;0;156;0
WireConnection;161;1;168;0
WireConnection;209;0;206;2
WireConnection;219;0;161;0
WireConnection;210;0;206;1
WireConnection;210;1;209;0
WireConnection;210;2;206;3
WireConnection;215;0;219;0
WireConnection;215;1;161;0
WireConnection;215;2;216;0
WireConnection;240;0;210;0
WireConnection;240;1;215;0
WireConnection;150;0;202;4
WireConnection;239;1;240;0
WireConnection;239;2;244;0
WireConnection;184;0;239;0
WireConnection;184;1;183;0
WireConnection;182;0;181;0
WireConnection;182;1;83;0
WireConnection;182;2;179;0
WireConnection;180;0;184;0
WireConnection;180;1;182;0
WireConnection;154;0;180;0
WireConnection;186;0;155;0
WireConnection;151;0;202;5
WireConnection;151;1;155;0
WireConnection;151;2;149;0
WireConnection;231;1;233;0
WireConnection;231;2;234;0
WireConnection;231;3;235;0
WireConnection;188;0;186;0
WireConnection;188;1;186;1
WireConnection;229;0;228;0
WireConnection;229;1;151;0
WireConnection;232;0;231;0
WireConnection;187;0;188;0
WireConnection;187;1;149;0
WireConnection;42;0;187;0
WireConnection;226;0;151;0
WireConnection;226;1;227;0
WireConnection;230;0;151;0
WireConnection;230;1;229;0
WireConnection;230;2;232;0
WireConnection;238;0;230;0
WireConnection;238;2;226;0
WireConnection;238;4;42;0
ASEEND*/
//CHKSM=5358FD97170D17953780B2F18F9B607F15B3B996