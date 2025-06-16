//https://github.com/mono/opentk/blob/main/Source/Examples/Shapes/Old/Cube.cs

#define CG_Debug
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing;

namespace gcgcg
{
  internal class Cubo : Objeto
  {
    Ponto4D[] vertices;
    Vector2[] texCoords;
    // int[] indices;
    // Vector3[] normals;
    // int[] colors;

    public Cubo(Objeto _paiRef, ref char _rotulo) : base(_paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.TriangleFan;
      PrimitivaTamanho = 10;

      vertices = new Ponto4D[]
      {
        new Ponto4D(-1.0f, -1.0f,  1.0f),
        new Ponto4D( 1.0f, -1.0f,  1.0f),
        new Ponto4D( 1.0f,  1.0f,  1.0f),
        new Ponto4D(-1.0f,  1.0f,  1.0f),
        new Ponto4D(-1.0f, -1.0f, -1.0f),
        new Ponto4D( 1.0f, -1.0f, -1.0f),
        new Ponto4D( 1.0f,  1.0f, -1.0f),
        new Ponto4D(-1.0f,  1.0f, -1.0f)
      };

      texCoords = new Vector2[]
      {
          new Vector2(0.0f, 0.0f),
          new Vector2(1.0f, 0.0f),
          new Vector2(1.0f, 1.0f),
          new Vector2(0.0f, 1.0f),
      };

       // 0, 1, 2, 3 Face da frente
      base.PontosAdicionarComTextura(vertices[0], texCoords[0]);
      base.PontosAdicionarComTextura(vertices[1], texCoords[1]);
      base.PontosAdicionarComTextura(vertices[2], texCoords[2]);
      base.PontosAdicionarComTextura(vertices[3], texCoords[3]);

       // 3, 2, 6, 7 Face de cima
      base.PontosAdicionarComTextura(vertices[3], texCoords[0]);
      base.PontosAdicionarComTextura(vertices[2], texCoords[1]);
      base.PontosAdicionarComTextura(vertices[6], texCoords[2]);
      base.PontosAdicionarComTextura(vertices[7], texCoords[3]);
      
       // 4, 7, 6, 5 Face do fundo
      base.PontosAdicionarComTextura(vertices[4], texCoords[0]);
      base.PontosAdicionarComTextura(vertices[7], texCoords[1]);
      base.PontosAdicionarComTextura(vertices[6], texCoords[2]);
      base.PontosAdicionarComTextura(vertices[5], texCoords[3]);
      
       // 0, 3, 7, 4 Face esquerda
      base.PontosAdicionarComTextura(vertices[0], texCoords[0]);
      base.PontosAdicionarComTextura(vertices[3], texCoords[1]);
      base.PontosAdicionarComTextura(vertices[7], texCoords[2]);
      base.PontosAdicionarComTextura(vertices[4], texCoords[3]);

       // 0, 4, 5, 1 Face de baixo
      base.PontosAdicionarComTextura(vertices[0], texCoords[0]);
      base.PontosAdicionarComTextura(vertices[4], texCoords[1]);
      base.PontosAdicionarComTextura(vertices[5], texCoords[2]);
      base.PontosAdicionarComTextura(vertices[1], texCoords[3]);

       // 1, 5, 6, 2 Face direita
      base.PontosAdicionarComTextura(vertices[1], texCoords[0]);
      base.PontosAdicionarComTextura(vertices[5], texCoords[1]);
      base.PontosAdicionarComTextura(vertices[6], texCoords[2]);
      base.PontosAdicionarComTextura(vertices[2], texCoords[3]);

      Atualizar();
    }

    private void Atualizar()
    {

      base.ObjetoAtualizar();
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Cubo _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return (retorno);
    }
#endif

  }
}
