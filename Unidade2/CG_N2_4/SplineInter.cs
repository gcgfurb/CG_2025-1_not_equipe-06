using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gcgcg
{
    internal class SplineInter : Objeto
    {
        private int selectedPoint = 0;
        private char currentLable;
        private readonly int splineMaxPoints = 10;
        private int splinePoints = 10;

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

        public SplineInter(Objeto _paiRef, ref char _rotulo) : base(_paiRef, ref _rotulo)
        {
            currentLable = _rotulo;
            PrimitivaTipo = PrimitiveType.LineStrip;
            PontosControle();
            PoliedroControle();
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

        private void PontosControle()
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

            poly = new Poligono(paiRef, ref currentLable, PolyPoints)
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
                var p0 = Matematica.InterpolarRetaPonto(pontos4d[0], pontos4d[1], t);
                var p1 = Matematica.InterpolarRetaPonto(pontos4d[1], pontos4d[2], t);
                var p2 = Matematica.InterpolarRetaPonto(pontos4d[2], pontos4d[3], t);

                var p3 = Matematica.InterpolarRetaPonto(p0, p1, t);
                var p4 = Matematica.InterpolarRetaPonto(p1, p2, t);

                var finalPoint = Matematica.InterpolarRetaPonto(p3, p4, t);

                PontosAdicionar(finalPoint);
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
