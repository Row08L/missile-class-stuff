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
        PointF mousePosition = new PointF(0,200);
        List<Missile> missileList = new List<Missile>();
        
        public Form1()
        {
            InitializeComponent();
            this.MouseMove += CursorPositionForm_MouseMove;
            this.MouseClick += Button1_MouseClick;
            PointF spawn = new PointF(20, 20);
            Vector2 velocity = new Vector2(new PointF(0, 0), new PointF(1, 1));
            ticks.Interval = 50/6; // Adjust interval as needed
            ticks.Tick += Timer_Tick;
            ticks.Start();
            missileTicks.Interval = 10; // Adjust interval as needed
            missileTicks.Tick += MissileTimer_Tick;
            missileTicks.Start();
        }
        private void Button1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PointF spawn = new PointF(200, 200);
                Vector2 velocity = new Vector2(new PointF(0, 0), new PointF(20, 20));
                missileList.Add(new Missile(spawn, mousePosition, velocity, 0.06, 10, 3f, 1f));
            }
            if (e.Button == MouseButtons.Right)
            {
                missileTicks.Stop();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            for(int missle1 = 0; missle1 < missileList.Count(); missle1++)
            {
                missileList[missle1].Target = mousePosition;
                missileList[missle1] = Missile.UpdateMissilePosition(missileList[missle1]);
                missileList[missle1] = Missile.RandomizeMissilePosition(missileList[missle1]);
            }
            Refresh();
        }
        private void MissileTimer_Tick(object sender, EventArgs e)
        {
            PointF spawn = new PointF(20, 20);
            Vector2 velocity = new Vector2(new PointF(0, 0), new PointF(0, 100));
            velocity.End = new PointF(10, 10);
            missileList.Add(new Missile(spawn, mousePosition, velocity, 0.06, 20, 3f, (float)(Math.PI * 2) / 10f));
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
                e.Graphics.FillEllipse(Brushes.Black, new RectangleF(missileList[missle1].Position.X, missileList[missle1].Position.Y, 10, 10));
                e.Graphics.FillEllipse(Brushes.Blue, new RectangleF(missileList[missle1].Target.X, missileList[missle1].Target.Y, 10, 10));
                
            }
        }
        private void CursorPositionForm_MouseMove(object sender, MouseEventArgs e)
        {
            mousePosition = this.PointToClient(Cursor.Position); ;
        }
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
            Vector2 addedVector = new Vector2(new PointF(0,0), new PointF(addedVectorX, addedVectorY));
            return addedVector;
            //0780
        }

        public static double CrossProduct(Vector2 v, Vector2 w)
        {
            return v.End.X * w.End.Y - v.End.Y* w.End.X;
        }

        public static Vector2 Normalize(Vector2 vectorToNormalize)
        {
            double magnitude = vectorToNormalize.Magnitude;
            float x = 0;
            float y = 0;
            if (magnitude != 0)
            {
                x = (float)(vectorToNormalize.End.X / magnitude);
                y =  (float)(vectorToNormalize.End.Y / magnitude);
            }
            return new Vector2(new PointF(0,0), new PointF(x, y));
        }

        public static double AngleBetween(Vector2 vector1, Vector2 vector2)
        {
            if (vector1.Magnitude == 0 || vector2.Magnitude == 0)
            {
                EasyDebugger.AfterOccuranceAmount(500, "shit");
            }

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
            currentMissile.changeVector.End = new PointF((float)(currentMissile.change.End.X* currentMissile.Drag), (float)(currentMissile.change.End.Y * currentMissile.Drag));
            float angleBetween = (float)Vector2.AngleBetween(currentMissile.desiredVelocity, currentMissile.changeVector);
            double crossProduct = Vector2.CrossProduct(currentMissile.desiredVelocity, currentMissile.changeVector);
            if (Double.IsNaN(angleBetween) == false) 
            {
                EasyDebugger.AfterOccuranceAmount(1, "");
            }
            if (angleBetween <= currentMissile.MaxRotateAngle)
            {
                if (angleBetween == 0 && crossProduct == 0)
                {
                    //the random thing
                }
                if(crossProduct > 0)
                {
                    currentMissile.changeVector.End = RotatePoint(currentMissile.changeVector.End, new PointF(0, 0), -(currentMissile.MaxRotateAngle - angleBetween));
                }
                else if(crossProduct < 0)
                {
                    currentMissile.changeVector.End = RotatePoint(currentMissile.changeVector.End, new PointF(0,0), currentMissile.MaxRotateAngle - angleBetween);
                }
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
            PointF perpPoint = new Point(0,0);
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
