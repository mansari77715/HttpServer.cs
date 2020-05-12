function CreateCommand(data,onResult,isEvent = false,id = null){

    if(id == null)
        this.id = CreateCommandID();
    else
        this.id = id;

    this.onResult = onResult;
    this.isEvent = isEvent;

    Command.commands.push(this);

    if(!isEvent)
        server.send(data);
    
}

function CreateCommandID(){

    var palete = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
    var res = "";
    for (let index = 0; index < 10; index++) {
        res += palete.charAt(Math.floor((Math.random() * palete.length - 1)));
    }

    var isOK = true
    Command.commands.forEach(element => {
        
        if(element.id == res)
            isOK = false;

    });

    if(isOK)
        return res;
    else
        return CreateCommandID();

}

class Command{

    //JS    => server.send(json);
    //CS    => Command.Send(json);

    static commands = [];

    static SendToCs(id){

        

    }

    //use from CS
    static Send(responce){

        var r = JSON.parse(responce);
        Command.commands.forEach((element,index) => {
            
            if(element.id == r.id) {

                element.onResult(r.values);
                if(!element.isEvent)
                    Command.commands.splice(index,1);

            }

        });

    }

}