using UnityEngine;
using System.Collections.Generic;
using OpenCVForUnity;

public class PatternDetector
{
    MatOfKeyPoint m_queryKeypoints = new MatOfKeyPoint();
    Mat m_queryDescriptors = new Mat();

    MatOfDMatch m_matches = new MatOfDMatch();
    List<MatOfDMatch> m_knnMatches = new List<MatOfDMatch>();

    Mat m_grayImg;// = new Mat();
    Mat m_warpedImg = new Mat();
    Mat m_roughHomography = new Mat();
    Mat m_refineHomography = new Mat();

    Pattern m_pattern;
    FeatureDetector m_detector;
    DescriptorMatcher m_matcher;
    DescriptorExtractor m_extractor;

    public bool enableRatioTest;
    public bool enableHomographyRefinement;
    public float homographyReprojectionThreshold;

    public PatternDetector() : this(
        FeatureDetector.create(FeatureDetector.ORB),
        DescriptorExtractor.create(DescriptorExtractor.ORB),
        DescriptorMatcher.create(DescriptorMatcher.BRUTEFORCE_HAMMINGLUT)//,//.BRUTEFORCE_HAMMINGLUT),//.FLANNBASED),//
        //true
        )
    {
    }

    public PatternDetector(
        FeatureDetector detector,
        DescriptorExtractor extractor,
        DescriptorMatcher matcher,
        bool ratioTest = false)
    {
        m_detector = detector;
        m_extractor = extractor;
        m_matcher = matcher;

        enableRatioTest = ratioTest;

        enableHomographyRefinement = false;

        homographyReprojectionThreshold = 3;
    }

    public void Train(Pattern pattern)
    {
        m_pattern = pattern;

        m_matcher.clear();

        List<Mat> descriptors = new List<Mat>() { pattern.descriptors.clone() };
        //descriptors.Add(pattern.descriptors.clone());

        m_matcher.add(descriptors);

        m_matcher.train();

    }

    public void BuildPatternFromImage(Mat image, Pattern pattern)
    {

        //int numImages = 4;
        //float step = Mathf.Sqrt(2.0f);

        // Image dimensions
        float w = image.cols();
        float h = image.rows();

        // Store original image in pattern structure
        pattern.size = new Size(w, h);
        pattern.frame = image.clone();
        pattern.grayImg = GetGray(image);//, out pattern.grayImg);

        // Build 2d and 3d contours (3d contour lie in XY plane since it's planar)
        pattern.points2d = new MatOfPoint2f();
        pattern.points3d = new MatOfPoint3f();
        
        // Normalized dimensions:
        float maxSize = Mathf.Max(w, h);
        float unitW = w / maxSize;
        float unitH = h / maxSize;

        List<Point3> pts3d = new List<Point3>();
        pts3d.Add(new Point3(-unitW, -unitH, 0));
        pts3d.Add(new Point3(unitW, -unitH, 0));
        pts3d.Add(new Point3(unitW, unitH, 0));
        pts3d.Add(new Point3(-unitW, unitH, 0));
        pattern.points3d.fromList(pts3d);

        List<Point> pts2d = new List<Point>();
        pts2d.Add(new Point(0, 0));
        pts2d.Add(new Point(w, 0));
        pts2d.Add(new Point(w, h));
        pts2d.Add(new Point(0, h));
        pattern.points2d.fromList(pts2d);
        
        pattern.keypoints = new MatOfKeyPoint();
        pattern.descriptors = new Mat();

        if (ExtractFeatures(pattern.grayImg, pattern.keypoints, pattern.descriptors))
        {
            Debug.Log(pattern.ToString());
            
        }



    }

