using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Emgu.CV;
using Emgu.CV.Structure;
/*
 * How can I optimize this?
 * 
 */
namespace FindChessBoardCornersConsoleApplication
{
    class Program
    {

        static int CalibrateCamera(int width, int height)
        {
            Capture capture = new Capture(); //create a camera captue
            

   
            
            //numbers of samples
            int samples = 10;

            Size size = new Size(width, height);
            Size ImageSize = new Size();
    
            int board_n = size.Width * size.Height;

            PointF[] corners = new PointF[] { };
            

            // define the chess board size
            Size patternSize = new Size(width, height);
            
            //double = CV32FC1
            //Matrix<double> object_points = new Matrix<double>(board_n , samples, 3);

            IntPtr image_points = CvInvoke.cvCreateMat( board_n * samples , 2, Emgu.CV.CvEnum.MAT_DEPTH.CV_32F);         
            IntPtr object_points = CvInvoke.cvCreateMat( board_n * samples , 3, Emgu.CV.CvEnum.MAT_DEPTH.CV_32F);

            IntPtr point_counts = CvInvoke.cvCreateMat(samples, 1, Emgu.CV.CvEnum.MAT_DEPTH.CV_32S);

            //allocate matrix
            IntPtr intrinsic_matrix = CvInvoke.cvCreateMat(3, 3, Emgu.CV.CvEnum.MAT_DEPTH.CV_32F);
            IntPtr distortion_coeffs = CvInvoke.cvCreateMat(5, 1, Emgu.CV.CvEnum.MAT_DEPTH.CV_32F);
    
            /*
            Mat src(1, 2, CV_32FC2);

           // isn't it nicer than CV_UGLY_AND_SCARY_MACRO()?    
           src.at<Vec2f>(0,1)[1] = 123; // (0,1) means row 0, col 1. [1] means channel 1. 
           EDIT

           From OpenCV:

           For single-channel matrices there is a macro CV_MAT_ELEM( matrix, elemtype, row, col ), i.e. for 32-bit floating point real matrix
             
             * * */
            int success = 0;

            while (success < samples)
            {

                // get the input gray scale image

                Image<Gray, Byte>  InputImage = capture.QueryFrame().Convert<Gray, Byte>();
                // new Image<Gray, Byte>("calib-checkerboard-300x211.png");


                // create a buffer to store the chess board corner locations

                // find the chess board corners
                corners = CameraCalibration.FindChessboardCorners(InputImage, patternSize, Emgu.CV.CvEnum.CALIB_CB_TYPE.ADAPTIVE_THRESH | Emgu.CV.CvEnum.CALIB_CB_TYPE.FILTER_QUADS);
                if (corners != null)
                {
                    CvInvoke.cvFindCornerSubPix(InputImage, corners, corners.Length, new Size(11, 11), new Size(-1, -1), new MCvTermCriteria(30, 0.1));

                    /*  How can I choose valid corners??
                     * 
                     * Drop frame if is like as a frame in storege
                     * 
                     * http://opencv-users.1802565.n2.nabble.com/How-to-improve-the-accuracy-when-calibrating-stereo-setup-td5928788.html
                     * 
                     */
                    if (corners.Length == board_n)
                    {
                        //improve
                        CameraCalibration.DrawChessboardCorners(InputImage, patternSize, corners);

                        /*
                         * Store corners
                         * */
                        int step = success * board_n;
                        for (int i = step, j = 0; i < corners.Length; i++, j++)
                        {
                            //Write in a IntPtr with cvSet.. function
                            CvInvoke.cvSet2D(image_points, step, 0, new MCvScalar(corners[j].X));
                            CvInvoke.cvSet2D(image_points, step, 1, new MCvScalar(corners[j].Y));

                            //image_points [success, i] = corners[i];
                            /* What are those?*/
                            CvInvoke.cvSet2D(object_points, step, 0, new MCvScalar(j / width));
                            CvInvoke.cvSet2D(object_points, step, 1, new MCvScalar(j % width));
                            CvInvoke.cvSet2D(object_points, step, 2, new MCvScalar(0));
                            //point_counts[success] = board_n;
                            CvInvoke.cvSet2D(point_counts, success, 0, new MCvScalar(board_n));
                        }
                        success++;
                        Console.WriteLine("Sampled {0} ", success);
                    }
                }

                // show the result

                CvInvoke.cvShowImage("Result", InputImage.Ptr);
                ImageSize = InputImage.Size;

                // Handle pause/unpause and ESC
                int c = CvInvoke.cvWaitKey(15);
                if (c == 'p')
                {
                    c = 0;
                    while (c != 'p' && c != 27)
                    {
                        c = CvInvoke.cvWaitKey(250);
                    }
                }
                
            }
            // At this point we have all the chessboard corners we need
            // Initiliazie the intrinsic matrix such that the two focal lengths
            // have a ratio of 1.0

            CvInvoke.cvSet2D(intrinsic_matrix, 0, 0, new MCvScalar(1));
            CvInvoke.cvSet2D(intrinsic_matrix, 1, 1, new MCvScalar(1));

            
            //calibration
            CvInvoke.cvCalibrateCamera2(object_points, image_points, point_counts, ImageSize, intrinsic_matrix, distortion_coeffs,
                IntPtr.Zero, IntPtr.Zero, Emgu.CV.CvEnum.CALIB_TYPE.CV_CALIB_FIX_ASPECT_RATIO);
            //Getted instrinsix Matrix and distotion matrix


            //Print Matrices
/*
	// Build the undistort map that we will use for all subsequent frames
	IplImage* mapx = cvCreateImage( cvGetSize( image ), IPL_DEPTH_32F, 1 );
	IplImage* mapy = cvCreateImage( cvGetSize( image ), IPL_DEPTH_32F, 1 );
	cvInitUndistortMap( intrinsic, distortion, mapx, mapy );

	// Run the camera to the screen, now showing the raw and undistorted image
	cvNamedWindow( "Undistort" );

	while( image ){
		IplImage *t = cvCloneImage( image );
		cvShowImage( "Calibration", image ); // Show raw image
		cvRemap( t, image, mapx, mapy ); // undistort image
		cvReleaseImage( &t );
		cvShowImage( "Undistort", image ); // Show corrected image

*/
            return 0;
        }


        static void Main(string[] args)
        {
            // get the chess board width
            Console.WriteLine("Chess board width");
            Int32 width = 9;

            // get the chess board height
            Console.WriteLine("Chess board height");
            Int32 height = 6;
            
            CalibrateCamera(width,height);
        }
    }
}