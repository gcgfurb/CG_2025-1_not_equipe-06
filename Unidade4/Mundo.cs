#define CG_DEBUG
#define CG_Gizmo
#define CG_OpenGL
// #define CG_OpenTK
// #define CG_DirectX      
// #define CG_Privado      

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Mathematics;
using System.Collections.Generic;


namespace gcgcg
{
    public class Mundo : GameWindow
    {
        private static Objeto mundo = null;
        private char rotuloNovo = '?';
        private Objeto objetoSelecionado = null;
        private Cubo _orbitingCube;

        private readonly float[] _sruEixos =
        {
            -0.5f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
			0.0f, -0.5f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
			0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.5f  /* Z+ */
		};

        private int _vertexBufferObject_sruEixos;
        private int _vertexArrayObject_sruEixos;

        private Shader _shaderBranca;
        private Shader _shaderVermelha;
        private Shader _shaderVerde;
        private Shader _shaderAzul;
        private Shader _shaderCiano;
        private Shader _shaderMagenta;
        private Shader _shaderAmarela;

        private readonly float[] _frontFace =
        {
             1.05f,  1.05f, 1.05f, 1.0f, 1.0f, // top right
			 1.05f, -1.05f, 1.05f, 1.0f, 0.0f, // bottom right
			-1.05f, -1.05f, 1.05f, 0.0f, 0.0f, // bottom left
			-1.05f,  1.05f, 1.05f, 0.0f, 1.0f  // top left
		};

        private readonly float[] _backFace =
        {
            -1.05f,  1.05f, -1.05f, 1.0f, 1.0f, // top right
			-1.05f, -1.05f, -1.05f, 1.0f, 0.0f, // bottom right
			1.05f, -1.05f, -1.05f, 0.0f, 0.0f, // bottom left
			1.05f,  1.05f, -1.05f, 0.0f, 1.0f  // top left
		};

        private readonly float[] _topFace =
        {
            1.05f,  1.05f, -1.05f, 1.0f, 1.0f, // top right
			1.05f,  1.05f,  1.05f, 1.0f, 0.0f, // bottom right
			-1.05f,  1.05f,  1.05f, 0.0f, 0.0f, // bottom left
			-1.05f,  1.05f, -1.05f, 0.0f, 1.0f  // top left
		};

        private readonly float[] _bottomFace =
        {
            1.05f, -1.05f,  1.05f, 1.0f, 1.0f, // top right
			1.05f, -1.05f, -1.05f, 1.0f, 0.0f, // bottom right
			-1.05f, -1.05f, -1.05f, 0.0f, 0.0f, // bottom left
			-1.05f, -1.05f,  1.05f, 0.0f, 1.0f  // top left
		};

        private readonly float[] _leftFace =
        {
            1.05f,  1.05f, -1.05f, 1.0f, 1.0f, // top right
			1.05f, -1.05f, -1.05f, 1.0f, 0.0f, // bottom right
			1.05f, -1.05f,  1.05f, 0.0f, 0.0f, // bottom left
			1.05f,  1.05f,  1.05f, 0.0f, 1.0f  // top left
		};

        private readonly float[] _rightFace =
        {
            -1.05f,  1.05f,  1.05f, 1.0f, 1.0f, // top right
			-1.05f, -1.05f,  1.05f, 1.0f, 0.0f, // bottom right
			-1.05f, -1.05f, -1.05f, 0.0f, 0.0f, // bottom left
			-1.05f,  1.05f, -1.05f, 0.0f, 1.0f  // top left
		};


        private readonly uint[] _index =
        {
            0, 1, 3,
            1, 2, 3
        };

        private readonly Vector3[] _pointLightPositions =
        {
            new Vector3(0.7f, 0.2f, 2.0f),
            new Vector3(2.3f, -3.3f, -4.0f),
            new Vector3(-4.0f, 2.0f, -12.0f),
            new Vector3(0.0f, 0.0f, -3.0f)
        };

        private int _frontElementBuffer;
        private int _frontVertexBuffer;
        private int _frontVertexArray;

        private int _backElementBuffer;
        private int _backVertexBuffer;
        private int _backVertexArray;

