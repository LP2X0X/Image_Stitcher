# Image Stitcher (Currently in Development)
This is a personal project.  
The purpose of this application is to stitch multiple images vertically or horizontally.

## Table of Contents
* [General Info](#general-info)
* [Technologies](#technologies)
* [Algorithm Implementation](#algorithm-implementation)
* [Resources](#resources)
* [References](#references)

## General Info
This UI for this application is created using C# with WPF Framework.</b>
The algorithm behind it is implemented using Python.

## Technologies
### User Interface
* Microsoft Visual Studio 2022 Community Edition (ver 17.3.3) using WPF Framwork with .NET 6.0 desktop development.

### Algorithm
* Python 3.x (preferred 3.9.6)
* OpenCV Library 4.x (preferred 4.5.5)
* Numpy Library 1.x (preferred 1.19.5)
* Imutils Module 0.x (preferred 0.5.4)

## Algorithm Implementation
For keypoints and features detection, the **Scale-Invariant Feature Transform (SIFT)** [1] algorithm is used from the **opencv** package.
</br>

<p align="justify">
Once the <b>keypoints</b> and <b>features descriptors</b> are obtained from a pair of images, <i>brute-force-matching</i> is performed using <b>Euclidean distance</b> as the metric. For each point in one image, two points with <i>lowest</i> Euclidean distance in the other image is obtained using <b>KNN algorithm</b> (indicating the top two matches). The reason we want the top two matches rather than just the top one match is because we need to apply David Lowe’s ratio test for false-positive match pruning.
</br>
</br>
With a list of matched points between two images, the <b>homography matrix</b> can be computed. However there can be oulier matches. In order to minimize the effect of outliers and to obtain the best homography matrix, RANSAC [2] algorithm is used.
</br>
</br>
Once a homography is obtained from one image to the other image, opencv's warp perspective function is used to transform the second image into the perspective of the first. The resultant image might be warped edges because of the change in perspective. Also, if the images are not of the same size, then there would be empty pixels in the stitched image. Therefore, I have implemented a method to elegantly remove these empty pixels and retain the maximum image information between a pair of images.
</br>
</br>
</p>

## Resources
[1] David Lowe, "Distinctive Image Features from Scale-Invariant Keypoints" - November, 2004 - International Journal of Computer Vision 60(2):91---110 - DOI: 10.1023/B:VISI.0000029664.99615.94 
[2] Fischler, Martin A., and Robert C. Bolles, "Random sample consensus: a paradigm for model fitting with applications to image analysis and automated cartography." - Communications of the ACM 24.6 (1981): 381-395 - DOI:10.1145/358669.358692

## References
1. https://docs.opencv.org/4.x/d9/dab/tutorial_homography.html
2. https://math.stackexchange.com/questions/494238/how-to-compute-homography-matrix-h-from-corresponding-points-2d-2d-planar-homog
