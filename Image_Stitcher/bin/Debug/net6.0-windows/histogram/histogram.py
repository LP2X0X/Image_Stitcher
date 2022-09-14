import cv2
import numpy as np
from matplotlib import pyplot as plt

img = cv2.imread("A.jpg")
img = cv2.cvtColor(img, cv2.COLOR_RGB2GRAY)
hist = cv2.calcHist([img], [0], None, [256], [0,256])
hist = hist.flatten().tolist()
hist = [int(x) for x in hist]

print(hist)
plt.plot(hist)
plt.xlim([0,256])
plt.show()
with open('A.txt', 'w') as file:
   file.write(str(hist))

