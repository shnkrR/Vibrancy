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
    public class DefaultEffect : Effect, IEffectMatrices
    {
        EffectParameter world;
        EffectParameter projection;
        EffectParameter view;
        EffectParameter viewProjection;

        EffectParameter worldInverse;

        EffectParameter texture;

        EffectParameter ambientColor;
        EffectParameter ambientIntensity;

        EffectParameter lightDirection;
        EffectParameter diffuseColor;
        EffectParameter diffuseIntensity;

        EffectParameter isLightingEnabled;

        EffectParameter shine;
        EffectParameter specularColor;
        EffectParameter specularIntensity;

        EffectParameter viewVector;

        EffectParameter colorForObject;

        public DefaultEffect(Effect effect)
            : base(effect)
        {
            world               =     Parameters["World"];
            view                =     Parameters["View"];
            projection          =     Parameters["Projection"];
            viewProjection      =     Parameters["ViewProjection"];

            worldInverse        =     Parameters["WorldInverse"];

            texture             =     Parameters["DiffuseTexture"];

            ambientColor        =     Parameters["AmbientColor"];
            ambientIntensity    =     Parameters["AmbientIntensity"];

            isLightingEnabled   =     Parameters["IsLightingEnabled"];
            
            lightDirection      =     Parameters["LightDirection"];

            diffuseColor        =     Parameters["DiffuseColor"];
            diffuseIntensity    =     Parameters["DiffuseIntensity"];

            shine               =     Parameters["Shine"];
            specularColor       =     Parameters["SpecularColor"];
            specularIntensity   =     Parameters["SpecularIntensity"];

            viewVector          =     Parameters["ViewVector"];

            colorForObject      =     Parameters["ColorFromSource"];
        }

        #region Cameras
        public Matrix World
        {
            get { return world.GetValueMatrix(); }
            set { world.SetValue(value); }
        }

        public Matrix View
        {
            get { return view.GetValueMatrix(); }
            set { view.SetValue(value); }
        }

        public Matrix Projection
        {
            get { return projection.GetValueMatrix(); }
            set { projection.SetValue(value); }
        }

        public Matrix ViewxProjection
        {
            get { return viewProjection.GetValueMatrix(); }
            set { viewProjection.SetValue(value); }
        }
        #endregion

        #region Texture
        public Texture2D Texture
        {
            get { return texture.GetValueTexture2D(); }
            set { texture.SetValue(value); }
        }
        #endregion

        #region AmbientLight
        public Vector4 AmbientLightColor
        {
            get { return ambientColor.GetValueVector4(); }
            set { ambientColor.SetValue(value); }
        }

        public float AmbientLightIntensity
        {
            get { return ambientIntensity.GetValueVector2().X; }
            set { ambientIntensity.SetValue(value); }
        }
        #endregion

        #region Light Direction
        public Vector3 LightDirection
        {
            get { return lightDirection.GetValueVector3(); }
            set { lightDirection.SetValue(value); }
        }
        #endregion

        #region Diffuse Light
        public Vector4 DiffuseColor
        {
            get { return diffuseColor.GetValueVector4(); }
            set { diffuseColor.SetValue(value); }
        }

        public float DiffuseLightIntensity
        {
            get { return diffuseIntensity.GetValueVector2().X; }
            set { diffuseIntensity.SetValue(value); }
        }
        #endregion

        #region ExtraMath
        public Matrix WorldInverseMatrix
        {
            get { return worldInverse.GetValueMatrix(); }
            set { worldInverse.SetValue(value); }
        }

        public Vector3 ViewVector
        {
            get { return viewVector.GetValueVector3(); }
            set { viewVector.SetValue(value); }
        }
        #endregion

        #region Specularity
        public float Shine
        { 
            get { return shine.GetValueVector2().X; }
            set { shine.SetValue(value); }
        }

        public Vector4 SpecularColor
        {
            get { return specularColor.GetValueVector4(); }
            set { specularColor.SetValue(value); }
        }

        public float SpecularIntensity
        { 
            get { return specularIntensity.GetValueVector2().X; }
            set { specularIntensity.SetValue(value); }
        }
        #endregion

        public bool IsLightingEnabled
        {
            get { return isLightingEnabled.GetValueBoolean(); }
            set { isLightingEnabled.SetValue(value); }
        }

        public Vector4 ColorFromSource
        {
            get { return colorForObject.GetValueVector4(); }
            set { colorForObject.SetValue(value); }
        }
    }
}
