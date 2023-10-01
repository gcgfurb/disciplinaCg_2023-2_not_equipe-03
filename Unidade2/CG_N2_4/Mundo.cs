﻿#define CG_Gizmo  // debugar gráfico.
#define CG_OpenGL // render OpenGL.
// #define CG_DirectX // render DirectX.
// #define CG_Privado // código do professor.

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

//FIXME: padrão Singleton

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    Objeto mundo;
    private char rotuloAtual = '?';
    private Objeto objetoSelecionado = null;

    private Objeto objetoItemUm = null;
    private Objeto objetoItemDois = null;
    private Objeto objetoItemTres = null;
    private Objeto objetoItemQuatro = null;
    private Objeto objetoItemCinco = null;
    private Objeto objetoItemSeis = null;

    private Objeto objetoItemSete = null;

    private Objeto retaUm = null;
    private Objeto retaDois = null;
    private Objeto retaTres = null;

    private int contagem = 10;

    private readonly float[] _sruEixos =
    {
      -0.5f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
       0.0f, -0.5f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
       0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.5f, /* Z+ */
    };

    List<SegReta> _pontosSpice = new List<SegReta>();

    private int _vertexBufferObject_sruEixos;
    private int _vertexArrayObject_sruEixos;

    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;

    private Shader _shaderMagenta;

    private bool _firstMove = true;
    private Vector2 _lastPos;
    private int contador = 0;
    private double anguloAtual = 45;
    private Ponto4D pontoAtual;

    public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
           : base(gameWindowSettings, nativeWindowSettings)
    {
      mundo = new Objeto(null, ref rotuloAtual);
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

    protected override void OnLoad()
    {
      base.OnLoad();

      Diretivas();

      GL.ClearColor(0.0f, 0.0f, 0.7f, 1.0f);

      #region Eixos: SRU  
      _vertexBufferObject_sruEixos = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
      GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
      _vertexArrayObject_sruEixos = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
      #endregion

      objetoItemUm = new Ponto(mundo, ref rotuloAtual, new Ponto4D(0.50, -0.50));
      objetoItemUm.PrimitivaTipo = PrimitiveType.Points;
      objetoItemUm.PrimitivaTamanho = 10;

      objetoItemUm = new Ponto(mundo, ref rotuloAtual, new Ponto4D(-0.50, 0.50));
      objetoItemUm.PrimitivaTipo = PrimitiveType.Points;
      objetoItemUm.PrimitivaTamanho = 10;

      objetoItemUm = new Ponto(mundo, ref rotuloAtual, new Ponto4D(-0.50, -0.50));
      objetoItemUm.PrimitivaTipo = PrimitiveType.Points;
      objetoItemUm.PrimitivaTamanho = 10;

      objetoItemUm = new Ponto(mundo, ref rotuloAtual, new Ponto4D(0.50, 0.50));
      objetoItemUm.PrimitivaTipo = PrimitiveType.Points;
      objetoItemUm.PrimitivaTamanho = 10;

      //Direita
      retaUm = new SegReta(mundo, ref rotuloAtual, new Ponto4D(0.50, -0.50), new Ponto4D(0.50, 0.50));

      //Esquerda
      retaDois = new SegReta(mundo, ref rotuloAtual, new Ponto4D(-0.50, 0.50), new Ponto4D(-0.50, -0.50));

      //Cima
      retaTres = new SegReta(mundo, ref rotuloAtual, new Ponto4D(-0.50, 0.50), new Ponto4D(0.50, 0.50));

      //Slice
      //retaTres = new SegReta(mundo, ref rotuloAtual, new Ponto4D(-0.50, -0.50), new Ponto4D(0, 0));
      //_pontosSpice.Add((SegReta)retaTres);

      
        var calculo = Matematica.GerarPtosCirculo(05, 0.50);
        var calculo2 = calculo;
        contagem = 180;

        for (int i = 0; i <= 180; i = i + 10){
          calculo = Matematica.GerarPtosCirculo(i, 0.50);
          objetoSelecionado = new Circulo(mundo, ref rotuloAtual, new Ponto4D(calculo.X, calculo.Y - 0.50));

          calculo2 = Matematica.GerarPtosCirculo(i + 10, 0.50);
          

          retaTres = new SegReta(mundo, ref rotuloAtual, new Ponto4D(calculo.X, calculo.Y - 0.50), new Ponto4D(calculo2.X, calculo2.Y - 0.50));
          _pontosSpice.Add((SegReta)retaTres);
      
        }


#if CG_Privado
      #region Objeto: circulo  
      objetoSelecionado = new Circulo(mundo, ref rotuloAtual, 0.2, new Ponto4D());
      objetoSelecionado.shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
      #endregion

      #region Objeto: SrPalito  
      objetoSelecionado = new SrPalito(mundo, ref rotuloAtual);
      #endregion

      #region Objeto: Spline
      objetoSelecionado = new Spline(mundo, ref rotuloAtual);
      #endregion
#endif

    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit);

#if CG_Gizmo      
      Sru3D();
#endif
      mundo.Desenhar();
      SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e){

      base.OnUpdateFrame(e);

      #region Teclado
      var input = KeyboardState;
      if (input.IsKeyDown(Keys.Escape))
      {
        Close();
      }else{
        
        if(input.IsKeyPressed(Keys.KeyPadAdd)){

         
          


            /*  var temX = _pontosSpice[_pontosSpice.Count-1].PontosId(0).X + 0.01;
              var temY = _pontosSpice[_pontosSpice.Count-1].PontosId(0).Y + 0.01;
            
             var temX1 = _pontosSpice[_pontosSpice.Count-1].PontosId(1).X;
             var temY1 = _pontosSpice[_pontosSpice.Count-1].PontosId(1).Y;
            
            if(_pontosSpice.Count == 1){
              retaTres = new SegReta(mundo, ref rotuloAtual, new Ponto4D(0.50, -0.50), new Ponto4D(0, 0));
              _pontosSpice.Add((SegReta)retaTres);
            }else if(_pontosSpice.Count == 2){
              
              _pontosSpice[0].PontosAlterar(new Ponto4D(-0.33, 0), 1);
              _pontosSpice[0].ObjetoAtualizar();

              _pontosSpice[1].PontosAlterar(new Ponto4D(0.33, 0), 1);
              _pontosSpice[1].ObjetoAtualizar();

              retaTres = new SegReta(mundo, ref rotuloAtual, new Ponto4D(-0.33, 0), new Ponto4D(0.33, 0));
              _pontosSpice.Add((SegReta)retaTres);
            

            }else if(_pontosSpice.Count == 3){

              _pontosSpice[0].PontosAlterar(new Ponto4D(-0.25, 0), 1);
              _pontosSpice[0].ObjetoAtualizar();

              _pontosSpice[1].PontosAlterar(new Ponto4D(0.25, 0), 1);
              _pontosSpice[1].ObjetoAtualizar();

              _pontosSpice[2].PontosAlterar(new Ponto4D(-0.25, 0), 0);
              _pontosSpice[2].PontosAlterar(new Ponto4D(0, 0.25), 1);
              _pontosSpice[2].ObjetoAtualizar();

              retaTres = new SegReta(mundo, ref rotuloAtual, new Ponto4D(0.25, 0), new Ponto4D(0, 0.25));
              _pontosSpice.Add((SegReta)retaTres);
            
            }else if(_pontosSpice.Count == 4){
              

            

            }*/
            


        }

        

      }
        
      
      #endregion

      #region  Mouse
      var mouse = MouseState;
      // Mouse FIXME: inverte eixo Y, fazer NDC para proporção em tela
      Vector2i janela = this.ClientRectangle.Size;

      if (input.IsKeyDown(Keys.LeftShift))
      {
        if (_firstMove)
        {
          _lastPos = new Vector2(mouse.X, mouse.Y);
          _firstMove = false;
        }
        else
        {
          var deltaX = (mouse.X - _lastPos.X) / janela.X;
          var deltaY = (mouse.Y - _lastPos.Y) / janela.Y;
          _lastPos = new Vector2(mouse.X, mouse.Y);

          objetoSelecionado.PontosAlterar(new Ponto4D(objetoSelecionado.PontosId(0).X + deltaX, objetoSelecionado.PontosId(0).Y + deltaY, 0), 0);
          objetoSelecionado.ObjetoAtualizar();
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

      GL.DeleteProgram(_shaderVermelha.Handle);
      GL.DeleteProgram(_shaderVerde.Handle);
      GL.DeleteProgram(_shaderAzul.Handle);

      base.OnUnload();
    }

#if CG_Gizmo
    private void Sru3D()
    {
#if CG_OpenGL && !CG_DirectX
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      // EixoX
      _shaderVermelha.Use();
      GL.DrawArrays(PrimitiveType.Lines, 0, 2);
      // EixoY
      _shaderVerde.Use();
      GL.DrawArrays(PrimitiveType.Lines, 2, 2);
      // EixoZ
      _shaderAzul.Use();
      GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
    }
#endif    

  }
}