        private int _topElementBuffer;
        private int _topVertexBuffer;
        private int _topVertexArray;

        private int _bottomElementBuffer;
        private int _bottomVertexBuffer;
        private int _bottomVertexArray;

        private int _leftElementBuffer;
        private int _leftVertexBuffer;
        private int _leftVertexArray;

        private int _rightElementBuffer;
        private int _rightVertexBuffer;
        private int _rightVertexArray;

        private int _vaoModel;

        private readonly Vector3 _lightPos = new Vector3(1.2f, 1.0f, 2.0f);

        private Shader _shader;

        private Shader _basicLighting;
        private Shader _lightingMaps;
        private Shader _directionalLights;
        private Shader _pointLights;
        private Shader _spotLight;
        private Shader _multipleLights;

        private Shader _shaderInUse;

        private Texture _texture;

        private readonly Vector3[] _posCube =
        {
            new Vector3(0.0f, 0.0f, 0.0f)
        };


        private Camera _camera;
        private bool _move = true;
        private Vector2 _lastPosition;
        private Vector3 _origin = new(0, 0, 0);
        private float speed = 0.10f;

        public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
               : base(gameWindowSettings, nativeWindowSettings)
        {
            mundo ??= new Objeto(null, ref rotuloNovo); 
        }


        protected override void OnLoad()
        {
            base.OnLoad();

            Utilitario.Diretivas();
#if CG_DEBUG
            Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif

            GL.Enable(EnableCap.DepthTest);

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            _shader = new Shader("Shaders/shaderTexture.vert", "Shaders/shaderTexture.frag");
            _shader.Use();

            _shaderInUse = _shader;

            _basicLighting = new Shader("Shaders/shaderLighting.vert", "Shaders/shaderLighting.frag");
            _lightingMaps = new Shader("Shaders/shaderMaps.vert", "Shaders/shaderMaps.frag");
            _directionalLights = new Shader("Shaders/shaderDirectional.vert", "Shaders/shaderDirectional.frag");
            _pointLights = new Shader("Shaders/shaderPoint.vert", "Shaders/shaderPoint.frag");
            _spotLight = new Shader("Shaders/shaderSpot.vert", "Shaders/shaderSpot.frag");
            _multipleLights = new Shader("Shaders/shaderMultiple.vert", "Shaders/shaderMultiple.frag");

            //front
            _frontVertexArray = GL.GenVertexArray();
            GL.BindVertexArray(_frontVertexArray);

            _frontVertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _frontVertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _frontFace.Length * sizeof(float), _frontFace, BufferUsageHint.StaticDraw);

            var frontVertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(frontVertexLocation);
            GL.VertexAttribPointer(frontVertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocationFront = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocationFront);
            GL.VertexAttribPointer(texCoordLocationFront, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            _frontElementBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _frontElementBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _index.Length * sizeof(uint), _index, BufferUsageHint.StaticDraw);

            //back
            _backVertexArray = GL.GenVertexArray();
            GL.BindVertexArray(_backVertexArray);

            _backVertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _backVertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _backFace.Length * sizeof(float), _backFace, BufferUsageHint.StaticDraw);

            var backVertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(backVertexLocation);
            GL.VertexAttribPointer(backVertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocationBack = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocationBack);
            GL.VertexAttribPointer(texCoordLocationBack, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            _backElementBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _backElementBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _index.Length * sizeof(uint), _index, BufferUsageHint.StaticDraw);

            //top
            _topVertexArray = GL.GenVertexArray();
            GL.BindVertexArray(_topVertexArray);

