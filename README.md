# Node.cs
<strong>simple lightweight csharp server . set up your site fast</strong> support xamarin platform too

## let's SET UP SERVER ->
create server
```
//a random free port select automaticaly
var server = new HTTPServer();

//server runs on selected port
var port = 2548;
var server = new HTTPServer(port);
```
set route to server
```
//this is excecute when we browse to http://127.0.0.1:{port}
server.router.Add("",(context,data) => {
  //Handling request
});

//excecute when we browse to http://127.0.0.1:{port}/user
server.router.Add("user",(context,data) => {
  //Handling request
});
```
## Handling request
gatting request datas
```
server.router.Add("",(context,data) => {

  /*
  {data} contains request datas
  */
  
  //conatain in url vars - consider this url -> http://127.0.0.1:{port}/user?id=2
  var id = data.urlVars["id"];
  
  //getting files that send trought multipart-form
  var file = data.files["userImage"];
  
  //getting datas that send trought form
  var userName = data.bodyVars["username"];
  
 //getting request coockies
  var token = data.coockies["token"];
  
 //getting request header
  var agent = data.coockies["User-Agent"];
  
  absolutePath
  
});
```
writing reponces
```
server.router.Add("",(context,data) => {

  /*
  {context} is a HttListenerCintext
  this means that it has all the features of this type plus some extetion functions that i write for better and faster handling
  i just intruduce extentions below
  */
  
  //setting a coockie to responce for 12 hours
  context.SetCoockie(new Coockie{ key = "id", val = "imX98v4Ts", maxAge = Coockie.MAX_AGE_HOUR * 12});
    
  //for writing a text to responce
  //every object tath send to this function this writes responceObject.ToString() result to responce
  //Defaut text encoding is utf-8 . you can send your prefered encoding as second parametr
  context.WriteAndClose("hellow world");
	
});
```
