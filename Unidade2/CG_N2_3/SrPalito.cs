using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gcgcg
{
    internal class SrPalito : Objeto
    {
        private double radius = 0.5;
        private double angle = 45;

        private Ponto4D feet = new(); //base
        private Ponto4D head = new(); //top

        public SrPalito(Objeto _paiRef, ref char _rotulo) : base(_paiRef, ref _rotulo)
        {
            PontosAdicionar(feet);
            PontosAdicionar(head);

            Atualizar();
        }

        public void Atualizar()
        {
            //update initial point (feet) with current point
            PontosAlterar(feet, 0);
            
            //create the head point with the current radius and angle
            head = Matematica.GerarPtosCirculo(angle, radius);

            //update coordinates
            head.X += feet.X;

            PontosAlterar(head, 1);
            ObjetoAtualizar();
        }

        public void AtualizarPe(double peInc)
        {
            //crate new point set on X
            feet = new Ponto4D(feet.X + peInc, feet.Y);
            Atualizar();
        }

        public void AtualizarRaio(double raioInc)
        {
            radius += raioInc;
            Atualizar();
        }

        public void AtualizarAngulo(double anguloInc)
        {
            angle += anguloInc;
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