            _topVertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _topVertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _topFace.Length * sizeof(float), _topFace, BufferUsageHint.StaticDraw);

            var topVertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(topVertexLocation);
            GL.VertexAttribPointer(topVertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocationTop = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocationTop);
            GL.VertexAttribPointer(texCoordLocationTop, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            _topElementBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _topElementBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _index.Length * sizeof(uint), _index, BufferUsageHint.StaticDraw);

            //bottom
            _bottomVertexArray = GL.GenVertexArray();
            GL.BindVertexArray(_bottomVertexArray);

            _bottomVertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _bottomVertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _bottomFace.Length * sizeof(float), _bottomFace, BufferUsageHint.StaticDraw);

            var bottomVertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(bottomVertexLocation);
            GL.VertexAttribPointer(bottomVertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocationBottom = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocationBottom);
            GL.VertexAttribPointer(texCoordLocationBottom, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            _bottomElementBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _bottomElementBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _index.Length * sizeof(uint), _index, BufferUsageHint.StaticDraw);

            //right
            _rightVertexArray = GL.GenVertexArray();
            GL.BindVertexArray(_rightVertexArray);

            _rightVertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _rightVertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _rightFace.Length * sizeof(float), _rightFace, BufferUsageHint.StaticDraw);

            var rightVertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(rightVertexLocation);
            GL.VertexAttribPointer(rightVertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocationRight = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocationRight);
            GL.VertexAttribPointer(texCoordLocationRight, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            _rightElementBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _rightElementBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _index.Length * sizeof(uint), _index, BufferUsageHint.StaticDraw);

            //left
            _leftVertexArray = GL.GenVertexArray();
            GL.BindVertexArray(_leftVertexArray);

            _leftVertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _leftVertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _leftFace.Length * sizeof(float), _leftFace, BufferUsageHint.StaticDraw);

            var leftVertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(leftVertexLocation);
            GL.VertexAttribPointer(leftVertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocationLeft = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocationLeft);
            GL.VertexAttribPointer(texCoordLocationLeft, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            _leftElementBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _leftElementBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _index.Length * sizeof(uint), _index, BufferUsageHint.StaticDraw);

            _texture = Texture.LoadFromFile("Resource/sarah.jpg");
            _texture.Use(TextureUnit.Texture0);


            _shader.SetInt("texture0", 0);

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

            #region Objeto: Cubo
            objetoSelecionado = new Cubo(mundo, ref rotuloNovo);
            objetoSelecionado.shaderCor = _shaderBranca;
            #endregion

            #region Objeto: Cubo Menor
            _orbitingCube = new Cubo(mundo, ref rotuloNovo);
            _orbitingCube.shaderCor = _shaderAmarela;
            _orbitingCube.MatrizEscalaXYZ(0.3, 0.3, 0.3);
            _orbitingCube.MatrizTranslacaoXYZ(3.0, 0.0, 0.0);
            #endregion

            _camera = new Camera(Vector3.UnitZ * 5, ClientSize.X / (float)ClientSize.Y);

            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            mundo.Desenhar(new Transformacao4D(), _camera);

            _texture.Use(TextureUnit.Texture0);

            GL.BindVertexArray(_vaoModel);

            if (_shaderInUse == _basicLighting)
            {
                _basicLighting.Use();

                _basicLighting.SetMatrix4("model", Matrix4.Identity);
                _basicLighting.SetMatrix4("view", _camera.GetViewMatrix());
                _basicLighting.SetMatrix4("projection", _camera.GetProjectionMatrix());

                _basicLighting.SetVector3("objectColor", new Vector3(1.0f, 0.5f, 0.31f));
                _basicLighting.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
                _basicLighting.SetVector3("lightPos", _lightPos);
                _basicLighting.SetVector3("viewPos", _camera.Position);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }

            if (_shaderInUse == _lightingMaps)
            {
                _lightingMaps.Use();

                _lightingMaps.SetMatrix4("model", Matrix4.Identity);
                _lightingMaps.SetMatrix4("view", _camera.GetViewMatrix());
                _lightingMaps.SetMatrix4("projection", _camera.GetProjectionMatrix());

                _lightingMaps.SetVector3("viewPos", _camera.Position);

                _lightingMaps.SetInt("material.diffuse", 0);
                _lightingMaps.SetInt("material.specular", 1);
                _lightingMaps.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
                _lightingMaps.SetFloat("material.shininess", 32.0f);

                _lightingMaps.SetVector3("light.position", _lightPos);
                _lightingMaps.SetVector3("light.ambient", new Vector3(0.2f));
                _lightingMaps.SetVector3("light.diffuse", new Vector3(0.5f));
                _lightingMaps.SetVector3("light.specular", new Vector3(1.0f));

                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }

            if (_shaderInUse == _directionalLights)
            {
                _directionalLights.Use();

                _directionalLights.SetMatrix4("model", Matrix4.Identity);
                _directionalLights.SetMatrix4("view", _camera.GetViewMatrix());
                _directionalLights.SetMatrix4("projection", _camera.GetProjectionMatrix());

                _directionalLights.SetVector3("viewPos", _camera.Position);

                _directionalLights.SetInt("material.diffuse", 0);
                _directionalLights.SetInt("material.specular", 1);
                _directionalLights.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
                _directionalLights.SetFloat("material.shininess", 32.0f);

                _directionalLights.SetVector3("light.direction", new Vector3(-0.2f, -1.0f, -0.3f));
                _directionalLights.SetVector3("light.ambient", new Vector3(0.2f));
                _directionalLights.SetVector3("light.diffuse", new Vector3(0.5f));
                _directionalLights.SetVector3("light.specular", new Vector3(1.0f));

                for (int i = 0; i < _posCube.Length; i++)
                {
                    Matrix4 model = Matrix4.CreateTranslation(_posCube[i]);
                    float angle = 20.0f * i;
                    model = model * Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), angle);
                    _directionalLights.SetMatrix4("model", model);

                    GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                }
            }

            if (_shaderInUse == _pointLights)
            {
                _pointLights.Use();

                _pointLights.SetMatrix4("model", Matrix4.Identity);
                _pointLights.SetMatrix4("view", _camera.GetViewMatrix());
                _pointLights.SetMatrix4("projection", _camera.GetProjectionMatrix());

                _pointLights.SetVector3("viewPos", _camera.Position);

                _pointLights.SetInt("material.diffuse", 0);
                _pointLights.SetInt("material.specular", 1);
                _pointLights.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
                _pointLights.SetFloat("material.shininess", 32.0f);

                _pointLights.SetVector3("light.position", _lightPos);
                _pointLights.SetFloat("light.constant", 1.0f);
                _pointLights.SetFloat("light.linear", 0.09f);
                _pointLights.SetFloat("light.quadratic", 0.032f);
                _pointLights.SetVector3("light.ambient", new Vector3(0.2f));
                _pointLights.SetVector3("light.diffuse", new Vector3(0.5f));
                _pointLights.SetVector3("light.specular", new Vector3(1.0f));

                for (int i = 0; i < _posCube.Length; i++)
                {
                    Matrix4 model = Matrix4.CreateTranslation(_posCube[i]);
                    float angle = 20.0f * i;
                    model = model * Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), angle);
                    _pointLights.SetMatrix4("model", model);

                    GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                }
            }

            if (_shaderInUse == _spotLight)
            {
                _spotLight.Use();

                _spotLight.SetMatrix4("view", _camera.GetViewMatrix());
                _spotLight.SetMatrix4("projection", _camera.GetProjectionMatrix());

                _spotLight.SetVector3("viewPos", _camera.Position);

                _spotLight.SetInt("material.diffuse", 0);
                _spotLight.SetInt("material.specular", 1);
                _spotLight.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
                _spotLight.SetFloat("material.shininess", 32.0f);

                _spotLight.SetVector3("light.position", _camera.Position);
                _spotLight.SetVector3("light.direction", _camera.Front);
                _spotLight.SetFloat("light.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
                _spotLight.SetFloat("light.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(17.5f)));
                _spotLight.SetFloat("light.constant", 1.0f);
                _spotLight.SetFloat("light.linear", 0.09f);
                _spotLight.SetFloat("light.quadratic", 0.032f);
                _spotLight.SetVector3("light.ambient", new Vector3(0.2f));
                _spotLight.SetVector3("light.diffuse", new Vector3(0.5f));
                _spotLight.SetVector3("light.specular", new Vector3(1.0f));

                for (int i = 0; i < _posCube.Length; i++)
                {
                    Matrix4 model = Matrix4.CreateTranslation(_posCube[i]);
                    float angle = 20.0f * i;
                    model = model * Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), angle);
                    _spotLight.SetMatrix4("model", model);

                    GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                }
            }

            if (_shaderInUse == _multipleLights)
            {
                _multipleLights.Use();

                _multipleLights.SetMatrix4("view", _camera.GetViewMatrix());
                _multipleLights.SetMatrix4("projection", _camera.GetProjectionMatrix());

                _multipleLights.SetVector3("viewPos", _camera.Position);

                _multipleLights.SetInt("material.diffuse", 0);
                _multipleLights.SetInt("material.specular", 1);
                _multipleLights.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
                _multipleLights.SetFloat("material.shininess", 32.0f);

                _multipleLights.SetVector3("dirLight.direction", new Vector3(-0.2f, -1.0f, -0.3f));
                _multipleLights.SetVector3("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
                _multipleLights.SetVector3("dirLight.diffuse", new Vector3(0.4f, 0.4f, 0.4f));
                _multipleLights.SetVector3("dirLight.specular", new Vector3(0.5f, 0.5f, 0.5f));

                for (int i = 0; i < _pointLightPositions.Length; i++)
                {
                    _multipleLights.SetVector3($"pointLights[{i}].position", _pointLightPositions[i]);
                    _multipleLights.SetVector3($"pointLights[{i}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
                    _multipleLights.SetVector3($"pointLights[{i}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
                    _multipleLights.SetVector3($"pointLights[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                    _multipleLights.SetFloat($"pointLights[{i}].constant", 1.0f);
                    _multipleLights.SetFloat($"pointLights[{i}].linear", 0.09f);
                    _multipleLights.SetFloat($"pointLights[{i}].quadratic", 0.032f);
                }

                _multipleLights.SetVector3("spotLight.position", _camera.Position);
                _multipleLights.SetVector3("spotLight.direction", _camera.Front);
                _multipleLights.SetVector3("spotLight.ambient", new Vector3(0.0f, 0.0f, 0.0f));
                _multipleLights.SetVector3("spotLight.diffuse", new Vector3(1.0f, 1.0f, 1.0f));
                _multipleLights.SetVector3("spotLight.specular", new Vector3(1.0f, 1.0f, 1.0f));
                _multipleLights.SetFloat("spotLight.constant", 1.0f);
                _multipleLights.SetFloat("spotLight.linear", 0.09f);
                _multipleLights.SetFloat("spotLight.quadratic", 0.032f);
                _multipleLights.SetFloat("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
                _multipleLights.SetFloat("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(17.5f)));

                for (int i = 0; i < _posCube.Length; i++)
                {
                    Matrix4 model = Matrix4.CreateTranslation(_posCube[i]);
                    float angle = 20.0f * i;
                    model = model * Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), angle);
                    _multipleLights.SetMatrix4("model", model);

                    GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                }
            }

            GL.BindVertexArray(_frontVertexArray);
            _texture.Use(TextureUnit.Texture0);
            _shaderInUse.Use();

            GL.DrawElements(PrimitiveType.Triangles, _index.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(_backVertexArray);
            _texture.Use(TextureUnit.Texture0);
            _shaderInUse.Use();

            GL.DrawElements(PrimitiveType.Triangles, _index.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(_topVertexArray);
            _texture.Use(TextureUnit.Texture0);
            _shaderInUse.Use();

            GL.DrawElements(PrimitiveType.Triangles, _index.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(_bottomVertexArray);
            _texture.Use(TextureUnit.Texture0);
            _shaderInUse.Use();

            GL.DrawElements(PrimitiveType.Triangles, _index.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(_rightVertexArray);
            _texture.Use(TextureUnit.Texture0);
            _shaderInUse.Use();

            GL.DrawElements(PrimitiveType.Triangles, _index.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(_leftVertexArray);
            _texture.Use(TextureUnit.Texture0);
            _shaderInUse.Use();

            GL.DrawElements(PrimitiveType.Triangles, _index.Length, DrawElementsType.UnsignedInt, 0);

#if CG_Gizmo
            Gizmo_Sru3D();
#endif
            SwapBuffers();
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            _orbitingCube.MatrizRotacao(speed);

            #region Teclado
            var estadoTeclado = KeyboardState;
            if (estadoTeclado.IsKeyDown(Keys.Escape))
                Close();

            if (estadoTeclado.IsKeyDown(Keys.D0))
                _shaderInUse = _shader;

            if (estadoTeclado.IsKeyDown(Keys.D1))
                _shaderInUse = _basicLighting;

            if (estadoTeclado.IsKeyDown(Keys.D2))
                _shaderInUse = _lightingMaps;

            if (estadoTeclado.IsKeyDown(Keys.D3))
                _shaderInUse = _directionalLights;

            if (estadoTeclado.IsKeyDown(Keys.D4))
                _shaderInUse = _pointLights;

            if (estadoTeclado.IsKeyDown(Keys.D5))
                _shaderInUse = _spotLight;

            if (estadoTeclado.IsKeyDown(Keys.D6))
                _shaderInUse = _multipleLights;

            const float cameraSpeed = 1.5f;

            var front = Vector3.Normalize(_origin - _camera.Position);
            var right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
            var up = Vector3.Normalize(Vector3.Cross(right, front));

            if (estadoTeclado.IsKeyDown(Keys.R))
                _camera.Position = Vector3.UnitZ * 5;

            if (estadoTeclado.IsKeyDown(Keys.W))
                _camera.Position += front * cameraSpeed * (float)e.Time;

            if (estadoTeclado.IsKeyDown(Keys.S))
                _camera.Position -= front * cameraSpeed * (float)e.Time;

            if (estadoTeclado.IsKeyDown(Keys.A))
                _camera.Position -= right * cameraSpeed * (float)e.Time;

            if (estadoTeclado.IsKeyDown(Keys.D))
                _camera.Position += right * cameraSpeed * (float)e.Time;

            if (estadoTeclado.IsKeyDown(Keys.RightShift))
                _camera.Position += up * cameraSpeed * (float)e.Time;

            if (estadoTeclado.IsKeyDown(Keys.LeftShift))
                _camera.Position -= up * cameraSpeed * (float)e.Time;

            #endregion

            #region  Mouse

            #endregion

            const float rotationSpeed = 35f;

            if (MouseState.IsButtonDown(MouseButton.Left))
            {
                int janelaLargura = ClientSize.X;
                int janelaAltura = ClientSize.Y;
                Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
                Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

                if (_move)
                {

                    _camera.Position = Vector3.UnitZ * 5;
                    _lastPosition = new Vector2((float)sruPonto.X, (float)sruPonto.Y);
                    _move = false;
                }
                else
                {
                    var deltaX = sruPonto.X - _lastPosition.X;
                    var deltaY = sruPonto.Y - _lastPosition.Y;
                    _lastPosition = new Vector2((float)sruPonto.X, (float)sruPonto.Y);

                    _camera.Pitch -= rotationSpeed * -(float)sruPonto.Y * (float)e.Time;
                    _camera.Position += _camera.Up * cameraSpeed * -(float)sruPonto.Y * (float)e.Time;

                    _camera.Yaw -= rotationSpeed * (float)sruPonto.X * (float)e.Time;
                    _camera.Position += _camera.Right * cameraSpeed * (float)1.9 * (float)sruPonto.X * (float)e.Time;
                }


            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

#if CG_DEBUG
            Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        }

        protected override void OnUnload()
        {
            mundo.OnUnload();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject_sruEixos);
            GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

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
            var model = Matrix4.Identity;
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            // Textura
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _shader.Use();
            // EixoX
            _shaderVermelha.SetMatrix4("model", model);
            _shaderVermelha.SetMatrix4("view", _camera.GetViewMatrix());
            _shaderVermelha.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _shaderVermelha.Use();
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
            // EixoY
            _shaderVerde.SetMatrix4("model", model);
            _shaderVerde.SetMatrix4("view", _camera.GetViewMatrix());
            _shaderVerde.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _shaderVerde.Use();
            GL.DrawArrays(PrimitiveType.Lines, 2, 2);
            // EixoZ
            _shaderAzul.SetMatrix4("model", model);
            _shaderAzul.SetMatrix4("view", _camera.GetViewMatrix());
            _shaderAzul.SetMatrix4("projection", _camera.GetProjectionMatrix());
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
