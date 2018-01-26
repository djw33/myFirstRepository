using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FernNamespace
{
    /*
     * this class draws a fractal fern when the constructor is called.
     */   
    class Fern
    {
        private static int TENDRILS = 1;
        private static double spacing = 3;
        private static double DELTATHETA = 0.1;
        private static double SEGLENGTH = 3.0;

        /* 
         * Fern constructor begins drawing fern
         * 
         * Size: number of 3-pixel segments of tendrils
         * spores: how many spores will the fern have (0= none, 0.2 = some)
         * Turnbias: how likely to turn right vs. left (0=always left, 0.5 = 50/50, 1.0 = always right)
         * branch: number of branches the fern will have( between 1 and 4, default set to 3)
         * canvas: the canvas that the fern will be drawn on
         */
        public Fern(double size, double spores, double turnbias, double branch, Canvas canvas)
        {
            TENDRILS = (int)branch;//number of branches or tendrils
            // draw a new fern at the center of the canvas with given parameters
            cluster((int) (canvas.Width / 2), (int)(canvas.Height / 2), size, spores, turnbias, canvas);       
        }

        /*
         * cluster draws a cluster at the given location and then draws a bunch of tendrils out in 
         * regularly-spaced directions out of the cluster.
         */
        private void cluster(int x, int y, double size, double spores, double turnbias, Canvas canvas)
        {
            for (int i = 0; i < TENDRILS; i++)
            {
                // compute the angle of the outgoing tendril
                double theta = i * 2 * Math.PI / TENDRILS;
                tendril(x, y, size, spores, turnbias, theta, canvas);
                spore(x, y, 5, canvas);
            }
        }

        /*
         * tendril draws a tendril (a randomly-wavy line) in the given direction, for the given length, 
         * and draws a cluster at the other end if the line is big enough.
         */
        private void tendril(int x1, int y1, double size, double spores, double turnbias, double direction, Canvas canvas)
        {
            int x2=x1, y2=y1;
            int lastrighttendril = 0; //keep track of the last time the tendril branched right
            int lastlefttendril = 0; //keep track of the last time the tendril branched left
            Random random = new Random();
            if (size > 1)
            {
                for (int i = 0; i < size; i++)
                {
                    direction += (random.NextDouble() > turnbias) ? -1 * DELTATHETA : DELTATHETA;
                    x1 = x2; y1 = y2;
                    x2 = x1 + (int)(SEGLENGTH * Math.Sin(direction));
                    y2 = y1 + (int)(SEGLENGTH * Math.Cos(direction));
                    byte red = (byte)(100 + size / 2);
                    byte green = (byte)(220 - size / 3);
                    //if (size>120) red = 138; green = 108;
                    line(x1, y1, x2, y2, red, green, 0, 1 + size / 80, canvas);
                    if(random.NextDouble() < spores)  // distributes the spores on the fern randomly
                        spore(x1, y1, 2, canvas);
                    if (i != 0) //if i is not 0 then tendril is large enough to be "eligable for a branch
                    {
                        /*this following if statement makes sure that the spacing between branched tendrils isnt to crammed(especially at the base)
                         * or to spread out (especially at the end) This spacing also uses a bit of a random variance to help make it looke more natural
                         * the spacing looks at when the last tendril branched, and what the size of that branch was to make an appropiate spacing 
                        */
                        if ((i - lastlefttendril) > (random.NextDouble()+1)*(size - i)/ (spacing*spacing))
                        {
                            tendril(x2, y2, (size - i) / spacing, spores, turnbias, direction + (Math.PI / 2), canvas);
                            lastlefttendril = i;
                        }
                        if ((i - lastrighttendril) > (random.NextDouble()+ 1) * (size - i)/ (spacing * spacing))
                        {
                            tendril(x2, y2, (size - i) / spacing, spores, turnbias, direction - (Math.PI / 2), canvas);
                            lastrighttendril = i;
                        }
                        
                    }
                }
            }
        }

        /*
         * draw a gray circle centered at (x,y), radius radius, with a black edge, onto canvas to represent a spore
         */
        private void spore(int x, int y, double radius, Canvas canvas)
        {
            Ellipse myEllipse = new Ellipse();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 149, 152, 161); //gray circles for the spores
            myEllipse.Fill = mySolidColorBrush;
            myEllipse.StrokeThickness = 1;
            myEllipse.Stroke = Brushes.Black;
            myEllipse.HorizontalAlignment = HorizontalAlignment.Center;
            myEllipse.VerticalAlignment = VerticalAlignment.Center;
            myEllipse.Width = 2 * radius;
            myEllipse.Height = 2 * radius;
            myEllipse.SetCenter(x, y);
            canvas.Children.Add(myEllipse);
        }

        /*
         * draw a line segment (x1,y1) to (x2,y2) with given color, thickness on canvas
         */
        private void line(int x1, int y1, int x2, int y2, byte r, byte g, byte b, double thickness, Canvas canvas)
        {
            Line myLine = new Line();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, r, g, b);
            myLine.X1 = x1;
            myLine.Y1 = y1;
            myLine.X2 = x2;
            myLine.Y2 = y2;
            myLine.Stroke = mySolidColorBrush;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.StrokeThickness = thickness;
            canvas.Children.Add(myLine);
        }
    }
}

/*
 * this class is needed to enable us to set the center for an ellipse (not built in?!)
 */
public static class EllipseX
{
    public static void SetCenter(this Ellipse ellipse, double X, double Y)
    {
        Canvas.SetTop(ellipse, Y - ellipse.Height / 2);
        Canvas.SetLeft(ellipse, X - ellipse.Width / 2);
    }
}

