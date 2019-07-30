Encog Extensions Library

Personally I found myself in a situation where I needed to use normalization with randomized 
in memory data. The data could not be saved to a CSV as this would conflict with another frameworks
persistence mechanism.

The encog analyst is fantastic for normalizing data. It can take information stored in a CSV file
and automatically determine the normalized fields and their type of encoding 
(including 1 of N equilateral encoding). 

The only downside of this is that the logic is tightly coupled with the ReadCSV class.
Favouring extension as opposed to modification I decided to go about creating extension methods and
alternative classes to create an analyst that would normalize a generic .NET dataset. 

At the moment this logic is a partial re-write of the existing encog analyst. For this reason
much of the code is duplicated (which I appreciate is a bad thing). My intention is to eventually 
bake the logic directly into encog-core-cs by modifying the existing implementation.





