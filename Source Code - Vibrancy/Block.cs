#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#endregion

namespace Vibrancy
{
    public class Block
    {
        VertexPositionNormalTexture[] boxData;
        VertexBuffer boxVB;
        IndexBuffer boxIB;
        GraphicsDevice _gDevice;
        public BoundingBox bBox;
        DefaultEffect effect;

        //Normals
        Vector2 topLeft;
        Vector2 topRight;
        Vector2 topCenter;
        Vector2 bottomRight;
        Vector2 bottomLeft;

        //transform Matrices
        public Matrix boxTransformMatrix;
        public Matrix meshTransformMatrix;

        public Block(GraphicsDevice graphicsDevice, DefaultEffect _effect)
        {
            _gDevice = graphicsDevice;
            effect = _effect;
        }

        #region Set Coords
        private void setBlockCoordinates()
        {
            topLeft = new Vector2(0.0f, 0.0f);
            topRight = new Vector2(1.0f, 0.0f);
            topCenter = new Vector2(0.5f, 0.0f);
            bottomLeft = new Vector2(0.0f, 1.0f);
            bottomRight = new Vector2(1.0f, 1.0f);

            #region Box Coords
            //Box Co-ords
            boxData = new VertexPositionNormalTexture[]
	        {
		        // Front Surface
		        new VertexPositionNormalTexture(new Vector3(-1.0f, -0.25f, 0.5f),Vector3.Backward,bottomLeft),
		        new VertexPositionNormalTexture(new Vector3(-1.0f, 0.25f, 0.5f),Vector3.Backward,topLeft), 
		        new VertexPositionNormalTexture(new Vector3(1.0f, -0.25f, 0.5f),Vector3.Backward,bottomRight),
		        new VertexPositionNormalTexture(new Vector3(1.0f, 0.25f, 0.5f),Vector3.Backward,topRight), 
                
                // Top Surface
		        new VertexPositionNormalTexture(new Vector3(-1.0f, 0.25f, 0.5f),Vector3.Up,bottomLeft),
		        new VertexPositionNormalTexture(new Vector3(-1.0f, 0.25f, -0.5f),Vector3.Up,topLeft),
		        new VertexPositionNormalTexture(new Vector3(1.0f, 0.25f, 0.5f),Vector3.Up,bottomRight),
		        new VertexPositionNormalTexture(new Vector3(1.0f, 0.25f, -0.5f),Vector3.Up,topRight), 

		        // Left Surface
		        new VertexPositionNormalTexture(new Vector3(-1.0f, -0.25f, -0.5f),Vector3.Left,bottomLeft),
		        new VertexPositionNormalTexture(new Vector3(-1.0f, 0.25f, -0.5f),Vector3.Left,topLeft),
		        new VertexPositionNormalTexture(new Vector3(-1.0f, -0.25f, 0.5f),Vector3.Left,bottomRight),
		        new VertexPositionNormalTexture(new Vector3(-1.0f, 0.25f, 0.5f),Vector3.Left,topRight),
                
                // Bottom Surface
		        new VertexPositionNormalTexture(new Vector3(-1.0f, -0.25f, -0.5f),Vector3.Down,bottomLeft),
		        new VertexPositionNormalTexture(new Vector3(-1.0f, -0.25f, 0.5f),Vector3.Down,topLeft),
		        new VertexPositionNormalTexture(new Vector3(1.0f, -0.25f, -0.5f),Vector3.Down,bottomRight),
		        new VertexPositionNormalTexture(new Vector3(1.0f, -0.25f, 0.5f),Vector3.Down,topRight),

		        // Right Surface
		        new VertexPositionNormalTexture(new Vector3(1.0f, -0.25f, 0.5f),Vector3.Right,bottomLeft),
		        new VertexPositionNormalTexture(new Vector3(1.0f, 0.25f, 0.5f),Vector3.Right,topLeft),
		        new VertexPositionNormalTexture(new Vector3(1.0f, -0.25f, -0.5f),Vector3.Right,bottomRight),
		        new VertexPositionNormalTexture(new Vector3(1.0f, 0.25f, -0.5f),Vector3.Right,topRight),

                // Back Surface
		        new VertexPositionNormalTexture(new Vector3(1.0f, -0.25f, -0.5f),Vector3.Forward,bottomLeft),
		        new VertexPositionNormalTexture(new Vector3(1.0f, 0.25f, -0.5f),Vector3.Forward,topLeft), 
		        new VertexPositionNormalTexture(new Vector3(-1.0f, -0.25f, -0.5f),Vector3.Forward,bottomRight),
		        new VertexPositionNormalTexture(new Vector3(-1.0f, 0.25f, -0.5f),Vector3.Forward,topRight), 
	        };
            #endregion

            short[] boxIndices = new short[] 
            { 
	            0, 1, 2, 2, 1, 3,   
	            4, 5, 6, 6, 5, 7,
	            8, 9, 10, 10, 9, 11, 
	            12, 13, 14, 14, 13, 15, 
	            16, 17, 18, 18, 17, 19,
	            20, 21, 22, 22, 21, 23
            };

            //Cube Buffers
            boxVB = new VertexBuffer(_gDevice, VertexPositionNormalTexture.VertexDeclaration, 24, BufferUsage.WriteOnly);
            boxIB = new IndexBuffer(_gDevice, IndexElementSize.SixteenBits, 36, BufferUsage.WriteOnly);

            bBox = new BoundingBox(boxData[0].Position, boxData[21].Position);

            boxVB.SetData(boxData);
            boxIB.SetData(boxIndices);
            boxData = null;
            boxIndices = null;
        }
        #endregion

