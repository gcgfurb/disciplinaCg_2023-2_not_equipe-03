#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Circulo : Objeto
  {
    public Circulo(Objeto paiRef, ref char _rotulo, Ponto4D pto) : base(paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.Points;
      PrimitivaTamanho = 5;

      //Para deixar o circulo amarelo
      shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");

      base.PontosAdicionar(pto);

      Atualizar();
    }

    public void Atualizar()
    {

      base.ObjetoAtualizar();
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Ponto _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return (retorno);
    }
#endif

  }
}
