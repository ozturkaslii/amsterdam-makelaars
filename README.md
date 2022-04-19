# Amsterdam Makelaars
Determine which makelaar's in Amsterdam have the most object listed for sale. Make a table of the top 10. Then do the same thing but only for objects with a tuin which are listed for sale.

## Project Specifications
This project is running on .net 6. It has caching and throttling mechanisms. Also, It has retry mechanism based on external API's limitation (100 requests/min). It has some unit tests but it should be improved.

## Installation
git clone https://github.com/ozturkaslii/amsterdam-makelaars.git

cd amsterdam-makelaars


Before running the project, you need to add TemporaryKey under appsettings.Development.json. After that, you can directly start the project and call the APIs.