        #region Draw Mesh
        public void DrawBox(Vector3 Translation)
        {
            if (boxVB == null && boxIB == null)
                setBlockCoordinates();

            boxTransformMatrix = Matrix.CreateTranslation(Translation);

            effect.World = boxTransformMatrix;
            effect.WorldInverseMatrix = Matrix.Invert(boxTransformMatrix);

            effect.CurrentTechnique.Passes[0].Apply();

            _gDevice.SetVertexBuffer(boxVB);
            _gDevice.Indices = boxIB;
            _gDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 24, 0, 12); 
        }

        public void DrawBox(Vector3 Translation, Vector3 Rotation)
        {
            if (boxVB == null && boxIB == null)
                setBlockCoordinates();

            boxTransformMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(Rotation.X)) *
                                 Matrix.CreateRotationY(MathHelper.ToRadians(Rotation.Y)) *
                                 Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation.Z)) * 
                                 Matrix.CreateTranslation(Translation);

            effect.World = boxTransformMatrix;
            effect.WorldInverseMatrix = Matrix.Invert(boxTransformMatrix);

            effect.CurrentTechnique.Passes[0].Apply();

            _gDevice.SetVertexBuffer(boxVB);
            _gDevice.Indices = boxIB;
            _gDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 24, 0, 12);
        }

        public void DrawBox(Vector3 Translation, Vector3 Rotation, Vector3 Scaling)
        {
            if (boxVB == null && boxIB == null)
                setBlockCoordinates();

            boxTransformMatrix = Matrix.CreateScale(Scaling)                              *
                                 Matrix.CreateRotationX(MathHelper.ToRadians(Rotation.X)) *
                                 Matrix.CreateRotationY(MathHelper.ToRadians(Rotation.Y)) *
                                 Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation.Z)) * 
                                 Matrix.CreateTranslation(Translation);

            effect.World = boxTransformMatrix;
            effect.WorldInverseMatrix = Matrix.Invert(boxTransformMatrix);

            effect.CurrentTechnique.Passes[0].Apply();

            _gDevice.SetVertexBuffer(boxVB);
            _gDevice.Indices = boxIB;
            _gDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 24, 0, 12);
        }
        
        #endregion

        #region Draw Pre-loaded Mesh
        public void DrawPreBox(Vector3 Translation, Model mesh)
        {
            meshTransformMatrix = Matrix.CreateTranslation(Translation);

            effect.World = meshTransformMatrix;
            effect.WorldInverseMatrix = Matrix.Invert(meshTransformMatrix);

            effect.CurrentTechnique.Passes[0].Apply();

            foreach (ModelMesh _mesh in mesh.Meshes)
            {
                foreach (ModelMeshPart part in _mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.World = _mesh.ParentBone.Transform * meshTransformMatrix;
                }
                _mesh.Draw();
            }
        }

        public void DrawPreBox(Vector3 Translation, Vector3 Rotation, Model mesh)
        {
            meshTransformMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(Rotation.X)) *
                                 Matrix.CreateRotationY(MathHelper.ToRadians(Rotation.Y)) *
                                 Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation.Z)) *
                                 Matrix.CreateTranslation(Translation);

            effect.World = meshTransformMatrix;
            effect.WorldInverseMatrix = Matrix.Invert(meshTransformMatrix);

            effect.CurrentTechnique.Passes[0].Apply();

            foreach (ModelMesh _mesh in mesh.Meshes)
            {
                foreach (ModelMeshPart part in _mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.World = _mesh.ParentBone.Transform * meshTransformMatrix;
                }
                _mesh.Draw();
            }
        }

        public void DrawPreBox(Vector3 Translation, Vector3 Rotation, Vector3 Scaling, Model mesh)
        {
            meshTransformMatrix = Matrix.CreateScale(Scaling) *
                                 Matrix.CreateRotationX(MathHelper.ToRadians(Rotation.X)) *
                                 Matrix.CreateRotationY(MathHelper.ToRadians(Rotation.Y)) *
                                 Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation.Z)) *
                                 Matrix.CreateTranslation(Translation);

            effect.World = meshTransformMatrix;
            effect.WorldInverseMatrix = Matrix.Invert(meshTransformMatrix);

            effect.CurrentTechnique.Passes[0].Apply();

            foreach (ModelMesh _mesh in mesh.Meshes)
            {
                foreach (ModelMeshPart part in _mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.World = _mesh.ParentBone.Transform * meshTransformMatrix;
                }
                _mesh.Draw();
            }
        }
        #endregion

        #region BunchRender
        public void AddBlockBunch(int numberOfBlocks, Vector3 loaction, Vector3 orientation, Vector3 size, float gap)
        {
            for (int i = 1; i <= numberOfBlocks; i++)
            {
                DrawBox(loaction, orientation, size);
                loaction.Z += gap;
            }
        }
        public void AddBlockBunch(int numberOfBlocks, Vector3 loaction, Vector3 orientation, Vector3 size, float gap, bool objectSwitch, Model model)
        {
            for (int i = 1; i <= numberOfBlocks; i++)
            {
                if (objectSwitch)
                    DrawPreBox(loaction, orientation, size, model);
                else
                    DrawBox(loaction, orientation, size);

                loaction.Z += gap;
            }
        }
        #endregion

        #region VectorBase
        public void DrawVector(float lineXPos, float lineYPos, float line1Zpos, float line2ZPos)
        {
            VertexPositionNormalTexture[] Line = new VertexPositionNormalTexture[] {
                new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(lineXPos,lineYPos,line1Zpos) , Vector3.Forward, topLeft),
                new VertexPositionNormalTexture(new Microsoft.Xna.Framework.Vector3(lineXPos,lineYPos,line2ZPos) , Vector3.Backward, topRight)
            };

            int lineCount = 1;
            int[] indices = new int[] { 0, 1 };
            _gDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.LineStrip, Line, 0, Line.Length, indices, 0, lineCount);
        }
        #endregion

    }
}