Shader "Unlit/Volume_Render"
{
    Properties
    {
      
    }
	SubShader
	{
        //AlphaTest Greater 0.3
        Tags{ "RenderType" = "Transparent" }
		Pass
		{  
            Cull Off
             // Blend Off // Default
            Blend SrcAlpha OneMinusSrcAlpha// Standard Transparency
            //Blend SrcAlpha One
            //Blend SrcAlpha OneMinusSrcAlpha
            //Blend One Zero,One Zero
            //Blend One OneMinusSrcAlpha, One Zero // Back to Front Compositing
            //ZWrite Off
            ZTest Off

            //Blend OneMinusDstAlpha One // Front to Back Compositing
            //BlendOp Max


			CGPROGRAM
			#pragma vertex vert //Vertex Shader will be called vert
			#pragma fragment frag //Fragment Shader will be called frag
            #define DELTA (0.01)
            #define THRESHOLD (0.1)

			
			#include "UnityCG.cginc"

			struct vin      //Sruct for the input vertices
			{
				float4 vertex : POSITION;
                float3 normal : NORMAL ;
			};

			struct vout     // Struct for the output vertices from slice_vertices function
            {
				float4 VertexOut: SV_POSITION;
                float3 normal : NORMAL ;
                float3 TexCoordOut : TEXCOORD0;
			};
           
            // Access the parameters sent by the script

            uniform float   _dPlaneStart; // The starting value of Plane
            uniform float   _dPlaneIncr; // Increment i.e distance between planes
            uniform int     _frontIdx;      // Given a position of camera , the index on bounding box that is nearest
            uniform float4  _vecView;  // The viewing direction
            uniform float4  _vecTranslate; // The origin needs to be shifted to camera's origin in local coord
            uniform float4  _vecVertices[8];  // The vertices of bounding box

            vout vert(vin Vinput) // The vertex Shader : Input - Input stream of vertices || Output : Transformed vertices
            {
                int v1[24] =        {   0,1,4,3,
                                        1,0,1,4,
                                        0,2,5,3,
                                        2,0,2,5,
                                        0,3,6,3,
                                        3,0,3,6  }; // Look up matirx containing the first vertex of edges to be checked

                int v2[24] =        {   1,4,7,3,
                                        5,1,4,7,
                                        2,5,7,3,
                                        6,2,5,7,
                                        3,6,7,3,
                                        4,3,6,7  }; // Look up matirx containing the second vertex of edges to be checked

                int nSequence[64]= {     0,1,2,3,4,5,6,7,
                                         1,4,5,0,3,7,2,6,
                                         2,6,0,5,7,3,1,4,
                                         3,0,6,4,1,2,7,5,
                                         4,3,7,1,0,6,5,2,
                                         5,2,1,7,6,0,4,3,
                                         6,7,3,2,5,4,0,1,
                                         7,5,4,6,2,1,3,0   }; // Permuataions of all the vertices of bounding box based on the front index
                vout Output; // The output of the vertex Shader

                float dPlane =_dPlaneStart + int(Vinput.vertex[1]) *_dPlaneIncr; // Finding the distance of plane to be evaluated from the origin
                float4 Position; // Point of intersection

                _vecView[3]=0; // 4th value in the vector should be zero
                _vecTranslate[3]=0;  // 4th value in the vector should be zero

                for(int e=0;e<4;++e)
                {
                    int vidx1 = nSequence[int(_frontIdx * 8 + v1[int(Vinput.vertex[0]) * 4 +e ])]; // According to the viewing direction, the index of the first vertex to be checked
                    int vidx2 = nSequence[int(_frontIdx * 8 + v2[int(Vinput.vertex[0]) * 4 +e ])]; // According to the viewing direction, the index of the second vertex to be checked
                    float4 vecV1 = _vecVertices[vidx1]; // 1st Vertex of edge to be checked for intersection
                    float4 vecV2 = _vecVertices[vidx2]; // 2nd Vertex of edge to be checked for intersection
                    float4 vecStart = vecV1 + _vecTranslate; // Translating origin to local camera position
                    // Note that if local cam is at (x,y,z) vecTranslate should be (-x,-y,-z)
                    float4 vecDir = vecV2 - vecV1;
                    float denom = dot(vecDir,_vecView);
                    float lambda = (denom!=0.0) ? (dPlane-dot(vecStart,_vecView))/denom : -1.0; // If denom is 0, lambda is -1.
                    if((lambda >=0.0 )&&(lambda <= 1.0)) // Condition that makes sure the intersection is happening between the vertices
                    {
                        Position = vecStart + lambda * vecDir; // Calculate the point of intersection

                        break;
                    }
                }
            
               Output.VertexOut = Position-_vecTranslate; // Translate origin back
               Output.VertexOut= UnityObjectToClipPos(Output.VertexOut); // Convert from local to world and then clip
               Position=Position-_vecTranslate;
               Output.TexCoordOut[0] = (Position[0])/_vecVertices[0][0] ; // normalizing and assigning UVW coords for each slice
               Output.TexCoordOut[1] = (Position[1])/_vecVertices[0][1] ;
               Output.TexCoordOut[2] = (Position[2])/_vecVertices[0][2] ;
               return Output;
            }
			sampler3D _3dTexture;
            sampler2D _transfer_function;
			fixed4 frag (vout i) : SV_Target
			{
				// sample the texture
                fixed4 col_index =tex3D(_3dTexture, i.TexCoordOut);
                float2 temp={float(col_index[0]),0};

                //fixed4 col = tex2D(_transfer_function,temp);

                 

                      // fixed4 col = fixed4(0.0f,0.0,0.0,0.0f);
                       fixed4 col=fixed4(col_index[0],col_index[0],col_index[0],col_index[0]);
                       return col;
                 
           }
			ENDCG
		}
	}
}