    public bool FindPattern(Mat image, out PatternTrackingInfo info)
    {

        info = new PatternTrackingInfo();
                
        m_grayImg = GetGray(image);//, out m_grayImg);

        ExtractFeatures(m_grayImg, m_queryKeypoints, m_queryDescriptors);
        Debug.Log("m_queryKeypoints : " + m_queryKeypoints.ToString());
        Debug.Log("m_queryDescriptors : " + m_queryDescriptors.ToString());

        m_matches = GetMatches(m_queryDescriptors);

        Debug.Log("Matches size : " + m_matches.toArray().Length);

        //if (m_roughHomography == null)
        m_roughHomography = new Mat();

        bool homographyFound = RefineMatchesWithHomography(
            m_queryKeypoints,
            m_pattern.keypoints,
            homographyReprojectionThreshold,
            m_matches,
            ref m_roughHomography);

        //Debug.Log("1 homographyFound is " + homographyFound);

        if (homographyFound)
        {
            if (enableHomographyRefinement)
            {
                m_warpedImg = new Mat();
                
                Imgproc.warpAffine(m_grayImg, m_warpedImg, m_roughHomography, m_pattern.size, Imgproc.WARP_INVERSE_MAP | Imgproc.INTER_CUBIC);

                MatOfKeyPoint warpedKeypoints = new MatOfKeyPoint();
                MatOfDMatch refineMatches = new MatOfDMatch();

                ExtractFeatures(m_warpedImg, warpedKeypoints, m_queryDescriptors);

                refineMatches = GetMatches(m_queryDescriptors);

                homographyFound = RefineMatchesWithHomography(
                    warpedKeypoints,
                    m_pattern.keypoints,
                    homographyReprojectionThreshold,
                    refineMatches,
                    ref m_refineHomography
                    );

                //Debug.Log("refineMatches : " + refineMatches.ToString());

                //Debug.Log("m_refineHomography : " + m_refineHomography.ToString());
                
                info.homography = m_roughHomography.cross(m_refineHomography.clone());

                //Debug.Log("info.homography : " + info.homography.ToString());

                //info.homography = Imgproc.getPerspectiveTransform(m_pattern.points2d, info.points2d);

                Core.perspectiveTransform(m_pattern.points2d, info.points2d, info.homography);// m_roughHomography);
            }
            else
            {

                Debug.Log("m_roughHomography : " + m_roughHomography.ToString());

                info.homography = m_roughHomography;
                Core.perspectiveTransform(m_pattern.points2d, info.points2d, info.homography);

            }
        }
        
        return homographyFound;
    }



    protected bool ExtractFeatures(Mat image, MatOfKeyPoint keypoints, Mat descriptors)
    {
        if (image.empty())
        {
            throw new System.Exception("PatternDetector.ExtractFeatures - image cannot be empty!");
        }
        
        if (image.channels() != 1)
        {
            throw new System.Exception("PatternDetector.ExtractFeatures - image.channel must be one!");
        }
        
        m_detector.detect(image, keypoints);
        
        if (keypoints.empty())
            return false;
        
        m_extractor.compute(image, keypoints, descriptors);
        
        if (keypoints.empty())
            return false;

        return true;
    }

