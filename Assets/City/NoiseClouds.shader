// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Julian/NoiseClouds"
{
	Properties
	{
		_NoiseScale("NoiseScale", Float) = 10
		_NoiseScale3("NoiseScale3", Float) = 10
		_NoiseScale2("NoiseScale2", Float) = 10
		_NoiseDirection("NoiseDirection", Vector) = (1,1,1,0)
		_Vector0("Vector 0", Vector) = (1,1,1,0)
		_TimeScaleNoise("TimeScaleNoise", Float) = 1
		_Float1("Float 1", Float) = 1
		_3DNoisceScale("3DNoisceScale", Vector) = (1,1,1,0)
		_Vector1("Vector 1", Vector) = (1,1,1,0)
		[HDR]_Color("Color", Color) = (1,1,1,1)
		_Transparency("Transparency", 2D) = "white" {}
		_SunStrength("SunStrength", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow noambient nofog nometa noforwardadd 
		struct Input
		{
			float3 worldPos;
			float3 viewDir;
			float2 uv_texcoord;
		};

		uniform half _NoiseScale3;
		uniform half3 _NoiseDirection;
		uniform half _TimeScaleNoise;
		uniform half3 _3DNoisceScale;
		uniform half _NoiseScale;
		uniform half _SunStrength;
		uniform half4 _Color;
		uniform half3 _Vector0;
		uniform half _Float1;
		uniform half3 _Vector1;
		uniform half _NoiseScale2;
		uniform sampler2D _Transparency;
		uniform half4 _Transparency_ST;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			half2 appendResult322 = (half2(ase_worldPos.x , ase_worldPos.z));
			half simplePerlin2D293 = snoise( appendResult322*_NoiseScale3 );
			simplePerlin2D293 = simplePerlin2D293*0.5 + 0.5;
			half mulTime231 = _Time.y * _TimeScaleNoise;
			half simplePerlin3D119 = snoise( ( ( _NoiseDirection * mulTime231 ) + ( _3DNoisceScale * ase_worldPos ) )*_NoiseScale );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			half3 ase_worldlightDir = 0;
			#else //aseld
			half3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			half dotResult302 = dot( i.viewDir , -ase_worldlightDir );
			half lerpResult318 = lerp( 1.0 , saturate( dotResult302 ) , _SunStrength);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			half4 ase_lightColor = 0;
			#else //aselc
			half4 ase_lightColor = _LightColor0;
			#endif //aselc
			o.Emission = ( ( simplePerlin2D293 + simplePerlin3D119 ) * lerpResult318 * _Color * saturate( ase_lightColor ) ).rgb;
			half mulTime273 = _Time.y * _Float1;
			half simplePerlin3D281 = snoise( ( ( _Vector0 * mulTime273 ) + ( _Vector1 * ase_worldPos ) )*_NoiseScale2 );
			half blendOpSrc282 = ( simplePerlin3D281 + simplePerlin2D293 );
			half blendOpDest282 = ( simplePerlin3D119 * 3.0 );
			float2 uv_Transparency = i.uv_texcoord * _Transparency_ST.xy + _Transparency_ST.zw;
			o.Alpha = ( ( saturate(  round( 0.5 * ( blendOpSrc282 + blendOpDest282 ) ) )) * tex2D( _Transparency, uv_Transparency ) * _Color.a ).r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
-280;695;1906;893;-579.8984;44.28409;1.302837;True;True
Node;AmplifyShaderEditor.RangedFloatNode;232;1272.557,-316.2724;Inherit;False;Property;_TimeScaleNoise;TimeScaleNoise;7;0;Create;True;0;0;0;False;0;False;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;272;1293.987,271.7246;Inherit;False;Property;_Float1;Float 1;8;0;Create;True;0;0;0;False;0;False;1;-0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;230;1519.669,-470.8026;Inherit;False;Property;_NoiseDirection;NoiseDirection;5;0;Create;True;0;0;0;False;0;False;1,1,1;1,0.5,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleTimeNode;273;1525.881,272.8371;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;226;1525.894,-78.75717;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;275;1527.324,509.2398;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;274;1521.347,355.395;Inherit;False;Property;_Vector1;Vector 1;10;0;Create;True;0;0;0;False;0;False;1,1,1;0.7,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;276;1521.099,117.1944;Inherit;False;Property;_Vector0;Vector 0;6;0;Create;True;0;0;0;False;0;False;1,1,1;3,1,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;234;1519.917,-232.602;Inherit;False;Property;_3DNoisceScale;3DNoisceScale;9;0;Create;True;0;0;0;False;0;False;1,1,1;0.2,0.5,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleTimeNode;231;1524.451,-315.1599;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;300;1883.979,-1173.504;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;287;1641.838,714.3197;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;277;1725.117,396.5;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;233;1721.948,-381.6336;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;278;1723.378,206.3634;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;235;1723.687,-191.497;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;279;1911.422,203.4991;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NegateNode;303;2102.713,-1143.477;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;280;1904.774,329.9839;Inherit;False;Property;_NoiseScale2;NoiseScale2;4;0;Create;True;0;0;0;False;0;False;10;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;292;1936.892,921.8698;Inherit;False;Property;_NoiseScale3;NoiseScale3;3;0;Create;True;0;0;0;False;0;False;10;100;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;301;1975.313,-1314.277;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;128;1903.344,-258.0131;Inherit;False;Property;_NoiseScale;NoiseScale;2;0;Create;True;0;0;0;False;0;False;10;0.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;322;1908.671,808.7627;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;236;1909.992,-384.4979;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;293;2121.676,786.6154;Inherit;True;Simplex2D;True;True;2;0;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;281;2089.558,194.7296;Inherit;True;Simplex3D;False;True;2;0;FLOAT3;1,1,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;119;2088.128,-393.2674;Inherit;True;Simplex3D;False;True;2;0;FLOAT3;1,1,1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;302;2281.313,-1248.277;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;283;2392.369,-265.4269;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;304;2431.313,-1277.577;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;296;2382.861,94.3317;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;305;2305.693,-549.1899;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;319;2237.819,-1056.528;Inherit;False;Property;_SunStrength;SunStrength;16;0;Create;True;0;0;0;False;0;False;1;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;314;2749.633,-479.4523;Inherit;True;Property;_Transparency;Transparency;15;0;Create;True;0;0;0;False;0;False;-1;None;0cec3948eed2c3d47b640ef2666b460f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;318;2613.519,-1109.828;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;298;2494.229,287.3094;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;259;2413.148,-871.6698;Inherit;False;Property;_Color;Color;11;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1.863926,1.766892,1.534011,0.2980392;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendOpsNode;282;2684.215,-189.2257;Inherit;True;HardMix;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;320;2478.319,-556.0277;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;264;2006.453,-655.3758;Inherit;False;Property;_WaveStrength;WaveStrength;14;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;248;1594.603,-926.7928;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.75,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;267;1429.967,-932.8452;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;1,1,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector2Node;255;1356.25,-791.6214;Float;False;Property;_WaveDirection;WaveDirection;1;0;Create;True;0;0;0;False;0;False;1,0;1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;250;1587.088,-705.0889;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;260;2684.438,-651.3824;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;315;3064.124,-372.0154;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;258;1804.114,-633.8912;Inherit;False;Property;_WaveScale;WaveScale;13;0;Create;True;0;0;0;False;0;False;1;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;269;1311.082,-1115.638;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;254;2056.327,-897.817;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;253;1847.038,-869.3438;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;256;1246.874,-944.6548;Inherit;False;Property;_WaveTiling;WaveTiling;12;0;Create;True;0;0;0;False;0;False;0,0;0.3,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.WorldPosInputsNode;268;1075.653,-1127.052;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;263;2292.188,-718.7296;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;251;1378.633,-650.0377;Float;False;Property;_WaveSpeed;WaveSpeed;0;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3213.986,-634.5416;Half;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Julian/NoiseClouds;False;False;False;False;True;False;False;False;False;True;True;True;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;273;0;272;0
WireConnection;231;0;232;0
WireConnection;277;0;274;0
WireConnection;277;1;275;0
WireConnection;233;0;230;0
WireConnection;233;1;231;0
WireConnection;278;0;276;0
WireConnection;278;1;273;0
WireConnection;235;0;234;0
WireConnection;235;1;226;0
WireConnection;279;0;278;0
WireConnection;279;1;277;0
WireConnection;303;0;300;0
WireConnection;322;0;287;1
WireConnection;322;1;287;3
WireConnection;236;0;233;0
WireConnection;236;1;235;0
WireConnection;293;0;322;0
WireConnection;293;1;292;0
WireConnection;281;0;279;0
WireConnection;281;1;280;0
WireConnection;119;0;236;0
WireConnection;119;1;128;0
WireConnection;302;0;301;0
WireConnection;302;1;303;0
WireConnection;283;0;119;0
WireConnection;304;0;302;0
WireConnection;296;0;281;0
WireConnection;296;1;293;0
WireConnection;318;1;304;0
WireConnection;318;2;319;0
WireConnection;298;0;293;0
WireConnection;298;1;119;0
WireConnection;282;0;296;0
WireConnection;282;1;283;0
WireConnection;320;0;305;0
WireConnection;248;0;267;0
WireConnection;267;0;256;0
WireConnection;267;1;269;0
WireConnection;250;0;251;0
WireConnection;260;0;298;0
WireConnection;260;1;318;0
WireConnection;260;2;259;0
WireConnection;260;3;320;0
WireConnection;315;0;282;0
WireConnection;315;1;314;0
WireConnection;315;2;259;4
WireConnection;269;0;268;1
WireConnection;269;1;268;3
WireConnection;254;0;253;0
WireConnection;254;1;258;0
WireConnection;253;0;248;0
WireConnection;253;2;255;0
WireConnection;253;1;250;0
WireConnection;263;0;254;0
WireConnection;263;1;264;0
WireConnection;0;2;260;0
WireConnection;0;9;315;0
ASEEND*/
//CHKSM=71BF16C20B4676281E194DEEC54D54A028632835