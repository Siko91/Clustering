# Associations

<hr/>

# Association Finder

The program generates makes all possible association claims about a set of data and checks them out.
Then it lists the 10 claims it's most confident in.

For example: 

Claim : If customer's age is < 16 and customer bought candy, then it's likely that the customer also bought Coca Cola.
This is one of the claims that the program will make

<hr/>
<hr/>
<hr/>

# Clustering

<hr/>

# K-Means Clustering

The program takes an image and numbers as inputs. 
A number represents how many clusters will the program look for in the image's pixels.
The program produces a simplified image, where the clustered pixels are shown with their average color.

For example: 

> KMeans.exe a.jpeg 4 25 100

will read this image:
<div>
<img src='https://s21.postimg.org/z53n19v0j/image.jpg' border='0' alt="a"/> 
</div>

and will produce these: 
<div>
<img src='https://s21.postimg.org/i5uom0jsz/a.jpg-4.jpeg.png' border='0' alt="a.jpg-4.jpeg"/>
<img src='https://s21.postimg.org/3nxhe0shv/a.jpg-25.jpeg.png' border='0' alt="a.jpg-25.jpeg"/>
<img src='https://s21.postimg.org/6jakkvwhv/a.jpg-100.jpeg.png' border='0' alt="a.jpg-100.jpeg"/> 
</div>