    protected MatOfDMatch GetMatches(Mat queryDescriptors)//, out MatOfDMatch matches)
    {

        //matches.clear();
        MatOfDMatch matches = new MatOfDMatch();
        
        if (enableRatioTest)
        {
            // To avoid NaN's when best match has zero distance we will use inversed ratio. 
            float minRatio = 1.0f / 1.5f;

            m_knnMatches.Clear();// = new List<MatOfDMatch>();
            // KNN match will return 2 nearest matches for each query descriptor
            m_matcher.knnMatch(queryDescriptors, m_knnMatches, 2);
            
            foreach (var match in m_knnMatches)
            {
                DMatch bestMatch = match.toArray()[0];
                DMatch betterMatch = match.toArray()[1];

                Debug.Log("Best Match : " + bestMatch);

                Debug.Log("Better Match : " + betterMatch);

                
                float distanceRatio = bestMatch.distance / betterMatch.distance;

                //Debug.Log("Distance Ratio : " + distanceRatio);

                // Pass only matches where distance ratio between 
                // nearest matches is greater than 1.5 (distinct criteria)
                if (distanceRatio < minRatio)
                {

                    matches.push_back(new MatOfDMatch(bestMatch));

                    
                    //Debug.Log("Result matches: " + matches);
                }

            }

            //Debug.Log("Result matches: " + matches);
        }
        else
        {
            // Perform regular match
            //Debug.Log("queryDescriptors : " + queryDescriptors.ToString());
            m_matcher.match(queryDescriptors, matches);

            float max_dist = 0;
            float min_dist = 100;
            
            foreach (var m in matches.toList())
            {
                float dist = m.distance;
                if (dist < min_dist) min_dist = dist;
                if (dist > max_dist) max_dist = dist;
            }

            float ratio = (max_dist / min_dist);

            Debug.Log(string.Format("min distance : {0}, max distance : {1}, ratio : {2}", min_dist, max_dist, ratio));

            var good_matches = new MatOfDMatch();

            foreach(var m in matches.toList())
            {
                if (m.distance <= Mathf.Max(2.0f * min_dist, 0.02f))
                {

                    good_matches.push_back(new MatOfDMatch(m));
                }
            }
            Debug.Log("Good Match : " + good_matches);
            return good_matches;

        }

        return matches;
    }



    protected Mat GetGray(Mat image)//,out Mat gray)
    {
        //gray = new Mat();
        

        if (image.channels() == 3)
        {
            Debug.Log("channels = 3");
            Mat gray = new Mat(image.size(), Imgproc.COLOR_BGR2GRAY);
            Imgproc.cvtColor(image, gray, Imgproc.COLOR_BGR2GRAY);
            return gray;
        }

        if (image.channels() == 4)
        {
            Debug.Log("channels = 4");
            Mat gray = new Mat(image.size(), Imgproc.COLOR_BGRA2GRAY);
            Imgproc.cvtColor(image, gray, Imgproc.COLOR_BGRA2GRAY);
            return gray;

        }
        /*else if (image.channels() == 1)
        {
            Debug.Log("channels = 1");
            gray = image;
        }*/

        return image;//.clone();
    }

    protected bool RefineMatchesWithHomography(
        MatOfKeyPoint queryKeypoints,
        MatOfKeyPoint trainKeypoints,
        float reprojectionThreshold,
        MatOfDMatch matches,
        ref Mat homography
        )
    {
        //homography = new Mat();

        Debug.Log("Refine !");

        int minNumberMatcherAllowed = 8;

        DMatch[] dMatches = matches.toArray();
        int size = dMatches.Length;
        Debug.Log("size !" + size);
        if (size < minNumberMatcherAllowed)
        {
            return false;
        }

        Point[] srcPoints = new Point[size];
        Point[] dstPoints = new Point[size];

        for (int i = 0; i < size; i++)
        {
            srcPoints[i] = trainKeypoints.toArray()[matches.toArray()[i].trainIdx].pt;
            dstPoints[i] = queryKeypoints.toArray()[matches.toArray()[i].queryIdx].pt;
        }

        Mat inliersMask = new Mat();

        homography = Calib3d.findHomography(
            new MatOfPoint2f(srcPoints),
            new MatOfPoint2f(dstPoints),
            Calib3d.RANSAC,
            reprojectionThreshold,
            inliersMask
            );

        //homography = Calib3d.findHomography(new MatOfPoint2f(srcPoints), new MatOfPoint2f(dstPoints), Calib3d.RANSAC, 3);

        List<DMatch> inliers = new List<DMatch>();

        for (int i= 0; i< size; i++)
        {
            if (inliersMask.get(i, 0)[0] == 1)
            {
                inliers.Add(dMatches[i]);
            }
        }
                
        matches = new MatOfDMatch(inliers.ToArray());

        Debug.Log("RefineMatchesWithHomography " + matches.ToString() + " Is " + (matches.toArray().Length >= minNumberMatcherAllowed).ToString());

        return matches.toArray().Length >= minNumberMatcherAllowed;
        
    }
}
