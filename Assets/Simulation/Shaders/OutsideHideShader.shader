Shader "Unlit/OutsideHideShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Main Color", Color) = (0.5,0.5,0.5,0.5)
    }
    SubShader
    {
        Tags 
        {
            "Queue" = "Transparent"
            "RenderType"="Transparent" 
        }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Front  // Let both faces through

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // Define our color of the vertex here for the fragment shader
                o.color = _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture and color
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                return col;
            }
            ENDCG
        }
    }
}
