#define CG_Gizmo  // debugar gráfico.
#define CG_OpenGL // render OpenGL.
// #define CG_DirectX // render DirectX.
// #define CG_Privado // código do professor.

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Mathematics;
using System.Collections.Generic;

//FIXME: padrão Singleton

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    Objeto mundo;
    private char rotuloNovo = '?';
    private Objeto objetoSelecionado = null;
    private Objeto objetoSelecionadobk = null;

    private readonly float[] _sruEixos =
    {
      -0.5f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
       0.0f, -0.5f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
       0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.5f  /* Z+ */
    };

    private int _vertexBufferObject_sruEixos;
    private int _vertexArrayObject_sruEixos;

    private int _vertexBufferObject_bbox;
    private int _vertexArrayObject_bbox;

    private Shader _shaderBranca;
    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;
    private Shader _shaderCiano;
    private Shader _shaderMagenta;
    private Shader _shaderAmarela;
    private Ponto4D pontoUm = null;
    private Ponto4D pontoDois = null;
    
    private List<Ponto4D> listaVazia = new List<Ponto4D>();

    private int contadorSelecionar = 0;
    private bool jaSelecionado = false;

    //Array com os poligonos da tela
    List<Objeto> poligonosDaTela = new List<Objeto>();
    //Array com os pontos da tela
    List<Ponto4D> pontosDaTela = new List<Ponto4D>();

    private double soma_total = 0;
          




    public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
           : base(gameWindowSettings, nativeWindowSettings)
    {
      mundo = new Objeto(null, ref rotuloNovo);
    }

    private void Diretivas()
    {
#if DEBUG
      Console.WriteLine("Debug version");
#endif      
#if RELEASE
    Console.WriteLine("Release version");
#endif      
#if CG_Gizmo      
      Console.WriteLine("#define CG_Gizmo  // debugar gráfico.");
#endif
#if CG_OpenGL      
      Console.WriteLine("#define CG_OpenGL // render OpenGL.");
#endif
#if CG_DirectX      
      Console.WriteLine("#define CG_DirectX // render DirectX.");
#endif
#if CG_Privado      
      Console.WriteLine("#define CG_Privado // código do professor.");
#endif
      Console.WriteLine("__________________________________ \n");
    }

    protected override void OnLoad(){

      base.OnLoad();

      Diretivas();

      GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

      #region Cores
      _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
      _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
      _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
      #endregion

      #region Eixos: SRU  
      _vertexBufferObject_sruEixos = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
      GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
      _vertexArrayObject_sruEixos = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
      #endregion





#if CG_Privado
      #region Objeto: circulo  
      objetoSelecionado = new Circulo(mundo, ref rotuloNovo, 0.2, new Ponto4D());
      #endregion

      #region Objeto: SrPalito  
      objetoSelecionado = new SrPalito(mundo, ref rotuloNovo);
      #endregion

      #region Objeto: Spline
      objetoSelecionado = new Spline(mundo, ref rotuloNovo);
      #endregion
#endif

    }

    protected override void OnRenderFrame(FrameEventArgs e){
      
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit);

      mundo.Desenhar(new Transformacao4D());

      #if CG_Gizmo      
        Gizmo_Sru3D();
        Gizmo_BBox();
      #endif

      SwapBuffers();

    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      // ☞ 396c2670-8ce0-4aff-86da-0f58cd8dcfdc   TODO: forma otimizada para teclado.
      #region Teclado
      var input = KeyboardState;
      
      //ESC - CLICA PARA FECHAR
      if (input.IsKeyDown(Keys.Escape)){
        Close();
      }
      
      //A TECLA ESPAÇO PARA SELECIONAR
      if (input.IsKeyPressed(Keys.Space)){
          
          objetoSelecionado = poligonosDaTela[contadorSelecionar];

          //AQUI DESENHA A BBOX
          Gizmo_BBox();
          
          if(contadorSelecionar <  poligonosDaTela.Count - 1){
            contadorSelecionar++;
          }else{
            contadorSelecionar = 0;
          }

          jaSelecionado = true;

      }

      //ITEM 03
      //[Peso 0,5] Utilize a tecla D para remover o polígono selecionado.
      //Quando clica em D, remover
      if(input.IsKeyPressed(Keys.D)){

        List<Objeto> poligonosDaTelaNova = new List<Objeto>();
    
        for(int i=0; i < poligonosDaTela.Count; i++){

          if(objetoSelecionado == poligonosDaTela[i]){
            
            objetoSelecionado.PrimitivaTamanho = 0;
            objetoSelecionado = null;
            jaSelecionado = false;

          }else{
            poligonosDaTelaNova.Add(poligonosDaTela[i]);
          }

        }

        poligonosDaTela = poligonosDaTelaNova;
        contadorSelecionar = 0;
        this.OnUnload();
        this.OnLoad();

        for(int i=0; i < poligonosDaTela.Count; i++){
          poligonosDaTela[i].ObjetoAtualizar();
          poligonosDaTela[i].shaderObjeto = _shaderBranca;
        }

      }

      //ITEM 8 - [Peso 0,5] Utilize o teclado (teclas R=vermelho,G=verde,B=azul) para trocar as cores dos polígonos selecionado.
      //Muda para cor vermelha ao clicar com R, azul ao clicar no B ou verde ao clicar no G
      if (input.IsKeyPressed(Keys.R) || input.IsKeyPressed(Keys.G) || input.IsKeyPressed(Keys.B)){

        for(int i=0; i < poligonosDaTela.Count; i++){

          if(objetoSelecionado == poligonosDaTela[i]){
            if (input.IsKeyPressed(Keys.R)){
              poligonosDaTela[i].shaderObjeto = _shaderVermelha;
            }else if(input.IsKeyPressed(Keys.G)){
              poligonosDaTela[i].shaderObjeto = _shaderVerde;
            }else if(input.IsKeyPressed(Keys.B)){
              poligonosDaTela[i].shaderObjeto = _shaderAzul;
            }
          }

        }
      }

      //ITEM 04
      //[Peso 0,5] Utilize a posição do mouse junto com a tecla V para mover vértice mais próximo do polígono selecionado.
      //           Atenção: no caso do mover o vértice o valores da coordenada é alterada e não os valores da matriz de transformação.
      //CLICANDO EM V ELE MOVE O MAIS PROXIMO
      if (input.IsKeyPressed(Keys.V)){

        if(objetoSelecionado != null && jaSelecionado){
          
          int janelaLargura = Size.X;
          int janelaAltura = Size.Y;
          Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
          Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

          this.OnUnload();
          this.OnLoad();

          for(int i=0; i < poligonosDaTela.Count; i++){
            if(poligonosDaTela[i] == objetoSelecionado){
              poligonosDaTela[i].PontosAlterar(sruPonto, objetoSelecionado.menorDistancia(sruPonto));
            }
            poligonosDaTela[i].ObjetoAtualizar();
          }

        }

      }

      //ITEM 10
      //[Peso 1,0] Utilizando as teclas das setas direcionais (cima/baixo,direita,esquerda) movimente o polígono selecionado.
      //           Atenção: usar matriz de transformação e não alterar os valores dos vértices dos polígonos.
      //MOVER AS SETAS E MOVER O POLIGNO
      if(input.IsKeyPressed(Keys.Left) || input.IsKeyPressed(Keys.Right) || input.IsKeyPressed(Keys.Up) || input.IsKeyPressed(Keys.Down)){

        for(int i=0; i < poligonosDaTela.Count; i++){

          if(poligonosDaTela[i] == objetoSelecionado){

            if(input.IsKeyPressed(Keys.Left)){
              objetoSelecionado.MatrizTranslacaoXYZ(-0.05, 0, 0);
            }else if(input.IsKeyPressed(Keys.Right)){
              objetoSelecionado.MatrizTranslacaoXYZ(0.05, 0, 0);
            }else if(input.IsKeyPressed(Keys.Up)){
              objetoSelecionado.MatrizTranslacaoXYZ(0, 0.05, 0);
            }else if(input.IsKeyPressed(Keys.Down)){
              objetoSelecionado.MatrizTranslacaoXYZ(0, -0.05, 0);
            }

            objetoSelecionado.newBbox();
            objetoSelecionado.ObjetoAtualizar();
            poligonosDaTela[i] = objetoSelecionado;
            
          }

        }

      }

      if(input.IsKeyPressed(Keys.E) && objetoSelecionado != null){

        int janelaLargura = Size.X;
        int janelaAltura = Size.Y;
        Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
        Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

        for(int i=0; i < poligonosDaTela.Count; i++){
          if(poligonosDaTela[i] == objetoSelecionado){
            poligonosDaTela[i].PontosRemover(objetoSelecionado.menorDistancia(sruPonto));
          }
          poligonosDaTela[i].ObjetoAtualizar();
        }

      }
      
      




      if (input.IsKeyPressed(Keys.G)){
        mundo.GrafocenaImprimir("");
      }

      if (input.IsKeyPressed(Keys.P) && objetoSelecionado != null){
        System.Console.WriteLine(objetoSelecionado.ToString());
      }

      if (input.IsKeyPressed(Keys.M) && objetoSelecionado != null){
        objetoSelecionado.MatrizImprimir();
      }

      //TODO: não está atualizando a BBox com as transformações geométricas
      if (input.IsKeyPressed(Keys.I) && objetoSelecionado != null){
        objetoSelecionado.MatrizAtribuirIdentidade();
      }

      if (input.IsKeyPressed(Keys.PageUp) && objetoSelecionado != null){
        objetoSelecionado.MatrizEscalaXYZ(2, 2, 2);
      }

      if (input.IsKeyPressed(Keys.PageDown) && objetoSelecionado != null){
        objetoSelecionado.MatrizEscalaXYZ(0.5, 0.5, 0.5);
      }

      if (input.IsKeyPressed(Keys.Home) && objetoSelecionado != null){
        objetoSelecionado.MatrizEscalaXYZBBox(0.5, 0.5, 0.5);
      }

      if (input.IsKeyPressed(Keys.End) && objetoSelecionado != null){
        objetoSelecionado.MatrizEscalaXYZBBox(2, 2, 2);
      }

      if (input.IsKeyPressed(Keys.D1) && objetoSelecionado != null){
        objetoSelecionado.MatrizRotacao(10);
      }

      if (input.IsKeyPressed(Keys.D2) && objetoSelecionado != null){
        objetoSelecionado.MatrizRotacao(-10);
      }

      if (input.IsKeyPressed(Keys.D3) && objetoSelecionado != null){
        objetoSelecionado.MatrizRotacaoZBBox(10);
      }

      if (input.IsKeyPressed(Keys.D4) && objetoSelecionado != null){
        objetoSelecionado.MatrizRotacaoZBBox(-10);
      }


      //QUANDO DER ENTER ELE ADICIONA O POLIGNO
      if(input.IsKeyPressed(Keys.Enter)){

        if(objetoSelecionado != null){
          poligonosDaTela.Add(objetoSelecionado);
          pontosDaTela = new List<Ponto4D>();
          objetoSelecionado = null;
        }

      }


      #endregion

      #region  Mouse

      
      //ITEM 09
      //[Peso 1,5] Utilize o mouse para clicar na tela com botão esquerdo para selecionar o polígono testando primeiro se o ponto do mouse está dentro da BBox do polígono e 
      //           depois usando o algoritmo Scan Line.
      //           Caso o polígono seja selecionado se deve exibir a sua BBbox, caso contrário a variável objetoSelecionado deve ser "null", e assim nenhum contorno 
      //           de BBox deve ser exibido.
      //CLICANDO COM O ESQUERDO DO MOUSE
      if (MouseState.IsButtonPressed(MouseButton.Left)){
        
        int janelaLargura = Size.X;
        int janelaAltura = Size.Y;
        Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
        Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

        for(int i=0; i < poligonosDaTela.Count; i++){
        
          if(poligonosDaTela[i].Bbox().Dentro(sruPonto)){
            jaSelecionado = true;
            objetoSelecionado = poligonosDaTela[i];
          }
        
        }

      }
      
      //ITEM 02
      //[Peso 1,0] Utilize o mouse para clicar na tela com botão direito e poder desenhar um novo polígono.
      //           Quando pressionar a tecla Enter finaliza o desenho do novo polígono.
      //CLIQUE DO MAUSE, AQUI ELE ADICIONA OS PONTOS
      if (MouseState.IsButtonReleased(MouseButton.Right)){

        int janelaLargura = Size.X;
        int janelaAltura = Size.Y;
        Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
        Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

        pontosDaTela.Add(sruPonto);

        if(pontosDaTela.Count == 2){

          List<Ponto4D> pontosPoligonoTriangulo = new List<Ponto4D>();
          pontosPoligonoTriangulo.Add(pontosDaTela[0]);
          pontosPoligonoTriangulo.Add(pontosDaTela[1]);

          objetoSelecionado = new Poligono(mundo, ref rotuloNovo, pontosPoligonoTriangulo);
        
        }else if(pontosDaTela.Count >= 3){
          objetoSelecionado.PontosAdicionar(sruPonto);
        }

      }

      #endregion

    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);

      GL.Viewport(0, 0, Size.X, Size.Y);
    }

    protected override void OnUnload()
    {
      mundo.OnUnload();

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
      GL.UseProgram(0);

      GL.DeleteBuffer(_vertexBufferObject_sruEixos);
      GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

      GL.DeleteBuffer(_vertexBufferObject_bbox);
      GL.DeleteVertexArray(_vertexArrayObject_bbox);

      GL.DeleteProgram(_shaderBranca.Handle);
      GL.DeleteProgram(_shaderVermelha.Handle);
      GL.DeleteProgram(_shaderVerde.Handle);
      GL.DeleteProgram(_shaderAzul.Handle);
      GL.DeleteProgram(_shaderCiano.Handle);
      GL.DeleteProgram(_shaderMagenta.Handle);
      GL.DeleteProgram(_shaderAmarela.Handle);

      base.OnUnload();
    }

