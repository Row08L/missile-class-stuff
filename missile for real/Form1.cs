using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace missile_for_real
{

    public partial class Form1 : Form
    {
        Timer ticks = new Timer();
        Timer missileTicks = new Timer();
        List<Missile> missileList = new List<Missile>();
        Camera playerCam;
        Player player1;
        bool wDown = false;
        bool sDown = false;
        bool aDown = false;
        bool dDown = false;

        public Form1()
        {
            InitializeComponent();
            //this.MouseMove += CursorPositionForm_MouseMove;
            this.MouseClick += Button1_MouseClick;
            PointF spawn = new PointF(20, 20);
            Vector2 velocity = new Vector2(new PointF(0, 0), new PointF(1, 1));
            ticks.Interval = 50 / 6; // Adjust interval as needed
            ticks.Tick += Timer_Tick;
            ticks.Start();
            missileTicks.Interval = 10; // Adjust interval as needed
            missileTicks.Tick += MissileTimer_Tick;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            missileTicks.Start();
            player1 = new Player(new PointF(0, 0), new Vector2(new PointF(0, 0), new PointF(0, -1)), 0.1, 32, (float)Math.PI / 64, (float)0.5);
            playerCam = new Camera(player1.Position, new SizeF(this.Width, this.Height));
            
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = true;
                    break;
                case Keys.S:
                    sDown = true;
                    break;
                case Keys.A:
                    aDown = true;
                    break;
                case Keys.D:
                    dDown = true;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = false;
                    break;
                case Keys.S:
                    sDown = false;
                    break;
                case Keys.A:
                    aDown = false;
                    break;
                case Keys.D:
                    dDown = false;
                    break;
            }
        }

        private void Button1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PointF spawn = new PointF(200, 200);
                Vector2 velocity = new Vector2(new PointF(0, 0), new PointF(20, 20));
                missileList.Add(new Missile(spawn, player1.Position, velocity, 0.06, 10, 3f, 1f));
            }
            if (e.Button == MouseButtons.Right)
            {
                missileTicks.Stop();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            player1.MovePlayer(wDown, aDown, sDown, dDown);
            for (int missle1 = 0; missle1 < missileList.Count(); missle1++)
            {
                missileList[missle1].Target = player1.Position;
                missileList[missle1] = Missile.UpdateMissilePosition(missileList[missle1]);
                missileList[missle1] = Missile.RandomizeMissilePosition(missileList[missle1]);
            }
            playerCam.SetPosition(player1.Position.X - this.Width / 2, player1.Position.Y - this.Height / 2);
            Bitmap formBitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            this.DrawToBitmap(formBitmap, new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height));
            playerCam.UpdateCamera(formBitmap);
            Refresh();
        }
        private void MissileTimer_Tick(object sender, EventArgs e)
        {
            PointF spawn = new PointF(20, 20);
            Vector2 velocity = new Vector2(new PointF(0, 0), new PointF(0, 100));
            velocity.End = new PointF(10, 10);
            missileList.Add(new Missile(spawn, player1.Position, velocity, 0.06, 20, 3f, (float)(Math.PI * 2) / 6f));
            Refresh();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Pen black = new Pen(Color.Black, 10);
            Pen orange = new Pen(Color.Orange, 10);
            for (int missle1 = 0; missle1 < missileList.Count(); missle1++)
            {
                //e.Graphics.DrawLine(black, new PointF(missileList[missle1].changeVector.End.X + missileList[missle1].Position.X, missileList[missle1].changeVector.End.Y + missileList[missle1].Position.Y), missileList[missle1].Position);
                //e.Graphics.DrawLine(orange, new PointF(missileList[missle1].Velocity1.End.X + missileList[missle1].Position.X, missileList[missle1].Velocity1.End.Y + missileList[missle1].Position.Y), missileList[missle1].Position);
                //e.Graphics.DrawLine(Pens.Green, new PointF(missileList[missle1].change.End.X + missileList[missle1].Position.X, missileList[missle1].change.End.Y + missileList[missle1].Position.Y), missileList[missle1].Position);
                e.Graphics.FillEllipse(Brushes.Black, new RectangleF(missileList[missle1].Position.X , missileList[missle1].Position.Y, 10, 10));
                //e.Graphics.FillEllipse(Brushes.Blue, new RectangleF(missileList[missle1].Target.X, missileList[missle1].Target.Y, 10, 10));
                e.Graphics.FillEllipse(Brushes.Blue, new RectangleF(player1.Position.X, player1.Position.Y, 10, 10));
                e.Graphics.DrawLine(Pens.Green, player1.Position, new PointF(player1.Position.X + player1.CurrentDircetion.End.X, player1.Position.Y + player1.CurrentDircetion.End.Y));
            }
            
            if (playerCam.ViewpostBitmap != null)
            {
                e.Graphics.DrawImage(playerCam.ViewpostBitmap, new PointF(0, 0));
            }
        }
        //private void CursorPositionForm_MouseMove(object sender, MouseEventArgs e)
        //{
        //    playerPosition = this.PointToClient(Cursor.Position); ;
        //}
    }


    public class Vector2
    {
        private PointF end;
        public PointF End
        {
            get { return end; }
            set
            {
                Magnitude = (float)Math.Sqrt(Math.Pow(End.X, 2) + Math.Pow(End.Y, 2));
                end = value;
            }
        }
        public float Magnitude { get; private set; }

        public Vector2(PointF start, PointF end)
        {
            if (start.X != 0 && start.Y != 0)
            {
                End = new PointF(end.X - start.X, end.Y - start.Y);
            }
            else
            {
                End = end;
            }
            Magnitude = (float)Math.Sqrt(Math.Pow(End.X, 2) + Math.Pow(End.Y, 2));
        }

        public static Vector2 VectorAddition(Vector2 vector1, Vector2 vector2)
        {
            float addedVectorX = vector1.End.X + vector2.End.X;
            float addedVectorY = vector1.End.Y + vector2.End.Y;
            Vector2 addedVector = new Vector2(new PointF(0, 0), new PointF(addedVectorX, addedVectorY));
            return addedVector;
            //0780
        }

        public static double CrossProduct(Vector2 v, Vector2 w)
        {
            return v.End.X * w.End.Y - v.End.Y * w.End.X;
        }

        public static Vector2 Normalize(Vector2 vectorToNormalize)
        {
            double magnitude = vectorToNormalize.Magnitude;
            float x = 0;
            float y = 0;
            if (magnitude != 0)
            {
                x = (float)(vectorToNormalize.End.X / magnitude);
                y = (float)(vectorToNormalize.End.Y / magnitude);
            }
            return new Vector2(new PointF(0, 0), new PointF(x, y));
        }

        public static double AngleBetween(Vector2 vector1, Vector2 vector2)
        {

            // Calculate the dot product of the two vectors
            double dotProduct = vector1.End.X * vector2.End.X + vector1.End.Y * vector2.End.Y;

            // Calculate the cosine of the angle between the vectors using the dot product and magnitudes
            double cosineTheta = dotProduct / (vector1.Magnitude * vector2.Magnitude);

            // Calculate the angle in radians using the inverse cosine function
            double angleRadians = Math.Acos(cosineTheta);

            return angleRadians;
        }

    }
    public class Missile
    {
        public PointF Position { get; set; }
        public PointF Target { get; set; }
        public Vector2 Velocity1 { get; set; }
        public double Drag { get; }
        public float MaxSpeed { get; private set; }
        public float BaseMaxSpeed { get; private set; }
        public float Randomness { get; set; }
        public float MaxRotateAngle { get; set; }

        public Vector2 changeVector = new Vector2(new PointF(0, 0), new PointF(0, 0));

        public Vector2 desiredVelocity = new Vector2(new PointF(0, 0), new PointF(0, 0));

        public Vector2 change = new Vector2(new PointF(0, 0), new PointF(0, 0));

        public PointF newPosition = new PointF(0, 0);

        public Stopwatch MissileTime = new Stopwatch();

        Random random = new Random();
        public Missile(PointF _Position, PointF _Target, Vector2 _Velocity1, double _Drag, float _MaxSpeed, float _Randomness, float _MaxRotateAngle)
        {
            Position = _Position;
            Target = _Target;
            Velocity1 = _Velocity1;
            Drag = _Drag;
            MaxSpeed = _MaxSpeed;
            Randomness = _Randomness;
            MaxRotateAngle = _MaxRotateAngle;
            BaseMaxSpeed = MaxSpeed;
            //interesting mechanic, fool around with later
            //MissileTime.Start();
        }

        public static Missile UpdateMissilePosition(Missile currentMissile)
        {

            Vector2 distancePT = new Vector2(currentMissile.Position, currentMissile.Target);
            Vector2 dessirecVelQ = new Vector2(currentMissile.Position, LineScaler(currentMissile.Position, currentMissile.Target, currentMissile.MaxSpeed));
            currentMissile.desiredVelocity.End = dessirecVelQ.End;
            //interesting mechanic, fool around with later
            //currentMissile.desiredVelocity = new Vector2(currentMissile.Position, LineScaler(currentMissile.Position, currentMissile.Target, currentMissile.MissileTime.ElapsedMilliseconds / 1000));
            currentMissile.change = new Vector2(currentMissile.Velocity1.End, dessirecVelQ.End);
            currentMissile.changeVector.End = new PointF((float)(currentMissile.change.End.X * currentMissile.Drag), (float)(currentMissile.change.End.Y * currentMissile.Drag));
            float angleBetween = (float)Vector2.AngleBetween(currentMissile.desiredVelocity, currentMissile.changeVector);
            double crossProduct = Vector2.CrossProduct(currentMissile.desiredVelocity, currentMissile.changeVector);

            if (angleBetween <= currentMissile.MaxRotateAngle)
            {
                currentMissile.MaxSpeed = currentMissile.BaseMaxSpeed;
                if (angleBetween == 0 && crossProduct == 0)
                {
                    //the random thing
                }
                if (crossProduct > 0)
                {
                    currentMissile.changeVector.End = RotatePoint(currentMissile.changeVector.End, new PointF(0, 0), -(currentMissile.MaxRotateAngle - angleBetween));
                }
                else if (crossProduct < 0)
                {
                    currentMissile.changeVector.End = RotatePoint(currentMissile.changeVector.End, new PointF(0, 0), currentMissile.MaxRotateAngle - angleBetween);
                }
            }
            else
            {
                currentMissile.MaxSpeed += 1f;
            }

            Vector2 newVel = Vector2.VectorAddition(currentMissile.Velocity1, currentMissile.changeVector);

            currentMissile.newPosition = new PointF(currentMissile.Position.X + (float)newVel.End.X, currentMissile.Position.Y + (float)newVel.End.Y);
            currentMissile.Velocity1 = newVel;
            currentMissile.Position = currentMissile.newPosition;

            return currentMissile;
        }

        public static Missile RandomizeMissilePosition(Missile currentMissile)
        {
            int posOrNeg = currentMissile.random.Next(-1, 2);
            PointF perpPoint = new Point(0, 0);
            if (posOrNeg > 0)
            {
                perpPoint = LineScaler(new PointF(0, 0), new PointF(posOrNeg * -currentMissile.Velocity1.End.Y, posOrNeg * currentMissile.Velocity1.End.X), (float)currentMissile.random.NextDouble() * currentMissile.Randomness);
            }
            else if (posOrNeg < 0)
            {
                perpPoint = LineScaler(new PointF(0, 0), new PointF(posOrNeg * -currentMissile.Velocity1.End.Y, posOrNeg * currentMissile.Velocity1.End.X), (float)currentMissile.random.NextDouble() * currentMissile.Randomness);
            }

            Vector2 perpendicularVector = new Vector2(new PointF(0, 0), perpPoint);
            currentMissile.Velocity1 = Vector2.VectorAddition(perpendicularVector, currentMissile.Velocity1);
            return currentMissile;
        }

        private static PointF RotatePoint(PointF point, PointF pivot, double radians)
        {
            var cosTheta = Math.Cos(radians);
            var sinTheta = Math.Sin(radians);

            var x = (cosTheta * (point.X - pivot.X) - sinTheta * (point.Y - pivot.Y) + pivot.X);
            var y = (sinTheta * (point.X - pivot.X) + cosTheta * (point.Y - pivot.Y) + pivot.Y);

            return new PointF((float)x, (float)y);
        }
        private static PointF LineScaler(PointF startPoint, PointF endPoint, float fixedLineLength)
        {
            float dx = endPoint.X - startPoint.X;
            float dy = endPoint.Y - startPoint.Y;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);

            float normalizedDx = dx / length;
            float normalizedDy = dy / length;

            // Calculate the scaled end point to achieve the fixed line length
            float scaledEndX = startPoint.X + normalizedDx * fixedLineLength;
            float scaledEndY = startPoint.Y + normalizedDy * fixedLineLength;
            return new PointF(scaledEndX, scaledEndY);
        }
    }
    public class Camera
    {
        public PointF Position { get; set; }
        public SizeF ViewportSize { get; set; }

        public Bitmap ViewpostBitmap { get; private set; }
        public RectangleF Viewport => new RectangleF(Position, ViewportSize);

        public Camera(PointF position, SizeF viewportSize)
        {
            Position = position;
            ViewportSize = viewportSize;
            ViewpostBitmap = new Bitmap((int)viewportSize.Width, (int)viewportSize.Height);
        }

        public void Move(float dx, float dy)
        {
            Position = new PointF(Position.X + dx, Position.Y + dy);
        }

        public void SetPosition(float x, float y)
        {
            Position = new PointF(x, y);
        }

        public void UpdateCamera(Bitmap formBitmap)
        {
            // Dispose of the previous bitmap if it exists
            if (ViewpostBitmap != null)
            {
                ViewpostBitmap.Dispose();
            }

            // Create a new bitmap for the viewport
            ViewpostBitmap = new Bitmap((int)ViewportSize.Width, (int)ViewportSize.Height);

            // Capture the contents of the form's bitmap within the viewport
            using (Graphics g = Graphics.FromImage(ViewpostBitmap))
            {
                // Calculate the source rectangle within the form's bitmap
                Rectangle sourceRect = new Rectangle((int)Position.X, (int)Position.Y, (int)ViewportSize.Width, (int)ViewportSize.Height);

                // Draw the section of the form's bitmap onto the viewport bitmap
                g.DrawImage(formBitmap, 0, 0, sourceRect, GraphicsUnit.Pixel);
            }
        }

        // Other methods needed, such as zooming, resetting position, etc.
    }

    public class Player
    {
        public PointF Position { get; private set; }

        public Vector2 Velocity1 { get; }

        public double Drag { get; }

        public float MaxSpeed { get; private set; }

        public float MaxRotateAngle { get; set; }

        public Stopwatch PlayerTime = new Stopwatch();

        public Vector2 CurrentVelocity { get; private set; }

        public Vector2 CurrentDircetion { get; set; }

        public float Acceleration { get; private set; }

        public Player(PointF _Position, Vector2 _Velocity1, double _Drag, float _MaxSpeed, float _MaxRotateAngle, float _Acceleration)
        {
            CurrentDircetion = _Velocity1;
            Position = _Position;
            Velocity1 = _Velocity1;
            Drag = _Drag;
            MaxSpeed = _MaxSpeed;
            MaxRotateAngle = _MaxRotateAngle;
            Acceleration = _Acceleration;
            CurrentVelocity = _Velocity1;
            PlayerTime.Start();
        }
        
        public void MovePlayer ( bool wDown, bool aDown, bool sDown, bool dDown)
        {
            if (CurrentVelocity.Magnitude > 0)
            {
                CurrentVelocity = Vector2.VectorAddition(CurrentVelocity, new Vector2(new PointF(0, 0), LineScaler(new PointF(0, 0), CurrentVelocity.End, (float)-Drag)));
            }

            float rotateAngle = MaxRotateAngle;
            if (wDown == true)
            {
                rotateAngle /= 4;
                if (aDown == true ^ dDown == true)
                {
                    if (aDown == true)
                    {
                        CurrentDircetion = new Vector2(new PointF(0, 0), RotatePoint(CurrentDircetion.End, new PointF(0, 0), -rotateAngle));
                    }
                    if (dDown == true)
                    {
                        CurrentDircetion = new Vector2(new PointF(0, 0), RotatePoint(CurrentDircetion.End, new PointF(0, 0), rotateAngle));
                    }
                }

                if (CurrentVelocity.Magnitude > 7)
                {
                    CurrentVelocity = Vector2.VectorAddition(CurrentVelocity, new Vector2(new PointF(0, 0), LineScaler(new PointF(0, 0), CurrentDircetion.End, Acceleration)));
                }
                else
                {
                    CurrentVelocity = Vector2.VectorAddition(CurrentVelocity, new Vector2(new PointF(0, 0), LineScaler(new PointF(0, 0), CurrentDircetion.End, 7)));
                }
                
            }
            else
            {
                if (aDown == true ^ dDown == true)
                {
                    if (aDown == true)
                    {
                        CurrentDircetion = new Vector2(new PointF(0, 0), RotatePoint(CurrentDircetion.End, new PointF(0, 0), -rotateAngle * 1.5));
                    }
                    if (dDown == true)
                    {
                        CurrentDircetion = new Vector2(new PointF(0, 0), RotatePoint(CurrentDircetion.End, new PointF(0, 0), rotateAngle * 1.5));
                    }
                }
            }
            CurrentDircetion = new Vector2(new PointF(0, 0), LineScaler(new PointF(0, 0), CurrentDircetion.End, 10));
            if (CurrentVelocity.Magnitude > MaxSpeed)
            {
                CurrentVelocity = new Vector2(new PointF(0, 0), LineScaler(new PointF(0, 0), CurrentVelocity.End, MaxSpeed));
            }
            Position = new PointF(Position.X + CurrentVelocity.End.X, Position.Y + CurrentVelocity.End.Y);
        }

        private static PointF RotatePoint(PointF point, PointF pivot, double radians)
        {
            var cosTheta = Math.Cos(radians);
            var sinTheta = Math.Sin(radians);

            var x = (cosTheta * (point.X - pivot.X) - sinTheta * (point.Y - pivot.Y) + pivot.X);
            var y = (sinTheta * (point.X - pivot.X) + cosTheta * (point.Y - pivot.Y) + pivot.Y);

            return new PointF((float)x, (float)y);
        }
        private static PointF LineScaler(PointF startPoint, PointF endPoint, float fixedLineLength)
        {
            float dx = endPoint.X - startPoint.X;
            float dy = endPoint.Y - startPoint.Y;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);

            float normalizedDx = dx / length;
            float normalizedDy = dy / length;

            // Calculate the scaled end point to achieve the fixed line length
            float scaledEndX = startPoint.X + normalizedDx * fixedLineLength;
            float scaledEndY = startPoint.Y + normalizedDy * fixedLineLength;
            return new PointF(scaledEndX, scaledEndY);
        }
    }
        class EasyDebugger
        {
            private static int NumberOfOccurances = 0;
            public static void AfterOccuranceAmount(int triggerAmount, string iDCode)
            {
                NumberOfOccurances += 1;
                if (triggerAmount / NumberOfOccurances == 1)
                {
                    //Place your break point here
                    NumberOfOccurances = 0;
                }
            }
        }  
}
