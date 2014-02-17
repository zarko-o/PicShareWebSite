PicShareWebSite
===============

ASP.NET WebSite for simple sharing of pictures.



I created this project because I needed a way to quickly share pictures 
(from my phone for example) with friends over the Internet, without uploading
them to various company sites. Since I have IIS Web Server at home I decided
to make a Web Site where I can copy my photos, and my friends can see those
photos after I send them the link to new folder.



To use the project follow these steps:

1. You need IIS Web Server with ASP.NET 4.5 (for ZipFile class).

2. This project is of type WebSite (not Web Application). Install the 
   project on your Web Server. Lets say you have that server on
   http://www.myphotos.com:8080

3. Copy folder(s) with photos under your Web Site. Thats why this project
   is practical only if you have the server at home.
   
   Only folders directly under the Web Site folder will be scanned by
   the Web Site and shown.
   
   Only folders that start with "20" will be found and recognized as
   image folders by the site, and offered as Web Page. The reason for
   this is that I am saving images in directories in this format:
   "20120625-AtTheBeach". "20yymmdd-description"...
   This logic can be changed in source code.
   
4. Now you should be able to see list of image folder that you copied
   under the Web Site (as long as those folders follow naming convention
   described above). Go to your site and use url parameter "user=chief"
   (can be configured in web.sonfig/appSettings).
   For example:
   http://www.myphotos.com:8080?user=chief
   This will show a list of image folders that you copied to the web server.
   
5. Now click on one of the links. A web page will open with photos from 
   that folder. Copy the link and send it to people you want to share
   the photos with.
   
   Notice 2 things:
   
   1. images shown on the page are not in the original size. They are 
   scaled down to have max width of 1024px (can be configured in
   web.config/appSettings). This scaling is practical because you do
   not want to send huge images upstream over your internet connection
   
   2. On top of the page there is a download ZIP link. Using this link
   visitors of the page can download whole directory (see url in your 
   browser, directory will be under ?dir= parameter) of photos, in one 
   ZIP file, in original size! Download link will serve original images
   zipped, not scaled down.

6. Modify AppSettings in web.config if needed:
   - "ChiefUser" - this is value of url user parameter that will show 
     all available image folders.
  - "ZipTempDir" - directory in which temporary ZIP file for download
    will be created. Notice {0} in the string.
  - "MaxWidth" - width to which images will be scaled.
