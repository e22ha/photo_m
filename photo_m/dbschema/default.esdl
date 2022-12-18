module default {
    abstract type Human {
        required property name -> str;
        property surname -> str;
        property full_name := .name ++ ' ' ++ .surname;
    }

    type Person extending Human {
        constraint exclusive on ( (.name, .surname) );
    }

    type Photographer extending Human{
        multi link camera -> Camera;
        property nick -> str {constraint exclusive; };
        multi link photos := .<author[is Photo];

    }

    type Camera {
        required property brand -> str;
        required property model -> str;
        required property name := .brand ++ ' ' ++ .model;
        multi link photographers := .<camera[is Photographer];
    }


    type Photo {
        required property name -> str;
        required property directory -> str;
        required property full_path := c_f_path(.directory, .name);
        constraint exclusive on ( (.name, .directory) );
        required link author -> Photographer;
        multi link face -> Person;
        property rating -> int64{
            default:=0;
        }
        link event -> Event;
    }

    type Event {
        required property title -> str;
        required property date -> datetime;
        constraint exclusive on ( (.title, .date) );
    }
    
    function c_f_path(dir: str, n: str)-> str
        using(
            SELECT(
                dir ++ n
            )
        )
    
}
