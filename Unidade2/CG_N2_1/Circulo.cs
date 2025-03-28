using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
    internal class Circulo : Objeto
    {
        private readonly double radius;

        public Circulo(Objeto _paiRef, ref char _rotulo, double _raio) : this(_paiRef, ref _rotulo, _raio, new Ponto4D(0.5, 0.5))
        {

        }

        public Circulo(Objeto _paiRef, ref char _rotulo, double _raio, Ponto4D ptoDeslocamento) : base(_paiRef, ref _rotulo)
        {
            radius = _raio;
            PrimitivaTipo = PrimitiveType.Points;
            PrimitivaTamanho = 5;

            for (var i = 0; i < 360; i += 5)
            {
                Ponto4D point = Matematica.GerarPtosCirculo(i, radius);
                PontosAdicionar(point + ptoDeslocamento);
            }

            Atualizar();
        }

        private void Atualizar()
        {
            
            base.ObjetoAtualizar();
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
