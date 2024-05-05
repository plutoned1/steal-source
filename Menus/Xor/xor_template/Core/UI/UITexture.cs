using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ark.Core.UI
{
    public class UITexture
    {
        public UITexture()
        {
            this.Texture = new Texture2D(2, 2);
            for (int i = 0; i < this.Texture.width; i++)
            {
                for (int j = 0; j < this.Texture.height; j++)
                {
                    this.Texture.SetPixel(i, j, DefaultColor);
                }
            }
        }

        public void Draw(Rect size, Color color, float r)
        {
            for (int i = 0; i < 5; i++)
                GUI.DrawTexture(
                    size,
                    this.Texture,
                    (ScaleMode)(0 + Type.EmptyTypes.Length),
                    Type.EmptyTypes.Length != 0,
                    0f,
                    color,
                    0f,
                    r
                );
        }

        public void DrawTransparent(Rect size, Color color, float r)
        {
            GUI.DrawTexture(
                size,
                this.Texture,
                (ScaleMode)(0 + Type.EmptyTypes.Length),
                Type.EmptyTypes.Length != 0,
                0f,
                color,
                0f,
                r
            );
        }

        public Texture2D Texture;
        public Color DefaultColor;
    }
}
