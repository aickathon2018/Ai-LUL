using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace AI_LUL
{
    class trackface
    {
        public static void measureface(Rectangle face, Image<Bgr, Byte> currentFrame, string humanheight)
        {
            PointF[] pts = new PointF[]
                {
                  new PointF(face.Left, face.Bottom),
                  new PointF(face.Right, face.Bottom),
                  new PointF(face.Right, face.Top),
                  new PointF(face.Left, face.Top)
                };

            Point[] vertpoints = Array.ConvertAll(pts, Point.Round);

            Point tlbl = Midpoint(vertpoints[0], vertpoints[3]);       //finds the midpoint between top left and bottom left  
            Point trbr = Midpoint(vertpoints[1], vertpoints[2]);       //finds the midpoint top right and bottom right
            //CvInvoke.Line(currentFrame, tlbl, trbr, new MCvScalar(0, 255, 0), 2); //draws out the width

            Point tltr = Midpoint(vertpoints[2], vertpoints[3]);       //finds the midpoint between top left and top right 
            Point blbr = Midpoint(vertpoints[0], vertpoints[1]);       //finds the midpoint between bottom left and bottom right
            //CvInvoke.Line(currentFrame, tltr, blbr, new MCvScalar(0, 255, 0), 2); //draws out the Height

            double widthdistance = Math.Sqrt(Math.Pow(tlbl.X - trbr.X, 2) + Math.Pow(tlbl.Y - trbr.Y, 2));
            double heightdistance = Math.Sqrt(Math.Pow(tltr.X - blbr.X, 2) + Math.Pow(tltr.Y - blbr.Y, 2));

            double cmperpixels = (Convert.ToDouble(humanheight) / 12) / widthdistance;   //a human head is 7.5 times smaller than its body height

            widthdistance = cmperpixels * widthdistance;
            heightdistance = cmperpixels * heightdistance;

            tltr.X = tltr.X - 30;    //these are for adjusting the text positions, nothign really crazy
            tltr.Y = tltr.Y - 10;
            tlbl.X = tlbl.X - 65;

            //CvInvoke.PutText(currentFrame, widthdistance.ToString("F"), trbr, Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.65, new MCvScalar(153, 50, 204), 2);
            //CvInvoke.PutText(currentFrame, heightdistance.ToString("F"), tltr, Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.65, new MCvScalar(153, 50, 204), 2);

            currentFrame.Draw(face, new Bgr(255, 255, 153), 3);
        }

        public static Point Midpoint(Point ptA, Point ptB)
        {
            Point Midpoint = new Point((ptA.X + ptB.X) / 2, (ptA.Y + ptB.Y) / 2);
            return Midpoint;
        }
    }
}
