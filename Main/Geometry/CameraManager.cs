using Main.Geometry;
using Main.Main;
using Main.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.StructureEditor
{
    class CameraManager
    {
        public enum CameraMode
        {
            Editor = 0, FreeCam = 1, EditorZoomable = 2
        }

        public Camera Camera { get; private set; }
        public Vector3 Position { get; set; }
        public Vector3 LookatPosition { get; set; } = new Vector3();
        //public Vector3 ViewDirection { get; set; }
        public InputManager Input { get; set; }

        public Cube LookatCube { get; set; }

        public bool CanMove { get; set; } = true;

        float CreativeVel = 20;
        float Sensitivity = 7.0f;

        float RotY = 0f;
        float RotXZ = 90;

        public float Distance { get; set; } = 10;

        float MinDistance = 5.0f;
        float MaxDistance = 25.0f;

        public long LastClick = 0;
        public long TickRate = 250;
        
        public List<Camera> CameraList = new List<Camera>();

        public CameraMode CurrentMode { get; set; } = CameraMode.Editor;

        public CameraManager(Vector3 Pos, RenderManager Render, InputManager InputManager)
        {
            Position = Pos;
            
            Camera = new Camera(new Vector3(10, 10, 30),Render.GetScreenWidth(), Render.GetScreenHeight(), Render.GetFarPlane(), 1, MathHelper.PiOver2);

            Input = InputManager;
            LookatCube = new Cube( LookatPosition, new Vector3(0.5f, 0.5f, 0.5f));
            LookatCube.GenerateLookatCube(Render.Graphics);
            Render.RootNode.Attach(LookatCube.Geom);
            LookatCube.Geom.HasCull = false;
        }
        
        public void ToggleCubeVisibility()
        {
            LookatCube.Geom.HasCull = !LookatCube.Geom.HasCull;
        }

        
        public void Update(GameTime GameTime)
        {
            UpdateMovement(GameTime);
            if (!LookatCube.Geom.HasCull)
            {
                LookatCube.SetPosition(LookatPosition);
            }
        }
        
        
        public void SetMode(int Mode)
        {
            if (Mode != (int)CurrentMode)
            {

                if (CurrentMode == CameraMode.FreeCam)
                {
                    //RotXZ = MathUtil.GetAngle(new Vector2(LookatPosition.X, LookatPosition.Y), new Vector2(Position.X, Position.Y));
                    //RotY = MathUtil.GetAngle(new Vector2(LookatPosition.X, LookatPosition.Y), new Vector2(Position.X, Position.Y));

                }
            }
            CurrentMode = (CameraMode)Mode;
        }

        public void SetPosition(Vector3 NewPosition)
        {   
            Position = NewPosition;
            Vector3 ViewDirection = Camera.ViewDirection;
            LookatPosition = Position + ViewDirection * 20;
            Camera.Position = Position;
            Camera.GenerateLookAtViewMatrix(Camera.Position, LookatPosition);
        }
        
        public void UpdateMovement(GameTime GameTime)
        {
            float Time = (GameTime.ElapsedGameTime.Milliseconds / 100.0f);

            Vector3 ViewDirection = Camera.ViewDirection;

            if (CanMove)
            {
                if (CurrentMode == CameraMode.Editor && !Input.HasMouseLock)
                {
                    Vector2 MouseMovement = Input.MouseMovement;

                    if (Input.HasRegisteredInput("Forward"))
                    {
                        Vector2 Dir = new Vector2( ViewDirection.X ,  ViewDirection.Y ) * Time;
                        LookatPosition = new Vector3(LookatPosition.X + Dir.X, LookatPosition.Y + Dir.Y, LookatPosition.Z);
                    }
                    if (Input.HasRegisteredInput("Backward"))
                    {
                        Vector2 Dir = new Vector2( -ViewDirection.X ,   -ViewDirection.Y ) * Time;
                        LookatPosition = new Vector3(LookatPosition.X + Dir.X, LookatPosition.Y + Dir.Y, LookatPosition.Z);
                    }
                    if (Input.HasRegisteredInput("Left"))
                    {
                        Vector2 Dir = new Vector2( -ViewDirection.Y , ViewDirection.X ) * Time;
                        LookatPosition = new Vector3(LookatPosition.X + Dir.X, LookatPosition.Y + Dir.Y, LookatPosition.Z);
                    }
                    if (Input.HasRegisteredInput("Right"))
                    {
                        Vector2 Dir = new Vector2( ViewDirection.Y ,  -ViewDirection.X ) * Time;
                        LookatPosition = new Vector3(LookatPosition.X + Dir.X, LookatPosition.Y + Dir.Y, LookatPosition.Z);
                    }

                    if (Input.HasRegisteredInput("Down"))
                    {
                        Position = new Vector3(Position.X, Position.Y, Position.Z - Time);
                        LookatPosition = new Vector3(LookatPosition.X, LookatPosition.Y, LookatPosition.Z - Time);
                    }
                    if (Input.HasRegisteredInput("Up"))
                    {
                        Position = new Vector3(Position.X, Position.Y, Position.Z + Time);
                        LookatPosition = new Vector3(LookatPosition.X, LookatPosition.Y, LookatPosition.Z + Time);
                    }

                    if (Input.HasRegisteredInput("Interact1"))
                    {
                        RotXZ = (RotXZ + (Sensitivity * Time));
                        RotXZ %= 360;
                    }

                    if (Input.HasRegisteredInput("Interact2"))
                    {
                        RotXZ = (RotXZ + (-Sensitivity * Time));
                        RotXZ %= 360;
                    }

                    if (Input.HasRegisteredMouseInput("Middle"))
                    {
                        float ValueX = -MouseMovement.X * Time * Sensitivity / 25.0f;

                        RotXZ = (RotXZ + (ValueX * Sensitivity));
                        RotXZ %= 360;

                        float ValueY = MouseMovement.Y * Time * Sensitivity / 25.0f;

                        Position = new Vector3(Position.X, Position.Y, Position.Z + ValueY);
                    }

                    float ScrollValue = Input.MouseWheelValue * Time / 10.0f;

                    if (ScrollValue != 0.0f)
                    {
                        Vector3 LookAt = Vector3.Normalize(LookatPosition - Position);
                        
                        Position += ViewDirection * -ScrollValue * Time * 5.0f;
                        LookatPosition += ViewDirection * -ScrollValue * Time * 5.0f;
                    }
                    
                    double Radian = MathUtil.GetRadian(RotXZ);
                    Vector2 NewPosition = Vector2.Transform(new Vector2(Distance), Matrix.CreateRotationZ((float)Radian));
                    Position = new Vector3(NewPosition.X + LookatPosition.X, NewPosition.Y + LookatPosition.Y, Position.Z);

                    bool HasClick = Input.HasRegisteredMouseInput("Primary") && LastClick > TickRate;

                    if (HasClick)
                    {
                        LastClick = 0;
                    }
                    LastClick += GameTime.ElapsedGameTime.Milliseconds;
                    LastClick %= 10000;

                    Camera.Position = Position;
                    
                    Camera.GenerateLookAtViewMatrix(Camera.Position, LookatPosition);
                    
                }
                else if (CurrentMode == CameraMode.FreeCam && Input.HasMouseLock)
                {
                    Vector2 MouseMovement = Input.MouseMovement;

                    RotY = (float)(RotY + (MouseMovement.Y / 572.958) * Sensitivity * Time);
                    RotY = RotY < -(1.0f - float.Epsilon) ? -(1.0f - float.Epsilon) : RotY;
                    RotY = RotY > 1.0f - float.Epsilon ? 1.0f - float.Epsilon : RotY;

                    RotXZ = (RotXZ + (-MouseMovement.X / 10) * Sensitivity * Time);
                    RotXZ %= 360;

                    double radian = MathUtil.GetRadian(RotXZ);
                    ViewDirection = new Vector3((float)Math.Cos(radian), (float)Math.Sin(radian), -RotY);
                    float Val = GameTime.ElapsedGameTime.Milliseconds * 0.001f * CreativeVel;

                    if (Input.HasRegisteredInput("Forward"))
                    {
                        Position += Camera.ViewDirection * Val;
                    }
                    if (Input.HasRegisteredInput("Backward"))
                    {
                        Position += Camera.ViewDirection * -Val;
                    }
                    if (Input.HasRegisteredInput("Left"))
                    {
                        Vector3 Add = Vector3.Transform(Camera.ViewDirection, Matrix.CreateRotationZ(MathUtil.GetRadian(90))) * Val;
                        Add.Z = 0;
                        Position += Add;

                    }
                    if (Input.HasRegisteredInput("Right"))
                    {
                        Vector3 Add = Vector3.Transform(Camera.ViewDirection, Matrix.CreateRotationZ(MathUtil.GetRadian(90))) * -Val;
                        Add.Z = 0;
                        Position += Add;
                    }
                    LookatPosition = Position + ViewDirection * 20;

                    Camera.Position = Position;
                    Camera.GenerateLookAtViewMatrix(Camera.Position, LookatPosition);

                }
                else if (CurrentMode == CameraMode.EditorZoomable && !Input.HasMouseLock)
                {

                    Vector2 MouseMovement = Input.MouseMovement;

                    if (Input.HasRegisteredInput("Forward"))
                    {
                        Vector2 Dir = new Vector2(ViewDirection.X, ViewDirection.Y) * Time;
                        LookatPosition = new Vector3(LookatPosition.X + Dir.X, LookatPosition.Y + Dir.Y, LookatPosition.Z);
                    }
                    if (Input.HasRegisteredInput("Backward"))
                    {
                        Vector2 Dir = new Vector2(-ViewDirection.X, -ViewDirection.Y) * Time;
                        LookatPosition = new Vector3(LookatPosition.X + Dir.X, LookatPosition.Y + Dir.Y, LookatPosition.Z);
                    }
                    if (Input.HasRegisteredInput("Left"))
                    {
                        Vector2 Dir = new Vector2(-ViewDirection.Y, ViewDirection.X) * Time;
                        LookatPosition = new Vector3(LookatPosition.X + Dir.X, LookatPosition.Y + Dir.Y, LookatPosition.Z);
                    }
                    if (Input.HasRegisteredInput("Right"))
                    {
                        Vector2 Dir = new Vector2(ViewDirection.Y, -ViewDirection.X) * Time;
                        LookatPosition = new Vector3(LookatPosition.X + Dir.X, LookatPosition.Y + Dir.Y, LookatPosition.Z);
                    }

                    if (Input.HasRegisteredInput("Down"))
                    {
                        Position = new Vector3(Position.X, Position.Y, Position.Z - Time);
                        LookatPosition = new Vector3(LookatPosition.X, LookatPosition.Y, LookatPosition.Z - Time);
                    }
                    if (Input.HasRegisteredInput("Up"))
                    {
                        Position = new Vector3(Position.X, Position.Y, Position.Z + Time);
                        LookatPosition = new Vector3(LookatPosition.X, LookatPosition.Y, LookatPosition.Z + Time);
                    }

                    if (Input.HasRegisteredInput("Interact1"))
                    {
                        RotXZ = (RotXZ + (Sensitivity * Time));
                        RotXZ %= 360;
                    }

                    if (Input.HasRegisteredInput("Interact2"))
                    {
                        RotXZ = (RotXZ + (-Sensitivity * Time));
                        RotXZ %= 360;
                    }

                    if (Input.HasRegisteredMouseInput("Middle"))
                    {
                        float ValueX = -MouseMovement.X * Time * Sensitivity / 25.0f;

                        RotXZ = (RotXZ + (ValueX * Sensitivity));
                        RotXZ %= 360;

                        float ValueY = MouseMovement.Y * Time * Sensitivity / 25.0f;

                        Position = new Vector3(Position.X, Position.Y, Position.Z + ValueY);
                    }

                    float ScrollValue = Input.MouseWheelValue * Time / 10.0f;

                    if (ScrollValue != 0.0f)
                    {
                        Distance += ScrollValue;

                        Distance = Distance < MinDistance ? MinDistance : Distance;
                        Distance = Distance > MaxDistance ? MaxDistance : Distance;

                        Position = Position.Z + ScrollValue > Distance || Position.Z + ScrollValue < -Distance ? new Vector3(Position.X, Position.Y, Position.Z + ScrollValue) : Position;
                    }

                    double Radian = MathUtil.GetRadian(RotXZ);
                    Vector2 NewPosition = Vector2.Transform(new Vector2(Distance), Matrix.CreateRotationZ((float)Radian));
                    Position = new Vector3(NewPosition.X + LookatPosition.X, NewPosition.Y + LookatPosition.Y, Position.Z);

                    bool HasClick = Input.HasRegisteredMouseInput("Primary") && LastClick > TickRate;

                    if (HasClick)
                    {
                        LastClick = 0;
                    }
                    LastClick += GameTime.ElapsedGameTime.Milliseconds;
                    LastClick %= 10000;

                    Camera.Position = Position;

                    Camera.GenerateLookAtViewMatrix(Camera.Position, LookatPosition);
                    

                }

            }
            
        }
        

    }
}
