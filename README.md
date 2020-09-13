# Handwritten Digit Recognizer (ml-mnist)
---
> An implementation of a image recognition neural network for handwritten digits using the ML.NET framework.

![Showcase](showcase.PNG)

---

### Model Statistics
  - Train data from publically available MNIST database
  - Trained regression model using Stochastic Dual Coordinate Ascent (Non-calibration)
  - Using Softmax and ReLU activation functions
  - Model accuracy was about 90% with a deviation of 6%
  
---

### Issues & Future Additions
  - Occasional prediction inconsistencies, likely due to bitmap preprocessing bugs
  - UI and formatting improvements to the Windows Form app
  - Bitmap preprocessing can be improved for better accuracy
  - Add additional network layers for better accuracy
  - Change to a nonlinear training method (kNN, SVM, or convolutional) to achieve 97%+ acc
  
---

### Development
```text
Developed by Jie "Jason" Liu (@jiejasonliu)

Contact: jasonningliu@gmail.com
```