#if CG_Gizmo
    private void Gizmo_Sru3D()
    {
#if CG_OpenGL && !CG_DirectX
      var transform = Matrix4.Identity;
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      // EixoX
      _shaderVermelha.SetMatrix4("transform", transform);
      _shaderVermelha.Use();
      GL.DrawArrays(PrimitiveType.Lines, 0, 2);
      // EixoY
      _shaderVerde.SetMatrix4("transform", transform);
      _shaderVerde.Use();
      GL.DrawArrays(PrimitiveType.Lines, 2, 2);
      // EixoZ
      _shaderAzul.SetMatrix4("transform", transform);
      _shaderAzul.Use();
      GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
    }
#endif    

#if CG_Gizmo
    private void Gizmo_BBox()   //FIXME: não é atualizada com as transformações globais
    {
      if (objetoSelecionado != null && jaSelecionado)
      {

#if CG_OpenGL && !CG_DirectX

        float[] _bbox =
        {
        (float) objetoSelecionado.Bbox().obterMenorX, (float) objetoSelecionado.Bbox().obterMenorY, 0.0f, // A
        (float) objetoSelecionado.Bbox().obterMaiorX, (float) objetoSelecionado.Bbox().obterMenorY, 0.0f, // B
        (float) objetoSelecionado.Bbox().obterMaiorX, (float) objetoSelecionado.Bbox().obterMaiorY, 0.0f, // C
        (float) objetoSelecionado.Bbox().obterMenorX, (float) objetoSelecionado.Bbox().obterMaiorY, 0.0f  // D
      };

        _vertexBufferObject_bbox = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_bbox);
        GL.BufferData(BufferTarget.ArrayBuffer, _bbox.Length * sizeof(float), _bbox, BufferUsageHint.StaticDraw);
        _vertexArrayObject_bbox = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject_bbox);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        var transform = Matrix4.Identity;
        GL.BindVertexArray(_vertexArrayObject_bbox);
        _shaderAmarela.SetMatrix4("transform", transform);
        _shaderAmarela.Use();
        GL.DrawArrays(PrimitiveType.LineLoop, 0, (_bbox.Length / 3));

#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif

      }
    }
#endif    

  }
}
