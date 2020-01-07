using System.Windows;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Assets;
using SharpGL.Enumerations;	
using System;
using SharpGL.SceneGraph.Primitives;
using System.Windows.Input;

namespace WPFOpenGl
{
	/// <summary>
	///    Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private MapHeight mapHeight;
		private double zoom = 0.007f;

		private double eyeZ = 1f, eyeX = 1f, eyeY = 0.5f;
		private double velocityCam = 0.01f;
		private GLColor[,] colors;
		private float texBit;
		private int numFrame = 0;

		Texture textureGround = new Texture();
		Texture textureSnow = new Texture();
		Texture textureWater = new Texture();

		private float rtri = 0;
		//Random rand = new Random();
		//double rotX = 0, rotY = 0, rotZ = 0;
		//private float rquad = 0;

		public MainWindow()
		{
			InitializeComponent();

			mapHeight = new MapHeight(256, 0.66, 0.25);

			texBit = 1.0f / mapHeight.MapSize;

			Random rand = new Random();
			colors = new GLColor[mapHeight.MapSize - 1, mapHeight.MapSize - 1];
			for (int i = 0; i < mapHeight.MapSize - 1; i++)
				for (int j = 0; j < mapHeight.MapSize - 1; j++)
				{
					float col = (float)rand.NextDouble() / 2.0f + 0.5f;
					//float col = (float)rand.NextDouble();
					colors[i, j] = new GLColor(col, col, col, 1f);
				}
		}

		private void OpenGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
		{
			numFrame++;
			if (numFrame % 1 == 0)
				mapHeight.update_map();

			//var gl = args.OpenGL;
			OpenGL gl = openGlCtrl.OpenGL;


			//gl.MatrixMode(OpenGL.GL_PROJECTION);

			//Очистка цветого буфера экрана и z-буфера
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
			gl.LoadIdentity();

			//  Преобразование
			//gl.Perspective(60.0f, (double)Width / (double)Height, 0.01, 100.0);
			//gl.LookAt(-1, -1, 0.2,    // Позиция самой камеры (x, y, z)
			//		   1, 1, 10,     // Направление, куда мы смотрим
			//		   0, 1, 0);    // Верх камеры
			//						//  Зададим модель отображения
			//gl.MatrixMode(OpenGL.GL_MODELVIEW);


			gl.LookAt(eyeX, eyeY, eyeZ, eyeX + 5f, eyeY + 5f, eyeZ + 5f, 0f, 1f, 0f);


			gl.Translate(1f, 0f, 1f);             //Сдвиг
			//gl.Rotate(rtri, 0f, 1f, 0f);      //Вращение
			gl.Translate(-1f, 0f, -1f);             //Сдвиг

			textureGround.Bind(gl);
			build_landscape(gl, TypeOfLandscape.Ground);

			textureSnow.Bind(gl);
			build_landscape(gl, TypeOfLandscape.Snow);

			textureWater.Bind(gl);
			build_landscape(gl, TypeOfLandscape.Water);

			rtri += 2f;
		}


		private void build_landscape(OpenGL gl, TypeOfLandscape typeLandscape)
		{
			gl.Begin(OpenGL.GL_TRIANGLES);
			for (int i = 0; i < mapHeight.MapSize - 1; i++)
				for (int j = 0; j < mapHeight.MapSize - 1; j++)
				{
					if (mapHeight.get_type(i, j) == typeLandscape)
					{
						double x = j * zoom;
						double y = i * zoom;

						gl.Color(1f, 1f, 1f);

						gl.TexCoord(i * texBit, j * texBit);
						gl.Vertex(x, mapHeight.get_height(i, j), y);
						gl.TexCoord(i * texBit, (j + 1) * texBit);
						gl.Vertex(x + zoom, mapHeight.get_height(i, j + 1), y);
						gl.TexCoord((i + 1) * texBit, j * texBit);
						gl.Vertex(x, mapHeight.get_height(i + 1, j), y + zoom);

						gl.TexCoord(i * texBit, (j + 1) * texBit);
						gl.Vertex(x + zoom, mapHeight.get_height(i, j + 1), y);
						gl.TexCoord((i + 1) * texBit, j * texBit);
						gl.Vertex(x, mapHeight.get_height(i + 1, j), y + zoom);
						gl.TexCoord((i + 1) * texBit, (j + 1) * texBit);
						gl.Vertex(x + zoom, mapHeight.get_height(i + 1, j + 1), y + zoom);
					}
				}
			gl.End();
			gl.Flush();
		}

		private void OpenGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
		{
			OpenGL gl = openGlCtrl.OpenGL;

			gl.ClearColor(0.1f, 0.4f, 0.2f, 0.3f);



			gl.Enable(OpenGL.GL_DEPTH_TEST);

			float[] globalAmbient = new float[] { 0.5f, 0.5f, 0.5f, 1f };
			float[] light0Pos = new float[] { 2f, 5f, 0f, 1f };
			float[] light0Ambient = new float[] { 0.2f, 0.2f, 0.2f, 1f };
			float[] light0Diffuse = new float[] { 0.3f, 0.3f, 0.3f, 1f };
			float[] light0Specular = new float[] { 0.8f, 0.8f, 0.8f, 1f };

			gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, globalAmbient);
			gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0Pos);
			gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0Ambient);
			gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0Diffuse);
			gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0Specular);

			//gl.Enable(OpenGL.GL_LIGHTING);
			gl.Enable(OpenGL.GL_LIGHT0);

			//gl.ShadeModel(OpenGL.GL_SMOOTH);

			gl.Enable(OpenGL.GL_TEXTURE_2D);
			string path = "H:\\Университет\\7 сем\\Комп. графика\\Курсач\\WPFOpenGl\\WPFOpenGl\\WPFOpenGl\\Moss_Dirt.bmp";
			textureGround.Create(gl, path);
			path = "H:\\Университет\\7 сем\\Комп. графика\\Курсач\\WPFOpenGl\\WPFOpenGl\\WPFOpenGl\\Moss_Dirt_Snow.bmp";
			textureSnow.Create(gl, path);
			path = "H:\\Университет\\7 сем\\Комп. графика\\Курсач\\WPFOpenGl\\WPFOpenGl\\WPFOpenGl\\Water.bmp";
			textureWater.Create(gl, path);
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.S:
					eyeY -= velocityCam;
					break;
				case Key.W:
					eyeY += velocityCam;
					break;
				case Key.A:
					eyeX += velocityCam;
					break;
				case Key.D:
					eyeX -= velocityCam;
					break;
				case Key.Q:
					//eyeZ += velocityCam;
					zoom += 0.0001;
					break;
				case Key.E:
					//eyeZ -= velocityCam;
					zoom -= 0.0001;
					break;
			}
		}

		private void OpenGLControl_Resized(object sender, OpenGLEventArgs args)
		{
		}

		
	}
}