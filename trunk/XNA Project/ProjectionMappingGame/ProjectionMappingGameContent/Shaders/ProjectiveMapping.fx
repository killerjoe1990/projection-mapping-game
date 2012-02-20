// Include all shared types
#include "Includes.inc"

// Projection Components
bool projecting;
float4 unprojectedAreaColor = float4(1.0f, 1.0f, 1.0f, 1.0f);
Texture projtex2D;
sampler2D projectiveMap = sampler_state 
{
	Texture = <projtex2D>;
	
   MinFilter = Linear;
   MagFilter = Linear;
   MipFilter = Linear;
	
	AddressU = Clamp;
	AddressV = Clamp;
};
float4x4 textureMatrix = {{0.5f, 0.0f, 0.0f, 0.5f},
						        {0.0f, 0.5f, 0.0f, 0.5f},
						        {0.0f, 0.0f, 0.5f, 0.5f},
						        {0.0f, 0.0f, 0.0f, 1.0f}};

// Shadow Properties
float depthBias = 0.001f;

//
//
// Input/Output
//
//

	struct ProjTexIn {
		float4	position	:	POSITION;
		float3	normal		:	NORMAL;
		float2  texCoord	:	TEXCOORD0;
	};
	
	struct ProjTexOut {
		float4  position		   :	POSITION;
		float2  texCoord	      :  TEXCOORD0;
		float4  Projector_pos	:  TEXCOORD1;
		float4  pos_w			   :	TEXCOORD2;
		float3  N				   :	TEXCOORD3;
	};

//
//
// Vertex Shaders
//
//

	ProjTexOut ProjectiveTexturingVertexShader(ProjTexIn input)
	{
		ProjTexOut output;
		
		// Create world, view, and projection matrix
		float4x4 worldViewProjMatrix = mul(mul(worldMatrix, viewMatrix), projectionMatrix);
		float4x4 projectorViewProjMatrix = mul(projectorViewMatrix, projectorProjectionMatrix);
		
		// Compute output position in projection space
		output.position = mul(input.position, worldViewProjMatrix);
		output.pos_w = mul(input.position, worldMatrix);
		output.Projector_pos = mul(textureMatrix, mul(input.position, mul(worldMatrix, projectorViewProjMatrix)));
		output.N = mul(input.normal, worldMatrix); 
		output.texCoord = input.texCoord;
		
		return output;
	}
	
//
//
// Pixel Shader
//
//

	float4 ProjectiveTexturingPixelShader(ProjTexOut input) : COLOR
	{	
      float4 projColor = unprojectedAreaColor;

      if (projecting)
      {
         input.Projector_pos /= input.Projector_pos.w;
		
		   if (input.Projector_pos.w >= 0
		       && input.Projector_pos.x > 0 && input.Projector_pos.x < 1
		       && input.Projector_pos.y > 0 && input.Projector_pos.y < 1)
         {
			   input.Projector_pos.y = 1.0 - input.Projector_pos.y;
            projColor = tex2Dproj(projectiveMap, input.Projector_pos);
         }
      }	
		
		float4 materialColor = materialEmissiveColor;
		
		// Calculate Diffuse Color
		float3 L = normalize(lightPosition - input.pos_w);
		input.N = normalize(input.N);
		
		float diffuseIntensity = max((0.5f * (dot(L, input.N) + 1)), 0.0f);
		float4 diffuseColor = materialDiffuseColor * diffuseIntensity;
				
		// Calculate Attached Texture Color
		float4 finalColor = 0;
      if (projecting && projColor.w == 1.0f)
      {
         if (diffuseIntensity > 0)
			   finalColor = (projColor * materialColor) + (projColor * diffuseColor);
      }
      else
      {
         if (diffuseIntensity > 0)
			   finalColor = materialColor + diffuseColor;
      }
		
		return finalColor;
	}
	
//
//
// Techniques
//
//

	technique ProjectiveTexturing
	{
		pass Pass0
		{
			VertexShader = compile vs_2_0 ProjectiveTexturingVertexShader();
			PixelShader = compile ps_2_0 ProjectiveTexturingPixelShader();
			
			CullMode = CCW;
		}
	}
	
	
	