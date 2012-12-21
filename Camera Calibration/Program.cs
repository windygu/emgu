using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

namespace Camera_Calibration
{
    class Program
    {

        static void Main(string[] args)
        {
            /* 
            Goal
            The sample application will:

            Determinate the distortion matrix
            Determinate the camera matrix
            Input from Camera, Video and Image file list
            Configuration from XML/YAML file
            Save the results into XML/YAML file
            Calculate re-projection error
             */
            Capture capture;
            int board_w = 9;
            int board_h = 6;
            int n_boards = 8; //numbers of boards
            int board_n = board_h * board_w;
            
            //matrix

            MCvMat image_points = new MCvMat();

            try
            {
                capture = new Capture();

                while (true)
                {
                    Image<Bgr, Byte> ImageFrame = capture.QueryFrame();
                    CvInvoke.cvShowImage("gray scale input image", ImageFrame.Ptr);
                    CvInvoke.cvWaitKey(1);

                }
                //CvInvoke.cvWaitKey(0);
            }
            catch (NullReferenceException excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }           
    }
}
