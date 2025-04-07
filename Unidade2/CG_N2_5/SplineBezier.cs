using CG_Biblioteca;
using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using System.Text;
using System.Threading.Tasks;

namespace gcgcg
{
    internal class SplineBezier : Objeto
    {
        private int selectedPoint = 0;
        private char currentLable;
        private readonly int splineMaxPoints = 10;
        private int splinePoints = 10;

        private double[,] bezierMatrix;
        private readonly Ponto[] controlPoints = new Ponto[4];
        private Poligono poly;
        private readonly Ponto4D[] originPoints =
        {
            new(0.5, -0.5),
            new(0.5, 0.5),
            new(-0.5, 0.5),
            new(-0.5, -0.5)
        };

        private readonly Shader redShader = new("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
        private readonly Shader whiteShader = new("Shaders/shader.vert", "Shaders/shaderBranca.frag");
        private readonly Shader cyanShader = new("Shaders/shader.vert", "Shaders/shaderCiano.frag");

        public SplineBezier(Objeto paiRef, ref char _rotulo) : base(paiRef, ref _rotulo)
        {
            bezierMatrix = new double[11, 4];
            currentLable = _rotulo;
            PrimitivaTipo = PrimitiveType.LineStrip;
            PontosControle();
            PoliedroControle();
            BezierMatrizPeso();
            Atualizar();
        }

        public void SplineQtdPto(int inc)
        {
            splinePoints += inc;

            if (splinePoints < 1)
                splinePoints = 1;

            if (splinePoints > splineMaxPoints)
                splinePoints = splineMaxPoints;

            Atualizar();
        }

        private void BezierMatrizPeso()
        {
            var i = 0;

            for (var u = 0.0; u <= 1.0; u += 1.0 / splinePoints)
            {
                bezierMatrix[i, 0] = Math.Pow(1.0 - u, 3.0);
                bezierMatrix[i, 1] = 3.0 * u * Math.Pow(1.0 - u, 2.0);
                bezierMatrix[i, 2] = 3.0 * Math.Pow(u, 2.0) * (1.0 - u);
                bezierMatrix[i, 3] = Math.Pow(u, 3.0);
                i++;
            }
        }

        public void PontosControle()
        {
            for (int i = 0; i < controlPoints.Length; i++)
            {
                controlPoints[i] = new Ponto(this, ref currentLable, originPoints[i]);
                FilhoAdicionar(controlPoints[i]);
                controlPoints[i].ObjetoAtualizar();
            }
        }

        private void PoliedroControle()
        {
            var PolyPoints = new List<Ponto4D>
            {
                controlPoints[0].PontosId(0),
                controlPoints[1].PontosId(0),
                controlPoints[2].PontosId(0),
                controlPoints[3].PontosId(0)
            };

            poly = new Poligono(this, ref currentLable, PolyPoints)
            {
                PrimitivaTipo = PrimitiveType.LineStrip,
                ShaderObjeto = cyanShader

            };

            FilhoAdicionar(poly);
            poly.ObjetoAtualizar();
        }

        public void Atualizar()
        {
            var pontos4d = new Ponto4D[controlPoints.Length];
            for (var i = 0; i < controlPoints.Length; i++)
            {
                pontos4d[i] = new Ponto4D(controlPoints[i].PontosId(0));
                //Trocar a cor de todos os pontos de controle para branco, exceto o selecionado que fica vermelho
                controlPoints[i].ShaderObjeto = i == selectedPoint ? redShader : whiteShader;
            }

            PontosApagar();
            for (var t = 0.0; t <= 1.0; t += 1.0 / splinePoints)
            {
                double x = Math.Pow(1 - t, 3) * pontos4d[0].X + 3 *
                           Math.Pow(1 - t, 2) * t * pontos4d[1].X + 3 * (1 - t) *
                           Math.Pow(t, 2) * pontos4d[2].X +
                           Math.Pow(t, 3) * pontos4d[3].X;

                double y = Math.Pow(1 - t, 3) * pontos4d[0].Y + 3 *
                           Math.Pow(1 - t, 2) * t * pontos4d[1].Y + 3 * (1 - t) *
                           Math.Pow(t, 2) * pontos4d[2].Y +
                           Math.Pow(t, 3) * pontos4d[3].Y;

                PontosAdicionar(new Ponto4D(x, y));
            }

            ObjetoAtualizar();
        }

        public void AtualizarSpline(Ponto4D ptoInc, bool proximo = false)
        {
            if (proximo)
                selectedPoint = selectedPoint >= 3 ? 0 : ++selectedPoint;

            controlPoints[selectedPoint].PontosAlterar(controlPoints[selectedPoint].PontosId(0) + ptoInc, 0);
            controlPoints[selectedPoint].ObjetoAtualizar();
            poly.PontosAlterar(controlPoints[selectedPoint].PontosId(0), selectedPoint);
            poly.ObjetoAtualizar();

            Atualizar();
        }

        public double this[int row, int col]
        {
            get { return bezierMatrix[row, col]; }
        }

#if CG_Debug
        public override string ToString()
        {
            System.Console.WriteLine("__________________________________ \n");
            string retorno;
            retorno = "__ Objeto Circulo _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
            retorno += base.ImprimeToString();
            return retorno;
        }
#endif
    }
}