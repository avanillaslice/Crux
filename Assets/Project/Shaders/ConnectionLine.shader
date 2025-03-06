Shader "Custom/ConnectionLine"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,1)
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _EmissionIntensity ("Emission Intensity", Range(0, 10)) = 1
        
        // Pulse properties
        _PulsePosition ("Pulse Position", Range(0, 1)) = 0
        _PulseColor ("Pulse Color", Color) = (1,1,1,1)
        _PulseWidth ("Pulse Width", Range(0.01, 0.5)) = 0.1
        _PulseEmissionColor ("Pulse Emission Color", Color) = (1,1,1,1)
        _PulseEmissionIntensity ("Pulse Emission Intensity", Range(0, 10)) = 2
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float lineCoord : TEXCOORD1;
            };
            
            float4 _Color;
            float4 _EmissionColor;
            float _EmissionIntensity;
            
            float _PulsePosition;
            float4 _PulseColor;
            float _PulseWidth;
            float4 _PulseEmissionColor;
            float _PulseEmissionIntensity;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                
                // LineRenderer passes progress along the line in the U coordinate
                o.lineCoord = v.uv.x;
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate distance from current fragment to pulse position
                float distanceToPulse = abs(i.lineCoord - _PulsePosition);
                
                // Create a smooth pulse effect
                float pulseEffect = saturate((_PulseWidth - distanceToPulse) / _PulseWidth);
                
                // Blend between base color and pulse color
                float4 baseColor = _Color * i.color;
                float4 finalColor = lerp(baseColor, _PulseColor, pulseEffect);
                
                // Calculate emission
                float4 baseEmission = _EmissionColor * _EmissionIntensity;
                float4 pulseEmission = _PulseEmissionColor * _PulseEmissionIntensity;
                float4 finalEmission = lerp(baseEmission, pulseEmission, pulseEffect);
                
                // Add emission to color
                finalColor.rgb += finalEmission.rgb;
                
                // Preserve alpha from base color
                return float4(finalColor.rgb, baseColor.a);
            }
            ENDCG
        }
    }
    
    Fallback "Particles/Additive"
} 